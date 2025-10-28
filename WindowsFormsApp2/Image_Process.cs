using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Basler.Pylon;
using System.Drawing.Imaging;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Globalization;
using static System.Windows.Forms.MonthCalendar;
using static System.Net.Mime.MediaTypeNames;
using OpenCvSharp.Dnn;
using System.IO;
using static WindowsFormsApp2.Image_Process;
using System.Reflection;
using System.Text.RegularExpressions;


namespace WindowsFormsApp2
{
    class Image_Process
    {
        /*
        private const double XRES = -101.55246; // 예시 값, 실제 값으로 대체 필요
        private const double Global.yPixelRate = 101.69609; // 예시 값, 실제 값으로 대체 필요
        private const double Robot_origin_X = 399591.0; // 예시 값, 실제 값으로 대체 필요
        private const double Robot_origin_Y = 210182.0; // 예시 값, 실제 값으로 대체 필요
        private const double Global.x_offset = 400.0; // 예시 값, 실제 값으로 대체 필요
        private const double Global.y_offset = 000.0; // 예시 값, 실제 값으로 대체 필요
        private const double Vision_origin_X = 798.0; // 예시 값, 실제 값으로 대체 필요
        private const double Vision_origin_Y = 239.0; // 예시 값, 실제 값으로 대체 필요
        */

        //중심점(550,550) 기준 robot 값
        //x ; 26.25
        //y : 460.964
        //1pixel = 0.132mm

        //private const double Global.x_offset = 98.85;
        //private const double Global.x_offset = 102.85;
        //private const double Global.y_offset = 388.364;
        //private const double Global.y_offset = 390.364;


        public class Product
        {
            public string Info { get; set; }
            public string X { get; set; }
            public string Y { get; set; }
            public double Deg { get; set; }
            public bool Picking { get; set; }
            public Rect BoundingRect { get; set; }
            public Point2f Center { get; set; }
            public Point2f C1 { get; set; }
            public Point2f C2 { get; set; }
            public Point2f AnglePoint { get; set; }
            public OpenCvSharp.Point[] Contour { get; set; }

            public Product(string info, string x, string y, double deg, bool picking, Rect boundingRect, Point2f center, Point2f c1, Point2f c2, Point2f anglePoint, OpenCvSharp.Point[] contour)
            {
                Info = info;
                X = x;
                Y = y;
                Deg = deg;
                Picking = picking;
                BoundingRect = boundingRect;
                Center = center;
                C1 = c1;
                C2 = c2;
                AnglePoint = anglePoint;
                Contour = contour;

            }
        }

        public List<Product> productInfoList = new List<Product>();

        //제품 정보 리스트 추가
        public void AddProductInfo(string info, string x, string y, double deg, bool picking, Rect boundingRect, Point2f center, Point2f c1, Point2f c2, Point2f anglePoint, OpenCvSharp.Point[] contour)
        {
            Product newProduct = new Product(info, x, y, deg, picking, boundingRect, center, c1, c2, anglePoint, contour);
            productInfoList.Add(newProduct);
        }

        public void AddTempProductInfo(bool oddyn)
        {
            productInfoList.Clear();
            if (oddyn)
            {
                Product newProduct1 = new Product("I", "12.123", "132.123", 12.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                Product newProduct1_1 = new Product("I", "22.123", "232.123", 22.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                Product newProduct2 = new Product("A", "32.123", "332.123", 32.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                Product newProduct2_1 = new Product("A", "42.123", "432.123", 42.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                Product newProduct3 = new Product("PIN", "52.123", "532.123", 52.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                Product newProduct3_1 = new Product("PIN", "62.123", "632.123", 62.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                productInfoList.Add(newProduct1);
                productInfoList.Add(newProduct1_1);
                productInfoList.Add(newProduct2);
                productInfoList.Add(newProduct2_1);
                productInfoList.Add(newProduct3);
                productInfoList.Add(newProduct3_1);

            }
            else
            {
                Product newProduct1 = new Product("I", "112.123", "1232.123", 122.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                Product newProduct1_1 = new Product("A", "122.123", "2232.123", 212.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                Product newProduct2 = new Product("A", "132.123", "3332.123", 321.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                Product newProduct2_1 = new Product("NG", "142.123", "4432.123", 412.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                Product newProduct3 = new Product("PIN", "152.123", "5532.123", 512.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                Product newProduct3_1 = new Product("NG", "162.123", "6632.123", 162.123, false, new Rect(), new Point2f(), new Point2f(), new Point2f(), new Point2f(), new OpenCvSharp.Point[1]);
                productInfoList.Add(newProduct1);
                productInfoList.Add(newProduct1_1);
                productInfoList.Add(newProduct2);
                productInfoList.Add(newProduct2_1);
                productInfoList.Add(newProduct3);
                productInfoList.Add(newProduct3_1);
            }
        }


        //제품 정보로 로봇 전송 메세지 생성
        /*
            bool varI : I 제품 인경우 True
            bool varA : A 제품 인경우 True
            bool isDetectedI : I 제품 검출 여부
            bool isDetectedA : A 제품 검출 여부
         */

        public string CreateMessageFromPickedProducts()
        {
            /*Console.WriteLine("Current productInfoList:");
            foreach (var product in productInfoList)
            {
                Console.WriteLine($"Info: {product.Info}, X: {product.X}, Y: {product.Y}, Deg: {product.Deg}, Picking: {product.Picking}");
            }*/
            var infoOrder = new[] { "I", "A", "PIN" };


            var sortedProducts = productInfoList
                .GroupBy(p => p.Info)
                .OrderBy(g => Array.IndexOf(infoOrder, g.Key))
                .SelectMany(g => g.OrderBy(p => double.Parse(p.Y)).ThenBy(p => double.Parse(p.X)))
                .ToList();

            /*Console.WriteLine("Current productInfoList:");
            foreach (var product in sortedProducts)
            {
                Console.WriteLine($"Info: {product.Info}, X: {product.X}, Y: {product.Y}, Deg: {product.Deg}, Picking: {product.Picking}");
            }*/

            // 1) Picking이 true인 데이터 2개 추출
            var pickedProducts = sortedProducts
                   .Where(p => p.Picking)
                   //.OrderBy(p => productInfoList.IndexOf(p))
                   .Take(2)
                   .ToList();

            // 2개 미만이면 에러 처리
            if (pickedProducts.Count < 2)
            {
                //throw new InvalidOperationException("Not enough picked products. At least 2 are required.");
                Console.WriteLine("Not enough picked products. At least 2 are required.");
                return "";
            }
            

            // Info를 숫자로 변환하는 함수
            int ConvertInfoToNumber(string info)
            {
                //불량 결과가 있을 경우 -1 return;
                if (!Global.detectLeft || !Global.detectRight)
                {
                    //return -1;
                    return 4;
                }

                switch (info.ToUpper())
                {
                    case "I": return 1;
                    case "A": return 2;
                    case "PIN": return 3;
                    case "NG": return 4;
                    default: return 4;
                }
            }

            // 2) 메시지 포맷 생성
            var product1 = pickedProducts[0];
            var product2 = pickedProducts[1];

            //Non Pick 은 전달하지 않는다.
            if ("Non Pick".Equals(product1))
            {
                return "";
            }

            string messageFormat = $",{ConvertInfoToNumber(product1.Info)}," +
                                   $"{product1.X},{product1.Y},{product1.Deg}," +
                                   $"{product2.X},{product2.Y},{product2.Deg},\n";

            return messageFormat;
        }

        public class ProductCount
        {
            public string Type { get; set; }
            public int Count { get; set; }
        }

        public class ProductManager
        {
            public List<ProductCount> ProductCounts { get; private set; }

            public ProductManager()
            {
                ProductCounts = new List<ProductCount>
                {
                    new ProductCount { Type = "I", Count = 0 },
                    new ProductCount { Type = "A", Count = 0 },
                    new ProductCount { Type = "PIN", Count = 0 },
                    new ProductCount { Type = "NG", Count = 0 }
                };
            }
            public void TempIncrementCounts(bool oddyn)
            {
                if (oddyn)
                {
                    ProductCounts[0].Count += 2;
                    ProductCounts[1].Count += 2;
                }
                else
                {
                    ProductCounts[2].Count += 2;
                    ProductCounts[3].Count += 2;
                }


            }

            public void IncrementCounts(string productName)
            {

                //불량 확인
                if (!Global.detectLeft || !Global.detectRight)
                {
                    productName = "NG";
                }


                foreach (var product in ProductCounts)
                {
                    if (product.Type.Equals(productName))
                    {
                        Console.WriteLine($"Type : {product.Type} count up => count : {product.Count + 2}");
                        product.Count += 2;
                    }
                }
            }
        }


        //각도 계산
        //Point2f p1 : 윗 작은변 중점
        // Point2f p2 :  아래 작은변 중점
        public static double CalculateAngle(Point2f p1, Point2f p2)
        {
            double deltaX = p2.X - p1.X;
            double deltaY = p2.Y - p1.Y;
            double angle = Math.Atan2(deltaY, deltaX) * (180 / Math.PI);
            return Math.Round(angle, 3);
        }

        //길이 계산
        //Point2f p1 : 윗 작은변 중점
        // Point2f p2 :  아래 작은변 중점
        public static int CalculateLineLength(Point2f p1, Point2f p2)
        {
            return (int)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        //최소 영역 검출
        public static (Mat, RotatedRect[], LineSegmentPoint[]) FindMinAreaRect(Mat image)
        {
            // 라벨링을 위한 3ch 이미지 변환
            Mat resultImg = new Mat();
            Cv2.CvtColor(image, resultImg, ColorConversionCodes.GRAY2BGR);

            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(image, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            // 최소 사각형 정보 리스트
            List<RotatedRect> rectInfo = new List<RotatedRect>();
            // 사각형 내 두 중점들의 포인트와 직선 길이 정보 리스트
            List<LineSegmentPoint> lineInfo = new List<LineSegmentPoint>();

            if (contours.Length > 0)
            {
                // 가장 큰 면적을 가진 컨투어 찾기
                int largestContourIndex = 0;
                double largestArea = 0;
                for (int i = 0; i < contours.Length; i++)
                {
                    double area = Cv2.ContourArea(contours[i]);
                    if (area > largestArea)
                    {
                        largestArea = area;
                        largestContourIndex = i;
                    }
                }

                var contour = contours[largestContourIndex];

                // 최소 사각형 검출
                RotatedRect rect = Cv2.MinAreaRect(contour);
                // 사각형 4 꼭지점 검출
                Point2f[] box = Cv2.BoxPoints(rect);
                OpenCvSharp.Point[] intBox = Array.ConvertAll(box, p => new OpenCvSharp.Point((int)p.X, (int)p.Y));

                // 빨간색으로 contour 그리기
                //Cv2.DrawContours(resultImg, new OpenCvSharp.Point[][] { intBox }, 0, new Scalar(0, 0, 255), 2);
                rectInfo.Add(rect);

                // 4변 길이 리스트
                var edges = new List<(Point2f, Point2f, double)>();
                for (int i = 0; i < 4; i++)
                {
                    Point2f p1 = box[i];
                    Point2f p2 = box[(i + 1) % 4];
                    // 각 변 길이 검출
                    double edgeLength = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                    edges.Add((p1, p2, edgeLength));
                }
                edges = edges.OrderBy(e => e.Item3).ToList();

                // 짧은 변 1
                var shortEdge1 = edges[0];
                // 짧은 변 2
                var shortEdge2 = edges[1];
                // 짧은 변 1 중점 계산

                /*
                double lengthDist = shortEdge1.Item3 - shortEdge2.Item3;
                Console.WriteLine("shortEdge1.Item3 : " + shortEdge1.Item3 + " shortEdge2.Item3 : " + shortEdge2.Item3);
                Console.WriteLine("shortEdge1.edgeLength -  shortEdge2. edgeLength: " + lengthDist);
                */
                Point2f center1 = new Point2f((shortEdge1.Item1.X + shortEdge1.Item2.X) / 2, (shortEdge1.Item1.Y + shortEdge1.Item2.Y) / 2);
                // 짧은 변 2 중점 계산
                Point2f center2 = new Point2f((shortEdge2.Item1.X + shortEdge2.Item2.X) / 2, (shortEdge2.Item1.Y + shortEdge2.Item2.Y) / 2);

                // 중점끼리 직선 생성-초록색
                //Cv2.Line(resultImg, new OpenCvSharp.Point((int)center1.X, (int)center1.Y), new OpenCvSharp.Point((int)center2.X, (int)center2.Y), new Scalar(0, 255, 0), 2);

                // LineSegmentPoint 생성 시 두 개의 Point를 인자로 전달
                lineInfo.Add(new LineSegmentPoint(
                    new OpenCvSharp.Point((int)center1.X, (int)center1.Y),
                    new OpenCvSharp.Point((int)center2.X, (int)center2.Y)
                ));
            }

            // 3ch 이미지와 사각형 리스트, 직선 길이 리스트 전달
            return (resultImg, rectInfo.ToArray(), lineInfo.ToArray());
        }



        public Mat FindContours_Image(Mat originalImage, Mat thresholdImage, out OpenCvSharp.Point[][] result_contours)
        {
            Mat temp_mat = new Mat();
            Mat contoursImage = new Mat();

            // 컨투어 찾기
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;

            if (thresholdImage.Channels() != 1)
            {
                Cv2.CvtColor(thresholdImage, thresholdImage, ColorConversionCodes.BGR2GRAY);
            }

            /*Mat invertedImage = new Mat();
            Cv2.BitwiseNot(thresholdImage, invertedImage);*/

            Cv2.FindContours(
                image: thresholdImage,
                contours: out contours,
                hierarchy: out hierarchy,
                mode: RetrievalModes.External,
                method: ContourApproximationModes.ApproxSimple
            );

            result_contours = contours;

            // 원본 이미지 복사 (컨투어를 그리기 위해)
            if (originalImage.Channels() == 1)
            {
                Cv2.CvtColor(originalImage, contoursImage, ColorConversionCodes.GRAY2BGR);
            }
            else
            {
                contoursImage = originalImage.Clone();
            }

            // 지정된 영역 내의 컨투어 카운트
            int validContours = 0;
            foreach (var contour in contours)
            {
                // 컨투어의 중심점 계산
                Moments moments = Cv2.Moments(contour);
                int centerX = (int)(moments.M10 / moments.M00);
                int centerY = (int)(moments.M01 / moments.M00);

                // 중심점이 지정된 영역 내에 있는지 확인
                if (centerX >= Global.topLeft.X && centerX <= Global.bottomRight.X &&
                    centerY >= Global.topLeft.Y && centerY <= Global.bottomRight.Y)
                {
                    validContours++;

                }
            }


            Console.WriteLine($"찾은 컨투어의 수: {contours.Length}");
            Global.contourCount = contours.Length;
            return contoursImage;

        }


        public Mat ProcessDistanceTransform(Mat thresholdImage, int threshold2Value)
        {
            Console.WriteLine("ProcessDistanceTransform start");
            if (thresholdImage.Channels() != 1)
            {
                Cv2.CvtColor(thresholdImage, thresholdImage, ColorConversionCodes.BGR2GRAY);
            }

            /*Mat invertedImage = new Mat();
            Cv2.BitwiseNot(thresholdImage, invertedImage);*/

            //Cv2.ImShow("invertedImage", invertedImage);
            // 거리 변환 적용
            Mat distTransform = new Mat();
            //Cv2.DistanceTransform(invertedImage, distTransform, DistanceTypes.L2, DistanceTransformMasks.Mask3);
            Cv2.DistanceTransform(thresholdImage, distTransform, DistanceTypes.L2, DistanceTransformMasks.Mask3);

            // 거리 변환 결과를 시각화
            Mat distTransformVisual = new Mat();
            Cv2.Normalize(distTransform, distTransformVisual, 0, 255, NormTypes.MinMax);
            distTransformVisual.ConvertTo(distTransformVisual, MatType.CV_8U);

            //Cv2.ImShow("dist",distTransformVisual);
            // 중간값 블러 적용
            Mat median = new Mat();
            Cv2.MedianBlur(distTransformVisual, median, 3);

            // 2차 임계값 처리
            Mat thresh2 = new Mat();
            Cv2.Threshold(median, thresh2, threshold2Value, 255, ThresholdTypes.Binary);

            // 결과 이미지 생성 (그레이스케일을 BGR로 변환)
            //Mat resultImg = new Mat();
            //Cv2.CvtColor(median, resultImg, ColorConversionCodes.GRAY2BGR);

            // 원본 이미지에 결과 오버레이 (선택사항)
            // resultImg.SetTo(new Scalar(0, 0, 255), thresh2);

            //return resultImg;
            Console.WriteLine("ProcessDistanceTransform end");
            return median;
        }


        //이미지 crop
        public Mat CropImage(Mat image, int x, int y, int w, int h)
        {
            return new Mat(image, new Rect(x, y, w, h));
        }



        public Mat ProcessContours(Mat resultImg, Mat distanceImage, Mat thresholdImage, OpenCvSharp.Point[][] contours,
                              double min_area, double max_area)
        {
            Console.WriteLine("ProcessContours Start");
            int sizeCircle = 5;
            double sizeText = 0.7;
            int sizeLine = 3;
            int sizeThickness = 2;
            int sizeYPosition = 60;
            double calibrationArea = 0.0;
            foreach (var contour in contours)
            {
                double area = Cv2.ContourArea(contour);
                //Console.WriteLine($"All area Value: {area}");
                //Console.WriteLine("contours foreach Start: " + area);
                if (min_area < area && area < max_area)
                {
                    Rect boundingRect = Cv2.BoundingRect(contour);
                    calibrationArea = AreaCorrector.CorrectArea(boundingRect.X, boundingRect.Y, area);
                    Console.WriteLine($"detected=> X : {boundingRect.X} Y {boundingRect.Y} area Value: {area}" +
                        $" , calibrate value {calibrationArea}");

                    /*
                    Cv2.Rectangle(resultImg, boundingRect, new Scalar(0, 255, 0), 3);
                    */

                    Mat croppedImage = CropImage(distanceImage, boundingRect.X, boundingRect.Y, boundingRect.Width, boundingRect.Height);

                    // 크롭된 이미지 표시
                    //Cv2.ImShow("Cropped Image", croppedImage);
                    //Console.WriteLine("Press any key to continue...");
                    //Cv2.WaitKey(0);
                    //Cv2.DestroyWindow("Cropped Image");

                    var (minAreaRectResult, rectangles, lines) = FindMinAreaRect(croppedImage);

                    // 크롭된 이미지 표시
                    //Cv2.ImShow("minAreaRectResult Image", minAreaRectResult);
                    //Console.WriteLine("Press any key to continue...");
                    //Cv2.WaitKey(0);
                    //Cv2.DestroyWindow("minAreaRectResult Image");



                    if (lines.Length > 0)
                    {
                        //Console.WriteLine($"lines_List.Length : {lines.Length}");
                        var line = lines[0];
                        Point2f p1 = new Point2f(boundingRect.X + line.P1.X, boundingRect.Y + line.P1.Y);
                        Point2f p2 = new Point2f(boundingRect.X + line.P2.X, boundingRect.Y + line.P2.Y);

                        //Console.WriteLine("p1.X : " + p1.X + "  p2.X : " + p2.X);
                        //Console.WriteLine("p1.Y : " + p1.Y + "  p2.Y : " + p2.Y);


                        double c1 = (p1.X + p2.X) / 2.0;
                        double c2 = (p1.Y + p2.Y) / 2.0;

                        int lineLength = CalculateLineLength(p1, p2);
                        Console.WriteLine($"line_length : {lineLength}");

                        //2024.11.05
                        //좌상단 예외로직 추가
                        if (boundingRect.X < 200 && boundingRect.Y < 100)
                        {
                            lineLength = lineLength + 2;
                        }

                        //if (80 <= lineLength && lineLength <= 120) // i detected
                        //if (100 <= lineLength && lineLength <= 120) // i detected
                        if (110 <= lineLength && lineLength <= 115) // i detected
                        {
                            //var result = CompareEndPointsHSV(thresholdImage, p1, p2);
                            var result = CompareEndPointsBinary(thresholdImage, p1, p2, boundingRect, area, lineLength , calibrationArea);
                            bool AFlag = false;
                            bool reverseYN = false;
                            bool mirrorYN = false;
                            PrintHSVValues(result);
                            if (result.ProductCheckYN)
                            {
                                AFlag = result.IsSignificantDifference;
                                reverseYN = result.IsDarkerPointHigher;
                                mirrorYN = result.isBrighterPointRighter;
                                if (Global.detectI && !AFlag)
                                {
                                    ProcessDetectedI(resultImg, boundingRect, c1, c2, p1.X, p1.Y, p2.X, p2.Y,
                                                lineLength, contour, sizeText, sizeThickness, sizeYPosition,
                                                sizeCircle, sizeLine, AFlag, reverseYN, mirrorYN);
                                }
                                else if (Global.detectA && AFlag)
                                {
                                    //2024.11.05
                                    //A제품의 경우 boundingRect 확대
                                    //boundingRect.X = boundingRect.X - 5;
                                    //boundingRect.Y = boundingRect.Y - 5;
                                    //boundingRect.Width = boundingRect.Width + 10;
                                    //boundingRect.Height = boundingRect.Width + 10;

                                    ProcessDetectedI(resultImg, boundingRect, c1, c2, p1.X, p1.Y, p2.X, p2.Y,
                                                lineLength, contour, sizeText, sizeThickness, sizeYPosition,
                                                sizeCircle, sizeLine, AFlag, reverseYN, mirrorYN);
                                }
                                else
                                {
                                    //미검출 제품도 Labeling 수행
                                    if (!Global.detectI)
                                    {
                                        ProcessDetectedNG(resultImg, boundingRect, c1, c2, p1.X, p1.Y, p2.X, p2.Y,
                                            lineLength, contour, sizeText, sizeThickness, sizeYPosition,
                                            sizeCircle, sizeLine, false,true);
                                        Console.WriteLine("I Product not Detect target:  " + Global.detectI);
                                    }

                                    if (!Global.detectA)
                                    {
                                        ProcessDetectedNG(resultImg, boundingRect, c1, c2, p1.X, p1.Y, p2.X, p2.Y,
                                            lineLength, contour, sizeText, sizeThickness, sizeYPosition,
                                            sizeCircle, sizeLine, false, true);
                                        Console.WriteLine("A Product not Detect target: " + Global.detectA);
                                    }
                                }
                            }
                            //NG 표기
                            else
                            {
                                ProcessDetectedNG(resultImg, boundingRect, c1, c2, p1.X, p1.Y, p2.X, p2.Y,
                                            lineLength, contour, sizeText, sizeThickness, sizeYPosition,
                                            sizeCircle, sizeLine, true, true);
                            }


                        }
                        //else if (132 <= lineLength  && lineLength <= 137)
                        //else if (134 <= lineLength && lineLength <= 137)
                        else if (147 <= lineLength && lineLength <= 153)
                        {
                            Console.WriteLine("제품 PIN 인 케이스");

                            /*Cv2.PutText(resultImg, $"None:{lineLength}", new OpenCvSharp.Point(boundingRect.X, boundingRect.Y),
                                        HersheyFonts.HersheySimplex, sizeText, new Scalar(0, 255, 0),
                                        sizeThickness, LineTypes.AntiAlias);*/
                            if (Global.detectP)
                            {
                                bool ProductYN = checkThreshCropImage("PIN", thresholdImage, boundingRect, area, lineLength, p1, p2 , calibrationArea);
                                Console.WriteLine($"PIN Product Check result => {ProductYN}");
                                if (ProductYN)
                                {
                                    ProcessDetectedPIN(resultImg, boundingRect, c1, c2, p1.X, p1.Y, p2.X, p2.Y,
                                            lineLength, contour, sizeText, sizeThickness, sizeYPosition,
                                            sizeCircle, sizeLine);
                                }
                                //NG 제품 라벨링
                                else
                                {
                                    ProcessDetectedNG(resultImg, boundingRect, c1, c2, p1.X, p1.Y, p2.X, p2.Y,
                                            lineLength, contour, sizeText, sizeThickness, sizeYPosition,
                                            sizeCircle, sizeLine, true, true);
                                }
                            }
                            else
                            {
                                //미검출 제품도 Labeling 수행
                                ProcessDetectedNG(resultImg, boundingRect, c1, c2, p1.X, p1.Y, p2.X, p2.Y,
                                            lineLength, contour, sizeText, sizeThickness, sizeYPosition,
                                            sizeCircle, sizeLine, false, true);
                                Console.WriteLine("PIN Product not Detect target: " + Global.detectP);
                            }
                        }
                        //중심선이 PIN 보다 큰 Error -> none Pick
                        else if(lineLength > 153)
                        {
                            Console.WriteLine("error None Pick 케이스");

                            /*
                            bool ProductYN = checkThreshCropImage("I", thresholdImage, boundingRect);
                            Console.WriteLine($"I Product Check result => {ProductYN}");
                            ProductYN = checkThreshCropImage("A", thresholdImage, boundingRect);
                            Console.WriteLine($"A Product Check result => {ProductYN}");
                            ProductYN = checkThreshCropImage("PIN", thresholdImage, boundingRect);
                            Console.WriteLine($"PIN Product Check result => {ProductYN}");
                            */

                            Moments moments = Cv2.Moments(contour);

                            // 중심 좌표 계산
                            double centerX = moments.M10 / moments.M00;
                            double centerY = moments.M01 / moments.M00;

                            // 정수형으로 변환

                            ProcessDetectedNG(resultImg, boundingRect, centerX, centerY, p1.X, p1.Y, p2.X, p2.Y,
                                            lineLength, contour, sizeText, sizeThickness, sizeYPosition,
                                            sizeCircle, sizeLine, true ,false);
                        }
                        //중심선이 A보다 작은 Error -> Pick
                        else
                        {
                            Console.WriteLine("error Pick 케이스");


                            Moments moments = Cv2.Moments(contour);

                            // 중심 좌표 계산
                            double centerX = moments.M10 / moments.M00;
                            double centerY = moments.M01 / moments.M00;

                            // 정수형으로 변환

                            ProcessDetectedNG(resultImg, boundingRect, centerX, centerY, p1.X, p1.Y, p2.X, p2.Y,
                                            lineLength, contour, sizeText, sizeThickness, sizeYPosition,
                                            sizeCircle, sizeLine, true,true);
                        }
                    }
                    else
                    {
                        Console.WriteLine("line None detected");
                    }
                }
                //영역 이상의 제품(NG)
                else
                {
                    if (max_area < area && area < max_area * 2)
                    {
                        Console.WriteLine($"detected Error area Value: {area}");

                        Rect boundingRect = Cv2.BoundingRect(contour);
                        Cv2.Rectangle(resultImg, boundingRect, new Scalar(0, 0, 255), 3);
                        Cv2.PutText(resultImg, "None Product",
                                new OpenCvSharp.Point(boundingRect.X, boundingRect.Y - 10),
                                HersheyFonts.HersheySimplex, sizeText, new Scalar(0, 0, 255),
                                sizeThickness, LineTypes.AntiAlias);
                    }
                }
            }

            return resultImg;
        }


        public struct HSVResult
        {
            public Vec3b HSV1;
            public Vec3b HSV2;
            public double Difference;
            public bool IsSignificantDifference;
            public bool IsDarkerPointHigher; // 새로 추가된 필드
            public bool isBrighterPointRighter; // 새로 추가된 필드
            public bool ProductCheckYN; // 새로 추가된 필드
        }
        public HSVResult CompareEndPointsBinary(Mat image, Point2f point1, Point2f point2, Rect boundingRect
            , double area, int lineLength,double calibrationArea, double threshold = 50, int offsetPixels = 10)
        {
            // 두 점을 연결하는 벡터 계산
            Point2f vector = new Point2f(point2.X - point1.X, point2.Y - point1.Y);

            // 벡터의 길이 계산
            double length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);

            // 단위 벡터 계산
            Point2f unitVector = new Point2f((float)(vector.X / length), (float)(vector.Y / length));

            // 오프셋 적용
            Point2f offsetPoint1 = new Point2f(
                point1.X + unitVector.X * offsetPixels,
                point1.Y + unitVector.Y * offsetPixels
            );
            Point2f offsetPoint2 = new Point2f(
                point2.X - unitVector.X * offsetPixels,
                point2.Y - unitVector.Y * offsetPixels
            );

            // 이미지 경계 체크
            offsetPoint1.X = Math.Max(0, Math.Min(offsetPoint1.X, image.Cols - 1));
            offsetPoint1.Y = Math.Max(0, Math.Min(offsetPoint1.Y, image.Rows - 1));
            offsetPoint2.X = Math.Max(0, Math.Min(offsetPoint2.X, image.Cols - 1));
            offsetPoint2.Y = Math.Max(0, Math.Min(offsetPoint2.Y, image.Rows - 1));

            // 각 점의 픽셀 값 추출
            byte value1 = image.At<byte>((int)offsetPoint1.Y, (int)offsetPoint1.X);
            byte value2 = image.At<byte>((int)offsetPoint2.Y, (int)offsetPoint2.X);

            // 픽셀 값 차이 계산
            double difference = Math.Abs(value1 - value2);

            // 어두운 점과 밝은 점 식별
            bool isPoint1Darker = value1 < value2;
            bool isDarkerPointHigher = isPoint1Darker ? offsetPoint1.Y < offsetPoint2.Y : offsetPoint2.Y < offsetPoint1.Y;
            bool isBrighterPointRighter = isPoint1Darker ? offsetPoint2.X > offsetPoint1.X : offsetPoint1.X > offsetPoint2.X;


            bool ProductYN = false;
            //제품 비교
            //A
            if (difference > threshold)
            {
                ProductYN = checkThreshCropImage("A", image, boundingRect, area, lineLength, point1, point2 , calibrationArea);
                Console.WriteLine($"A Product Check result => {ProductYN}");
            }
            //I
            else
            {
                ProductYN = checkThreshCropImage("I", image, boundingRect, area, lineLength, point1, point2 , calibrationArea);
                Console.WriteLine($"I Product Check result => {ProductYN}");
            }




            return new HSVResult
            {
                HSV1 = new Vec3b(value1, value1, value1),
                HSV2 = new Vec3b(value2, value2, value2),
                Difference = difference,
                IsSignificantDifference = difference > threshold,
                IsDarkerPointHigher = isDarkerPointHigher,
                isBrighterPointRighter = isBrighterPointRighter,
                ProductCheckYN = ProductYN
            };
        }

        public bool checkThreshCropImage(string productName, Mat threshImage, Rect boundingRect, double area, int lineLength
            , Point2f point1, Point2f point2 , double calibrationArea)
        {
            bool check_Result = true;
            Mat checkThreshCropImage = CropImage(threshImage, boundingRect.X, boundingRect.Y, boundingRect.Width, boundingRect.Height);
            Mat checkImage;
            int IA_Line = 111;
            //int PIN_Line = 136;
            int PIN_Line = 150;
            int I_Area = 7300;
            int A_Area = 6200;
            int PIN_Area = 12300;

            if ("I".Equals(productName))
            {
                int dif_length = IA_Line - lineLength;
                double calcul_area = area + dif_length * 200.0;
                //double dif_area = Math.Abs(I_Area - calcul_area);
                double dif_area = Math.Abs(I_Area - calibrationArea);
                if (dif_area > 230)
                {
                    Console.WriteLine($"{productName} ,X : {boundingRect.X} ,Y : {boundingRect.Y} Product Area not Match!!! : {dif_area}");
                    Console.WriteLine($"area : {area} ,calcul_area : {calcul_area} ,dif_area : {dif_area}");
                    return false;
                }
                else
                {
                    Console.WriteLine($"{productName} ,X : {boundingRect.X} ,Y : {boundingRect.Y} Product Area Match!!! : {dif_area}");
                }

                checkImage = Cv2.ImRead("./thresh_image/I.jpg");
            }
            else if ("A".Equals(productName))
            {
                int dif_length = IA_Line - lineLength;
                double calcul_area = area + dif_length * 100.0;
                //double dif_area = Math.Abs(A_Area - calcul_area);
                double dif_area = Math.Abs(A_Area - calibrationArea);
                if (dif_area > 200)
                {
                    Console.WriteLine($"{productName} ,X : {boundingRect.X} ,Y : {boundingRect.Y} Product Area not Match!!! : {dif_area}");
                    Console.WriteLine($"area : {area} ,calcul_area : {calcul_area} ,dif_area : {dif_area}");
                    return false;
                }
                else
                {
                    Console.WriteLine($"{productName} ,X : {boundingRect.X} ,Y : {boundingRect.Y} Product Area Match!!! : {dif_area}");
                }

                checkImage = Cv2.ImRead("./thresh_image/A.jpg");
            }
            //PIN
            else
            {
                int dif_length = PIN_Line - lineLength;
                double calcul_area = area + dif_length * 200.0;
                //double dif_area = Math.Abs(PIN_Area - calcul_area);
                double dif_area = Math.Abs(PIN_Area - calibrationArea);
                if (dif_area > 350)
                {
                    Console.WriteLine($"{productName} ,X : {boundingRect.X} ,Y : {boundingRect.Y} Product Area not Match!!! : {dif_area}");
                    Console.WriteLine($"area : {area} ,calcul_area : {calcul_area} ,dif_area : {dif_area}");
                    return false;
                }
                else
                {
                    Console.WriteLine($"{productName} ,X : {boundingRect.X} ,Y : {boundingRect.Y} Product Area Match!!! : {dif_area}");
                }

                checkImage = Cv2.ImRead("./thresh_image/PIN.jpg");
            }
            if (checkImage.Channels() != 1)
            {
                Cv2.CvtColor(checkImage, checkImage, ColorConversionCodes.BGR2GRAY);
            }

            //이미지 회전 각 추출
            /*
            double angleDeg = CalculateAngle(point1, point2) * -1;
            Point2f center = new Point2f(checkImage.Width / 2f, checkImage.Height / 2f);

            // 이미지 회전
            //Cv2.WarpAffine(checkImage, rotatedImage, rotationMatrix, checkImage.Size());
            // 회전 각도를 라디안으로 변환
            double angleRadians = angleDeg * Math.PI / 180.0;

            // 회전 후 필요한 새 크기 계산
            double cos = Math.Abs(Math.Cos(angleRadians));
            double sin = Math.Abs(Math.Sin(angleRadians));

            int newWidth = (int)(checkImage.Width * cos + checkImage.Height * sin);
            int newHeight = (int)(checkImage.Width * sin + checkImage.Height * cos);

            // 새로운 중심점 계산
            Point2f newCenter = new Point2f(newWidth / 2f, newHeight / 2f);

            // 회전 행렬 생성
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(center, angleDeg, 1.0);

            // 평행이동 성분 조정
            rotationMatrix.At<double>(0, 2) += (newWidth - checkImage.Width) / 2;
            rotationMatrix.At<double>(1, 2) += (newHeight - checkImage.Height) / 2;

            // 새로운 크기로 이미지 회전
            Mat rotatedImage = new Mat();
            Cv2.WarpAffine(checkImage, rotatedImage, rotationMatrix, new OpenCvSharp.Size(newWidth, newHeight));

            // 두 이미지 크기 맞추기
            Cv2.Resize(checkThreshCropImage, checkThreshCropImage, rotatedImage.Size());

            // MatchShapes 수행
            double minMatchValue = Cv2.MatchShapes(rotatedImage, checkThreshCropImage, ShapeMatchModes.I2, 0);

            Cv2.ImWrite(productName + "_rotate.jpg", rotatedImage);
            Cv2.ImWrite(productName + "_rotate_tmp.jpg", checkThreshCropImage);

            Console.WriteLine($"{productName} rotate angleDeg : {angleDeg}");
            Console.WriteLine($"{productName} ,X : {boundingRect.X} ,Y : {boundingRect.Y} Product Match Result : {minMatchValue}");
            if (minMatchValue >= 0.004)
            {
                Console.WriteLine($"{productName} Product is Not Match!!!!!");
                check_Result = false;
            }
            */
            // 두 이미지 크기 맞추기
            //Cv2.Resize(checkThreshCropImage, checkThreshCropImage, checkImage.Size());

            //double matchValue = Cv2.MatchShapes(checkImage, checkThreshCropImage,  ShapeMatchModes.I2,  0);
            //2024.10.31
            //templete 매칭 잠시 보류
            /*
            double minMatchValue = double.MaxValue;
            int bestAngle = 0;
            int maxAngle = 180;
            if ("A".Equals(productName))
            {
                maxAngle = 359;
            }
            // 이미지 중심점 계산
            Point2f center = new Point2f(checkImage.Width / 2f, checkImage.Height / 2f);

            // 1도씩 180도까지 회전하면서 매칭
            for (int angle = 0; angle <= maxAngle; angle++)
            {
                // 회전 행렬 생성
                Mat rotationMatrix = Cv2.GetRotationMatrix2D(center, angle, 1.0);
                Mat rotatedImage = new Mat();

                // 이미지 회전
                Cv2.WarpAffine(checkImage, rotatedImage, rotationMatrix, checkImage.Size());

                // MatchShapes 수행
                double currentMatchValue = Cv2.MatchShapes(rotatedImage, checkThreshCropImage, ShapeMatchModes.I2, 0);
            
                // 최소값 업데이트
                if (currentMatchValue < minMatchValue)
                {
                    minMatchValue = currentMatchValue;
                    bestAngle = angle;
                }

                rotatedImage.Dispose();
                rotationMatrix.Dispose();
            }

            Console.WriteLine($"{productName} ,X : {boundingRect.X} ,Y : {boundingRect.Y} Product Match Result : {minMatchValue}");
            //if (minMatchValue >= 0.01)
            if (minMatchValue >= 0.007)
            {
                Console.WriteLine($"{productName} Product is Not Match!!!!!");
                check_Result = false;f
            }

            */
            return check_Result;
        }

        public void PrintHSVValues(HSVResult result)
        {
            Console.WriteLine($"Point 1 HSV: H={result.HSV1[0]}, S={result.HSV1[1]}, V={result.HSV1[2]}");
            Console.WriteLine($"Point 2 HSV: H={result.HSV2[0]}, S={result.HSV2[1]}, V={result.HSV2[2]}");
            Console.WriteLine($"Total Difference: {result.Difference}");
            Console.WriteLine($"Significant Difference: {result.IsSignificantDifference}");
            Console.WriteLine($"Is Darker Point Higher: {result.IsDarkerPointHigher}");
            Console.WriteLine($"Is Brighter Point Righterr: {result.isBrighterPointRighter}");
            Console.WriteLine($"Is ProductCheckYN: {result.ProductCheckYN}");
        }

        //각도 그리기에 필요한 좌표 검출
        public Point2f checkAnglePoint(double angleDeg, Point2f point1, Point2f point2)
        {
            if (angleDeg > 0)
            {
                if (point1.Y > point2.Y)
                {
                    return point2;
                }
                else
                {
                    return point1;
                }
            }
            else
            {
                if (point1.Y > point2.Y)
                {
                    return point1;
                }
                else
                {
                    return point2;
                }
            }
        }

        public void CalculrateRobotLimit(int limitSize)
        {

            //double robot_min_X = Math.Round((1100 - limitSize) * Global.xPixelRate + Global.x_offset, 0);
            double robot_min_X = Math.Round((900 - limitSize) * Global.xPixelRate + Global.x_offset, 0);
            double robot_max_X = Math.Round(limitSize * Global.xPixelRate + Global.x_offset, 0)  - 2;

            double robot_min_Y = Math.Round(limitSize * Global.yPixelRate + Global.y_offset, 0);
            //double robot_max_Y = Math.Round((1100 - limitSize) * Global.yPixelRate + Global.y_offset, 0);
            double robot_max_Y = Math.Round((1050 - limitSize) * Global.yPixelRate + Global.y_offset, 0);

            Global.robot_x_min = (int)robot_min_X;
            Global.robot_x_max = (int)robot_max_X;

            Global.robot_y_min = (int)robot_min_Y;
            Global.robot_y_max = (int)robot_max_Y;

            Console.WriteLine($"Global.robot_x_min : {Global.robot_x_min} , Global.robot_x_max : {Global.robot_x_max}  ");
            Console.WriteLine($"Global.robot_y_min : {Global.robot_y_min} , Global.robot_y_max : {Global.robot_y_max}  ");
        }

        private void ProcessDetectedI(Mat resultImg, Rect boundingRect, double c1, double c2,
                                      double x1, double y1, double x2, double y2, double lineLength, OpenCvSharp.Point[] contour,
                                      double sizeText, int sizeThickness, int sizeYPosition,
                                      int sizeCircle, int sizeLine, bool AFlag, bool reverseYN, bool mirrorYN)
        {

            string productName = "I";
            if (AFlag)
            {
                productName = "A";
            }

            Console.WriteLine("ProcessDetectedI Start");
            Console.WriteLine("productName :  " + productName);

            /*
            Cv2.PutText(resultImg, $"{productName}:{lineLength}{{{(int)c1},{(int)c2}}}",
                        new OpenCvSharp.Point(boundingRect.X, boundingRect.Y - 10),
                        HersheyFonts.HersheySimplex, sizeText, new Scalar(0, 255, 0),
                        sizeThickness, LineTypes.AntiAlias);
            */
            /*double angleDeg = CalculateAngle(new OpenCvSharp.Point(boundingRect.X + x2, boundingRect.Y + y2),
                                             new OpenCvSharp.Point(boundingRect.X + x1, boundingRect.Y + y1)) * -1;*/

            Point2f line_c1 = new OpenCvSharp.Point(x1, y1);
            Point2f line_c2 = new OpenCvSharp.Point(x2, y2);
            Point2f product_center = new OpenCvSharp.Point((int)c1, (int)c2);

            double angleDeg = CalculateAngle(line_c2, line_c1) * -1;

            Console.WriteLine($"origin  angle :{angleDeg}");
            if (angleDeg > 180)
            {
                angleDeg = (angleDeg - 360);
            }

            if (angleDeg < -180)
            {
                angleDeg = (angleDeg + 360);
            }


            if (AFlag)
            {
                Console.WriteLine($"product A turnning  angle :{angleDeg}");
                //A제품인데 제품이 우하향 하고 있을 경우 -180도 처리하여 각도 회전을 변경한다.
                if (reverseYN)
                {
                    if (angleDeg > 0)
                    {

                        angleDeg = (angleDeg - 180);
                    }

                }

                else
                {
                    if (angleDeg < 0)
                    {
                        angleDeg = (angleDeg + 180);
                    }
                }

                Console.WriteLine($"product A first turnning  angle :{angleDeg}");
            }
            //I 제품 ( -90 ~ 90 angle limit)
            else
            {
                Console.WriteLine($"product I turnning  angle :{angleDeg}");
                //A제품인데 제품이 우하향 하고 있을 경우 -180도 처리하여 각도 회전을 변경한다.
                if (angleDeg > 90)
                {
                    angleDeg = (angleDeg - 180);
                }
                else if (angleDeg < -90)
                {
                    angleDeg = (angleDeg + 180);
                }

                Console.WriteLine($"product I first turnning  angle :{angleDeg}");
            }

            //2024.10.16
            //I,A 제품은 - 90
            angleDeg = angleDeg - 90;

            if (AFlag)
            {
                Console.WriteLine($"product A second turnning  angle :{angleDeg}");
                if (angleDeg < -180)
                {
                    angleDeg = angleDeg + 360;
                }

                if (angleDeg > 180)
                {
                    angleDeg = angleDeg - 360;
                }

                //2024.10.18
                //제품이 土90도 일 경우 mirrorYN 확인
                if (Math.Abs(angleDeg) == 90)
                {
                    //제품이 정위치
                    if (mirrorYN)
                    {
                        angleDeg = -90;
                    }
                    //제품이 역위치
                    else
                    {
                        angleDeg = 90;
                    }
                    Console.WriteLine($"product A mirrorYN turnning  angle :{angleDeg} , mirrorYN : {mirrorYN}");
                }


            }
            else
            {
                Console.WriteLine($"product I second turnning  angle :{angleDeg}");
                if (angleDeg < -90)
                {
                    angleDeg = angleDeg + 180;
                }

                if (angleDeg > 90)
                {
                    angleDeg = angleDeg - 180;
                }
            }

            Console.WriteLine("CalculateAngle complete");

            angleDeg = Math.Round(angleDeg, 3);

            Console.WriteLine("CalculateAngle :" + angleDeg);

            Cv2.PutText(resultImg, $"{productName} deg:{angleDeg}",
                        //Cv2.PutText(resultImg, $"{productName}  ,X : {boundingRect.X} ,Y : {boundingRect.Y} deg:{angleDeg}",
                        //new OpenCvSharp.Point(boundingRect.X, boundingRect.Y - sizeYPosition),
                        //new OpenCvSharp.Point(boundingRect.X + 3 * sizeYPosition, boundingRect.Y - 10),
                        new OpenCvSharp.Point(boundingRect.X, boundingRect.Y - 10),
                        HersheyFonts.HersheySimplex, sizeText, new Scalar(255, 0, 0),
                        sizeThickness, LineTypes.AntiAlias);

            Cv2.Circle(resultImg, new OpenCvSharp.Point((int)c1, (int)c2), sizeCircle, new Scalar(255, 0, 255), -1);
            /*
            Cv2.Line(resultImg, new OpenCvSharp.Point(boundingRect.X + x1, boundingRect.Y + y1),
                     new OpenCvSharp.Point(boundingRect.X + x2, boundingRect.Y + y2),
                     new Scalar(255, 125, 25), sizeLine);
            */
            Cv2.Line(resultImg, new OpenCvSharp.Point(x1, y1), new OpenCvSharp.Point(x2, y2), new Scalar(255, 125, 25), sizeLine);


            //double robotX = Math.Round(((c1 - Vision_origin_X) * XRES + Robot_origin_X + Global.x_offset) / 1000, 3);
            //double robotY = Math.Round(((c2 - Vision_origin_Y) * Global.yPixelRate + Robot_origin_Y + Global.y_offset) / 1000, 3);
            //string formattedRobotX = robotX.ToString("F3").PadLeft(7, '0');
            //string formattedRobotY = robotY.ToString("F3").PadLeft(7, '0');
            Console.WriteLine($"C1:{c1}, c2:{c2}");
            Console.WriteLine($"xRES:{Global.xPixelRate},ROBOT_X :{Global.x_offset} , Global.yPixelRate:{Global.yPixelRate},ROBOT_Y :{Global.y_offset}");


            //double robotX = Math.Round(c1 * Global.xPixelRate + Global.x_offset, 3);
            //double robotY = Math.Round(c2 * Global.yPixelRate + Global.y_offset, 3);
            double robotX, robotY;
            (robotX, robotY) = calibrateRobotPosition(productName, new Point2f((float)c1, (float)c2), angleDeg);
            string formattedRobotX = robotX.ToString();
            string formattedRobotY = robotY.ToString();

            Console.WriteLine($"R:{robotX},{robotY}");
            Cv2.PutText(resultImg, $"R:{robotX},{robotY}",
                        new OpenCvSharp.Point(boundingRect.X, boundingRect.Y + sizeYPosition + 15),
                        HersheyFonts.HersheySimplex, sizeText, new Scalar(0, 255, 255),
                        sizeThickness, LineTypes.AntiAlias);

            Point2f check_angle_point = checkAnglePoint(angleDeg, line_c2, line_c1);

            AddProductInfo(productName, formattedRobotX, formattedRobotY, angleDeg, false, boundingRect, product_center, line_c1, line_c2, check_angle_point, contour);
        }


        private void ProcessDetectedPIN(Mat resultImg, Rect boundingRect, double c1, double c2,
                                      double x1, double y1, double x2, double y2, double lineLength, OpenCvSharp.Point[] contour,
                                      double sizeText, int sizeThickness, int sizeYPosition,
                                      int sizeCircle, int sizeLine)
        {

            Console.WriteLine("ProcessDetectedPIN Start");
           
            Point2f line_c1 = new OpenCvSharp.Point(x1, y1);
            Point2f line_c2 = new OpenCvSharp.Point(x2, y2);
            Point2f product_center = new OpenCvSharp.Point((int)c1, (int)c2);

            
            double angleDeg = CalculateAngle(line_c2, line_c1) * -1;
           
            Console.WriteLine("product C turnning  angle ");
            //A제품인데 제품이 우하향 하고 있을 경우 -180도 처리하여 각도 회전을 변경한다.
            if (angleDeg > 90)
            {
                angleDeg = (angleDeg - 180);
            }
            else if (angleDeg < -90)
            {
                angleDeg = (angleDeg + 180);
            }

            Console.WriteLine("CalculateAngle complete");

            angleDeg = Math.Round(angleDeg, 3);

            Console.WriteLine("CalculateAngle :" + angleDeg);

            Cv2.PutText(resultImg, $"PIN deg:{angleDeg}",
                        //Cv2.PutText(resultImg, $"PIN , X: {boundingRect.X} ,Y: {boundingRect.Y} deg:{angleDeg}",
                        //new OpenCvSharp.Point(boundingRect.X , boundingRect.Y - sizeYPosition),
                        //new OpenCvSharp.Point(boundingRect.X + 3 * sizeYPosition, boundingRect.Y - 10),
                        new OpenCvSharp.Point(boundingRect.X, boundingRect.Y - 10),
                        HersheyFonts.HersheySimplex, sizeText, new Scalar(255, 0, 0),
                        sizeThickness, LineTypes.AntiAlias);

            Cv2.Circle(resultImg, new OpenCvSharp.Point((int)c1, (int)c2), sizeCircle, new Scalar(255, 0, 255), -1);
      
            Console.WriteLine($"C1:{c1}, c2:{c2}");
            Console.WriteLine($"xRES:{Global.xPixelRate},ROBOT_X :{Global.x_offset} , Global.yPixelRate:{Global.yPixelRate},ROBOT_Y :{Global.y_offset}");

            //2024.11.1
            //각도 보정 로직 추가
            // 각도에 따른 x값 보정
            /*
            double angleCorrection = 0;
            double yCorrection = 0;
            if (Math.Abs(angleDeg) > 30 && Math.Abs(angleDeg) < 90)  // 45도 이상일 때부터 보정 시작
            {
                // 90도에 가까워질수록 보정값 증가
                double correctionFactor = Math.Abs(angleDeg) / 90.0;  // 0.5 ~ 1.0


                //double maxCorrection = 17.8;  // 최대 보정값
                //double maxYCorrection = 14.6;   // y축 최대 보정값 (더 크게 설정)
                double maxCorrection = 14.8;  // 최대 보정값
                double maxYCorrection = 11.6;   // y축 최대 보정값 (더 크게 설정)


                if (angleDeg > 0)  // 양수 각도(90도 방향)
                {
                    angleCorrection = maxCorrection * correctionFactor;  // x값을 줄임
                    yCorrection = -maxYCorrection * correctionFactor;     // y축 보정 추가
                }
                else  // 음수 각도(-90도 방향)
                {
                    angleCorrection = -maxCorrection * correctionFactor;  // x값을 늘림
                    yCorrection = -maxYCorrection * correctionFactor;     // y축 보정 추가
                }
            }

            */

            //double origin_robotX = Math.Round(c1 * Global.xPixelRate + Global.x_offset, 3);
            //double origin_robotY = Math.Round(c2 * Global.yPixelRate + Global.y_offset, 3);
            // 보정된 x값 계산
            //double robotX = Math.Round((c1 + angleCorrection) * Global.xPixelRate + Global.x_offset, 3);
            //double robotY = Math.Round((c2 + yCorrection) * Global.yPixelRate + Global.y_offset, 3);

            //Console.WriteLine($"origin_robotX : {origin_robotX} robotX : {robotX} diff : {origin_robotX - robotX} ");
            //Console.WriteLine($"origin_robotY : {origin_robotY} robotY : {robotY} diff : {origin_robotY - robotY} ");

            //double robotX = Math.Round(c1 * Global.xPixelRate + Global.x_offset, 3);
            //double robotY = Math.Round(c2 * Global.yPixelRate + Global.y_offset, 3);
            double robotX, robotY;
            (robotX, robotY) = calibrateRobotPosition("PIN", new Point2f((float)c1, (float)c2), angleDeg);
            string formattedRobotX = robotX.ToString();
            string formattedRobotY = robotY.ToString();

            Cv2.PutText(resultImg, $"R:{robotX},{robotY}",
                        new OpenCvSharp.Point(boundingRect.X, boundingRect.Y + sizeYPosition + 15),
                        HersheyFonts.HersheySimplex, sizeText, new Scalar(0, 255, 255),
                        sizeThickness, LineTypes.AntiAlias);

            //AddProductInfo("P", formattedRobotX, formattedRobotY, angleDeg);
            Point2f check_angle_point = checkAnglePoint(angleDeg, line_c2, line_c1);

            AddProductInfo("PIN", formattedRobotX, formattedRobotY, angleDeg, false, boundingRect, product_center, line_c1, line_c2, check_angle_point, contour);
        }

        private void ProcessDetectedNG(Mat resultImg, Rect boundingRect, double c1, double c2,
                                      double x1, double y1, double x2, double y2, double lineLength, OpenCvSharp.Point[] contour,
                                      double sizeText, int sizeThickness, int sizeYPosition,
                                      int sizeCircle, int sizeLine, bool NGyn , bool PickYN)
        {

            Console.WriteLine("ProcessDetectedNG Start");

            Point2f line_c1 = new OpenCvSharp.Point(x1, y1);
            Point2f line_c2 = new OpenCvSharp.Point(x2, y2);
            Point2f product_center = new OpenCvSharp.Point((int)c1, (int)c2);
            double angleDeg = CalculateAngle(line_c2, line_c1) * -1;

            string labelName = "";
            if (NGyn)
            {
                labelName = "NG";
            }
            else
            {
                labelName = "Non Pick";
            }

            if (!PickYN)
            {
                labelName = "Non Pick";
            }
            Console.WriteLine("product NG turnning  angle ");

            //각도 보정
            if (angleDeg > 90)
            {
                angleDeg = (angleDeg - 180);
            }
            else if (angleDeg < -90)
            {
                angleDeg = (angleDeg + 180);
            }

            Console.WriteLine("CalculateAngle complete");

            angleDeg = Math.Round(angleDeg, 3);

            Console.WriteLine("CalculateAngle :" + angleDeg);

            Cv2.PutText(resultImg, $"{labelName} deg:{angleDeg}",
            //Cv2.PutText(resultImg, $"{labelName} , X: {boundingRect.X} ,Y: {boundingRect.Y} deg:{angleDeg}",
                        new OpenCvSharp.Point(boundingRect.X, boundingRect.Y - 10),
                        HersheyFonts.HersheySimplex, sizeText, new Scalar(0, 0, 255),
                        sizeThickness, LineTypes.AntiAlias);

            Cv2.Circle(resultImg, new OpenCvSharp.Point((int)c1, (int)c2), sizeCircle, new Scalar(255, 0, 255), -1);

            Console.WriteLine($"C1:{c1}, c2:{c2}");
            Console.WriteLine($"xRES:{Global.xPixelRate},ROBOT_X :{Global.x_offset} , Global.yPixelRate:{Global.yPixelRate},ROBOT_Y :{Global.y_offset}");
            double robotX = Math.Round(c1 * Global.xPixelRate + Global.x_offset, 3);
            double robotY = Math.Round(c2 * Global.yPixelRate + Global.y_offset, 3);
            string formattedRobotX = robotX.ToString();
            string formattedRobotY = robotY.ToString();

            /*
            Cv2.PutText(resultImg, $"R:{robotX},{robotY}",
                        new OpenCvSharp.Point(boundingRect.X, boundingRect.Y + sizeYPosition + 15),
                        HersheyFonts.HersheySimplex, sizeText, new Scalar(0, 255, 255),
                        sizeThickness, LineTypes.AntiAlias);
            */

            Point2f check_angle_point = checkAnglePoint(angleDeg, line_c2, line_c1);
            AddProductInfo(labelName, formattedRobotX, formattedRobotY, angleDeg, false, boundingRect, product_center, line_c1, line_c2, check_angle_point, contour);
            
        }

        public string ProcessAndDisplayProducts(Mat origin_image, PictureBox pictureBox2, PictureBox pictureBox3)
        {
            //var infoOrder = new[] { "I", "A", "PIN" };
            var infoOrder = new[] { "I", "A", "PIN", "NG" };
            string resultProduct = "";

            //2024.10.17
            //X좌표가 -12 보다 낮은 값은 검출 하지 않는다.
            var sortedProducts = productInfoList
                    .GroupBy(p => p.Info)
                    .OrderBy(g => Array.IndexOf(infoOrder, g.Key))
                    .SelectMany(g => g.Where(p =>
                    {
                        double x = double.Parse(p.X);
                        double y = double.Parse(p.Y);
                        return x > Global.robot_x_min && x < Global.robot_x_max && y > Global.robot_y_min && y < Global.robot_y_max;
                    })
                    .OrderBy(p => double.Parse(p.Y))
                    .ThenBy(p => double.Parse(p.X)))
                    .ToList();

            /*
            Console.WriteLine("Sorted product list:");
            foreach (var product in sortedProducts)
            {
                Console.WriteLine($"Info: {product.Info}, X: {product.X}, Y: {product.Y}");
            }
            */

            var processedProducts = new List<Product>();

            foreach (var infoType in infoOrder)
            {
                var products = sortedProducts.Where(p => p.Info == infoType).ToList();
                if (products.Count >= 2)
                {
                    Console.WriteLine($"Processing Info type: {infoType}");
                    Console.WriteLine($"First product: Info: {products[0].Info}, X: {products[0].X}, Y: {products[0].Y}");
                    Console.WriteLine($"Second product: Info: {products[1].Info}, X: {products[1].X}, Y: {products[1].Y}");

                    products[0].Picking = true;
                    products[1].Picking = true;
                    processedProducts.Add(products[0]);
                    processedProducts.Add(products[1]);
                    break;
                }
                else
                {
                    Console.WriteLine($"Skipping Info type {infoType}: less than 2 products found.");
                }
            }

            if (processedProducts.Count == 2)
            {
                Console.WriteLine("Displaying cropped images:");
                resultProduct = processedProducts[0].Info;
                Console.WriteLine($"Product 1: Info: {processedProducts[0].Info}, X: {processedProducts[0].X}, Y: {processedProducts[0].Y}");
                Console.WriteLine($"Product 1: area: {Cv2.ContourArea(processedProducts[0].Contour)}");
                Console.WriteLine($"Product 2: Info: {processedProducts[1].Info}, X: {processedProducts[1].X}, Y: {processedProducts[1].Y}");
                Console.WriteLine($"Product 2: area: {Cv2.ContourArea(processedProducts[1].Contour)}");


                //제품 불량 검사
                DisplayCroppedImages(origin_image, processedProducts[0], processedProducts[1], pictureBox2, pictureBox3);
            }
            else
            {
                Console.WriteLine("Not enough products found to display.");
                Global.contourCount = 0;
            }

            // Update the original list with the modified Picking values
            foreach (var product in processedProducts)
            {
                var originalProduct = productInfoList.First(p => p.X == product.X && p.Y == product.Y && p.Info == product.Info);
                originalProduct.Picking = true;
            }

            //2024.10.18
            //Picking 객체 labeling 별도 표기


            if ("P".Equals(resultProduct))
            {
                resultProduct = "PIN";
            }
            return resultProduct;
        }

        public void ProductLabel(Mat result_image)
        {

            //2024.10.18
            //Picking 객체 labeling 별도 표기
            foreach (var product in productInfoList)
            {
                if (product.Picking)
                {
                    Cv2.Rectangle(result_image, product.BoundingRect, new Scalar(50, 255, 50), 3);
                }
                //NG or Non Pick
                else if ("NG".Equals(product.Info) || "Non Pick".Equals(product.Info))
                {
                    Cv2.Rectangle(result_image, product.BoundingRect, new Scalar(0, 0, 255), 3);
                }
                else
                {
                    Cv2.Rectangle(result_image, product.BoundingRect, new Scalar(255, 165, 50), 3);
                }

            }

            //이미지 전체 
            //result_image = DrawCenteredDottedRectangle(result_image,(float)(1100-Global.detectLimitSize));
            result_image = DrawCenteredDottedRectangle(result_image, (float)(Global.detectLimitSize));

            //return result_image;
        }


        private void DisplayCroppedImages(Mat origin_image, Product product1, Product product2, PictureBox pictureBox2, PictureBox pictureBox3)
        {
            var crop1 = new Mat(origin_image, product1.BoundingRect);
            var crop2 = new Mat(origin_image, product2.BoundingRect);
            SaveMatImageWithDateFolder(crop1, "_left_ori");
            SaveMatImageWithDateFolder(crop2, "_right_ori");
            double crop1_defectArea = 0;
            double crop2_defectArea = 0;

            //11.05
            //NG제품으로 들어왔으면 2개 모두 false 처리
            if ("NG".Equals(product1.Info))
            {
                Global.detectLeft = false;
                Global.detectRight = false;
            }

            //불량 검출 로직 수행(이미지 내 묻은 내역 확인)

            if (Global.defectCheckYN)
            {
                //Product 1
                //I , A 검사
                if ("I".Equals(product1.Info) || "A".Equals(product1.Info))
                {
                    (crop1, crop1_defectArea) = DetectDefectAreaIAndA(crop1, product1.Contour, product1.BoundingRect.X, product1.BoundingRect.Y);
                    Console.WriteLine($"crop1 defect Area : {crop1_defectArea}");
                    if (crop1_defectArea > Global.defectArea)
                    {
                        Console.WriteLine($"crop1 defect Area => Defect NG");
                        Global.detectLeft = false;
                    }

                    /* double area = Cv2.ContourArea(product1.Contour);
                     if ("I".Equals(product1.Info) && area < 5700) {
                         Console.WriteLine($"crop1 I Area Incorrect :{area} => Defect NG ");
                         Global.detectLeft = false;
                     }

                     if ("A".Equals(product1.Info) && area > 5900)
                     {
                         Console.WriteLine($"crop1 A Area Incorrect :{area} => Defect NG ");
                         Global.detectLeft = false;
                     }*/
                }
                //PIN 검사
                else
                {
                    bool circleErrorYN = true;
                    (crop1, crop1_defectArea, circleErrorYN) = DetectDefectAreaPIN(crop1, product1.Contour, product1.BoundingRect.X, product1.BoundingRect.Y);
                    Console.WriteLine($"crop1 defect Area : {crop1_defectArea}");
                    if (crop1_defectArea > Global.defectArea / 3)
                    {
                        Console.WriteLine($"crop1 defect Area => Defect NG");
                        Global.detectLeft = false;
                    }

                    if (!circleErrorYN)
                    {
                        Global.detectLeft = circleErrorYN;
                    }

                }

                //Product 2
                if ("I".Equals(product2.Info) || "A".Equals(product2.Info))
                {
                    (crop2, crop2_defectArea) = DetectDefectAreaIAndA(crop2, product2.Contour, product2.BoundingRect.X, product2.BoundingRect.Y);
                    Console.WriteLine($"crop2 defect Area : {crop2_defectArea}");
                    if (crop2_defectArea > Global.defectArea)
                    {
                        Console.WriteLine($"crop2 defect Area => Defect NG");
                        Global.detectRight = false;
                    }

                    /*double area = Cv2.ContourArea(product2.Contour);
                    if ("I".Equals(product2.Info) && area < 5700)
                    {
                        Console.WriteLine($"crop2 I Area Incorrect :{area} => Defect NG ");
                        Global.detectRight = false;
                    }

                    if ("A".Equals(product2.Info) && area > 5900)
                    {
                        Console.WriteLine($"crop2 A Area Incorrect :{area} => Defect NG ");
                        Global.detectRight = false;
                    }*/


                }
                //PIN 검사
                else
                {
                    bool circleErrorYN = true;

                    (crop2, crop2_defectArea, circleErrorYN) = DetectDefectAreaPIN(crop2, product2.Contour, product2.BoundingRect.X, product2.BoundingRect.Y);
                    Console.WriteLine($"crop2 defect Area : {crop2_defectArea}");
                    if (crop2_defectArea > Global.defectArea / 3)
                    {
                        Console.WriteLine($"crop2 defect Area => Defect NG");
                        Global.detectRight = false;
                    }

                    if (!circleErrorYN)
                    {
                        Global.detectRight = circleErrorYN;
                    }
                }

            }


            Cv2.Circle(crop1, (int)(product1.Center.X - product1.BoundingRect.X), (int)(product1.Center.Y - product1.BoundingRect.Y)
                , 6, new Scalar(255, 0, 255), -1);
            Cv2.Circle(crop2, (int)(product2.Center.X - product2.BoundingRect.X), (int)(product2.Center.Y - product2.BoundingRect.Y)
                , 6, new Scalar(255, 0, 255), -1);

            Cv2.PutText(crop1, $"deg:{product1.Deg}",
                        new OpenCvSharp.Point(1, 13),
                        HersheyFonts.HersheySimplex, 0.3, new Scalar(255, 0, 0),
                        1, LineTypes.AntiAlias);
            Cv2.PutText(crop2, $"deg:{product2.Deg}",
                        new OpenCvSharp.Point(1, 13),
                        HersheyFonts.HersheySimplex, 0.3, new Scalar(255, 0, 0),
                        1, LineTypes.AntiAlias);

            Cv2.Line(crop1, new OpenCvSharp.Point((int)(product1.C1.X - product1.BoundingRect.X)
                , (int)(product1.C1.Y - product1.BoundingRect.Y))
                , new OpenCvSharp.Point((int)(product1.C2.X - product1.BoundingRect.X)
                , (int)(product1.C2.Y - product1.BoundingRect.Y)), new Scalar(255, 125, 25), 3);
            Cv2.Line(crop2, new OpenCvSharp.Point((int)(product2.C1.X - product2.BoundingRect.X)
                , (int)(product2.C1.Y - product2.BoundingRect.Y))
                , new OpenCvSharp.Point((int)(product2.C2.X - product2.BoundingRect.X)
                , (int)(product2.C2.Y - product2.BoundingRect.Y)), new Scalar(255, 125, 25), 3);



            pictureBox2.Image = BitmapConverter.ToBitmap(crop1);
            pictureBox3.Image = BitmapConverter.ToBitmap(crop2);

            //NG,OK Label 표기
            //AddResultIndicatorToPictureBox(pictureBox2, crop1_defectYN);
            //AddResultIndicatorToPictureBox(pictureBox3, crop2_defectYN);



            //SaveMatImageWithDateFolder(crop1, "_left_pre");
            //SaveMatImageWithDateFolder(crop2, "_right_pre");

            Console.WriteLine($"Displayed in PictureBox2: Info: {product1.Info}, X: {product1.X}, Y: {product1.Y}");
            Console.WriteLine($"Displayed in PictureBox3: Info: {product2.Info}, X: {product2.X}, Y: {product2.Y}");
        }

        public static (Mat resultImage, double area) DetectDefectAreaIAndA(Mat inputImage, OpenCvSharp.Point[] contour, int X, int Y)
        {
            // contour 좌표 보정
            OpenCvSharp.Point[] adjustedContour = new OpenCvSharp.Point[contour.Length];
            for (int i = 0; i < contour.Length; i++)
            {
                adjustedContour[i] = new OpenCvSharp.Point(contour[i].X - X, contour[i].Y - Y);
            }

            // 그레이스케일 이미지로 변환
            Mat grayImage = new Mat();
            Cv2.CvtColor(inputImage, grayImage, ColorConversionCodes.BGR2GRAY);
            //Cv2.ImWrite("temp1.jpg", grayImage);

            // 이진화: 검은색 영역을 흰색으로, 나머지를 검은색으로 만듦
            Mat binaryImage = new Mat();
            Cv2.Threshold(grayImage, binaryImage, Global.defectbinaryThresh, 255, ThresholdTypes.Binary);

            // 이진화 이미지를 반전시켜 검은색 영역만 흰색으로 만듦
            Mat invertedBinary = new Mat();
            Cv2.BitwiseNot(binaryImage, invertedBinary);

            // 보정된 Contour 내부의 영역만 남기기 위해 마스크 생성
            //Mat contourMask = Mat.Zeros(inputImage.Size(), MatType.CV_8UC1);
            //Cv2.DrawContours(contourMask, new OpenCvSharp.Point[][] { adjustedContour }, 0, Scalar.White, -1);

            // Contour 내부의 검은색 영역만 남기기
            //Mat maskedInvertedBinary = new Mat();
            //Cv2.BitwiseAnd(invertedBinary, contourMask, maskedInvertedBinary);
            // 보정된 Contour 내부의 영역만 남기기 위해 마스크 생성
            Mat contourMask = Mat.Zeros(inputImage.Size(), MatType.CV_8UC1);
            Cv2.DrawContours(contourMask, new OpenCvSharp.Point[][] { adjustedContour }, 0, Scalar.White, -1);

            // contour 마스크를 안쪽으로 줄이기
            Mat erodedMask = new Mat();
            Mat erodeKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3)); // 3x3 커널로 1픽셀 침식
            //Mat erodeKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(5, 5)); // 3x3 커널로 1픽셀 침식
                                                                                                       // 더 많이 줄이려면 커널 크기를 키우거나 반복 횟수를 늘립니다
            int erosionIterations = 4; // 1~2픽셀 줄이기 위한 반복 횟수
            Cv2.Erode(contourMask, erodedMask, erodeKernel, iterations: erosionIterations);


            // 외곽 2픽셀을 제외하기 위한 마스크 생성
            Mat borderMask = Mat.Ones(inputImage.Size(), MatType.CV_8UC1);
            borderMask.Rectangle(new OpenCvSharp.Rect(0, 0, borderMask.Width, 2), Scalar.Black, -1); // 상단
            borderMask.Rectangle(new OpenCvSharp.Rect(0, borderMask.Height - 2, borderMask.Width, 2), Scalar.Black, -1); // 하단
            borderMask.Rectangle(new OpenCvSharp.Rect(0, 0, 2, borderMask.Height), Scalar.Black, -1); // 좌측
            borderMask.Rectangle(new OpenCvSharp.Rect(borderMask.Width - 2, 0, 2, borderMask.Height), Scalar.Black, -1); // 우측

            // 외곽을 제외한 마스크와 eroded 마스크를 결합
            Mat finalMask = new Mat();
            Cv2.BitwiseAnd(erodedMask, borderMask, finalMask);

            // Contour 내부의 검은색 영역만 남기기
            Mat maskedInvertedBinary = new Mat();
            //Cv2.BitwiseAnd(invertedBinary, erodedMask, maskedInvertedBinary);
            Cv2.BitwiseAnd(invertedBinary, finalMask    , maskedInvertedBinary);

            // 디버깅을 위한 중간 결과 출력


            // Contour 내부의 결함 영역의 면적 계산
            double defectArea = Cv2.CountNonZero(maskedInvertedBinary);

            // 결과 이미지 생성 (원본 이미지에 결함 영역 표시)
            Mat resultImage = inputImage.Clone();
            resultImage.SetTo(new Scalar(0, 0, 255), maskedInvertedBinary);  // 빨간색으로 표시

            Console.WriteLine($"Defect area: {defectArea} pixels");

            return (resultImage, defectArea);
        }

        /*  
        public static (Mat resultImage, double area) DetectDefectAreaIAndA(Mat inputImage, OpenCvSharp.Point[] contour, int X, int Y)
        {
            // contour 좌표 보정
            OpenCvSharp.Point[] adjustedContour = new OpenCvSharp.Point[contour.Length];
            for (int i = 0; i < contour.Length; i++)
            {
                adjustedContour[i] = new OpenCvSharp.Point(contour[i].X - X, contour[i].Y - Y);
            }

            // 그레이스케일 이미지로 변환
            Mat grayImage = new Mat();
            Cv2.CvtColor(inputImage, grayImage, ColorConversionCodes.BGR2GRAY);

            // 히스토그램 평활화 적용
            Mat equalizedImage = new Mat();
            Cv2.EqualizeHist(grayImage, equalizedImage);

            // CLAHE(Contrast Limited Adaptive Histogram Equalization) 적용
            var clahe = Cv2.CreateCLAHE(clipLimit: 2.0, tileGridSize: new OpenCvSharp.Size(8, 8));
            Mat claheImage = new Mat();
            clahe.Apply(grayImage, claheImage);

            // 감마 보정 적용
            Mat gammaImage = new Mat();
            byte[] gammatable = new byte[256];
            double gamma = 0.7; // 감마값 조정 (1보다 작으면 어두운 부분 강조)
            for (int i = 0; i < 256; ++i)
                gammatable[i] = (byte)(Math.Pow(i / 255.0, gamma) * 255.0);
            using (var lookUpTable = InputArray.Create(gammatable))
            {
                Cv2.LUT(claheImage, lookUpTable, gammaImage);
            }

            // 이진화 전에 가우시안 블러 적용 (노이즈 제거)
            Mat blurredImage = new Mat();
            Cv2.GaussianBlur(gammaImage, blurredImage, new OpenCvSharp.Size(3, 3), 0);

            // 적응형 이진화 적용
            Mat adaptiveThresh = new Mat();
            Cv2.AdaptiveThreshold(blurredImage, adaptiveThresh, 255,
                AdaptiveThresholdTypes.GaussianC,
                ThresholdTypes.Binary,
                5, // 블록 크기
                2  // C 상수
            );

            // 이진화 이미지를 반전
            Mat invertedBinary = new Mat();
            Cv2.BitwiseNot(adaptiveThresh, invertedBinary);

            // 보정된 Contour 내부의 영역만 남기기 위해 마스크 생성
            Mat contourMask = Mat.Zeros(inputImage.Size(), MatType.CV_8UC1);
            Cv2.DrawContours(contourMask, new OpenCvSharp.Point[][] { adjustedContour }, 0, Scalar.White, -1);

            // contour 마스크를 안쪽으로 줄이기
            Mat erodedMask = new Mat();
            Mat erodeKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3)); // 3x3 커널로 1픽셀 침식
                                                                                                       // 더 많이 줄이려면 커널 크기를 키우거나 반복 횟수를 늘립니다
            int erosionIterations = 1; // 1~2픽셀 줄이기 위한 반복 횟수
            Cv2.Erode(contourMask, erodedMask, erodeKernel, iterations: erosionIterations);

            // Contour 내부의 검은색 영역만 남기기
            Mat maskedInvertedBinary = new Mat();
            Cv2.BitwiseAnd(invertedBinary, erodedMask, maskedInvertedBinary);

            // 모폴로지 연산으로 노이즈 제거 및 결함 영역 강화
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
            Mat cleanedMask = new Mat();
            Cv2.MorphologyEx(maskedInvertedBinary, cleanedMask, MorphTypes.Open, kernel);
            Cv2.MorphologyEx(cleanedMask, cleanedMask, MorphTypes.Close, kernel);

            // Contour 내부의 결함 영역의 면적 계산
            double defectArea = Cv2.CountNonZero(cleanedMask);

            // 결과 이미지 생성 (원본 이미지에 결함 영역 표시)
            Mat resultImage = inputImage.Clone();
            resultImage.SetTo(new Scalar(0, 0, 255), cleanedMask);  // 빨간색으로 표시

            // 디버깅을 위해 중간 결과들 저장
            Cv2.ImWrite("1_gray.jpg", grayImage);
            Cv2.ImWrite("2_equalized.jpg", equalizedImage);
            Cv2.ImWrite("3_clahe.jpg", claheImage);
            Cv2.ImWrite("4_gamma.jpg", gammaImage);
            Cv2.ImWrite("5_adaptive_thresh.jpg", adaptiveThresh);
            Cv2.ImWrite("6_cleaned.jpg", cleanedMask);

            Console.WriteLine($"Defect area: {defectArea} pixels");
            return (resultImage, defectArea);
        }
        */

        public static (Mat resultImage, double area, bool circleErrorYN) DetectDefectAreaPIN
            (Mat inputImage, OpenCvSharp.Point[] contour, int X, int Y)
        {
            // contour 좌표 보정
            OpenCvSharp.Point[] adjustedContour = new OpenCvSharp.Point[contour.Length];
            for (int i = 0; i < contour.Length; i++)
            {
                adjustedContour[i] = new OpenCvSharp.Point(contour[i].X - X, contour[i].Y - Y);
            }
            // 그레이스케일 이미지로 변환
            Mat grayImage = new Mat();
            Cv2.CvtColor(inputImage, grayImage, ColorConversionCodes.BGR2GRAY);
            // 기존의 결함 검출 로직
            Mat binaryImage = new Mat();
            //Cv2.Threshold(grayImage, binaryImage, Global.defectbinaryThresh, 255, ThresholdTypes.Binary);
            Cv2.Threshold(grayImage, binaryImage, Global.defectbinaryThresh- 35, 255, ThresholdTypes.Binary);

            Mat invertedBinary = new Mat();
            Cv2.BitwiseNot(binaryImage, invertedBinary);
            Mat contourMask = Mat.Zeros(inputImage.Size(), MatType.CV_8UC1);
            Cv2.DrawContours(contourMask, new OpenCvSharp.Point[][] { adjustedContour }, 0, Scalar.White, -1);
            // contour 영역 내부로 원본 이미지 마스킹
            Mat maskedInvertedBinary = new Mat();
            Cv2.BitwiseAnd(invertedBinary, contourMask, maskedInvertedBinary);


            //원검출 바이너리 이미지는 별도 활용
            //112
            /*
            Mat circle_binaryImage = new Mat();
            Cv2.Threshold(grayImage, circle_binaryImage, Global.defectbinaryThresh - 23, 255, ThresholdTypes.Binary);

            Mat circle_invertedBinary = new Mat();
            Cv2.BitwiseNot(circle_binaryImage, circle_invertedBinary);
            Mat circle_contourMask = Mat.Zeros(inputImage.Size(), MatType.CV_8UC1);
            Cv2.DrawContours(circle_contourMask, new OpenCvSharp.Point[][] { adjustedContour }, 0, Scalar.White, -1);

            // contour 영역 내부로 원본 이미지 마스킹
            Mat circle_maskedInvertedBinary = new Mat();
            Cv2.BitwiseAnd(circle_invertedBinary, circle_contourMask, circle_maskedInvertedBinary);
            */

            Mat erodedMask = new Mat();
            Mat erodeKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
            int erosionIterations = 4;
            Cv2.Erode(contourMask, erodedMask, erodeKernel, iterations: erosionIterations);

            
            Mat resultImage = inputImage.Clone();

            // 원 검출 - maskedInvertedBinary를 사용하여 contour 내부에서만 검출
            double dp = 1;
            double minDist = 60;
            //double param1 = 15;
            //double param2 = 7.5;
            double param1 = 35;
            double param2 = 17;
            int minRadius = 10;
            int maxRadius = 15;

            CircleSegment[] circles = Cv2.HoughCircles(
                //circle_maskedInvertedBinary, // contour로 마스킹된 이미지 사용
                grayImage,
                HoughModes.Gradient,
                dp,
                minDist,
                param1,
                param2,
                minRadius,
                maxRadius
            );

            // 원 영역을 마스킹하기 위한 새로운 Mat 생성
            Mat circleMask = Mat.Ones(inputImage.Size(), MatType.CV_8UC1) * 255;

            if (circles != null)
            {
                // contour 내부의 원만 필터링
                List<CircleSegment> validCircles = new List<CircleSegment>();

                foreach (var circle in circles)
                {
                    // 원의 중심점이 contour 내부에 있는지 확인
                    OpenCvSharp.Point circleCenter = new OpenCvSharp.Point((int)circle.Center.X, (int)circle.Center.Y);
                    if (Cv2.PointPolygonTest(adjustedContour, circleCenter, false) >= 0)
                    {
                        validCircles.Add(circle);

                        // 검출된 유효한 원을 마스크에서 제외
                        Cv2.Circle(
                            circleMask,
                            (int)circle.Center.X,
                            (int)circle.Center.Y,
                            (int)(circle.Radius + 4),
                            new Scalar(0),
                            -1
                        );

                        // 결과 이미지에 원 표시
                        Cv2.Circle(
                            resultImage,
                            (int)circle.Center.X,
                            (int)circle.Center.Y,
                            (int)circle.Radius,
                            new Scalar(0, 255, 0),
                            2
                        );
                    }
                }

                circles = validCircles.ToArray(); // 유효한 원들로 업데이트
            }

            Console.WriteLine($"Defect circles.length: {(circles?.Length ?? 0)}");

            // 원 영역을 제외한 결함 영역 계산
            Mat defectMaskExcludingCircles = new Mat();
            Cv2.BitwiseAnd(maskedInvertedBinary, circleMask, defectMaskExcludingCircles);

            Cv2.ImWrite("contourMask.jpg", contourMask);
            Cv2.ImWrite("erodedMask.jpg", erodedMask);
            Cv2.ImWrite("defectMaskExcludingCircles.jpg", defectMaskExcludingCircles);


            // 원 영역을 제외한 결함 영역의 면적 계산
            double defectArea = Cv2.CountNonZero(defectMaskExcludingCircles);

            // 결과 이미지 업데이트 (원 영역을 제외한 결함만 표시)
            resultImage.SetTo(new Scalar(0, 0, 255), defectMaskExcludingCircles);

            bool circleErrorYN = circles != null && circles.Length == 2;
            Console.WriteLine($"Defect area (excluding circles): {defectArea} pixels");
            Console.WriteLine($"Detected circles: {(circles != null ? circles.Length : 0)}");

            return (resultImage, defectArea, circleErrorYN);
        }

         public bool SaveMatImageWithDateFolder(Mat image, string end_name, string baseFolder = @"c:\saveimg")
        {
            try
            {
                DateTime now = DateTime.Now;
                string dateFolderName = now.ToString("yyyy-MM-dd");
                string dateFolder = Path.Combine(baseFolder, dateFolderName);

                if (!Directory.Exists(dateFolder))
                {
                    Directory.CreateDirectory(dateFolder);
                }

                string fileName = now.ToString("yyyy-MM-dd HH-mm-ss_") + end_name + ".jpg";
                string fullPath = Path.Combine(dateFolder, fileName);

                Cv2.ImWrite(fullPath, image);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
                return false;
            }
        }

        public Mat DrawCenteredDottedRectangle(Mat image, float expansion)
        {
            // 이미지 중심 계산
            Point2f center = new Point2f(image.Cols / 2f, image.Rows / 2f);

            // 사각형의 좌상단과 우하단 점 계산
            //Point2f topLeft = new Point2f(center.X - expansion, center.Y - expansion);
            //Point2f bottomRight = new Point2f(center.X + expansion, center.Y + expansion);
            Point2f topLeft = new Point2f(expansion + 50, expansion);
            Global.topLeft = topLeft;
            //Point2f bottomRight = new Point2f(image.Width - expansion - 200, image.Height - expansion);
            Point2f bottomRight = new Point2f(image.Width - expansion - 200, image.Height - expansion - 50);
            Global.bottomRight = bottomRight;

            // 주황색 정의 (BGR 형식)
            Scalar orangeColor = new Scalar(0, 165, 255);

            // 점선 사각형 그리기
            DrawDottedLine(image, topLeft, new Point2f(bottomRight.X, topLeft.Y), orangeColor);
            DrawDottedLine(image, new Point2f(bottomRight.X, topLeft.Y), bottomRight, orangeColor);
            DrawDottedLine(image, bottomRight, new Point2f(topLeft.X, bottomRight.Y), orangeColor);
            DrawDottedLine(image, new Point2f(topLeft.X, bottomRight.Y), topLeft, orangeColor);

            return image;
        }
        public Mat ResizeImage(Mat image)
        {

            int x = 50;
            int y = 0;
            int w = 850;
            int h = 1050;
            if (image == null)
            {
                return null;
            }

            return CropImage(image, x, y, w, h);
        }

        private void DrawDottedLine(Mat image, Point2f start, Point2f end, Scalar color)
        {
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            int steps = (int)distance;

            for (int i = 0; i < steps; i++)
            {
                float t = i / (float)steps;
                int x = (int)(start.X + t * dx);
                int y = (int)(start.Y + t * dy);

                if (i % 10 < 5)  // 5픽셀 그리고 5픽셀 건너뛰기
                {
                    Cv2.Circle(image, new OpenCvSharp.Point(x, y), 1, color, -1, LineTypes.AntiAlias);
                }
            }
        }

        public Mat CountCornerWhitePixels(Mat gammaImg)
        {
            Mat gray_img = new Mat();
            Cv2.CvtColor(gammaImg, gray_img, ColorConversionCodes.RGB2GRAY);
            //Cv2.ImWrite("gray_img.jpg", gray_img);
            //Mat blur_img = Cv2.MedianBlur(origin_image,5) ;
            Mat blur_img = new Mat();
            //Cv2.MedianBlur(src: origin_image, dst: blur_img, 5);
            Cv2.MedianBlur(src: gray_img, dst: blur_img, 5);
            Mat invertedImage = new Mat();
            Cv2.BitwiseNot(blur_img, invertedImage);

            // Threshold 적용
            Mat binary = new Mat();
            
            //Cv2.Threshold(src: blur_img, dst: thresholdImage, binaryThresh, maxval: 255, type: ThresholdTypes.Binary);
            Cv2.Threshold(src: invertedImage, dst: binary, Global.binaryThresh, maxval: 255, type: ThresholdTypes.Binary);

            // 고정된 검사 영역 크기
            const int REGION_SIZE = 100;

            // 각 코너의 흰색 픽셀 수를 저장할 변수들
            int topLeft = 0, topRight = 0, bottomLeft = 0, bottomRight = 0;

            // 이미지 크기 확인
            if (binary.Width < REGION_SIZE || binary.Height < REGION_SIZE)
            {
                Console.WriteLine("이미지가 너무 작습니다.");
                return null;
            }

            // 좌상단 코너 계산 (0,0) 부터 (200,200)
            for (int y = 0; y < REGION_SIZE; y++)
            {
                for (int x = 0; x < REGION_SIZE; x++)
                {
                    if (binary.At<byte>(y, x) == 255)
                        topLeft++;
                }
            }

            // 우상단 코너 계산 (width-200,0) 부터 (width,200)
            for (int y = 0; y < REGION_SIZE; y++)
            {
                for (int x = binary.Width - REGION_SIZE; x < binary.Width; x++)
                {
                    if (binary.At<byte>(y, x) == 255)
                        topRight++;
                }
            }

            // 좌하단 코너 계산 (0,height-200) 부터 (200,height)
            for (int y = binary.Height - REGION_SIZE; y < binary.Height; y++)
            {
                for (int x = 0; x < REGION_SIZE; x++)
                {
                    if (binary.At<byte>(y, x) == 255)
                        bottomLeft++;
                }
            }

            // 우하단 코너 계산 (width-200,height-200) 부터 (width,height)
            for (int y = binary.Height - REGION_SIZE; y < binary.Height; y++)
            {
                for (int x = binary.Width - REGION_SIZE; x < binary.Width; x++)
                {
                    if (binary.At<byte>(y, x) == 255)
                        bottomRight++;
                }
            }

            //좌상단 값은 약간 보정 적용
            topLeft = topLeft + 300;

            // 결과 출력
            Console.WriteLine($"좌상단 흰색 픽셀 수 (200x200): {topLeft}");
            Console.WriteLine($"우상단 흰색 픽셀 수 (200x200): {topRight}");
            Console.WriteLine($"좌하단 흰색 픽셀 수 (200x200): {bottomLeft}");
            Console.WriteLine($"우하단 흰색 픽셀 수 (200x200): {bottomRight}");

            // 디버깅을 위한 시각화 (선택사항)
            Mat debugImage = binary.Clone();
            Cv2.CvtColor(debugImage, debugImage, ColorConversionCodes.GRAY2BGR);

            // 검사 영역을 사각형으로 표시
            Cv2.Rectangle(debugImage, new OpenCvSharp.Point(0, 0), new OpenCvSharp.Point(REGION_SIZE, REGION_SIZE), new Scalar(128), 2);
            Cv2.Rectangle(debugImage, new OpenCvSharp.Point(binary.Width - REGION_SIZE, 0), new OpenCvSharp.Point(binary.Width, REGION_SIZE), new Scalar(128), 2);
            Cv2.Rectangle(debugImage, new OpenCvSharp.Point(0, binary.Height - REGION_SIZE), new OpenCvSharp.Point(REGION_SIZE, binary.Height), new Scalar(128), 2);
            Cv2.Rectangle(debugImage, new OpenCvSharp.Point(binary.Width - REGION_SIZE, binary.Height - REGION_SIZE), new OpenCvSharp.Point(binary.Width, binary.Height), new Scalar(128), 2);

            Cv2.PutText(debugImage,$"Top Left Pixel : {topLeft}", new OpenCvSharp.Point(20, REGION_SIZE + 10), HersheyFonts.HersheySimplex, (double)0.7
                , new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
            Cv2.PutText(debugImage, $"Top Right Pixel : {topRight}", new OpenCvSharp.Point(debugImage.Width - 300, REGION_SIZE + 10), HersheyFonts.HersheySimplex, (double)0.7
                , new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
            Cv2.PutText(debugImage, $"Bottom Left Pixel : {bottomLeft}", new OpenCvSharp.Point(20, debugImage.Height - REGION_SIZE - 10), HersheyFonts.HersheySimplex, (double)0.7
                , new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
            Cv2.PutText(debugImage, $"Bottom Right Pixel : {bottomRight}", new OpenCvSharp.Point(debugImage.Width - 300, debugImage.Height - REGION_SIZE - 10), HersheyFonts.HersheySimplex, (double)0.7
                , new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);

            Cv2.ImWrite("debug.jpg",debugImage);
            // 디버그 이미지 저장
            //Cv2.ImWrite("debug_corners.png", debugImage);

            // 리소스 해제
            binary.Dispose();

            if (Global.autoImageCoodinateFlag)
            {
                int xCoodRate = 140;
                int yCoodRate = 140;

                if (Math.Abs(topLeft - topRight) > 100)
                {
                    int diff = topLeft - topRight;
                    int offset_value = Math.Abs(diff / xCoodRate);
                    if (offset_value % 2 > 0)
                    {
                        offset_value = offset_value + 1;
                    }
                    if (diff > 0 )
                    {
                        Global.xMoveCoodinate = Global.xMoveCoodinate - offset_value;
                    } else
                    {
                        Global.xMoveCoodinate = Global.xMoveCoodinate + offset_value;
                    }
                }

                if (Math.Abs(topLeft - bottomLeft) > 100)
                {
                    int diff = topLeft - bottomLeft;
                    int offset_value = Math.Abs(diff / xCoodRate);
                    if (offset_value % 2 > 0)
                    {
                        offset_value = offset_value + 1;
                    }
                    if (diff > 0)
                    {
                        Global.yMoveCoodinate = Global.yMoveCoodinate - offset_value;
                        
                    }
                    else
                    {
                        Global.yMoveCoodinate = Global.yMoveCoodinate + offset_value;
                        
                    }
                }

                Console.WriteLine($"Global.xMoveCoodinate : {Global.xMoveCoodinate} , Global.yMoveCoodinate : {Global.yMoveCoodinate}");
            }


            return debugImage;

        }

        public Point2f MovePointByAngle(Point2f point, double angle,bool reverseYN, string productName)
        {
            // 수직 각도 계산 (90도 회전)
            //double perpendicularAngle = (angle + 90) % 360;
            if ("I".Equals(productName))
            {
                double perpendicularAngle = angle;

                // 각도의 절대값 계산
                double absAngle = Math.Abs(angle);


                // 이동할 거리 계산 (3~5 픽셀 사이로 비례)
                double distance = 0; 
                if (absAngle <= 30)
                {
                    //distance = 6.0; // 30도 이하면 최소 3픽셀
                }
                else if (absAngle >= 90)
                {
                    //distance = 11.0; // 90도 이상이면 최대 5픽셀
                }
                else
                {
                    // 30도에서 90도 사이에서 3~5픽셀로 선형 보간
                    double ratio = (absAngle - 30) / (90 - 30);
                    distance = 6.0 + (ratio * 5.0); // 3에서 5 사이로 보간
                }

                Console.WriteLine($"perpendicularAngle : {perpendicularAngle}");

                // 수직 각도를 라디안으로 변환
                double radians = perpendicularAngle * (Math.PI / 180);
                Console.WriteLine($"X Math.Cos(radians) : {Math.Cos(radians)} , distance : {distance}");
                Console.WriteLine($"Y Math.Sin(radians) : {Math.Sin(radians)} , distance : {distance}");
                // 새로운 좌표 계산
                float reverse = 1;
                if (!reverseYN)
                {
                    reverse = -1;
                }
                //float newX = point.X + (float)(Math.Cos(radians) * distance) * reverse;
                //float newY = point.Y - (float)(Math.Sin(radians) * distance);

                float newX = point.X;
                float newY = point.Y;

                return new Point2f(newX, newY);
            }
            else if ("A".Equals(productName))
            {
                double perpendicularAngle = angle;

                // 각도의 절대값 계산
                double absAngle = Math.Abs(angle);
                if (absAngle > 90)
                {
                    absAngle = 180 - absAngle;
                }


                // 이동할 거리 계산 (3~5 픽셀 사이로 비례)
                double distance = 0;
                if (absAngle <= 30)
                {
                    //distance = 6.0; // 30도 이하면 최소 3픽셀
                }
                else if (absAngle >= 150)
                {
                    //distance = 11.0; // 90도 이상이면 최대 5픽셀
                }
                else
                {
                    // 30도에서 150도 사이에서 3~5픽셀로 선형 보간
                    double ratio = (absAngle - 30) / (90 - 30);
                    //distance = 6.0 + (ratio * 5.0); // 3에서 5 사이로 보간
                    distance = 2.0 + (ratio * 4.0); // 3에서 5 사이로 보간
                }

                Console.WriteLine($"perpendicularAngle : {perpendicularAngle}");

                // 수직 각도를 라디안으로 변환
                double radians = perpendicularAngle * (Math.PI / 180);
                Console.WriteLine($"X Math.Cos(radians) : {Math.Cos(radians)} , distance : {distance}");
                Console.WriteLine($"Y Math.Sin(radians) : {Math.Sin(radians)} , distance : {distance}");
                // 새로운 좌표 계산
                float reverse = 1;
                if (!reverseYN)
                {
                    reverse = -1;
                }

                //float newX = point.X + (float)(Math.Cos(radians) * distance) * reverse;
                //float newY = point.Y - (float)(Math.Sin(radians) * distance);

                float newX = point.X;
                float newY = point.Y;

                //2025.10.28
                //-90 < angle < 0 일 경우 y값 추가 보정 필요
                if (!reverseYN  && Math.Abs(angle) < 100)
                {
                    double ratio = (absAngle - 0) / 100;
                    //distance = 6.0 + (ratio * 5.0); // 3에서 5 사이로 보간
                    distance = 4.0+ (ratio * 2.0); // 에서 5 사이로 보간

                    newY = point.Y + (float)(Math.Sin(radians) * distance);
                }

                return new Point2f(newX, newY);
            }
            else if ("PIN".Equals(productName))
            {
                /*
                double perpendicularAngle = 0;
                if (angle > 0 )
                {
                    perpendicularAngle = angle - 90;
                }
                else
                {
                    perpendicularAngle = angle + 90;
                }
                //double perpendicularAngle = angle;

                // 각도의 절대값 계산
                double absAngle = Math.Abs(angle);


                // 이동할 거리 계산 (3~5 픽셀 사이로 비례)
                double distance = 0;
                if (absAngle <= 30)
                {
                    //distance = 6.0; // 30도 이하면 최소 3픽셀
                }
                else if (absAngle >= 90)
                {
                    //distance = 11.0; // 90도 이상이면 최대 5픽셀
                }
                else
                {
                    // 30도에서 90도 사이에서 3~5픽셀로 선형 보간
                    double ratio = (absAngle - 30) / (90 - 30);
                    distance = 7 + (ratio * 5.0); // 3에서 5 사이로 보간
                }

                Console.WriteLine($"perpendicularAngle : {perpendicularAngle}");

                // 수직 각도를 라디안으로 변환
                double radians = perpendicularAngle * (Math.PI / 180);
                Console.WriteLine($"X Math.Cos(radians) : {Math.Cos(radians)} , distance : {distance}");
                Console.WriteLine($"Y Math.Sin(radians) : {Math.Sin(radians)} , distance : {distance}");
                // 새로운 좌표 계산
                float reverse = 1;
                if (!reverseYN)
                {
                    reverse = -1;
                }
                //float newX = point.X + (float)(Math.Cos(radians) * distance) * reverse;
                //float newY = point.Y - (float)(Math.Sin(radians) * distance);
                float newX = point.X + (float)(Math.Sin(radians) * distance) * reverse;
                float newY = point.Y - (float)(Math.Cos(radians) * distance) ;
                */

                //double perpendicularAngle = 0;

                double perpendicularAngle = angle;

                // 각도의 절대값 계산
                //double absAngle = Math.Abs(angle);
                double absAngle = Math.Abs(angle);


                // 이동할 거리 계산 (3~5 픽셀 사이로 비례)
                double distance = 0;
                if (absAngle <= 30)
                {
                    //distance = 6.0; // 30도 이하면 최소 3픽셀
                }
                else if (absAngle >= 90)
                {
                    //distance = 11.0; // 90도 이상이면 최대 5픽셀
                }
                //2025.10.28 
                //PIN 제품의 각도가 70도 이상일때 오차가 발생하는 것으로 확인
                else if (angle > 60)
                {
                    double ratio = (angle - 60) / (90 - 60);
                    distance = 2 + (ratio * 2.0); // 2에서 4 사이로 보간
                }
                else
                {
                    // 30도에서 90도 사이에서 3~5픽셀로 선형 보간
                    double ratio = (absAngle - 30) / (90 - 30);
                    //distance = 5 + (ratio * 5.0); // 3에서 5 사이로 보간
                    //distance = 5 + (ratio * 2.0); // 3에서 5 사이로 보간
                    distance = 2 + (ratio * 5.0); // 2에서 7 사이로 보간
                }

                Console.WriteLine($"perpendicularAngle : {perpendicularAngle}");

                // 수직 각도를 라디안으로 변환
                //double radians = perpendicularAngle * (Math.PI / 180);
                double radians = perpendicularAngle * (Math.PI / 180);
                //double radians = Math.Abs(perpendicularAngle);
                Console.WriteLine($"X Math.Cos(radians) : {Math.Cos(radians)} , distance : {distance}");
                Console.WriteLine($"Y Math.Sin(radians) : {Math.Sin(radians)} , distance : {distance}");
                // 새로운 좌표 계산
                float reverse = 1;
                if (!reverseYN)
                {
                    reverse = -1;
                }
                //float newX = point.X + (float)(Math.Cos(radians) * distance) * reverse;
                //float newY = point.Y - (float)(Math.Sin(radians) * distance);
                //float newX = point.X + (float)(Math.Sin(radians) * distance) ;
                //float newY = point.Y - (float)(Math.Cos(radians) * distance) ;
                float newX = 0;
                float newY = 0;
                //angle > 0
                if (reverseYN)
                {
                    //2025.10.28 이전 보정값
                    //newX = point.X + (float)(Math.Cos(radians) * (distance*1.5));
                    //newY = point.Y - (float)(Math.Sin(radians) * distance);
                    //2025.10.28 이후 보정값
                    newX = point.X + (float)(Math.Sin(radians) * (distance*1.5));
                    if (angle < 45)
                    {
                        newY = point.Y + (float)(Math.Cos(radians) * (distance));
                    }
                    else
                    {
                        newY = point.Y + (float)(Math.Sin(radians) * (distance));
                    }
                    
                    //newY = point.Y;
                    
                }
                else
                //angle < 0 
                {
                    //newX = point.X + (float)(Math.Cos(radians) * distance/2);
                    //newY = point.Y - (float)(Math.Sin(radians) * distance/3);
                    //newY = point.Y - (float)(Math.Sin(radians) * distance / 4);
                    //상단 위치 & 각도 -50일때 기준 수정(x,y값 모두 낮춰야 함)
                    //2025.10.28 이전 보정값
                    newX = point.X + (float)(Math.Cos(radians) * distance);
                    newY = point.Y;
                    //2025.10.28 기준 보정값
                    //newX = point.X + (float)(Math.Cos(radians) * distance);
                    //newY = point.Y + (float)(Math.Sin(radians) * distance);
                }

                return new Point2f(newX, newY);
            }
            else
            {
                float newX = point.X;
                float newY = point.Y;

                return new Point2f(newX, newY);
            }
            
        }

        public (double robotX, double robotY) calibrateRobotPosition(string productName, Point2f center_point, double angle)
        {
            double robotX = 0.0, robotY = 0.0;

            if ("I".Equals(productName))
            {
                if (Math.Abs(angle)> 30)
                {
                    /*if (angle > 0)
                    {

                    }
                    else
                    {

                    }*/
                    Point2f calibrate_point = new Point2f();
                    calibrate_point = MovePointByAngle(center_point, Math.Abs(angle),angle > 0 ,productName);
                    Console.WriteLine($"center_point.X : {center_point.X} , center_point.Y : {center_point.Y}");
                    Console.WriteLine($"calibrate_point.X : {calibrate_point.X} , calibrate_point.Y : {calibrate_point.Y}");
                    robotX = Math.Round(calibrate_point.X * Global.xPixelRate + Global.x_offset, 3);
                    robotY = Math.Round(calibrate_point.Y * Global.yPixelRate + Global.y_offset, 3);
                }else
                {
                    robotX = Math.Round(center_point.X * Global.xPixelRate + Global.x_offset, 3);
                    robotY = Math.Round(center_point.Y * Global.yPixelRate + Global.y_offset, 3);
                }
            } 
            else if ("A".Equals(productName))
            {
                if (Math.Abs(angle) >= 30 && Math.Abs(angle) <= 150)
                {
                    Point2f calibrate_point = new Point2f();
                    calibrate_point = MovePointByAngle(center_point, Math.Abs(angle), angle > 0, productName);
                    Console.WriteLine($"center_point.X : {center_point.X} , center_point.Y : {center_point.Y}");
                    Console.WriteLine($"calibrate_point.X : {calibrate_point.X} , calibrate_point.Y : {calibrate_point.Y}");
                    robotX = Math.Round(calibrate_point.X * Global.xPixelRate + Global.x_offset, 3);
                    robotY = Math.Round(calibrate_point.Y * Global.yPixelRate + Global.y_offset, 3);
                }
                else
                {
                    robotX = Math.Round(center_point.X * Global.xPixelRate + Global.x_offset, 3);
                    robotY = Math.Round(center_point.Y * Global.yPixelRate + Global.y_offset, 3);
                }
            }
            else if ("PIN".Equals(productName))
            {
                if (Math.Abs(angle) > 30)

                {
                    Point2f calibrate_point = new Point2f();
                    calibrate_point = MovePointByAngle(center_point, angle , angle > 0, productName);
                    
                    robotX = Math.Round(calibrate_point.X * Global.xPixelRate + Global.x_offset, 3);
                    robotY = Math.Round(calibrate_point.Y * Global.yPixelRate + Global.y_offset, 3);

                    Console.WriteLine($"center_point.X : {center_point.X} , center_point.Y : {center_point.Y}");
                    Console.WriteLine($"[Calibration]calibrate_point.X : {calibrate_point.X} , calibrate_point.Y : {calibrate_point.Y}");
                    Console.WriteLine($"origin_robot.X : {Math.Round(center_point.X * Global.xPixelRate + Global.x_offset, 3)} , origin_robot.Y : {Math.Round(center_point.Y * Global.yPixelRate + Global.y_offset, 3)}");
                    Console.WriteLine($"[Calibration]calibrate_Robot.X : {robotX} , calibrate_Robot.Y : {robotY}");

                }
                else
                {
                    Console.WriteLine($"[None Calibration Coordi]center_point.X : {center_point.X} , center_point.Y +2: {center_point.Y +2}");
                    //Console.WriteLine($"[None Calibration Coordi]center_point.X : {center_point.X} , center_point.Y : {center_point.Y}");
                    robotX = Math.Round(center_point.X * Global.xPixelRate + Global.x_offset, 3);
                    //robotY = Math.Round(center_point.Y * Global.yPixelRate + Global.y_offset, 3);
                    robotY = Math.Round((center_point.Y +2) * Global.yPixelRate + Global.y_offset, 3);
                }
            }
            else
            {
                robotX = Math.Round(center_point.X * Global.xPixelRate + Global.x_offset, 3);
                robotY = Math.Round(center_point.Y * Global.yPixelRate + Global.y_offset, 3) ;
                
            }



            return (robotX , robotY);
        }


    }

    public class AreaCorrector
    {
        private const double TARGET_AREA = 11800.0;          // 목표 기준 면적
        private const double OPTIMAL_Y = 500.0;              // 최적 Y 위치
        private const int IMAGE_WIDTH = 1100;
        private const int IMAGE_HEIGHT = 1100;

        // 완화된 보정 계수들
        private const double Y_COEFFICIENT_TOP = -0.00012;    // Y > 800
        private const double Y_COEFFICIENT_HIGH = -0.00010;   // 600 < Y <= 800
        private const double Y_COEFFICIENT_MID = -0.00008;    // 400 < Y <= 600
        private const double Y_COEFFICIENT_LOW = -0.00010;    // 200 < Y <= 400
        //private const double Y_COEFFICIENT_BOTTOM = -0.00012; // Y <= 200
        private const double Y_COEFFICIENT_BOTTOM = -0.00017; // Y <= 200

        // X축 보정도 약하게 조정
        private const double X_COEFFICIENT_CENTER = -0.00005; // 중앙 영역
        private const double X_COEFFICIENT_EDGE = -0.00008;   // 가장자리 영역

        public static double CorrectArea(int x, int y, double measuredArea)
        {
            // Y축 보정 계수 선택 (약화됨)
            double yCoeff;
            if (y > 800) yCoeff = Y_COEFFICIENT_TOP;
            else if (y > 600) yCoeff = Y_COEFFICIENT_HIGH;
            else if (y > 400) yCoeff = Y_COEFFICIENT_MID;
            else if (y > 200) yCoeff = Y_COEFFICIENT_LOW;
            //else if (y < 100 && x < 200 ) yCoeff = Y_COEFFICIENT_BOTTOM * 1.5;
            else yCoeff = Y_COEFFICIENT_BOTTOM;

            if (measuredArea < 6000 && y < 100 && x < 200)
            {
                yCoeff = Y_COEFFICIENT_BOTTOM * 1.5;
            }

            //PIN 좌상단 보정 로직 약화
            if (measuredArea > 10000 && y < 200)
            {
                yCoeff = Y_COEFFICIENT_BOTTOM * 0.5;
            }

            // Y축 거리에 따른 보정
            double yDistance = Math.Abs(y - OPTIMAL_Y);
            double yCorrection = 1.0 + (yCoeff * yDistance);

            // X축 보정 계수 선택
            double xDistance = Math.Abs(x - IMAGE_WIDTH / 2.0);
            double xCoeff = (xDistance > IMAGE_WIDTH / 4.0) ? X_COEFFICIENT_EDGE : X_COEFFICIENT_CENTER;

            // X축 보정
            double xCorrection = 1.0 + (xCoeff * xDistance);

            // 코너 추가 보정 (약화됨)
            double cornerCorrection = 1.0;
            //PIN 제품 보정 로직 제외
            if ((x < 200 || x > IMAGE_WIDTH - 200) && (y < 200 || y > IMAGE_HEIGHT - 200) && measuredArea < 10000)
            {
                //cornerCorrection = 1.01; // 코너 영역 1% 추가 보정
                cornerCorrection = 0.99; // 코너 영역 1% 추가 보정
            }

            // 최종 보정값 계산
            double correctedArea = measuredArea / (xCorrection * yCorrection * cornerCorrection);

            // 기준값과의 차이를 줄이기 위한 약한 스케일링
            double scalingFactor = TARGET_AREA / 11600.0;
            correctedArea = (correctedArea * scalingFactor + correctedArea * 3) / 4; // 75% 원본 유지

            return correctedArea;
        }

        // 이물 판정을 위한 보정 범위 확인 함수
        public static bool IsWithinNormalRange(double area)
        {
            return area >= 11000 && area <= 12200; // 넓은 허용 범위
        }
    }
}