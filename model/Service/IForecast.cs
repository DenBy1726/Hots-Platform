using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Service
{
    public abstract class IForecast
    {
        [DllImport("forecast.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double forecast(double[] input);
        public abstract double Compute(double[] input);
    }
}
