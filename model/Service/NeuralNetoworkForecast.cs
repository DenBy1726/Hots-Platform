using Accord.Math.Random;
using HoTS_Service.Entity;
using HoTS_Service.Entity.AIDto;
using HoTS_Service.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace HoTS_Service.Service
{
    
    public class NeuralNetworkForecast : IForecast
    {
        Accord.Neuro.ActivationNetwork network;

        string method = "";

        Func<HeroData, int> clusterFunction;

        public string Method { get => method; set => method = value; }
        public Func<HeroData, int> ClusterFunction { get => clusterFunction; set => clusterFunction = value; }

        public double Compute(double[] input)
        {
            return network.Compute(input)[0];
        }

        public double Compute(HeroData[] heroes)
        {
            double[] input = new double[network.InputsCount];
            int[] clusters = heroes.Select(ClusterFunction).ToArray();
            for(int i = 0; i < 5; i++)
            {
                input[clusters[i]]++;
            }
            for (int i = 0; i < 5; i++)
            {
                input[2*clusters[i + 5]]++;
            }
            return Compute(input);
        }

        public NeuralNetworkForecast Load(string file)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            var metaNetwork =  js.Deserialize<MetaNetwork>(File.ReadAllText(file));
            network = metaNetwork.Item1;
            Method = metaNetwork.Item2;

            switch (Method)
            {
                case "NNData":
                    ClusterFunction = (x) => (int)x.Clusters.SubGroupCluster;
                    break;

                case "NNDataGauss":
                    ClusterFunction = (x) => (int)x.Clusters.Cluster;
                    break;

                default:
                    ClusterFunction = (x) => (int)x.Hero.SubGroup;
                    break;
            }

            return this;
        }
    }
}
