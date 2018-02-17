using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Service
{
    public class NeuralNetworkForecast : IForecast
    {

        public override double Compute(double[] input)
        {
            return forecast(input);
        }
    }
}
