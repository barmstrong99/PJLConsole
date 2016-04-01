using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

///
///  Look at article at URL: http://www.odetocode.com/Humor/68.aspx
///  or http://kovaya.com/miscellany/2007/10/get-weather-on-your-hp-4200.html
///  
/// Test command lines: 
///     PrinterName /date 
///     PrinterName /reset
///     PrinterName /status
///     PrinterName /random
///     PrinterName "Type in some string here"
///

namespace PJLconsole
{
    class Program
    {
        protected static string _message = "NO MESSAGE";
        protected static int _maxMessageLength = 32;

        protected static bool _pause = true;

        private static HPPrinterSocket _hpPrinter;

        static int Main(string[] args)
        {
            if (!ParseArgs(args))
                return -1;

            Console.WriteLine();
            Console.WriteLine("HP Printer Display Updater");
            Console.WriteLine("Host: {0}", args[0]);
            Console.WriteLine("Message: {0}\n", _message);

            string printerName = args[0];

            int result = -1;
            if (args[1].ToUpper() == "/status".ToUpper())
                result = Convert.ToInt32(GetPrinterStatus(printerName));
            else
                result = Convert.ToInt32(SetPrinterDisplay(printerName));

            Console.WriteLine("Finished");
            Console.WriteLine();

#if DEBUG
            if (_pause)
                Console.ReadKey();
#endif
            return 0;
        }


        protected static bool ParseArgs(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine(
                          "HP Display Updater: " +
                          "PJLconsole printername|IPAddress \"message\" "
                    );
                return false;
            }

            if (args[1].Length > _maxMessageLength)
            {
                Console.WriteLine("Message must be <= {0} characters", _maxMessageLength);
                return false;
            }

            if (args[1].ToUpper().CompareTo("/random".ToUpper()) == 0)
            {
                _message = GetRandomMessage();
            }
            else if (args[1].ToUpper().CompareTo("/reset".ToUpper()) == 0)
                {
                    _message = "READY";
                }
            else
            {
                _message = args[1];
            }

            if (args[1].ToUpper().Contains("/date".ToUpper()))
            {
                string date = DateTime.Now.Date.ToShortDateString();
                _message = "READY".PadRight(16) + date;
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        private static bool SetPrinterDisplay(string printerName)
        {
            string ipAddress = ResolvePrinterName(printerName);

            if (ipAddress == string.Empty)
                return false;

            _hpPrinter = new HPPrinterSocket();

            bool result = _hpPrinter.SetPrinterDisplay(ipAddress, _message);

            _hpPrinter = null; 

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        private static bool GetPrinterStatus(string printerName)
        {
            string ipAddress = ResolvePrinterName(printerName);

            if (ipAddress == string.Empty)
                return false;

            _hpPrinter = new HPPrinterSocket();

            string receiveString = string.Empty;
            bool result = _hpPrinter.GetPrinterStatus(ipAddress, out receiveString);

            _hpPrinter = null; 

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        private static string ResolvePrinterName(string printerName)
        {
            string[] PrinterNames = {
                                        "Vanprint01",   
                                        "HP_LaserJet"  
                                    };

            string[] PrinterIPs = {
                                      "10.183.57.50",
                                      "10.183.57.43",
                                  };

            // already an IP?
            if (printerName.Contains("."))
                return printerName;

            printerName = printerName.ToLower();
            for (int i = 0; i < PrinterNames.Length; i++)
            {
                if (PrinterNames[i].ToLower() == printerName)
                {
                    Console.WriteLine(PrinterNames[i] + " resolves to " + PrinterIPs[i]);
                    return PrinterIPs[i];
                }
            }

            return string.Empty;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetRandomMessage()
        {
            string[] Messages = { 
                                 "TOUCH ME",
                                 "STEP AWAY",
                                 "SET TO STUN",
                                 "SCORE = 3413",
                                 "FEED ME",
                                 "GO AWAY",
                                 "NEED MORE SPACE",
                                 "POUR ME A DRINK",
                                 "IN DISTRESS",
                                 "NICE SHIRT",
                                 "GO AWAY",
                                 "NO PRINT FOR YOU",
                                 "RADIATION LEAK",
                                 "HANDS UP",
                                 "PRESS MY BUTTON",
                                 "TAKE ME HOME",
                                 "LOOKS LIKE RAIN",
                                 "HELLO WORLD",
                                 "NICE HAIR",
                                 "NEED A MINT?",
                                 "BE GENTLE",
                                 "BE KIND",
                                 "INSERT DISK",
                                 "BUY ME LUNCH",
                                 "DONT STOP",
                                 "COME CLOSER",
                                 "TAKE A BREAK",
                                 "INSERT QUARTER"
                                };

            Random r = new Random();
            return Messages[r.Next() % Messages.Length];
        }

    }
}
