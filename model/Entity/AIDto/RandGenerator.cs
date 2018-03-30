using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity.AIDto
{
    public class RandGenerator
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int Length { get; set; }
        public double Mean { get; set; }
        public double Variance { get; set; }
        public double Mode { get; set; }
        public int Entropy { get; set; }
        public Support Support { get; set; }
        public Quartiles Quartiles { get; set; }
        public double Median { get; set; }
        public double StandardDeviation { get; set; }
    }
}
