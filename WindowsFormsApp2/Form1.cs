using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basler.Pylon;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Net.NetworkInformation;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;
using static WindowsFormsApp2.Image_Process;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenCvSharp.Flann;
using System.Runtime.CompilerServices;
using OpenCvSharp.Dnn;
using System.Security.Cryptography;


namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {

        public Mat origin_image;
        public Mat gamma_image;

        public Mat thresholdImage;
        public Mat contourImage;
        public Mat distanceImage;
        //int binaryThresh = 106;

        Basler.Pylon.Camera main_camera = new Basler.Pylon.Camera();
        //Basler.Pylon.Camera main_camera;

        Image_Process image_process = new Image_Process();
        ProductManager pmManager = new ProductManager();
        private Table productDisplay;
        Util timer = new Util();
        SocketClient IA_socket;

        private Panel titleBar;
        private Label titleLabel;
        private Button closeButton, maximizeButton, minimizeButton;
        private bool isDragging = false;
        private System.Drawing.Point dragCursorPoint;
        private System.Drawing.Point dragFormPoint;


        public Form1()
        {
            InitializeComponent();
            //InitializeControls();

            //Global 변수 세팅
            LoadSettings();


            InitializeTabControl();
            this.ClientSize = new System.Drawing.Size(1920, 1050);
            SetupCustomTitleBar();
            //search_camera();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            call_init_linear_motor();
            InitIO();
            backgroundCameraActivation();
            backgroundIOTriggerActivation();
            

            image_process.CalculrateRobotLimit(Global.detectLimitSize);

            productDisplay = new Table(listView1,listView2);

            Bitmap bmp = new Bitmap("LOGO.png");
            bmp.MakeTransparent();
            pictureBox4.Image = bmp;

            DrawBorderOnPictureBox(pictureBox1, Color.White, 2f);
            DrawBorderOnPictureBox(pictureBox2, Color.White, 2f);
            DrawBorderOnPictureBox(pictureBox3, Color.White, 2f);

            pictureBox1.Paint += PictureBox_Paint;
            pictureBox2.Paint += PictureBox_Paint;
            pictureBox3.Paint += PictureBox_Paint;
            //grab_async();
            //LoadAndDisplayImage();
            tabControl1.Invalidate(); // TabControl을 다시 그리도록 요청

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            InitializeControls(this);

            //listView1.dou
            socketInit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            IA_socket.CloseConnection();

            Global.programRunningFlag = false;
            SerialCommunication.Motor_SendFreeMessage(">off\r");
            Thread.Sleep(50);
            // 폼이 닫힐 때 연결 종료
            /*if (client != null && client.Connected)
            {
                client.Close();
            }*/
        }


        private void SetupCustomTitleBar()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;

            titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.FromArgb(45, 45, 48)
            };

            titleLabel = new Label
            {
                Text = "Dmillion Vision System",
                ForeColor = Color.White,
                Dock = DockStyle.Left,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Size = new System.Drawing.Size(200, 30)
            };

            closeButton = CreateButton("×", Color.FromArgb(232, 17, 35));
            maximizeButton = CreateButton("□", Color.FromArgb(45, 45, 48));
            minimizeButton = CreateButton("—", Color.FromArgb(45, 45, 48));

            titleBar.Controls.Add(titleLabel);
            titleBar.Controls.Add(minimizeButton);
            titleBar.Controls.Add(maximizeButton);
            titleBar.Controls.Add(closeButton);

            this.Controls.Add(titleBar);

            // 메인 콘텐츠를 위한 패널 추가
            Panel mainContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = this.BackColor
            };
            this.Controls.Add(mainContent);

            titleBar.MouseDown += TitleBar_MouseDown;
            titleBar.MouseMove += TitleBar_MouseMove;
            titleBar.MouseUp += TitleBar_MouseUp;

            closeButton.Click += (s, e) => this.Close();
            maximizeButton.Click += (s, e) =>
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.ClientSize = new System.Drawing.Size(1920, 1050);
                }
                else
                {
                    this.WindowState = FormWindowState.Maximized;
                }
            };
            minimizeButton.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
        }

        private Button CreateButton(string text, Color color)
        {
            return new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = color,
                ForeColor = Color.White,
                Dock = DockStyle.Right,
                Width = 45,
                Height = 30
            };
        }

        /*protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // 폼 테두리 그리기
            using (Pen pen = new Pen(Color.FromArgb(45, 45, 48), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }*/

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                System.Drawing.Point dif = System.Drawing.Point.Subtract(Cursor.Position, new System.Drawing.Size(dragCursorPoint));
                this.Location = System.Drawing.Point.Add(dragFormPoint, new System.Drawing.Size(dif));
            }
        }

        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }
        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb != null)
            {
                using (Pen pen = new Pen(Color.White, 2))
                {
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, pb.Width - 1, pb.Height - 1));
                }
            }
        }

        private void InitializeTabControl()
        {
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += TabControl1_DrawItem;
            tabControl1.Paint += TabControl1_Paint;
            

            // 배경색 변경을 위한 이벤트 핸들러 추가
            //tabControl1.Paint += TabControl1_Paint;
            // 추가: 탭 컨트롤의 스타일 변경
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new System.Drawing.Size(120, 30); // 탭 버튼의 크기 조정

            // 각 탭 페이지의 배경색 설정
            foreach (TabPage page in tabControl1.TabPages)
            {
                page.BackColor = Color.FromArgb(64, 64, 64);
            }

            // 추가: 전체 TabControl의 배경색 설정
            tabControl1.BackColor = Color.FromArgb(64, 64, 64);
        }


        private void TabControl1_Paint(object sender, PaintEventArgs e)
        {
            // 탭 버튼 영역을 제외한 나머지 영역만 채우기
            Rectangle tabArea = tabControl1.DisplayRectangle;
            tabArea.Inflate(3, 3); // 약간의 여백 추가

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(64, 64, 64))) // 원하는 배경색으로 변경
            {
                e.Graphics.FillRectangle(brush, tabArea);
            }
        }

        private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = tabControl1.TabPages[e.Index];
            Color backColor = Color.FromArgb(64, 64, 64); // 탭 버튼 배경색
            Color foreColor = Color.White; // 탭 버튼 텍스트 색
            Font font = new Font("맑은고딕", 9, FontStyle.Bold);

            e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);

            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
            paddedBounds.Offset(1, yOffset);

            TextRenderer.DrawText(e.Graphics, page.Text, font, paddedBounds, foreColor);
        }

        public void socketInit(){
            IA_socket = new SocketClient();
           /* if (!Global.socketOpen)
            {
                IA_socket.InitializeClient();                
            }
            Thread.Sleep(10);*/

        }

       

        private Mat ConvertBufferToMat(byte[] buffer, int width, int height, string pixelFormat)
        {

            // 이미지를 3ch 이미지로 변환
            Mat tempImage = new Mat(height, width, MatType.CV_8UC3);

            // 이미지 데이터 복사
            Marshal.Copy(buffer, 0, tempImage.Data, buffer.Length);
            //Mat temp3ch = new Mat();
            //Cv2.CvtColor(tempImage, temp3ch, ColorConversionCodes.BGR2RGB);



            if (pixelFormat.StartsWith("Bayer"))
            {
                Mat bayer = new Mat(height, width, MatType.CV_8UC1);
                //bayer.SetArray(0, 0, buffer);
                bayer.SetArray<byte>(buffer); // 명시적으로 byte 타입 지정
                tempImage = new Mat();
                //Cv2.CvtColor(bayer, tempImage, ColorConversionCodes.BayerRG2BGR);
                Cv2.CvtColor(bayer, tempImage, ColorConversionCodes.BayerRG2RGB);
            }

            //origin_image = tempImage.Clone();

            // Cv2.ImWrite("ori.jpg", origin_image);

            // 이미지를 180도 회전
            Mat rotatedImage = new Mat();
            Cv2.Rotate(tempImage, rotatedImage, RotateFlags.Rotate180);

            return rotatedImage;
        }





        public void backgroundCameraActivation()
        {
            System.Threading.Thread.Sleep(10);
            Console.WriteLine("camera_Grab Server Start");
            Thread thread = new Thread(() => GrabImageThread());
            thread.IsBackground = true;
            thread.Start();
        }

        public void backgroundIOTriggerActivation()
        {
            System.Threading.Thread.Sleep(10);
            Console.WriteLine("IOTrigger Start");
            Thread thread = new Thread(() => IOWatchDog());
            thread.IsBackground = true;
            thread.Start();
        }

        public void IOWatchDog()
        {
            while (Global.programRunningFlag)
            {
                
                ushort result_detect = 0;

                ushort hopper_detect = 0;

                USBDASK.UD_DI_ReadLine(Global.dask_card_num, Global.dask_port_num, 1, out result_detect);

                USBDASK.UD_DO_ReadLine(Global.dask_card_num, Global.dask_port_num, 2, out hopper_detect);

                try
                {
                    if (hopper_detect == 1)
                    {
                        this.Invoke(new Action(delegate ()
                        {
                            label41.Text = "ON";
                            label41.BackColor = Color.Green;

                        }));
                        //hopperFlag -> false 일 경우 중지 신호 전달
                        if (!Global.HopperFlag)
                        {
                            USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 2, 0);
                        }

                    }
                    else
                    {
                        this.Invoke(new Action(delegate ()
                        {

                            label41.Text = "OFF";
                            label41.BackColor = Color.Red;


                        }));


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }


                if (result_detect == 1 && !Global.processFlag)
                {
                    /*
                    if (!Global.ioCheckFlag)
                    {
                        IOTriggerThread();
                        Global.ioCheckFlag = true;
                    }
                    */
                    Global.imageCheckFlag = false;

                    IOTriggerThread();
                    if (Global.feederViveCount == 0 && Global.contourCount < 10)
                    {
                        SerialCommunication.Motor_SendSineMessage(Global.feederVivePower);
                        Thread.Sleep(Global.feederViveDulation);
                        SerialCommunication.Motor_SendStopMessage();
                        Thread.Sleep(Global.feederAfterGrabDuration);
                    }

                    //timer.Start("main_Process");
                    Global.processFlag = true;
                    Global.detectFlag = true;
                    Thread.Sleep(10);
                    //await Task.Delay(10);
                    grab_image_trigger(true);
                    //Console.WriteLine($"[origin_image]grab_image_trigger Complete: {timer.Lap("main_Process").TotalMilliseconds} ms");
                    Thread.Sleep(50);
                    //await Task.Delay(50);
                    grab_image_trigger(false);
                    //timer.Stop("main_Process");
                    

                }
            }

        }

        public void IOTriggerThread()
        {


            //USBDASK.UD_DO_WriteLine(0, 0, 0, 1);
            ushort result_detect = 0;
            ushort result_I = 0;
            ushort result_A = 0;
            ushort result_P = 0;

            USBDASK.UD_DI_ReadLine(Global.dask_card_num, Global.dask_port_num, 1, out result_detect);
            USBDASK.UD_DI_ReadLine(Global.dask_card_num, Global.dask_port_num, 2, out result_I);
            USBDASK.UD_DI_ReadLine(Global.dask_card_num, Global.dask_port_num, 3, out result_A);
            USBDASK.UD_DI_ReadLine(Global.dask_card_num, Global.dask_port_num, 4, out result_P);

            //제품 검사 요청
            if (result_detect != 1)
            {
                Global.detectFlag = false;
            }
            else
            {
                Global.detectFlag = true;
            }

            //I 제품 검사 여부
            if (result_I != 1)
            {
                Global.detectI = false;
            }
            else
            {
                Global.detectI = true;
            }

            //A 제품 검사 여부
            if (result_A != 1)
            {
                Global.detectA = false;
            }
            else
            {
                Global.detectA = true;
            }

            //Pin 제품 검사 여부
            if (result_P != 1)
            {
                Global.detectP = false;
            }
            else
            {
                Global.detectP = true;
            }
            /*
            SetRadioButtonChecked(radioButton4, Global.detectFlag);
            SetRadioButtonChecked(radioButton5, Global.detectI);
            SetRadioButtonChecked(radioButton6, Global.detectA);
            SetRadioButtonChecked(radioButton7, Global.detectP);
            */
            UpdateStatus(0, Global.detectFlag);
            UpdateStatus(1, Global.detectI);
            UpdateStatus(2, Global.detectA);
            UpdateStatus(3, Global.detectP);

            Console.WriteLine($"IO Trigger Result result_detect  : {result_detect} result_I : {result_I} result_A : {result_A} result_P : {result_P} ");
            Console.WriteLine($"IO Trigger Result detect_flag : {Global.detectFlag} detect_I : {Global.detectI} detect_A : {Global.detectA} detect_P : {Global.detectP} ");
        }

        private void SetRadioButtonChecked(RadioButton radioButton, bool isChecked)
        {

            this.Invoke(new Action(delegate ()
            {
                //bool wasEnabled = radioButton.Enabled;
                //radioButton.Enabled = true;
                radioButton.Checked = isChecked;
                //radioButton.Enabled = wasEnabled;
            }));
        }

        public void GrabImageThread()
        {
            Console.WriteLine("Grab Image Thread Start");
            try
            {
                Console.WriteLine("### 0 ###");

                Console.WriteLine($"Using camera {main_camera.CameraInfo[CameraInfoKey.ModelName]}.");
                main_camera.CameraOpened += Configuration.AcquireContinuous;
                main_camera.Open();

                // 카메라 설정
                main_camera.Parameters[PLCamera.TriggerSelector].SetValue(PLCamera.TriggerSelector.FrameStart);
                //main_camera.Parameters[PLCamera.TriggerSelector].SetValue(PLCamera.TriggerSelector.AcquisitionStart);
                main_camera.Parameters[PLCamera.TriggerMode].SetValue(PLCamera.TriggerMode.On);
                main_camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                
                main_camera.Parameters[PLCamera.GammaSelector].SetValue(PLCamera.GammaSelector.User);
                //main_camera.Parameters[PLCamera.ExposureTimeAbs].SetValue(Global.originImageExposeTime);
                main_camera.Parameters[PLCamera.ExposureTimeAbs].SetValue(28000);
                //main_camera.Parameters[PLCamera.ExposureTimeAbs].SetValue(50000);
                //main_camera.Parameters[PLCamera.Gamma].SetValue(2.4);
                main_camera.Parameters[PLCamera.Gamma].SetValue(3.8);
                string pixelFormat = main_camera.Parameters[PLCamera.PixelFormat].GetValue();


                
                Console.WriteLine($"Pixel Format: {pixelFormat}");

                while (true)
                {
                    //Console.WriteLine("### 1 ###");
                    
                    if (Global.cameraGrab && Global.detectFlag)
                    {
                        //Console.WriteLine("### 2 ###");
                        main_camera.Parameters[PLCamera.OffsetX].SetValue(Global.xMoveCoodinate);
                        main_camera.Parameters[PLCamera.OffsetY].SetValue(Global.yMoveCoodinate);
                        main_camera.Parameters[PLCamera.GammaEnable].SetValue(Global.gammaGrab);
                        if (Global.gammaGrab)
                        {
                            //Console.WriteLine("### 3 ###");
                            //main_camera.Parameters[PLCamera.ExposureTimeAbs].SetValue(26000);
                            main_camera.Parameters[PLCamera.ExposureTimeAbs].SetValue(Global.gammaImageExposeTime);
                            
                        }
                        else
                        {
                            //Console.WriteLine("### 4 ###");
                            //main_camera.Parameters[PLCamera.ExposureTimeAbs].SetValue(50000);
                            main_camera.Parameters[PLCamera.ExposureTimeAbs].SetValue(Global.originImageExposeTime);
                        }


                        if (!main_camera.StreamGrabber.IsGrabbing)
                        {
                            //Console.WriteLine("### 5 ###");
                            //Console.WriteLine($"main_camera.StreamGrabber.IsGrabbing: {main_camera.StreamGrabber.IsGrabbing}");
                            main_camera.StreamGrabber.Start(1);
                        }

                        try
                        {
                            Console.WriteLine("Grab Try Logic Start");
                            using (IGrabResult grabResult = main_camera.StreamGrabber.RetrieveResult(-1, TimeoutHandling.ThrowException))
                            {
                                if (grabResult.GrabSucceeded)
                                {
                                    Console.WriteLine($"SizeX: {grabResult.Width}, SizeY: {grabResult.Height}");

                                    byte[] buffer = grabResult.PixelData as byte[];
                                    Console.WriteLine($"Gray value of first pixel: {buffer[0]}");

                                    Mat processedImage = ConvertBufferToMat(buffer, grabResult.Width, grabResult.Height, pixelFormat);

                                    if (Global.gammaGrab)
                                    {

                                        //2024.10.29 검출 영역 재정의
                                        gamma_image = processedImage;
                                        //gamma_image = image_process.ResizeImage(processedImage);
                                       
                                        if (gamma_image != null)
                                        {
                                            if (Global.imageCheckFlag)
                                            {
                                                Mat debugImg = image_process.CountCornerWhitePixels(gamma_image);
                                                if (!Global.autoImageCoodinateFlag)
                                                {
                                                    this.Invoke(new Action(delegate ()
                                                    {
                                                        //pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(gamma_image);
                                                        pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(debugImg);
                                                    }));
                                                }
                                                
                                                Global.processFlag = false;
                                                Global.detectFlag = false;
                                                Global.programTestFlag = false;
                                                Global.imageCheckFlag = false;
                                                
                                            }
                                            else
                                            {

                                                image_process.SaveMatImageWithDateFolder(gamma_image, "gamma");
                                                //Cv2.ImWrite("gamma.jpg", gamma_image);

                                                //origin_image가 취득되지 않으면 로직 skip
                                                if (origin_image != null)
                                                {
                                                    //Console.WriteLine($"[Image_Process]Image_Process start: {timer.Lap("main_Process").TotalMilliseconds} ms");

                                                    LoadAndDisplayImage();


                                                }
                                                else
                                                {
                                                    Console.WriteLine("Origin_image is null => Image Process Skip");
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {

                                        //2024.10.29 검출 영역 재정의
                                        origin_image = processedImage;
                                        //origin_image = image_process.ResizeImage(processedImage);
                                        

                                        if (origin_image != null)
                                        {
                                            if (Global.imageCheckFlag)
                                            {
                                              
                                                this.Invoke(new Action(delegate ()
                                                {
                                                    pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(origin_image);
                                                }));
                                                Global.processFlag = false;
                                                Global.detectFlag = false;
                                                Global.programTestFlag = false;
                                                Global.imageCheckFlag = false;
                                            }
                                            else
                                            {
                                                image_process.SaveMatImageWithDateFolder(origin_image, "ori");
                                            }
                                            //Cv2.ImWrite("ori.jpg", origin_image);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Origin_image is null => Origin Image Save Failed!!");
                                            //Global.processFlag = false;
                                        }
                                    }                                   

                                    Console.WriteLine("Image processing complete");
                                }
                                else
                                {
                                    Console.WriteLine($"Error: {grabResult.ErrorCode} {grabResult.ErrorDescription}");
                                }
                            }
                        }
                        finally
                        {
                            main_camera.StreamGrabber.Stop();
                            Global.cameraGrab = false;
                            Console.WriteLine("Camera grab cycle complete");
                        }
                    }

                    //await Task.Delay(100); // 짧은 대기 시간을 두어 CPU 사용량 감소
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Exception in GrabImageAsync: {e.Message}");
            }
        }

        

        private void ApplyThreshold()
        {
            if (origin_image == null || origin_image.Empty())
            {
                //MessageBox.Show("이미지를 먼저 로드해주세요.");
                string origin_imagePath = "./error_image/2024-10-22 09-22-04_ori.jpg";
                string gamma_imagePath = "./error_image/2024-10-22 09-22-04_gamma.jpg";

                //string origin_imagePath = "./error_image/2024-10-22 09-22-06_ori.jpg";
                //string gamma_imagePath = "./error_image/2024-10-22 09-22-06_gamma.jpg";

                
                gamma_image = Cv2.ImRead(gamma_imagePath);
                origin_image = Cv2.ImRead(origin_imagePath);
                Global.tempImageCheckFlag = true;
                ProcessImage();
                //return;
            }
            else
            {
                Global.tempImageCheckFlag=true;
                ProcessImage();
            }

            Global.tempImageCheckFlag = false;
            //Console.WriteLine($"Threshold가 {trackBar.Value} 값으로 적용되었습니다.");
        }


       

        private void LoadAndDisplayImage()
        {
            //string imagePath = "./demo_image/Image__2024-10-12__00-02-38.jpg";
            //string imagePath = "./demo_image/Image__2024-10-12__00-04-38.jpg";
            //string imagePath = "./demo_image/Image__2024-10-12__00-02-31.jpg";
            //string imagePath = "./demo_image/Image__2024-10-12__00-02-16.jpg";

            try
            {              
                ProcessImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 로드 중 오류 발생: {ex.Message}");
                Console.WriteLine(ex.Message);
            }
        }

        

        // Mat 객체 사용 예시
        public void ProcessImage()
        {

            this.Invoke(new Action(delegate ()
            {
                image_process.productInfoList.Clear();
                /*
                string imagePath = "./demo_image/Image__2024-10-12__00-02-16.jpg";
                gamma_image = Cv2.ImRead(imagePath);
                origin_image = Cv2.ImRead(imagePath);
                */

                Mat gray_img = new Mat();
                Cv2.CvtColor(gamma_image, gray_img, ColorConversionCodes.RGB2GRAY);
                //Cv2.ImWrite("gray_img.jpg", gray_img);
                //Mat blur_img = Cv2.MedianBlur(origin_image,5) ;
                Mat blur_img = new Mat();
                //Cv2.MedianBlur(src: origin_image, dst: blur_img, 5);
                Cv2.MedianBlur(src: gray_img, dst: blur_img, 5);
                Mat invertedImage = new Mat();
                Cv2.BitwiseNot(blur_img, invertedImage);

                // Threshold 적용
                thresholdImage = new Mat();
                contourImage = new Mat();
                distanceImage = new Mat();

                //Cv2.Threshold(src: blur_img, dst: thresholdImage, binaryThresh, maxval: 255, type: ThresholdTypes.Binary);
                Cv2.Threshold(src: invertedImage, dst: thresholdImage, Global.binaryThresh, maxval: 255, type: ThresholdTypes.Binary);

                if (thresholdImage.Channels() != 1)
                {
                    Cv2.CvtColor(thresholdImage, thresholdImage, ColorConversionCodes.BGR2GRAY);
                }
                //Cv2.ImWrite("thresholdImage.jpg", thresholdImage);
                image_process.SaveMatImageWithDateFolder(thresholdImage,"thresh");
                //Console.WriteLine($"[First Threshhold]First Threshold Comp: {timer.Lap("main_Process").TotalMilliseconds} ms");
                //pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(thresholdImage);
                
                OpenCvSharp.Point[][] contours;
                //contourImage = image_process.FindContours_Image(origin_image,thresholdImage ,out contours);
                contourImage = image_process.FindContours_Image(origin_image, thresholdImage, out contours);
                Cv2.ImWrite("contourImage.jpg", contourImage);
                //image_process.SaveMatImageWithDateFolder(contourImage, "contour");
                
                //Console.WriteLine($"[FindContours_Image]FindContours_Image Comp: {timer.Lap("main_Process").TotalMilliseconds} ms");
                distanceImage = image_process.ProcessDistanceTransform(thresholdImage, Global.binaryThresh2);
                //image_process.SaveMatImageWithDateFolder(distanceImage, "distance");

                // PictureBox에 이진화된 이미지 표시
                if (Global.tempImageCheckFlag)
                {
                    //pictureBox2.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(thresholdImage);
                    //pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    //Console.WriteLine($"[ProcessDistanceTransform]ProcessDistanceTransform Comp: {timer.Lap("main_Process").TotalMilliseconds} ms");

                    //pictureBox2.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(contourImage);
                    //pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox2.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(contourImage);

                    //pictureBox3.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(distanceImage);
                    //pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox3.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(distanceImage);

                    Mat tempResultImg = new Mat();
                    //Cv2.CvtColor(origin_image, resultImg, ColorConversionCodes.GRAY2BGR);
                    if (origin_image.Channels() == 1)
                    {
                        Cv2.CvtColor(origin_image, tempResultImg, ColorConversionCodes.GRAY2BGR);
                    }
                    else if (origin_image.Channels() == 3)
                    {
                        tempResultImg = origin_image.Clone();
                    }

                    tempResultImg = image_process.ProcessContours(tempResultImg, distanceImage, thresholdImage, contours, Global.minArea_value, Global.maxArea_value);
                    //Console.WriteLine($"[ProcessContours]ProcessContours Comp: {timer.Lap("main_Process").TotalMilliseconds} ms");


                    string tempResultProduct = "";
                    if (Global.tempImageDefectCheckFlag)
                    {
                         tempResultProduct = image_process.ProcessAndDisplayProducts(origin_image, pictureBox2, pictureBox3);
                    }
                    

                    Global.tempImageCheckFlag = false;

                    image_process.ProductLabel(tempResultImg);

                    image_process.AddTempProductInfo(test_flag);
                    pmManager.TempIncrementCounts(test_flag);

                    pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(tempResultImg);

                    test_flag = !test_flag;
                    DisplayProcessedResults();
                    return;
                }


                Mat resultImg = new Mat();
                //Cv2.CvtColor(origin_image, resultImg, ColorConversionCodes.GRAY2BGR);
                if (origin_image.Channels() == 1)
                {
                    Cv2.CvtColor(origin_image, resultImg, ColorConversionCodes.GRAY2BGR);
                }
                else if (origin_image.Channels() == 3)
                {
                    resultImg = origin_image.Clone();
                }

                resultImg = image_process.ProcessContours(resultImg, distanceImage, thresholdImage, contours, Global.minArea_value, Global.maxArea_value);
                //Console.WriteLine($"[ProcessContours]ProcessContours Comp: {timer.Lap("main_Process").TotalMilliseconds} ms");


                string resultProduct = image_process.ProcessAndDisplayProducts(origin_image, pictureBox2, pictureBox3);

                //Console.WriteLine($"[ProcessAndDisplayProducts]ProcessAndDisplayProducts Comp: {timer.Lap("main_Process").TotalMilliseconds} ms");

                //NG,OK Label 변경
                UpdateDetectResult();

                pmManager.IncrementCounts(resultProduct);
                //resultImg = image_process.ProductLabel(resultImg);
                image_process.ProductLabel(resultImg);

                DisplayProcessedResults();


                //Console.WriteLine($"[DisplayProcessedResults]DisplayProcessedResults Comp: {timer.Lap("main_Process").TotalMilliseconds} ms");

                //2024.10.31
                //image Resize 처리
                //pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(resultImg);
                pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image_process.ResizeImage(resultImg));



                string result_msg = image_process.CreateMessageFromPickedProducts();
                


                image_process.SaveMatImageWithDateFolder(resultImg, "pre");
                //Console.WriteLine($"[SaveMatImageWithDateFolder]

                if (!"".Equals(result_msg))
                {
                    Global.feederViveCount = 0;
                    send_IA_Msg(result_msg);

                    //message 처리 시간을 위해 sleep
                    
                    
                }
                //Picking 가능 대상이 없을 경우 feeder 진동 수행
                else
                {
                    if (Global.programTestFlag)
                    {
                        Global.ioCheckFlag = false;
                        Global.processFlag = false;
                        Global.programTestFlag = false;
                    } 
                    else
                    {
                        Global.feederViveCount++;
                        if (Global.feederViveCount < Global.maxFeederViveCount)
                        {
                            //2024.11.1
                            //contour 객체가 많을 경우 skip
                            if (Global.contourCount < 10)
                            {
                                SerialCommunication.Motor_SendSineMessage(Global.feederVivePower);
                                Thread.Sleep(Global.feederViveDulation);
                                SerialCommunication.Motor_SendStopMessage();
                                Thread.Sleep(Global.feederAfterGrabDuration);

                            }
                            else
                            {
                                Console.WriteLine($"contour Count Many => feeder vive Skip : {Global.contourCount}");
                            }
                            Global.ioCheckFlag = false;
                            Global.processFlag = false;

                        }
                        //feeder 진동 5회 이상 일 경우 제품 투입
                        else
                        {
                            Global.feederViveCount = 0;

                            Task.Run(async () =>
                            {
                                await HopperProcessAsync();
                            });

                        }
                    }
                    
                }
                //timer.Stop("main_Process");

                //origin_image = null;
                //gamma_image = null;
                Global.originImageRefreshFlag = false;
                Global.gammaImageRefreshFlag = false;
                Global.detectLeft = true;
                Global.detectRight = true;
            }));
        }

        private async Task HopperProcessAsync()
        {
            // call_hopper_motor 완료를 기다림
            await call_hopper_motor().ConfigureAwait(false);

            SystemLogMessage("Motor_SendSineMessage start -");
            SerialCommunication.Motor_SendSineMessage(Global.feederVivePower);
            await Task.Delay(Global.feederViveDulation);

            SystemLogMessage("Motor_SendStopMessage start -");
            SerialCommunication.Motor_SendStopMessage();
            await Task.Delay(Global.feederAfterGrabDuration);

            SystemLogMessage("Motor_SendStopMessage end -");
            Global.ioCheckFlag = false;
            Global.processFlag = false;
        }

        // 이미지 처리 후 결과를 표시하는 메서드
        private void DisplayProcessedResults()
        {
            // 이미지 처리 결과를 가져옴
            //List<Image_Process.Product> products = image_process.productInfoList;

            // TableLayoutPanel에 결과 표시
            //image_process.DisplaySortedProductInfo(tableLayoutPanel1, products);
            this.Invoke(new Action(delegate ()
            {

                Console.WriteLine($"productInfoList count: {image_process.productInfoList.Count}");
                Console.WriteLine($"pmManager ProductCounts count: {pmManager.ProductCounts.Count}");

                productDisplay.DisplaySortedProductInfo(image_process.productInfoList);
                
                productDisplay.DisplayProductCountInfo(pmManager);

                //productDisplay.reFreshTable();


            }));
            
        }

        private async void call_init_linear_motor()
        {
            await init_linear_motor();
        }

      

        private async Task init_linear_motor()
        {
            //Thread.Sleep(100);
            SerialCommunication.Motor_SendFreeMessage(">on\r");
            //Thread.Sleep(50);
            await Task.Delay(50);
            SerialCommunication.Motor_SendFreeMessage(">hm\r");
            //Thread.Sleep(1000);
            await Task.Delay(1000);
            SerialCommunication.Motor_SendFreeMessage(">ma -10000\r");
            //Thread.Sleep(100);
            await Task.Delay(100);
            SerialCommunication.Motor_SetViveParameterMessage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ApplyThreshold();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SerialCommunication.Motor_SendSineMessage(Global.feederVivePower);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SerialCommunication.Motor_SendStopMessage();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //Red
            if (radioButton3.Checked)
            {
                SerialCommunication.LED_SendFreeMessage("W");
            }
            //Green
            else if (radioButton2.Checked)
            {
                SerialCommunication.LED_SendFreeMessage("G");
            }
            //Blue
            else if (radioButton1.Checked)
            {
                SerialCommunication.LED_SendFreeMessage("B");
            }
            else
            {
                SerialCommunication.LED_SendFreeMessage("X");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            SerialCommunication.LED_SendFreeMessage("X");
        }
        public bool InitIO()
        {
            try
            {
                
                var nRet = USBDASK.UD_Register_Card(USBDASK.USB_7230, Global.dask_card_num);
                if (nRet == 0)
                {
                    Console.WriteLine("IO 연결 완료");
                } else
                {
                    Console.WriteLine("IO 연결 실패 nRet : " +nRet);
                }
                

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public void grab_image_trigger(bool originYN)
        {
            Global.detectFlag = true;
            //Thread.Sleep(10);
            if (originYN)
            {
                //3번째 변수가 do port number
                Console.WriteLine("origin_image grab start");
                origin_image = null;
                Global.cameraGrab = true;
                Global.gammaGrab = false;
                short ret;
                //main_camera.Parameters[PLCamera.GammaEnable].SetValue(false);
                //SerialCommunication.LED_SendFreeMessage("X");
                //Thread.Sleep(20);
                Thread.Sleep(10);
                //await Task.Delay(10);
                ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 0, 1);

                //short gab_detect = 0;
                ushort gab_detect;
                USBDASK.UD_DO_ReadLine(Global.dask_card_num, Global.dask_port_num, 0, out gab_detect);


                Thread.Sleep(Global.originImageExposeTime / 1000 + 140);

                Console.WriteLine($"grab  test result on trigger: {gab_detect}");

                ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 0, 0);


               
                USBDASK.UD_DO_ReadLine(Global.dask_card_num, Global.dask_port_num, 0, out gab_detect);


                Console.WriteLine($"grab  test result  off trigger: {gab_detect}");

                // Console.

                Console.WriteLine("origin_image grab end");

                if (origin_image == null)
                {
                    //3번째 변수가 do port number
                    Console.WriteLine("origin_image grab restart");
                    Global.cameraGrab = true;
                    Global.gammaGrab = false;

                    //main_camera.Parameters[PLCamera.GammaEnable].SetValue(false);
                    //SerialCommunication.LED_SendFreeMessage("X");
                    Thread.Sleep(10);
                    //await Task.Delay(10);
                    ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 0, 1);
                    Thread.Sleep(Global.originImageExposeTime / 1000 + 140);

                    USBDASK.UD_DO_ReadLine(Global.dask_card_num, Global.dask_port_num, 0, out gab_detect);


                    Console.WriteLine($"grab  test result  trigger on : {gab_detect}");
                    //await Task.Delay(80);
                    //Thread.Sleep(40);
                    ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 0, 0);

                    USBDASK.UD_DO_ReadLine(Global.dask_card_num, Global.dask_port_num, 0, out gab_detect);
                    Console.WriteLine($"grab  test result  trigger off : {gab_detect}");


                    Console.WriteLine("origin_image grab restart end");

                }
            }
            else
            {
                if (origin_image == null && !Global.imageCheckFlag)
                {
                    grab_image_trigger(true);
                }
                gamma_image = null;
                Console.WriteLine("gamma_image grab start");
                //3번째 변수가 do port number
                Global.cameraGrab = true;
                Global.gammaGrab = true;
                //main_camera.Parameters[PLCamera.GammaEnable].SetValue(true);

                //SerialCommunication.LED_SendFreeMessage("B");
                SerialCommunication.LED_SendFreeMessage("W");
                Thread.Sleep(10);
                //await Task.Delay(10);
                short ret;
                ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 0, 1);
                Thread.Sleep(Global.gammaImageExposeTime / 1000 + 40);
                //await Task.Delay(80);
                //Thread.Sleep(40);
                ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 0, 0);
                //cameraGrab = false;
                //grab_image();
                SerialCommunication.LED_SendFreeMessage("X");

                Console.WriteLine("gamma_image grab end");

                if (gamma_image == null)
                {
                    Console.WriteLine("gamma_image grab re-start");
                    //3번째 변수가 do port number
                    Global.cameraGrab = true;
                    Global.gammaGrab = true;
                    //main_camera.Parameters[PLCamera.GammaEnable].SetValue(true);

                    //SerialCommunication.LED_SendFreeMessage("B");
                    SerialCommunication.LED_SendFreeMessage("W");
                    Thread.Sleep(10);
                    //await Task.Delay(10);
                    ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 0, 1);
                    Thread.Sleep(Global.gammaImageExposeTime / 1000 + 40);
                    //await Task.Delay(80);
                    //Thread.Sleep(40);
                    ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 0, 0);
                    //cameraGrab = false;
                    //grab_image();
                    SerialCommunication.LED_SendFreeMessage("X");


                    Console.WriteLine("gamma_image grab re-start end");

                    //image grab failed -> loop restart
                    if (origin_image == null || gamma_image == null)
                    {
                        Global.processFlag = false;
                    }

                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            grab_image_trigger(true);
        }

      
        private async Task call_hopper_motor()
        {
            vive_hopper();
            Console.WriteLine("hopper start");
            await Task.Delay(Global.HopperViveDulation);
            vive_hopper();
            Console.WriteLine("hopper end");
            Global.waitFeederFlag = false;

            await Task.Delay(500);
        }

        private async void vive_hopper()
        {
            //3번째 변수가 do port number
            
            short ret;

            if (!Global.HopperFlag)
            {
                ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 2, 1);
                if (ret == 0)
                {
                    Console.WriteLine($"USB DASK Success - > {ret}");
                }
                else
                {
                    Console.WriteLine($"USB DASK Failed - > {ret}");
                }
                Global.HopperFlag = true;
            }
            else 
            {
                ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 2, 0);


                if (ret == 0)
                {
                    Console.WriteLine($"USB DASK Success - > {ret}");
                    await Task.Delay(100);
                    ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 2, 0);
                }
                else
                {
                    Console.WriteLine($"USB DASK Failed - > {ret}");
                    //Thread.Sleep(100);
                    await Task.Delay(100);
                    ret = USBDASK.UD_DO_WriteLine(Global.dask_card_num, Global.dask_port_num, 2, 0);
                }
                Global.HopperFlag = false;
            }

            //Thread.Sleep(Global.HopperViveDulation);
            //await Task.Delay(Global.HopperViveDulation);
            
                        
        }

        private void button4_Click(object sender, EventArgs e)
        {
            call_hopper_motor().ConfigureAwait(false);
        }

        

        private void button5_Click(object sender, EventArgs e)
        {
            grab_image_trigger(false);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //timer.Start("main_Process");
            Global.processFlag = true;
            Global.detectFlag = true;
            Global.programTestFlag = true;
            Thread.Sleep(10);
            grab_image_trigger(true);
            //Console.WriteLine($"[origin_image]grab_image_trigger Complete: {timer.Lap("main_Process").TotalMilliseconds} ms");
            Thread.Sleep(50);
            grab_image_trigger(false);
            
        }


        int i = 0;
        private void button7_Click(object sender, EventArgs e)
        {
            //IA_socket.InitializeClient();
            if (i% 2 == 0)
            {
                //IA_socket.SendMessageIAMsg(",0,123.123,123.470,-76.342,123.339,123.470,123.338,\n");
                IA_socket.SendMessageIAMsg(",4,123.123,123.470,-76.342,123.339,123.470,123.338,\n");
            } else
            {
                IA_socket.SendMessageIAMsg(",2,456.123,456.470,-76.342,456.339,456.470,456.338,\n");
            }
            i++;
            
        }

        public async void send_IA_Msg(string msg)
        {
            //IA_socket = new SocketClient();
            //IA_socket.InitializeClient();
            if (!Global.socketOpen)
            {
                Console.WriteLine("SocketOpen");
                IA_socket.InitializeClient();
                Global.socketOpen = true;
                //Thread.Sleep(1000);
                await Task.Delay(1000);
            }
            
            IA_socket.SendMessageIAMsg(msg);
            //Thread.Sleep(1000);
            //IA_socket.CloseConnection();
            //Thread.Sleep(1000);
            //Thread.Sleep(50);
            Console.WriteLine("Send Message Complete -> Delay Start");
            await Task.Delay(5000);
            Global.ioCheckFlag = false;
            Global.processFlag = false;
            Console.WriteLine("Send Message Complete -> Delay end");


        }

        private void button12_Click(object sender, EventArgs e)
        {
            IA_socket.CloseConnection();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            IA_socket.InitializeClient();
            
        }

        private void button14_Click(object sender, EventArgs e)
        {
            IOTriggerThread();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        public void DrawBorderOnPictureBox(PictureBox pictureBox, Color borderColor, float borderWidth)
        {
            using (Graphics g = pictureBox.CreateGraphics())
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                g.DrawRectangle(pen, new Rectangle(0, 0, pictureBox.Width - 1, pictureBox.Height - 1));
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            pmManager = new ProductManager();

            this.Invoke(new Action(delegate ()
            {
                productDisplay.DisplayProductCountInfo(pmManager);
            }));
        }

        

        public void UpdateStatus(int index, bool isActive)
        {
            /*if (index >= 0 && index < indicators.Length)
            {
                indicators[index].IsActive = isActive;
                indicators[index].Invalidate(); // 컨트롤 다시 그리기
            }*/
            this.Invoke(new Action(delegate ()
            {

                if (index == 0)
                {
                    if (isActive)
                    {
                        label25.Text = "ON";
                        label25.BackColor = Color.Green;
                    }
                    else
                    {
                        label25.Text = "OFF";
                        label25.BackColor = Color.Red;
                    }
                }

                if (index == 1)
                {
                    if (isActive)
                    {
                        label24.Text = "ON";
                        label24.BackColor = Color.Green;
                    }
                    else
                    {
                        label24.Text = "OFF";
                        label24.BackColor = Color.Red;
                    }
                }

                if (index == 2)
                {
                    if (isActive)
                    {
                        label23.Text = "ON";
                        label23.BackColor = Color.Green;
                    }
                    else
                    {
                        label23.Text = "OFF";
                        label23.BackColor = Color.Red;
                    }
                }

                if (index == 3)
                {
                    if (isActive)
                    {
                        label22.Text = "ON";
                        label22.BackColor = Color.Green;
                    }
                    else
                    {
                        label22.Text = "OFF";
                        label22.BackColor = Color.Red;
                    }
                }
            }));   
        }

        public void UpdateDetectResult()
        {
           
            this.Invoke(new Action(delegate ()
            {

                if (Global.detectLeft)
                {
                    
                    label26.Text = "OK";
                    label26.BackColor = Color.Blue;
                    label26.Visible = true;    
                } 
                else
                {
                    label26.Text = "NG";
                    label26.BackColor = Color.Red;
                    label26.Visible = true;
                }

                if (Global.detectRight)
                {
                    label27.Text = "OK";
                    label27.BackColor = Color.Blue;
                    label27.Visible = true;
                }
                else
                {
                    label27.Text = "NG";
                    label27.BackColor = Color.Red;
                    label27.Visible = true;
                }

            }));
        }

       

        private enum ValueType
        {
            Integer,
            Double
        }

        private class ControlPair
        {
            public TrackBar TrackBar { get; set; }
            public TextBox TextBox { get; set; }
            public Func<object> GetValue { get; set; }
            public Action<object> SetValue { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
            public ValueType Type { get; set; }
            public int Scale { get; set; }
        }

        private List<ControlPair> controlPairs = new List<ControlPair>();

        public void InitializeControls(Form form)
        {
            // TrackBar와 TextBox 쌍 초기화
            //image Parameter
            InitializeControlPair(form, "trackBar1", "textBox1", () => Global.binaryThresh, value => Global.binaryThresh = (int)value, 0, 255, ValueType.Integer, 1);
            InitializeControlPair(form, "trackBar2", "textBox2", () => Global.binaryThresh2, value => Global.binaryThresh2 = (int)value, 0, 255, ValueType.Integer, 1);
            InitializeControlPair(form, "trackBar3", "textBox3", () => Global.defectbinaryThresh, value => Global.defectbinaryThresh = (int)value, 0, 255, ValueType.Integer, 1);
            InitializeControlPair(form, "trackBar4", "textBox4", () => Global.defectArea, value => Global.defectArea = (int)value, 0, 500, ValueType.Integer, 1);
            InitializeControlPair(form, "trackBar5", "textBox5", () => Global.detectLimitSize,value => Global.detectLimitSize = (int)value,0, 265, ValueType.Integer, 1);
            //InitializeControlPair(form, "trackBar17", "textBox17", () => Global.originImageExposeTime, value => Global.originImageExposeTime = (int)value, 20000, 100000, ValueType.Integer, 10000);
            //InitializeControlPair(form, "trackBar18", "textBox18", () => Global.gammaImageExposeTime, value => Global.gammaImageExposeTime = (int)value, 10000, 50000, ValueType.Integer, 10000);
            InitializeTextControlPair(form, "textBox17", () => Global.originImageExposeTime, value => Global.originImageExposeTime = (int)value, 20000, 100000, ValueType.Integer, 1000);
            InitializeTextControlPair(form, "textBox18", () => Global.gammaImageExposeTime, value => Global.gammaImageExposeTime = (int)value, 10000, 50000, ValueType.Integer, 1000);
            InitializeTextControlPair(form, "textBox19", () => Global.xMoveCoodinate, value => Global.xMoveCoodinate = (int)value, 0, 1100, ValueType.Integer, 1);
            InitializeTextControlPair(form, "textBox20", () => Global.yMoveCoodinate, value => Global.yMoveCoodinate = (int)value, 0, 1100, ValueType.Integer, 1);

            // Motor Parameter
            InitializeControlPair(form, "trackBar6", "textBox6",
                () => Global.feederViveMaxHeight,
                value => Global.feederViveMaxHeight = (int)value,
                0, 9000, ValueType.Integer, 1);

            InitializeControlPair(form, "trackBar7", "textBox7",
                () => Global.feederViveMinHeight,
                value => Global.feederViveMinHeight = (int)value,
                10000, 70000, ValueType.Integer, 1);

            InitializeControlPair(form, "trackBar8", "textBox8",
                () => Global.feederViveDulation,
                value => Global.feederViveDulation = (int)value,
                0, 2000, ValueType.Integer, 1);

            InitializeControlPair(form, "trackBar9", "textBox9",
                () => Global.feederVivePower,
                value => Global.feederVivePower = (int)value,
                0, 50, ValueType.Integer, 1);

            InitializeControlPair(form, "trackBar10", "textBox10",
                () => Global.maxFeederViveCount,
                value => Global.maxFeederViveCount = (int)value,
                0, 10, ValueType.Integer, 1);

            InitializeControlPair(form, "trackBar11", "textBox11",
                () => Global.HopperViveDulation,
                value => Global.HopperViveDulation = (int)value,
                10, 5000, ValueType.Integer, 1);

            InitializeControlPair(form, "trackBar16", "textBox16",
               () => Global.feederAfterGrabDuration,
               value => Global.feederAfterGrabDuration = (int)value,
               10, 2000, ValueType.Integer, 1);

            // Robot Parameter
            InitializeControlPair(form, "trackBar12", "textBox12",
                () => Global.xPixelRate,
                value => Global.xPixelRate = (double)value,
                -1, 1, ValueType.Double, 1000);

            InitializeControlPair(form, "trackBar13", "textBox13",
                () => Global.yPixelRate,
                value => Global.yPixelRate = (double)value,
                -1, 1, ValueType.Double, 1000);

            InitializeControlPair(form, "trackBar14", "textBox14",
                () => Global.x_offset,
                value => Global.x_offset = (double)value,
                0, 500, ValueType.Double, 1000);

            InitializeControlPair(form, "trackBar15", "textBox15",
                () => Global.y_offset,
                value => Global.y_offset = (double)value,
                0, 500, ValueType.Double, 1000);


            // 필요한 만큼 추가 TrackBar/TextBox 쌍 초기화...

        }

        private void InitializeControlPair(Form form, string trackBarName, string textBoxName,
        Func<object> getValue, Action<object> setValue, double min, double max, ValueType type, int scale)
        {
           
            var trackBar = form.Controls.Find(trackBarName, true).FirstOrDefault() as TrackBar;
            var textBox = form.Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

            if (trackBar != null && textBox != null)
            {
                var pair = new ControlPair
                {
                    TrackBar = trackBar,
                    TextBox = textBox,
                    GetValue = getValue,
                    SetValue = setValue,
                    Min = min,
                    Max = max,
                    Type = type,
                    Scale = scale
                };

                var initialValue = getValue();
                trackBar.Minimum = 0;
                trackBar.Maximum = (int)((max - min) * scale);
                trackBar.Value = ValueToTrackBar(Convert.ToDouble(initialValue), min, scale);
                textBox.Text = initialValue.ToString();

                trackBar.Scroll += (sender, e) => TrackBar_Scroll(pair);
                textBox.TextChanged += (sender, e) => TextBox_TextChanged(pair,true);

                controlPairs.Add(pair);
            }
        }

        private void InitializeTextControlPair(Form form,string textBoxName,
       Func<object> getValue, Action<object> setValue, double min, double max, ValueType type, int scale)
        {

           
            var textBox = form.Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

            if (textBox != null)
            {
                var pair = new ControlPair
                {
                    TrackBar = null,
                    TextBox = textBox,
                    GetValue = getValue,
                    SetValue = setValue,
                    Min = min,
                    Max = max,
                    Type = type,
                    Scale = scale
                };

                var initialValue = getValue();
                textBox.Text = initialValue.ToString();
                textBox.TextChanged += (sender, e) => TextBox_TextChanged(pair,false);

                controlPairs.Add(pair);
            }
        }

        private void TrackBar_Scroll(ControlPair pair)
        {
            double value = TrackBarToValue(pair.TrackBar.Value, pair.Min, pair.Scale);
            pair.TextBox.Text = value.ToString();
            SetValueSafely(pair, value);
            if ("trackBar1".Equals(pair.TrackBar.Name) || "trackBar2".Equals(pair.TrackBar.Name))
            {
                ApplyThreshold();
            } else
            {
                Console.WriteLine($"pair.TrackBar.Name : {pair.TrackBar.Name} ,pair.TextBox.Name : {pair.TextBox.Name}");
            }
        }

        private void TextBox_TextChanged(ControlPair pair,bool trackBarYN)
        {
            if (TryParseValue(pair.TextBox.Text, pair.Type, out object value))
            {
                if ("textBox19".Equals(pair.TextBox.Name) || "textBox20".Equals(pair.TextBox.Name))
                {
                    int reminder = (int)value % 2;
                    int result_value = 0;
                    if (reminder == 0)
                    {
                        result_value = (int)value;
                    }
                    else
                    {
                        result_value = (int)value + 1 ;
                    }


                    if (result_value >= pair.Min && result_value <= pair.Max)
                    {
                        SetValueSafely(pair, value);
                    }
                }
                else
                {
                    double doubleValue = Convert.ToDouble(value);
                    if (doubleValue >= pair.Min && doubleValue <= pair.Max)
                    {
                        if (trackBarYN)
                        {
                            pair.TrackBar.Value = ValueToTrackBar(doubleValue, pair.Min, pair.Scale);
                        }
                        SetValueSafely(pair, value);
                    }
                }
                
               
            }
            if (trackBarYN)
            {
                if ("trackBar1".Equals(pair.TrackBar.Name) || "trackBar2".Equals(pair.TrackBar.Name))
                {
                    ApplyThreshold();
                }
                else
                {
                    Console.WriteLine($"pair.TrackBar.Name : {pair.TrackBar.Name} ,pair.TextBox.Name : {pair.TextBox.Name}");
                }
            }
            
        }

        private void SetValueSafely(ControlPair pair, object value)
        {
            try
            {
                if (pair.Type == ValueType.Integer)
                {
                    pair.SetValue(Convert.ToInt32(value));
                }
                else
                {
                    pair.SetValue(Convert.ToDouble(value));
                }
                SaveSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting value: {ex.Message}");
            }
        }

        private bool TryParseValue(string text, ValueType type, out object result)
        {
            if (type == ValueType.Integer)
            {
                if (int.TryParse(text, out int intValue))
                {
                    result = intValue;
                    return true;
                }
            }
            else
            {
                if (double.TryParse(text, out double doubleValue))
                {
                    result = doubleValue;
                    return true;
                }
            }
            result = null;
            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SerialCommunication.Motor_SetViveParameterMessage();
        }

        private int ValueToTrackBar(double value, double min, int scale)
        {
            return (int)Math.Round((value - min) * scale);
        }

        private double TrackBarToValue(int trackBarValue, double min, int scale)
        {
            return (trackBarValue / (double)scale) + min;
        }

        private void checkTextBoxValue()
        {
            this.Invoke(new Action(delegate ()
            {
                textBox19.Text = Global.xMoveCoodinate.ToString();
                textBox20.Text = Global.yMoveCoodinate.ToString();

            }));
        }


        private static string SettingsFilePath = Global.cfg_fath + "/globalsettings.json";

        public static void SaveSettings()
        {
            var settings = new
            {
                Global.binaryThresh,
                Global.binaryThresh2,
                Global.defectbinaryThresh,
                Global.defectArea,
                Global.detectLimitSize,
                Global.originImageExposeTime,
                Global.gammaImageExposeTime,
                Global.xMoveCoodinate,
                Global.yMoveCoodinate,
                Global.feederViveMaxHeight,
                Global.feederViveMinHeight,
                Global.feederViveDulation,
                Global.feederAfterGrabDuration,
                Global.maxFeederViveCount,
                Global.feederVivePower,
                Global.HopperViveDulation,
                Global.xPixelRate,
                Global.yPixelRate,
                Global.x_offset,
                Global.y_offset
            };

            string jsonString = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsFilePath, jsonString);
            Console.WriteLine("Settings saved successfully.");
        }

        public static void LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                string jsonString = File.ReadAllText(SettingsFilePath);
                JObject settings = JObject.Parse(jsonString);

                Global.binaryThresh = (int)settings["binaryThresh"];
                Global.binaryThresh2 = (int)settings["binaryThresh2"];
                Global.defectbinaryThresh = (int)settings["defectbinaryThresh"];
                Global.defectArea = (int)settings["defectArea"];
                Global.detectLimitSize = (int)settings["detectLimitSize"];
                Global.originImageExposeTime = (int)settings["originImageExposeTime"];
                Global.gammaImageExposeTime = (int)settings["gammaImageExposeTime"];
                Global.xMoveCoodinate = (int)settings["xMoveCoodinate"];
                Global.yMoveCoodinate = (int)settings["yMoveCoodinate"];
                Global.feederViveMaxHeight = (int)settings["feederViveMaxHeight"];
                Global.feederViveMinHeight = (int)settings["feederViveMinHeight"];
                Global.feederViveDulation = (int)settings["feederViveDulation"];
                Global.feederAfterGrabDuration = (int)settings["feederAfterGrabDuration"];
                Global.maxFeederViveCount = (int)settings["maxFeederViveCount"];
                Global.feederVivePower = (int)settings["feederVivePower"];
                Global.HopperViveDulation = (int)settings["HopperViveDulation"];
                Global.xPixelRate = (double)settings["xPixelRate"];
                Global.yPixelRate = (double)settings["yPixelRate"];
                Global.x_offset = (double)settings["x_offset"];
                Global.y_offset = (double)settings["y_offset"];
                

                Console.WriteLine("Settings loaded successfully.");
            }
            else
            {
                Console.WriteLine("Settings file not found. Using default values.");
            }
        }

        private static readonly object _lock = new object();

        private void button15_Click(object sender, EventArgs e)
        {
            ApplyThreshold();
        }

        bool test_flag = true;
        private void button16_Click(object sender, EventArgs e)
        {

            /*
             임시 데이터 생성
             */
            image_process.AddTempProductInfo(test_flag);
            pmManager.TempIncrementCounts(test_flag);

            test_flag = !test_flag;
            DisplayProcessedResults();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            ApplyThreshold();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Global.tempImageDefectCheckFlag = true;
            ApplyThreshold();
            Global.tempImageDefectCheckFlag = false;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            PictureBox clickedPictureBox = (PictureBox)sender;
            if (clickedPictureBox.Image != null)
            {
                // 이미지의 복사본을 생성하여 전달
                using (System.Drawing.Image imageCopy = (System.Drawing.Image)clickedPictureBox.Image.Clone())
                {
                    using (ImagePopupForm popupForm = new ImagePopupForm(imageCopy))
                    {
                        popupForm.ShowDialog();
                    }
                }
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            PictureBox clickedPictureBox = (PictureBox)sender;
            if (clickedPictureBox.Image != null)
            {
                // 이미지의 복사본을 생성하여 전달
                using (System.Drawing.Image imageCopy = (System.Drawing.Image)clickedPictureBox.Image.Clone())
                {
                    using (ImagePopupForm popupForm = new ImagePopupForm(imageCopy))
                    {
                        popupForm.ShowDialog();
                    }
                }
            }
        }

        private void button_ori_Img_check_Click(object sender, EventArgs e)
        {
            //timer.Start("main_Process");
            Global.processFlag = true;
            Global.detectFlag = true;
            Global.programTestFlag = true;
            Global.imageCheckFlag = true;
            Thread.Sleep(10);
            grab_image_trigger(true);
            
            //Console.WriteLine($"[origin_image]grab_image_trigger Complete: {timer.Lap("main_Process").TotalMilliseconds} ms");
            //Thread.Sleep(50);
            //grab_image_trigger(false);

        }

        private void button_gam_Img_check_Click(object sender, EventArgs e)
        {
            //timer.Start("main_Process");
            Global.processFlag = true;
            Global.detectFlag = true;
            Global.programTestFlag = true;
            Global.imageCheckFlag = true;
            Thread.Sleep(10);
            grab_image_trigger(false);
            
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            Global.processFlag = true;
            Global.detectFlag = true;
            Global.programTestFlag = true;
            Global.imageCheckFlag = true;
            Global.autoImageCoodinateFlag = true;
            Thread.Sleep(10);
            grab_image_trigger(false);
            checkTextBoxValue();
            Global.processFlag = true;
            Global.detectFlag = true;
            Global.programTestFlag = true;
            Global.imageCheckFlag = true;
            Global.autoImageCoodinateFlag = false;
            Thread.Sleep(100);
            //Thread.Sleep(10);
            grab_image_trigger(false);


        }

        private const string BaseFolder = "./SystemLog";

        public static void SystemLogMessage(string message)
        {
            try
            {
                DateTime now = DateTime.Now;
                string fileName = now.ToString("yyyy-MM-dd") + ".txt";
                string fullPath = Path.Combine(BaseFolder, fileName);

                if (!Directory.Exists(BaseFolder))
                {
                    Directory.CreateDirectory(BaseFolder);
                }

                string logEntry = $"[{now:yyyy-MM-dd HH:mm:ss}] {message}";

                lock (_lock)
                {
                    File.AppendAllText(fullPath, logEntry + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging message: {ex.Message}");
                SystemLogMessage($"Error logging message: {ex.Message}");
            }
        }
    }

    public class Global
    {
        public static bool cameraGrab = false;
        public static bool gammaGrab = false;
        public static bool programRunningFlag = true;
        public static bool programTestFlag = false;
        public static bool imageCheckFlag = false;
        public static bool autoImageCoodinateFlag = false;

        public static bool detectFlag = false;
        public static bool detectI = true;
        public static bool detectA = true;
        public static bool detectP = true;

        public static bool socketOpen = false;
        public static bool SerialPortOpen = false;
        public static bool ioCheckFlag = false;
        public static bool processFlag = false;
        public static bool HopperFlag = false;
        public static bool waitFeederFlag = false;

        public static bool tempImageCheckFlag = false;
        public static bool tempImageDefectCheckFlag = false;
        public static bool originImageRefreshFlag = false;
        public static bool gammaImageRefreshFlag = false;


        public static double x_offset = 96.737;
        public static double y_offset = 389.628;

        public static double xPixelRate = -0.131818;
        public static double yPixelRate = 0.131818;


        public static int robot_x_min = -12;
        public static int robot_x_max = 90;
        public static int robot_y_min = 400;
        public static int robot_y_max = 525;

        
        public static int feederViveCount = 0;
        public static int maxFeederViveCount = 3;
        public static int feederVivePower = 25;
        public static int feederViveMaxHeight = 8000;
        public static int feederViveMinHeight = 35000;
        public static int feederViveDulation = 500;

        public static int HopperViveDulation = 3000;

        public static int detectLimitSize = 0;

        public static int binaryThresh = 161;
        public static int binaryThresh2 = 110;

        public static int defectbinaryThresh = 80;
        public static int defectArea = 300;

        public static ushort dask_card_num = 0;
        public static ushort dask_port_num = 0;

        public static int feederAfterGrabDuration = 300;

        public static int originImageExposeTime = 105000;
        public static int gammaImageExposeTime = 33000;
        public static int xMoveCoodinate = 724;
        public static int yMoveCoodinate = 486;


        public static int contourCount = 0;

        public static Point2f topLeft;
        public static Point2f bottomRight;

        public static bool detectLeft = true;
        public static bool detectRight = true;

        //double minArea_value = 14000; //8800
        //public static double minArea_value = 5000; //8800
        public static double minArea_value = 4800; //8800
        //public static double maxArea_value = 12000; //10800
        //public static double maxArea_value = 13000; //10800
        public static double maxArea_value = 12800; //10800

        public static string cfg_fath = "c:\\workspace\\ia_demo\\WindowsFormsApp2";
    }

    



}
