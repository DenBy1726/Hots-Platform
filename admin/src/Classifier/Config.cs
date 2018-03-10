using HoTS_Service.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Classifier
{

    [DataContract]
    public class Config
    {
        string[] input = new string[] { "./Source/Model/NNDataGauss.csv" };
        NetworkConfig network = new NetworkConfig();
        CancelationConfig cancelation = new CancelationConfig();
        ValidationConfig validation = new ValidationConfig();
        FilterConfig filter = new FilterConfig();
        bool moreInfoLog = true;

        [DataMember]
        public string[] Input { get => input; set => input = value; }
        [DataMember]
        public NetworkConfig Network { get => network; set => network = value; }
        [DataMember]
        public CancelationConfig Cancelation { get => cancelation; set => cancelation = value; }
        [DataMember]
        public ValidationConfig Validation { get => validation; set => validation = value; }
        [DataMember]
        public FilterConfig Filter { get => filter; set => filter = value; }
        [DataMember]
        public bool MoreInfoLog { get => moreInfoLog; set => moreInfoLog = value; }

        public Config() { }
        public Config(string file)
        {
            Config json = (Config)JSonParser.Load(File.ReadAllText(file), typeof(Config));
            input = json.Input;
            network = json.Network;
            cancelation = json.Cancelation;
            validation = json.Validation;
            filter = json.Filter;
            MoreInfoLog = json.MoreInfoLog;
        }
        public override string ToString()
        {
            return HoTS_Service.Util.ToString.ReflexString(this);
        }
    }

    [DataContract]
    public class NetworkConfig
    {
        int[] layers = new int[] { 25 };

        double validationPercent = 0.15;

        bool shuffle = true;

        [DataMember]
        public int[] Layers { get => layers; set => layers = value; }
        [DataMember]
        public double ValidationPercent { get => validationPercent; set => validationPercent = value; }
        [DataMember]
        public bool Shuffle { get => shuffle; set => shuffle = value; }

        public override string ToString()
        {
            return HoTS_Service.Util.ToString.ReflexString(this);
        }
    }

    [DataContract]
    public class CancelationConfig
    {
        int maxEpoch = 5000;
        double error = 0.001;
        double step = 0;
        double percent = 0.8;

        [DataMember]
        public int MaxEpoch { get => maxEpoch; set => maxEpoch = value; }
        [DataMember]
        public double Error { get => error; set => error = value; }
        [DataMember]
        public double Step { get => step; set => step = value; }
        [DataMember]
        public double Percent { get => percent; set => percent = value; }

        public override string ToString()
        {
            return HoTS_Service.Util.ToString.ReflexString(this);
        }
    }

    [DataContract]
    public class ValidationConfig
    {
        double percent = 0.8;

        [DataMember]
        public double Percent { get => percent; set => percent = value; }

        public override string ToString()
        {
            return HoTS_Service.Util.ToString.ReflexString(this);
        }
    }

    [DataContract]
    public class FilterConfig
    {
        Range[] range = new Range[]
        {
            new Range()
            {
                From = 0,
                To = 1
            }
        };

        [DataMember]
        public Range[] Range { get => range; set => range = value; }
        [DataMember]
        public int Max { get => max; set => max = value; }

        private int max = 10000000;

        public bool IsInside(double value)
        {
            return range.Any(x => x.IsInside(value));
        }

        public override string ToString()
        {
            return HoTS_Service.Util.ToString.ReflexString(this);
        }
    }

    [DataContract]
    public class Range
    {
        double from;
        double to;

        [DataMember]
        public double From { get => from; set => from = value; }
        [DataMember]
        public double To { get => to; set => to = value; }

        public bool IsInside(double value)
        {
            return value > from && value < to;
        }

        public override string ToString()
        {
            return HoTS_Service.Util.ToString.ReflexString(this);
        }
    }
}
