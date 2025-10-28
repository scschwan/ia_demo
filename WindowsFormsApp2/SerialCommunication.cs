using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    internal class SerialCommunication
    {
        private const string Motor_PortName = "COM4";
        private const int Motor_BaudRate = 115200; // 적절한 baud rate로 변경하세요
        private const int ReadTimeout = 1000; // 1초 타임아웃

        private const string LED_PortName = "COM5";
        private const int LED_BaudRate = 9600;

        public static void Motor_SendSineMessage(int frequency)
        {
            using (SerialPort port = new SerialPort(Motor_PortName, Motor_BaudRate))
            {
                try
                {
                    if (!Global.SerialPortOpen)
                    {
                        port.Open();
                    }
                    
                    string message = $">sine {frequency}\r";
                    port.Write(message);
                    Console.WriteLine($"Sent message: {message}");

                    // 응답 대기 및 읽기
                    StringBuilder response = new StringBuilder();
                    Thread.Sleep(10); // 장치가 응답할 시간을 줍니다

                    while (port.BytesToRead > 0)
                    {
                        int byteRead = port.ReadByte();
                        if (byteRead != -1)
                        {
                            response.Append((char)byteRead);
                        }
                    }

                    // 응답 출력
                    if (response.Length > 0)
                    {
                        Console.WriteLine($"Received response: {response}");
                    }
                    else
                    {
                        Console.WriteLine("No response received.");
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Timeout: No response received within the specified time.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in serial communication: {ex.Message}");
                }
            }
        }

        public static void Motor_SendStopMessage()
        {
            using (SerialPort port = new SerialPort(Motor_PortName, Motor_BaudRate))
            {
                try
                {
                    if (!Global.SerialPortOpen)
                    {
                        port.Open();
                    }
                    string message = ">st\r";
                    port.Write(message);
                    Console.WriteLine($"Sent message: {message}");

                    // 응답 대기 및 읽기
                    StringBuilder response = new StringBuilder();

                    Thread.Sleep(10); // 장치가 응답할 시간을 줍니다
                    while (port.BytesToRead > 0)
                    {
                        int byteRead = port.ReadByte();
                        if (byteRead != -1)
                        {
                            response.Append((char)byteRead);
                        }
                    }

                    // 응답 출력
                    if (response.Length > 0)
                    {
                        Console.WriteLine($"Received response: {response}");
                    }
                    else
                    {
                        Console.WriteLine("No response received.");
                    }

                    

                    //지정 위치 이동

                    message = ">ma -10000\r";
                    port.Write(message);
                    Console.WriteLine($"Sent message: {message}");

                    // 응답 대기 및 읽기
                    response = new StringBuilder();

                    Thread.Sleep(10); // 장치가 응답할 시간을 줍니다
                    while (port.BytesToRead > 0)
                    {
                        int byteRead = port.ReadByte();
                        if (byteRead != -1)
                        {
                            response.Append((char)byteRead);
                        }
                    }

                    // 응답 출력
                    if (response.Length > 0)
                    {
                        Console.WriteLine($"Received response: {response}");
                    }
                    else
                    {
                        Console.WriteLine("No response received.");
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Timeout: No response received within the specified time.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in serial communication: {ex.Message}");
                }
            }
        }

        public static void Motor_SetViveParameterMessage()
        {
            using (SerialPort port = new SerialPort(Motor_PortName, Motor_BaudRate))
            {
                try
                {
                    if (!Global.SerialPortOpen)
                    {
                        port.Open();
                    }

                    string message = "";
                    if (Global.feederViveMaxHeight > 0 )
                    {
                        message = ">sinep -" + Global.feederViveMinHeight + " -" + Global.feederViveMaxHeight + "\r";
                    } else
                    {
                        message = ">sinep -" + Global.feederViveMinHeight + " " + Global.feederViveMaxHeight + "\r";
                    }
                    port.Write(message);
                    Console.WriteLine($"Sent message: {message}");

                    // 응답 대기 및 읽기
                    StringBuilder response = new StringBuilder();
                    Thread.Sleep(10); // 장치가 응답할 시간을 줍니다

                    while (port.BytesToRead > 0)
                    {
                        int byteRead = port.ReadByte();
                        if (byteRead != -1)
                        {
                            response.Append((char)byteRead);
                        }
                    }

                    // 응답 출력
                    if (response.Length > 0)
                    {
                        Console.WriteLine($"Received response: {response}");
                    }
                    else
                    {
                        Console.WriteLine("No response received.");
                    }

                    
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Timeout: No response received within the specified time.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in serial communication: {ex.Message}");
                }
            }
        }


        public static void Motor_SendFreeMessage(String msg)
        {
            using (SerialPort port = new SerialPort(Motor_PortName, Motor_BaudRate))
            {
                try
                {
                    if (!Global.SerialPortOpen)
                    {
                        port.Open();
                    }
                    string message = msg;
                    port.Write(message);
                    Console.WriteLine($"Sent message: {message}");

                    // 응답 대기 및 읽기
                    StringBuilder response = new StringBuilder();
                    Thread.Sleep(10); // 장치가 응답할 시간을 줍니다

                    while (port.BytesToRead > 0)
                    {
                        int byteRead = port.ReadByte();
                        if (byteRead != -1)
                        {
                            response.Append((char)byteRead);
                        }
                    }

                    // 응답 출력
                    if (response.Length > 0)
                    {
                        Console.WriteLine($"Received response: {response}");
                    }
                    else
                    {
                        Console.WriteLine("No response received.");
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Timeout: No response received within the specified time.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in serial communication: {ex.Message}");
                }
            }
        }

        public static void LED_SendFreeMessage(String msg)
        {
            using (SerialPort port = new SerialPort(LED_PortName, LED_BaudRate))
            {
                try
                {
                    port.Open();
                    string message = msg;
                    port.Write(message);
                    Console.WriteLine($"Sent message: {message}");

                    // 응답 대기 및 읽기
                    StringBuilder response = new StringBuilder();
                    Thread.Sleep(10); // 장치가 응답할 시간을 줍니다

                    while (port.BytesToRead > 0)
                    {
                        int byteRead = port.ReadByte();
                        if (byteRead != -1)
                        {
                            response.Append((char)byteRead);
                        }
                    }

                    // 응답 출력
                    if (response.Length > 0)
                    {
                        Console.WriteLine($"Received response: {response}");
                    }
                    else
                    {
                        Console.WriteLine("No response received.");
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Timeout: No response received within the specified time.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in serial communication: {ex.Message}");
                }
            }
        }
    }
}