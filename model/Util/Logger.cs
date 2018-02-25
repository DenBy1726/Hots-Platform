using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Util
{
    public static class Logger
    {
        public static void log(string type, string message)
        {

            type = type.ToUpper();
            switch (type)
            {
                case "ERROR":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "WARNG":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "SUCCES":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "INFO":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case "PRGRS":
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

            Console.WriteLine("{1,6} {0} {2}", DateTime.Now.ToString("hh:mm:ss"), type, message);
        }
    }
}
