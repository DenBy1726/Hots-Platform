using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Service
{
    public interface IForecast
    {
        double Compute(double[] input);
    }
}
