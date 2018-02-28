﻿using AForge;
using MyExams.Models;
using MyExams.Services.Contracts;
using MyExams.TestProcessing.Contracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MyExams.TestProcessing
{
    public class TestCheckProcess:ITestCheckProcess
    {
        private readonly ITestService _testService;
        private readonly IGAnswerSheetService _gAnswerSheetService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly ITeacherService _teacherService;
        private readonly IUploadSessionService _uploadSessionService;
        private readonly IFileDirectoryService _fileDirectoryService;
        
        private Bitmap _bitmap;
        private Graphics g;
        private string FileName;
        private FileDirectory BitmapFileDirectory;
        public TestCheckProcess(ITestService testService, IGAnswerSheetService gAnswerSheetService, IQuestionService questionService, IAnswerService answerService, ITeacherService teacherService, IUploadSessionService uploadSessionService, IFileDirectoryService fileDirectoryService)
        {
            if(_testService==null)_testService = testService;
            if (_gAnswerSheetService == null) _gAnswerSheetService = gAnswerSheetService;
            if (_questionService == null) _questionService = questionService;
            if (_answerService == null) _answerService = answerService;
            if (_teacherService == null) _teacherService = teacherService;
            if (_uploadSessionService == null) _uploadSessionService = uploadSessionService;
            if (_fileDirectoryService == null) _fileDirectoryService = fileDirectoryService;
        }
        public void SetBitmap(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                _bitmap = bitmap;
                g = Graphics.FromImage(bitmap);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public void SetSaveFileName(string path)
        {
            FileName = path;
        }
        public void SetBitmapFileDirectory(FileDirectory file)
        {
            BitmapFileDirectory = file;
        }
        public GTest StartChecking()
        {
            if (_bitmap == null) throw new ArgumentNullException("bitmap", "Bitmap is null. Use SetBitmap first");
            if (FileName == null) throw new ArgumentNullException("FileDirectory", "FileDirectory is null. Use SetSaveFileName first");
            var uploadSession = _uploadSessionService.GetUploadSessionBy(BitmapFileDirectory);
            var uploadSessionFileDirectory = _uploadSessionService.GetAllUploadSessionFileDirectory().Where(x => x.FileDirectory.Id == BitmapFileDirectory.Id).FirstOrDefault();

            AnswerSheetRecognition sheetRecognition = new AnswerSheetRecognition(_bitmap);
            var barcode = sheetRecognition.BarcodeRecognize();
            if (barcode != string.Empty)
            {
                var answerSheet = _gAnswerSheetService.GetAllGAnswerSheet().Where(x => x.Barcode == barcode).FirstOrDefault();
                if (answerSheet != null)
                {
                    if (!answerSheet.IsChecked)
                    {
                        List<int> AnswerMatix = new List<int>();
                        var gTest = _testService.GetAllGTests().Where(x => x.Id == answerSheet.GTest.Id).First();
                        var teacher = _teacherService.GetAll().Where(x => x.Id == gTest.Teacher.Id).First();

                        Pen yellowPen = new Pen(Color.Yellow, 4);
                        Pen redPen = new Pen(Color.Red, 4);
                        Pen greenPen = new Pen(Color.Green, 4);

                        // get the AnswerMatix from the xml text
                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(gTest.Xml);
                        for (int i = answerSheet.FirstQuestionNo; i <= answerSheet.LastQuestionNo; i++)
                        {
                            var node = xml.DocumentElement.ChildNodes[i];
                            AnswerMatix.Add((node.ChildNodes.Count) > 0 ? node.ChildNodes.Count : 1);
                        }
                        //recognise the answerSheet
                        sheetRecognition.AnswersMatrix = AnswerMatix;
                        var recognisedRowsAndShapes = sheetRecognition.ProcessImage();
                        //get the correctAnswers from the xml string
                        var correctAnswers = CorrectAnswers(xml, answerSheet.FirstQuestionNo, answerSheet.LastQuestionNo);
                        int totalPoints = 0;
                        for (int i = 0; i < recognisedRowsAndShapes.Count; i++) // start comparing
                        {
                            if (recognisedRowsAndShapes[i].Count == 1)  // these are text questions
                            {
                                var rectangle = new Rectangle(recognisedRowsAndShapes[i][0].TopLeftPoint.X, recognisedRowsAndShapes[i][0].TopLeftPoint.Y, (int)recognisedRowsAndShapes[i][0].TopLeftPoint.DistanceTo(recognisedRowsAndShapes[i][0].TopRightPoint), (int)recognisedRowsAndShapes[i][0].TopLeftPoint.DistanceTo(recognisedRowsAndShapes[i][0].BottomLeftPoint));
                                var croppedImage = _bitmap.Clone(rectangle, _bitmap.PixelFormat);
                                var imagename = RandomString(20);
                                var path = Path.Combine(FileName, imagename + ".jpg");
                                BitmapSave(croppedImage, path);
                                //save the piece containing the answer
                                var gWrittenQuestion = new GWrittenQuestion()
                                {
                                    FileName = path,
                                    GTest = gTest,
                                    GQuestionId = i + answerSheet.FirstQuestionNo
                                };
                                _gAnswerSheetService.AddGWrittenQuestion(gWrittenQuestion);

                                _gAnswerSheetService.AddGQuestionToBeChecked(new GQuestionToBeChecked()
                                {
                                    GWrittenQuestion = gWrittenQuestion,
                                    Teacher = teacher
                                });
                                //add the location on the file in the db for future reference
                                DrawShape(recognisedRowsAndShapes[i][0], yellowPen);
                            }
                            else
                            {
                                var isCorrect = false; // used to store wheather the whole question is true or false
                                for (int p = 0; p < recognisedRowsAndShapes[i].Count; p++)
                                {

                                    if (recognisedRowsAndShapes[i][p].IsChecked && correctAnswers[i].CorrectAnswers[p])
                                    {
                                        isCorrect = true;
                                        var answerAttr = xml.CreateAttribute("s"); // short for status
                                        answerAttr.Value = "2"; //correct code
                                        xml.DocumentElement.ChildNodes[answerSheet.FirstQuestionNo + i].ChildNodes[p].Attributes.Append(answerAttr);

                                        DrawShape(recognisedRowsAndShapes[i][p], greenPen);
                                    }
                                    else if (recognisedRowsAndShapes[i][p].IsChecked && !correctAnswers[i].CorrectAnswers[p])
                                    {
                                        var answerAttr = xml.CreateAttribute("s"); // short for status
                                        answerAttr.Value = "1"; //checked but not correct code
                                        xml.DocumentElement.ChildNodes[answerSheet.FirstQuestionNo + i].ChildNodes[p].Attributes.Append(answerAttr);
                                        DrawShape(recognisedRowsAndShapes[i][p], redPen);
                                    }
                                    else if (!recognisedRowsAndShapes[i][p].IsChecked)
                                    {
                                        var answerAttr = xml.CreateAttribute("s"); // short for status
                                        answerAttr.Value = "0"; //unchecked
                                        xml.DocumentElement.ChildNodes[answerSheet.FirstQuestionNo + i].ChildNodes[p].Attributes.Append(answerAttr);
                                        DrawShape(recognisedRowsAndShapes[i][p], yellowPen);
                                    }

                                }
                                if (isCorrect)
                                {
                                    var questionAttr = xml.CreateAttribute("rp"); // short for received points
                                    questionAttr.Value = correctAnswers[i].Points.ToString();
                                    xml.DocumentElement.ChildNodes[answerSheet.FirstQuestionNo + i].Attributes.Append(questionAttr);

                                    totalPoints += correctAnswers[i].Points;
                                }
                                else
                                {
                                    var questionAttr = xml.CreateAttribute("rp"); // short for received points
                                    questionAttr.Value = "0";
                                    xml.DocumentElement.ChildNodes[answerSheet.FirstQuestionNo + i].Attributes.Append(questionAttr);
                                }
                            }
                        }
                        // mark the answerSheet as done 
                        using (StringWriter sw = new StringWriter())
                        {
                            using (XmlTextWriter xw = new XmlTextWriter(sw))
                            {
                                xml.WriteTo(xw);
                                gTest.Xml = sw.ToString();
                            }
                        }
                        var imageName = RandomString(20);
                        var path1 = Path.Combine(FileName, imageName + ".jpg");
                        BitmapSave(_bitmap, path1);
                        var checkedFileDirectory = new FileDirectory()
                        {
                            FileName = path1
                        };
                        _fileDirectoryService.AddFileDirectory(checkedFileDirectory);

                        answerSheet.CheckedFileName = checkedFileDirectory;
                        answerSheet.IsUploaded = true;
                        answerSheet.ScannedFileName = BitmapFileDirectory;
                        answerSheet.IsChecked = true;

                        gTest.ProcessedAnswerSheets++;
                        gTest.ReceivedPoints += totalPoints;
                        uploadSessionFileDirectory.UploadedFileStatus = UploadedFileStatus.Checked;

                        _testService.Update();

                        yellowPen.Dispose();
                        greenPen.Dispose();
                        redPen.Dispose();
                        return gTest;
                    }
                    else
                    {
                        uploadSessionFileDirectory.UploadedFileStatus = UploadedFileStatus.AlreadyChecked;
                    }
                }
                else
                {
                    uploadSessionFileDirectory.UploadedFileStatus = UploadedFileStatus.HaveProblem;
                }
            }
            else
            {
                uploadSessionFileDirectory.UploadedFileStatus = UploadedFileStatus.FileNotRecognised;
            }
            uploadSession.TotalFinished++;
            _testService.Update();
            return null;
        }

        private List<Question> CorrectAnswers(XmlDocument xml, int firstQ, int lastQ)
        {
            var result = new List<Question>();
            for (int i =firstQ; i <=lastQ; i++)
            {
                var node  = xml.DocumentElement.ChildNodes[i];
                var id = xml.DocumentElement.ChildNodes[i].Attributes["id"].Value;
                var questionPoints = _questionService.GetPoints(int.Parse(id));
                result.Add(new Question()
                {
                    CorrectAnswers = new List<bool>(),
                    Points = questionPoints
                });
                foreach (XmlNode item in node)
                {
                    var answerId = item.Attributes["id"].Value;
                    var isCorrect = _answerService.GetIsCorrect(int.Parse(answerId));
                    result.Last().CorrectAnswers.Add(isCorrect);
                }
            }
            return result;
        }

        private void DrawShape(Shape shape, Pen pen)
        {
            if (g != null)
            {

                List<IntPoint> corners = new List<IntPoint>
                {
                    new IntPoint()
                    {
                        X = shape.TopLeftPoint.X,
                        Y = shape.TopLeftPoint.Y
                    },
                    new IntPoint()
                    {
                        X = shape.BottomLeftPoint.X,
                        Y = shape.BottomLeftPoint.Y
                    },

                    new IntPoint()
                    {
                        X = shape.BottomRightPoint.X,
                        Y = shape.BottomRightPoint.Y
                    },
                    new IntPoint()
                    {
                        X = shape.TopRightPoint.X,
                        Y = shape.TopRightPoint.Y
                    }
                };
                g.DrawPolygon(pen, ToPointsArray(corners));
            }
        }
        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }
        private class Question
        {
            public int Id { get; set; }
            public int Points { get; set; }
            public List<bool> CorrectAnswers { get; set; }
        }
        private class Answer
        {
            public int Id { get; set; }
            public int MyProperty { get; set; }
        }
        private static string RandomString(int length)
        {
            var random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void BitmapSave(Bitmap bitmap, string path)
        {
            var isSaved = false;
            do
            {
                try
                {
                    bitmap.Save(path, ImageFormat.Jpeg);
                    isSaved = true;
                }
                catch (Exception)
                {

                    isSaved = false;
                }
            } while (!isSaved);
        }

    
    }
}