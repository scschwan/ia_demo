using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{

    public class SocketClient
    {
        private TcpClient clientSocket;
        private NetworkStream stream;
        private const string HOST = "192.168.50.3";
        //private const string HOST = "127.0.0.1";
        private const int PORT = 64512;
        private Thread receiveThread;
        private const int TIMEOUT_MS = 10000; // 10 seconds timeout

        private static readonly object _lock = new object();
        private const string BaseFolder = "./TcpLog";

        public SocketClient()
        {
            //InitializeClient();
        }

        public void  InitializeClient()
        {
            try
            {
                clientSocket = new TcpClient();
                IAsyncResult result = clientSocket.BeginConnect(HOST, PORT, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(TIMEOUT_MS, true);

                if (!success)
                {
                    clientSocket.Close();
                    throw new TimeoutException("Connection attempt timed out after 10 seconds.");
                }

                clientSocket.EndConnect(result);
                clientSocket.SendTimeout = TIMEOUT_MS;
                clientSocket.ReceiveTimeout = TIMEOUT_MS;

                stream = clientSocket.GetStream();

                if (!Global.socketOpen)
                {
                    Console.WriteLine("SocketOpen");
                    Global.socketOpen = true;
                }
                //receiveThread = new Thread(new ThreadStart(ReceiveMessages));
                //receiveThread.Start();
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show($"Connection timed out: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}");
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"IA KOREA Receive Message : {receivedMessage}\n");
                }
                catch (IOException ex) when (ex.InnerException is SocketException socketException && socketException.SocketErrorCode == SocketError.TimedOut)
                {
                    Console.WriteLine("Receive operation timed out after 10 seconds.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error receiving message: {ex.Message}");
                    break;
                }
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                Console.WriteLine($"IA KOREA Send Message : {message}\n");
                stream.Write(buffer, 0, buffer.Length);

                LogMessage(message);
            }
            catch (IOException ex) when (ex.InnerException is SocketException socketException && socketException.SocketErrorCode == SocketError.TimedOut)
            {
                Console.WriteLine("Send operation timed out after 10 seconds.");
            }
            catch(IOException ex)
            {
                Console.WriteLine("IO Exception Exist");
                Console.WriteLine("Socket.open Restart");


                try
                {
                    clientSocket = new TcpClient();
                    IAsyncResult result = clientSocket.BeginConnect(HOST, PORT, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(TIMEOUT_MS, true);

                    if (!success)
                    {
                        clientSocket.Close();
                        throw new TimeoutException("Connection attempt timed out after 10 seconds.");
                    }

                    clientSocket.EndConnect(result);
                    clientSocket.SendTimeout = TIMEOUT_MS;
                    clientSocket.ReceiveTimeout = TIMEOUT_MS;

                    stream = clientSocket.GetStream();
                    //receiveThread = new Thread(new ThreadStart(ReceiveMessages));
                    //receiveThread.Start();

                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    Console.WriteLine($"IA KOREA Send Message : {message}\n");
                    stream.Write(buffer, 0, buffer.Length);
                    LogMessage(message);
                }
                catch (TimeoutException tex)
                {
                    MessageBox.Show($"Connection timed out: {tex.Message}");
                }
                catch (Exception cex)
                {
                    MessageBox.Show($"Error connecting to server: {cex.Message}");
                }

                

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        public void SendMessageIAMsg(string message)
        {
            SendMessage(message);
        }

      

        public static void LogMessage(string message)
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
            }
        }


        public void CloseConnection()
        {
            if (stream != null)
                stream.Close();
            if (clientSocket != null)
                clientSocket.Close();
            /*if (receiveThread != null && receiveThread.IsAlive)
                receiveThread.Abort();*/
        }
    }
}
