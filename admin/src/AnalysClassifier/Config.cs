using HoTS_Service.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AnalysClassifier
{
    [DataContract]
    class Config
    {
        string targetPath = "./Source/Network/NNData/Report/Target.csv";
        string outputPath = "./Source/Network/NNData/Report/Output.csv";
        string errorPath = "./Source/Network/NNData/Report/Error.csv";
        bool plotroc = true;
        bool plothist = true;
        bool plotconfucion = true;

        [DataMember]
        public string TargetPath { get => targetPath; set => targetPath = value; }
        [DataMember]
        public string OutputPath { get => outputPath; set => outputPath = value; }
        [DataMember]
        public string ErrorPath { get => errorPath; set => errorPath = value; }
        [DataMember]
        public bool Plotroc { get => plotroc; set => plotroc = value; }
        [DataMember]
        public bool Plothist { get => plothist; set => plothist = value; }
        [DataMember]
        public bool Plotconfucion { get => plotconfucion; set => plotconfucion = value; }

        public Config() { }
        public Config(string file)
        {
            Config json = (Config)JSonParser.Load(File.ReadAllText(file), typeof(Config));
            TargetPath = json.TargetPath;
            OutputPath = json.OutputPath;
            ErrorPath = json.ErrorPath;
            Plotroc = json.Plotroc;
            Plothist = json.Plothist;
            Plotconfucion = json.Plotconfucion;
        }

        public override string ToString()
        {
            return HoTS_Service.Util.ToString.ReflexString(this);
        }
    }
}
