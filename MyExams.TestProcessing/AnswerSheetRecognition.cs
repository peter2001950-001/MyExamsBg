using AForge;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using ZXing;

namespace MyExams.TestProcessing
{
    public class AnswerSheetRecognition
    {
        private Bitmap _bitmap;
        private UnsafeBitmap _lockBitmap;
        private List<List<Shape>> RowsAndShapes { get;  set; }
        public List<Line> AngleLines { get; private set; }
        public List<int> AnswersMatrix { private get; set; }
        private int minLineHeight;
        private int maxWidth;
        private int minWidth;

        public AnswerSheetRecognition(Bitmap bitmap)
        {
            _bitmap = bitmap;
            RowsAndShapes = new List<List<Shape>>();
            AngleLines = new List<Line>();
            _lockBitmap = new UnsafeBitmap(_bitmap);
            _lockBitmap.LockBitmap();
            minLineHeight = (int)(_bitmap.Height * 0.017);
            maxWidth = (int)(_bitmap.Width * 0.04884);
            minWidth = (int)(_bitmap.Width * 0.024007);
        }

        public string BarcodeRecognize()
        {
            var croppedRect = new Rectangle((int)(_bitmap.Width * 0.9), (int)(0.30 * _bitmap.Height), (int)(0.1 * _bitmap.Width), (int)(0.40 * _bitmap.Height));
            var croppedBitmap = CropBarcode(croppedRect);
            if (croppedBitmap != null)
            {
                IBarcodeReader reader = new BarcodeReader()
                {
                    Options = new ZXing.Common.DecodingOptions()
                    {
                        PossibleFormats = new List<BarcodeFormat>()
                      {
                           BarcodeFormat.CODE_128
                      },
                        TryHarder = true,

                    }
                };

                var barcodeDecoded = reader.Decode(croppedBitmap);
               // croppedBitmap.Save(@"C:\Users\Peter\Documents\Visual Studio 2015\Projects\MyExams\MyExams\App_Data\barcode.jpg", ImageFormat.Jpeg);
                croppedBitmap.Dispose();
                if (barcodeDecoded != null)
                {
                    return barcodeDecoded.Text;
                }
            }
            return null;
        }
        public List<List<Shape>> ProcessImage()
        {
            List<Shape> result = new List<Shape>();
            IntPoint verticalLinePoint = new IntPoint(0, 0);
            List<Line> lines = new List<Line>();

            result = GetShapes((int)(0.1 * _bitmap.Width), 0, (int)(0.9 * _bitmap.Width), _bitmap.Height - 100);
            result = result.OrderBy(x => x.TopLeftPoint.Y).ToList();


            List<List<Shape>> recongizedRowsAndShapes = new List<List<Shape>>();
            while (result.Count > 0)
            {
                var point = result[0].TopLeftPoint;
                var height = result[0].TopLeftPoint.DistanceTo(result[0].BottomLeftPoint);
                var row = result.Where(x => x.TopLeftPoint.Y - point.Y > height * -1 && x.TopLeftPoint.Y - point.Y < height).OrderBy(x => x.TopLeftPoint.X).ToList();
                foreach (var item in row)
                {
                    result.Remove(item);
                }
                recongizedRowsAndShapes.Add(row);
            }
            for (int i = 0; i < recongizedRowsAndShapes.Count; i++)
            {
                if (recongizedRowsAndShapes[i].Count != AnswersMatrix[i])
                {
                    int startingX = (i > 0) ? recongizedRowsAndShapes[i - 1][0].TopLeftPoint.X - 10 : 100;
                    var anotherTry = GetShapes(startingX, recongizedRowsAndShapes[i][0].TopLeftPoint.Y - 15, _bitmap.Width - 100, recongizedRowsAndShapes[i][0].BottomLeftPoint.Y + 15, AnswersMatrix[i]);
                    if (anotherTry.Count == AnswersMatrix[i])
                    {
                        recongizedRowsAndShapes[i] = anotherTry;
                    }
                }
                RowsAndShapes.Add(IsChecked(recongizedRowsAndShapes[i]));
            }
            return recongizedRowsAndShapes;

        }


        private List<Shape> GetShapes(int startingX, int startingY, int endingX, int endingY, int targetShapes = 0)
        {
            List<Shape> result = new List<Shape>();
            IntPoint verticalLinePoint = new IntPoint(0, 0);
            List<Line> lines = new List<Line>();
            int consequentWhitePixel = 0;
            bool breakOnNext = false;
            int counter = 1;
            if (targetShapes > 0)
            {
                counter = 6;
            }
            for (int p = 0 + ((targetShapes > 0) ? 1 : 0); p < counter; p++)
            {
                for (int x = startingX; x < endingX; x++)
                {
                    for (int y = startingY; y < endingY; y++)
                    {

                        var pixel = _lockBitmap.GetPixel(x, y);

                        if (pixel.R < 230 + 3 * p && pixel.G < 230 + 3 * p && pixel.B < 230 + 3 * p && !(pixel.R < 100 && pixel.G < 100 && pixel.B > 60))
                        {

                            if (verticalLinePoint.X == 0 && verticalLinePoint.Y == 0)
                            {
                                verticalLinePoint.X = x;
                                verticalLinePoint.Y = y;
                                consequentWhitePixel = 0;
                            }
                        }
                        else
                        {
                            consequentWhitePixel++;
                            if ((verticalLinePoint.X > 0 || verticalLinePoint.Y > 0) && consequentWhitePixel > 10 + p)
                            {

                                var line = new Line()
                                {
                                    BPoint = new IntPoint() { X = x, Y = y },
                                    APoint = verticalLinePoint
                                };

                                if (line.Height > minLineHeight - 10)
                                {
                                    if (line.Height > 500)
                                    {
                                        AngleLines.Add(line);
                                    }
                                    else
                                    {
                                        var linesPartFromOne = lines.Where(item => item.APoint.Y - line.BPoint.Y > -10 && item.APoint.Y - line.BPoint.Y < 10)
                                            .Where(listItem => listItem.APoint.X - line.APoint.X > -20 && listItem.APoint.X - line.APoint.X < 20).ToList();

                                        if (linesPartFromOne.Count > 0)
                                        {
                                            linesPartFromOne.Add(line);
                                            linesPartFromOne.OrderBy(item => item.APoint.Y);
                                            lines.Add(new Line { APoint = linesPartFromOne[0].APoint, BPoint = linesPartFromOne.Last().BPoint });
                                            foreach (var item in linesPartFromOne)
                                            {
                                                lines.Remove(item);
                                            }
                                        }

                                        var similarLine = lines.Where(listItem => listItem.APoint.Y - line.APoint.Y > listItem.Height * -1 && listItem.APoint.Y - line.APoint.Y < listItem.Height)
                                            .Where(listItem => listItem.APoint.X - line.APoint.X > -20 && listItem.APoint.X - line.APoint.X < 20).FirstOrDefault();
                                        if (similarLine != null)
                                        {
                                            if (similarLine.Height <= line.Height)
                                            {
                                                lines.Remove(similarLine);
                                                lines.Add(line);
                                            }

                                        }
                                        else
                                        {
                                            lines.Add(line);
                                        }
                                    }
                                }

                                verticalLinePoint.X = 0;
                                verticalLinePoint.Y = 0;

                            }
                        }

                    }
                }
                result = MatchLinesToShapes(lines);
                if (result.Count == targetShapes || breakOnNext)
                {
                    return result;
                }
                else if (result.Count >= targetShapes)
                {
                    p -= 2;
                    breakOnNext = true;
                }
            }
            return result;

        }
        private List<Shape> MatchLinesToShapes(List<Line> lines)
        {
            var result = new List<Shape>();
            while (lines.Count > 0)
            {
                var item = lines[0];
                var nearbyItems = lines.Where(x => x.APoint.Y - item.APoint.Y > -1 * minLineHeight && x.APoint.Y - item.APoint.Y < minLineHeight)
                    .OrderBy(x => x.APoint.X);

                if (nearbyItems.Count() > 1)
                {

                    var nearbyItem = nearbyItems.ToList()[1];
                    if (nearbyItem != null)
                    {
                        if (item.APoint.DistanceTo(nearbyItem.APoint) > minWidth)
                        {
                            result.Add(new Shape(item.APoint, nearbyItem.APoint, item.BPoint, nearbyItem.BPoint));
                            lines.Remove(nearbyItem);
                            lines.Remove(item);
                        }
                        else
                        {
                            lines.Remove(nearbyItem);
                            var st = item.APoint.DistanceTo(nearbyItem.APoint);
                        }
                    }
                }
                else

                {
                    lines.Remove(item);
                }

            }
            return result;
        }

        private List<Shape> IsChecked(List<Shape> row)
        {
            foreach (var item in row)
            {

                Rectangle newRectangle = new Rectangle(item.TopLeftPoint.X, item.TopLeftPoint.Y, (int)item.TopLeftPoint.DistanceTo(item.TopRightPoint), (int)item.TopLeftPoint.DistanceTo(item.BottomLeftPoint));

                double totalPixels = 0;
                double coloredPixels = 0;
                for (int x = newRectangle.X + 10; x < newRectangle.Right - 10; x++)
                {
                    for (int y = newRectangle.Y + 10; y < newRectangle.Bottom - 10; y++)
                    {
                        totalPixels++;
                        var pixel = _bitmap.GetPixel(x, y);
                        if (pixel.R < 100 && pixel.G < 100 && pixel.B > 60)
                        {
                            coloredPixels++;
                        }
                    }
                }
                double result = (coloredPixels / totalPixels) * 100;
                if (result > 2 && result < 15)
                {
                    item.IsChecked = true;
                }
                else
                {
                    item.IsChecked = false;
                }
            }
            return row;
        }

        private Bitmap CropBarcode(Rectangle rectangle)
        {
            var isFirstLineSet = false;
            var toBreak = false;
            var firstHozilonalLine = new Line();
            var lastHorizontalLine = new Line();
            var staringPoint = new IntPoint(0, 0);
            for (int y = rectangle.Y; y < rectangle.Bottom; y++)
            {
                for (int x = rectangle.X; x < rectangle.Right; x++)
                {
                    var pixel = _lockBitmap.GetPixel(x, y);

                  
                    if (pixel.B < 90 && pixel.G < 90 && pixel.R < 90)
                    {
                        if(staringPoint.X == 0 && staringPoint.Y == 0)
                        {
                            staringPoint.X = x;
                            staringPoint.Y = y;
                        }
                    }
                    else
                    {
                        if (staringPoint.X != 0 || staringPoint.Y != 0)
                        {
                            if (x - staringPoint.X > 50)
                            {
                                if (!isFirstLineSet)
                                {
                                    firstHozilonalLine.APoint = staringPoint;
                                    firstHozilonalLine.BPoint = new IntPoint(x, y);
                                    isFirstLineSet = true;
                                    staringPoint.X = 0;
                                    staringPoint.Y = 0;
                                }
                                else
                                {

                                    lastHorizontalLine.APoint = staringPoint;
                                    lastHorizontalLine.BPoint = new IntPoint(x, y);
                                    staringPoint.X = 0;
                                    staringPoint.Y = 0;
                                }
                            }
                        }

                        if (lastHorizontalLine.APoint.Y != 0 && y - lastHorizontalLine.APoint.Y > 150)
                        {
                            toBreak = true;
                            break;
                        }
                    }
                }
                if (toBreak) break;
            }
            if (firstHozilonalLine.APoint.X != 0 && firstHozilonalLine.APoint.Y != 0 && lastHorizontalLine.APoint.X != 0 && lastHorizontalLine.BPoint.Y != 0)
            {

                var croppedRect = new Rectangle(firstHozilonalLine.APoint.X-20, firstHozilonalLine.APoint.Y-20, firstHozilonalLine.BPoint.X - firstHozilonalLine.APoint.X+40, lastHorizontalLine.BPoint.Y - firstHozilonalLine.APoint.Y+40);
                var croppedBitmap = _bitmap.Clone(croppedRect, _bitmap.PixelFormat);
                return croppedBitmap;
            }
            return null;
        }
    }
}
