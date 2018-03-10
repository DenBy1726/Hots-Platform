using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CConsole = Colorful.Console;

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

    public static class NNLoger
    {
        public static void log(int iteration, double error,double validError, double percent,
            double validPercent)
        {

            Color color = GetBlendedColor((int)(percent * 100));

            CConsole.WriteLine($"It = {iteration}" +
                $" Er = {Math.Round(error,6)}" +
                $" V.Er = {Math.Round(validError, 6)} P = {Math.Round(percent*100, 2)}%" +
                $" V.P = {Math.Round(validPercent * 100, 2)}%", color);
        }

        public static void smalllog(int iteration, double error)
        {
            Console.WriteLine($"It = {iteration}" +
                $" Er = {Math.Round(error, 6)}");
        }

        static Color GetBlendedColor(int percentage)
        {
            if (percentage < 50)
                return Interpolate(Color.Red, Color.Yellow, percentage / 50.0);
            return Interpolate(Color.Yellow, Color.Lime, (percentage - 50) / 50.0);
        }

        static Color Interpolate(Color color1, Color color2, double fraction)
        {
            double r = Interpolate(color1.R, color2.R, fraction);
            double g = Interpolate(color1.G, color2.G, fraction);
            double b = Interpolate(color1.B, color2.B, fraction);
            return Color.FromArgb((int)Math.Round(r), (int)Math.Round(g), (int)Math.Round(b));
        }

        static double Interpolate(double d1, double d2, double fraction)
        {
            return d1 + (d2 - d1) * fraction;
        }


    }
}
