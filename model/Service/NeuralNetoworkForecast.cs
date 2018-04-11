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

        Func<HeroData, int> clusterFunction;

        public TrainMeta Meta { get; set; }
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
            for (int i = 5; i < 10; i++)
            {
                input[(input.Length / 2) + clusters[i]]++;
            }
            return Compute(input);
        }

        public NeuralNetworkForecast Load(string file)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            var metaNetwork =  js.Deserialize<MetaNetwork>(File.ReadAllText(file));
            network = metaNetwork.Network;
            Meta = metaNetwork.Meta;

            ClusterFunction = (x) =>
            {
                try
                {
                    int value = FindPath.GetDeepPropertyValue<int>(x, Meta.ClusterPath);
                    if (Meta.ClusterPath.Contains("SubGroup"))
                        return value - 1;
                    else
                        return value;
                }
                catch
                {
                    try
                    {
                        return FindPath.GetDeepPropertyValue<int>(x, "Hero.SubGroup")-1;
                    }
                    catch
                    {
                        throw new ArgumentException("Объект героя должен по крайней мере" +
                            "содержать кластер по умолчанию");
                    }
                }
            };

            return this;
        }
    }
}
