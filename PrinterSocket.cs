using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PJLconsole
{
    public class PrinterSocket
    {
        private Socket _socket;

        ~PrinterSocket()
        {
            _socket = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        protected bool InitializeSocket(string ipAddress, int port)
        {
            if (ipAddress == string.Empty)
                return false;

            IPEndPoint ipEndPoint = new IPEndPoint(Dns.GetHostEntry(ipAddress).AddressList[0], port);

            Console.WriteLine("Host is {0}", ipEndPoint.ToString());

            try
            {
                _socket = new Socket(
                                      AddressFamily.InterNetwork,
                                      SocketType.Stream,
                                      ProtocolType.Tcp
                                     );

                int sendTimeOut = _socket.SendTimeout;
                int receiveTimeOut = _socket.ReceiveTimeout;

                _socket.Connect(ipEndPoint);
            }
            catch
            {
                _socket = null;
            }

            return (_socket != null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="sendString"></param>
        /// <returns></returns>
        protected bool SendToPrinterSocket(string ipAddress, int port, string sendString)
        {
            if (ipAddress == string.Empty)
                return false;

            if (!InitializeSocket(ipAddress, port))
                return false;

            if (_socket != null)
            {
                byte[] sendData;
                sendData = Encoding.ASCII.GetBytes(sendString);

                int result = _socket.Send(sendData, sendData.Length, SocketFlags.None);
                if (result == 0)
                {
                    Console.WriteLine("Could not send on socket");
                }

                _socket.Close();
                _socket = null; 

                return (result > 0);
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="sendString"></param>
        /// <param name="receiveString"></param>
        /// <returns></returns>
        protected bool SendReceiveFromPrinterSocket(string ipAddress, int port, string sendString, out string receiveString)
        {
            receiveString = string.Empty;

            if (ipAddress == string.Empty)
                return false;

            if (!InitializeSocket(ipAddress, port))
                return false;

            if (_socket != null)
            {

                byte[] sendData;
                sendData = Encoding.ASCII.GetBytes(sendString);

                int result;
                result = _socket.Send(sendData, sendData.Length, 0);

                if (result == 0)
                {
                    Console.WriteLine("Could not send on socket");
                }

                byte[] receiveData = new Byte[1000];  //64
                result = _socket.Receive(receiveData);
                if (result == 0)
                {
                    Console.WriteLine("Could not receive on socket");
                }
                else
                {
                    // get a string but it has lots of \0 characters
                    receiveString = Encoding.ASCII.GetString(receiveData);
                    // remove the \0's
                    receiveString = receiveString.TrimEnd('\0');

                    //receiveString = receiveString.Replace("\f", "\0");
                    Console.WriteLine("Display: {0}", receiveString);
                }

                _socket.Close();
                _socket = null;

                return (result > 0);
            }

            return false;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class HPPrinterSocket : PrinterSocket
    {
        public const int PJL_PORT = 9100;

        public const string RDYMSGFormat = "\x1B%-12345X@PJL RDYMSG DISPLAY = \"{0}\"\r\n\x1B%-12345X\r\n";
        public const string StatusFormat = "\x1B%-12345X@PJL INFO STATUS\r\n\x1B%-12345X\r\n";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SetPrinterDisplay(string ipAddress, string message)
        {
            if (ipAddress == string.Empty)
                return false;

            string sendString = String.Format(RDYMSGFormat, message);

            if (!SendToPrinterSocket(ipAddress, PJL_PORT, sendString))
                return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public bool GetPrinterStatus(string ipAddress, out string receiveString)
        {
            receiveString = String.Empty;

            if (ipAddress == string.Empty)
                return false;

            string sendString = String.Format(StatusFormat);

            if (!SendReceiveFromPrinterSocket(ipAddress, PJL_PORT, sendString, out receiveString))
            {
                return false;
            }

            return true;
        }
    }


}
