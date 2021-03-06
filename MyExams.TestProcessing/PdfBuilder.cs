﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using MyExams.Models;
using MyExams.Services.Contracts;
using MyExams.TestProcessing.TestClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ZXing;

namespace MyExams.TestProcessing
{
    class PdfBuilder
    {
        private static Font f12;
        private static Font f12Bold;
        private static Font f7;
        private string bgAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";

        private readonly ITestService _testService;
        private readonly IGAnswerSheetService _gAnswerSheetService;
        private static TPTest CurrentTest;

        public PdfBuilder(ITestService testService, IGAnswerSheetService gAnswerSheetService)
        {
            _testService = testService;
            _gAnswerSheetService = gAnswerSheetService;

            
            var basePath = System.AppDomain.CurrentDomain.RelativeSearchPath;
            string ARIALUNI_TFF = basePath + @"\ARIALUNI.TTF";
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            f12 = new Font(bf, 12, Font.NORMAL);
            f12Bold = new Font(bf, 12, Font.BOLD);
            f7 = new Font(bf, 7, Font.NORMAL);

        }

        private class Header : PdfPageEventHelper
        {
            public override void OnStartPage(PdfWriter writer, Document document)
            {
                var studentTable = GetStudentTable();
                if (studentTable != null)
                {
                    document.Add(studentTable);
                }
                else
                {
                    document.Add(new Phrase("  "));
                }
            }
        }

        public FileContentResult TPTestToPdf(List<TPTest> list)
        {
            Byte[] bytes;
            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 50, 50, 70, 70);
                var writer = PdfWriter.GetInstance(doc, ms);
                Header header = new Header();
                writer.PageEvent = header;
                CurrentTest = list[0];
                doc.Open();
                foreach (var item in list)
                {
                    var gTest = _testService.GetAllGTests().Where(x => x.Id == item.GTestId).First();
                     CurrentTest = item;
                    if (list.First() != item) doc.NewPage();
                    var currentPage = writer.PageNumber;
                    int numberingQuestions = 1;
                   
                 
                    // Adding Questions 
                  

                    foreach (var section in item.Sections)
                    {
                        doc.Add(new Paragraph(section.Title, f12));
                        if (section.ImageFileName != null)
                        {
                            try
                            {
                            iTextSharp.text.Image sectionImage = iTextSharp.text.Image.GetInstance(section.ImageFileName);
                            var number = sectionImage.Height / sectionImage.Width;
                            sectionImage.ScaleAbsolute((float)(doc.PageSize.Width * 0.7), (float)(doc.PageSize.Width * 0.7 * number));
                            PdfPTable table = new PdfPTable(1);
                            table.AddCell(sectionImage);
                            doc.Add(table);
                            }
                            catch (Exception)
                            {
                            }
                           
                        }
                        
                        foreach (var question in section.Questions)
                        {
                            
                            if (question.Type == Models.QuestionType.Choice)
                            {
                                var table = DisplayQuestion(numberingQuestions + ". " + question.Title, question.Answers.Select(x => x.Text).ToArray());
                                doc.Add(table);
                            }
                            else
                            {
                                var table = DisplayQuestion(numberingQuestions + ". " + question.Title);
                                doc.Add(table);
                            }
                            numberingQuestions++;
                        }
                    }
                    if (writer.PageNumber % 2 != 0)
                    {
                      CurrentTest = null;
                      doc.NewPage();
                       CurrentTest = item;
                    }
                    doc.NewPage();

       //AnswerSheet Generation
                    PdfContentByte cb = writer.DirectContent;
                    cb.SetColorStroke(BaseColor.BLACK);
                    cb.SetLineWidth(1.8);

                    ColumnText ct = new ColumnText(cb);
                    string[] letters = { "А", "Б", "В", "Г", "Д", "Е", "Ж", "З", "И" };
                    Random rm = new Random();

                    int currentYPixel = (int)doc.PageSize.Height - 160;
                    int rowCount = -1;
                    int firstQuestion = 0;
                    int pageNo = 1;

                    //// Students Details 
                    //studentTable.WriteSelectedRows(0, -1, 60, currentYPixel + 20, cb);
                    //currentYPixel -= 35;


                    //Barcode 

                    var barcode = _gAnswerSheetService.BarcodeGenerate();
                    gTest.TotalAnswerSheets++;
                    _testService.Update();
                    var barcodeWriter = new BarcodeWriter
                    {
                        Format = BarcodeFormat.CODE_128,
                        Options = new ZXing.Common.EncodingOptions
                        {
                            Height = 20,
                            Width = 150,
                            Margin = 1,
                            PureBarcode = true
                        }
                    };
                    var barcodeBitmap = barcodeWriter.Write(barcode);
                    var barcodeMemoryStream = new MemoryStream();
                        barcodeBitmap.Save(barcodeMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        barcodeMemoryStream.Seek(0, SeekOrigin.Begin);
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(barcodeMemoryStream);
                        image.RotationDegrees = 90;
                        image.SetAbsolutePosition((int)doc.PageSize.Width - 20, (int)doc.PageSize.Height/2 + 25);
                        cb.AddImage(image);
                        foreach (var section in item.Sections)
                        {
                            foreach (var question in section.Questions)
                            {

                                rowCount++;

                                //QuestionNumbering
                                Phrase questionNumLabel = new Phrase(rowCount + 1 + ".");
                                ct.SetSimpleColumn(questionNumLabel, 60, currentYPixel, 104, currentYPixel + 20, 15, Element.ALIGN_CENTER);
                                ct.Go();

                                if (question.Type == Models.QuestionType.Choice)
                                {

                                    for (int i = 0; i < question.Answers.Count; i++)
                                    {
                                        var rect = new iTextSharp.text.Rectangle(104 + 40 * i, currentYPixel, 25, 15);
                                        rect.Border = Rectangle.BOX;
                                        cb.Rectangle(104 + 40 * i, currentYPixel, 25, 15);
                                        cb.Stroke();
                                        Phrase myText = new Phrase(letters[i], f7);
                                        ct.SetSimpleColumn(myText, 104 + 40 * i, currentYPixel, 129 + 40 * i, currentYPixel + 20, 15, Element.ALIGN_CENTER);
                                        ct.Go();

                                    }
                                    currentYPixel -= 35;
                                }
                                else if (question.AnswerSize == Models.QuestionAnswerSize.Small)
                                {
                                    cb.Rectangle(104, currentYPixel, 250, 25);
                                    cb.Stroke();
                                    currentYPixel -= 35;
                                }
                                else if (question.AnswerSize == Models.QuestionAnswerSize.Medium)
                                {
                                    cb.Rectangle(104, currentYPixel, 400, 25);
                                    cb.Stroke();
                                    currentYPixel -= 35;
                                }
                                else if (question.AnswerSize == Models.QuestionAnswerSize.Long)
                                {
                                    cb.Rectangle(104, currentYPixel - 45, 400, 70);
                                    cb.Stroke();
                                    currentYPixel -= 80;
                                }

                                if (currentYPixel < 70&&section.Questions.FindIndex(x=>x.QuestionId == question.QuestionId) != section.Questions.Count-1)
                                {
                                    doc.NewPage();
                                    currentYPixel = (int)doc.PageSize.Height - 70;
                                   //studentTable.WriteSelectedRows(0, -1, 60, currentYPixel + 20, cb);
                                    currentYPixel -= 35;

                                    gTest.TotalAnswerSheets++;
                                    GAnswerSheet answerSheet = new GAnswerSheet()
                                    {
                                        FirstQuestionNo = firstQuestion,
                                        GTest = gTest,
                                        LastQuestionNo = rowCount,
                                        Barcode = barcode,
                                        PageNo = pageNo

                                    };
                                    _gAnswerSheetService.AddGAnswerSheet(answerSheet);
                                    pageNo++;
                                    firstQuestion = rowCount + 1;

                                    // Add barcode to the next page
                                    barcodeMemoryStream = new MemoryStream(barcodeMemoryStream.Capacity);
                                    barcode = _gAnswerSheetService.BarcodeGenerate();
                                    barcodeBitmap = barcodeWriter.Write(barcode);
                                    barcodeBitmap.Save(barcodeMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    barcodeMemoryStream.Seek(0, SeekOrigin.Begin);
                                    image = Image.GetInstance(barcodeMemoryStream);
                                    image.RotationDegrees = 90;
                                    image.SetAbsolutePosition((int)doc.PageSize.Width - 20, (int)doc.PageSize.Height / 2 + 25);
                                    cb.AddImage(image);
                                    cb.SetColorStroke(BaseColor.BLACK);
                                    cb.SetLineWidth(1.8);
                                }
                            }
                        }
                        GAnswerSheet answerSheet1 = new GAnswerSheet()
                        {
                            FirstQuestionNo = firstQuestion,
                            GTest = gTest,
                            LastQuestionNo = rowCount,
                            Barcode = barcode,
                            PageNo = pageNo

                        };
                        _gAnswerSheetService.AddGAnswerSheet(answerSheet1);

                    barcodeMemoryStream.Dispose();
                    barcodeBitmap.Dispose();
                    if (writer.PageNumber % 2 != 0)
                    {
                       CurrentTest = null;
                        doc.NewPage();
                       CurrentTest = item;
                    }
                }
                
                doc.Close();
                bytes = ms.ToArray();
            }
            return new FileContentResult(bytes, "application/pdf");
        }

        private static PdfPTable GetStudentTable()
        {
            if (CurrentTest == null) return null;
            //Adding Student Details Table 
            PdfPTable studentTable = new PdfPTable(2)
            {
                WidthPercentage = 100
            };
            PdfPCell cellName = new PdfPCell(new Phrase("Име:   " + CurrentTest.StudentDetails.FullName, f12))
            {
                Border = Rectangle.BOTTOM_BORDER,
                PaddingBottom = 5
            };
            studentTable.AddCell(cellName);
            PdfPCell cellClassNo = new PdfPCell(new Phrase("№" + CurrentTest.StudentDetails.NoInClass + "  Клас: " + CurrentTest.StudentDetails.ClassName, f12))
            {
                HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                Border = Rectangle.BOTTOM_BORDER,
                PaddingBottom = 5
            };
            studentTable.AddCell(cellClassNo);

            studentTable.SpacingAfter = 25;
            return studentTable;
        }
        private PdfPTable DisplayQuestion(string questionText, params string[] options)
        {

            PdfPTable table = new PdfPTable(1);
            table.WidthPercentage = 100;
            List<PdfPCell> cells = new List<PdfPCell>();
            var questionParagraph = new Paragraph(questionText, f12Bold);
            var questionCell = new PdfPCell(questionParagraph)
            {
                PaddingBottom = 10,
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Rectangle.ALIGN_JUSTIFIED
            };
            questionCell.SetLeading(4, 1);
            table.AddCell(questionCell);

            int counter = 0;
            foreach (var item in options)
            {
                var cell = new PdfPCell(new Phrase(bgAlphabet[counter] + ") " + item, f12))
                {
                    PaddingLeft = 20,
                    Border = Rectangle.NO_BORDER,
                    PaddingTop = 2,
                    HorizontalAlignment = Rectangle.ALIGN_LEFT
                };

                table.AddCell(cell);
                counter++;
            }

            table.SpacingAfter = 14;
            return table;
        }
        
    }

}
