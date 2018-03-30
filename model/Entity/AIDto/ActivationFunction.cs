using Accord.Neuro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity.AIDto
{
    public class ActivationFunction
    {
        public int Alpha { get; set; }

        public static implicit operator SigmoidFunction(ActivationFunction l)
        {
            SigmoidFunction sigm = new SigmoidFunction(l.Alpha);
            return sigm;
        }
    }
}
