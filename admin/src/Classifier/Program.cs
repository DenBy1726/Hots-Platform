using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoTS_Service;
using HoTS_Service.Service;
using HoTS_Service.Entity;
using Accord.MachineLearning.Clustering;
using Accord.Math;
using Accord.Controls;
using Accord.MachineLearning;
using HoTS_Service.Entity.Enum;
using HoTS_Service.Util;
using System.IO;

namespace Classifier
{
    class ClusterMatch
    {
        private int count;
        private int[] cluster;
        private int[] subgroup;

        public int Count { get => count; set => count = value; }
        public int[] Cluster { get => cluster; set => cluster = value; }
        public int[] Subgroup { get => subgroup; set => subgroup = value; }

        public override string ToString()
        {
            return HoTS_Service.Util.ToString.ReflexString(this);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            HeroStatisticService heroes = new HeroStatisticService();
            HeroService hero = new HeroService();
            heroes.Load("./Source/Replay/Statistic_sho.json");
            hero.Load("./Source/Hero/Hero.json");

            HeroStatisticItemAvg[] avg = heroes.All().Item1;
            double[][] inputs = avg.Select(x => new double[]{
                x.assassinRating,x.warriorRating,x.supportRating,x.specialistRating
            }).ToArray();

            // кластаризация
            GaussianMixtureModel gmm = new GaussianMixtureModel(9)
            {
                Options =
                {
                    Regularization = 1e-10
                }
            };

            // обучаем модель
            var clusters = gmm.Learn(inputs);

            // результат
            int[] predicted = clusters.Decide(inputs);

            // We can also obtain the log-likelihoods for each sample:
            double[] logLikelihoods = clusters.LogLikelihood(inputs);

            // As well as the probability of belonging to each cluster
            double[][] probabilities = clusters.Probabilities(inputs);

            //соединяем входные кластера, с id
            var predictedWithId = predicted
                .Select((Cluster, Id) => new { Cluster, Id })
                .ToArray();

            //соединяем подгруппы, с id
            var clusteredSubGroups = hero
                .All()
                .Select((Hero, Id) => new { SubGroup = (int)Hero.SubGroup, Id })
                .ToArray();

            //соединяем подгруппы с кластерами 
            var clusterMatch = predictedWithId
                .Join(clusteredSubGroups, e => e.Id, o => o.Id, (e, o) => new
                {
                    o.SubGroup,
                    e.Cluster
                })
                .ToArray();

            //получаем уникальные соответсвия подгруппа-кластер
            var uniqueMatch = clusterMatch
                .Select(g => new
                {
                    Cluster = g.Cluster,
                    SubGroup = g.SubGroup,
                    Count = clusterMatch.
                    Where(x => x.Cluster == g.Cluster && x.SubGroup == g.SubGroup).Count()
                }).Distinct().ToArray();

            //результат: те соответсвия, которые имеют достаточное количество элементов
            var endClusters = uniqueMatch
                .Where(x => x.Count > inputs.Length / uniqueMatch.Length)
                .Select(y => new ClusterMatch()
                {
                    Count = y.Count,
                    Cluster = new int[]{ y.Cluster },
                    Subgroup = new int[] { y.SubGroup }
                }).ToList();


            //все что не попало
            var littleClusters = uniqueMatch
              .Where(x => x.Count <= inputs.Length / uniqueMatch.Length)
              .ToArray();

            //группируем по кластерам
            var littleClustersByCluster = littleClusters
                .GroupBy(x => x.Cluster)
                .ToArray();

            //группируем по подгруппам
            var littleClustersBySubGroup = littleClusters
               .GroupBy(x => x.SubGroup)
               .ToArray();

            //группа с большими кластерами(значит меньше подгрупп)
            var biggerClustersGroup = littleClustersByCluster.Length < littleClustersBySubGroup.Length 
                ? littleClustersByCluster : littleClustersBySubGroup;

            //объединение большей группы класетров
            var resolved = biggerClustersGroup.Select(x =>
            {
                var group = x.ToArray();
                int sum = 0;
                List<int> clusters2 = new List<int>();
                List<int> subgroups = new List<int>();
                foreach(var it in group)
                {
                    sum += it.Count;
                    clusters2.Add(it.Cluster);
                    subgroups.Add(it.SubGroup);
                }
                return new ClusterMatch(){
                    Count = sum,
                    Cluster = clusters2.Distinct().ToArray(),
                    Subgroup = subgroups.Distinct().ToArray()
                };
            }).ToArray();

            endClusters.AddRange(resolved);

            var result = clusterMatch.Select((x, Id) =>
            {
                return new HeroClusters(Id)
                {
                    Cluster = endClusters.FindIndex(y => y.Cluster.Contains(x.Cluster) && y.Subgroup.Contains(x.SubGroup)),
                    Gaussian = new Gaussian()
                    {
                        Cluster = x.Cluster,
                        Probability = probabilities[Id],
                        LogLikelihoods = logLikelihoods[Id]
                    },
                    SubGroupCluster = (HeroSubGroup)x.SubGroup
                };
            }).ToArray();

            string json = JSonParser.Save(result, typeof(HeroClusters[]));
            File.WriteAllText("./Source/Hero/HeroClusters.json",json);
        }
    }
}
