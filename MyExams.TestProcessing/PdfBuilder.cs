using iTextSharp.text;
using iTextSharp.text.pdf;
using MyExams.TestProcessing.TestClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MyExams.TestProcessing
{
    public class PdfBuilder
    {
        private Font f;
        private Font fBold;
        private string bgAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";
        public PdfBuilder()
        {
            var appDomain = System.AppDomain.CurrentDomain;
            var basePath = appDomain.RelativeSearchPath;
            string ARIALUNI_TFF = basePath + @"\ARIALUNI.TTF";
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            f = new Font(bf, 12, Font.NORMAL);
            fBold = new Font(bf, 12, Font.BOLD);

        }

        public FileContentResult TPTestToPdf(List<TPTest> list)
        {
            Byte[] bytes;
            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 50, 50, 70, 70);
                var writer = PdfWriter.GetInstance(doc, ms);

                doc.Open();
                foreach (var item in list)
                {
                    doc.NewPage();
                    var studentsDetails = new Paragraph("Име: " + item.StudentDetails.FullName + " Клас: " + item.StudentDetails.ClassName + " №" + item.StudentDetails.NoInClass, f);
                    studentsDetails.SpacingAfter = 25;
                    doc.Add(studentsDetails);
                    int numberingQuestions = 1;
                    foreach (var section in item.Sections)
                    {
                        foreach (var question in section.Questions)
                        {
                            if (question.Type == Models.QuestionType.Choice)
                            {
                                var table = DisplayQuestion(numberingQuestions+ ". " + question.Title, question.Answers.Select(x => x.Text).ToArray());
                                doc.Add(table);
                            }
                            else
                            {
                                var table = DisplayQuestion(question.Title);
                                doc.Add(table);
                            }
                            numberingQuestions++;
                        }
                    }

                }

                doc.Close();
                bytes = ms.ToArray();
            }
            return new FileContentResult(bytes, "application/pdf");
        }
        private PdfPTable DisplayQuestion(string questionText, params string[] options)
        {

            PdfPTable table = new PdfPTable(1);
            table.WidthPercentage = 100;
            List<PdfPCell> cells = new List<PdfPCell>();
            var questionParagraph = new Paragraph(questionText, fBold);
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
                var cell = new PdfPCell(new Phrase(bgAlphabet[counter] + ") " + item, f))
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
