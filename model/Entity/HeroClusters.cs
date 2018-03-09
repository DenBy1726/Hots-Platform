using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoTS_Service.Entity.Enum;
using System.Runtime.Serialization;

namespace HoTS_Service.Entity
{
    [DataContract]
    public class HeroClusters : StormObject
    {
        [DataMember]
        private int id;
        [DataMember]
        private Enum.HeroSubGroup subGroupCluster;
        [DataMember]
        private int cluster;
        [DataMember]
        Gaussian gaussian;
      

        public int Id { get => id; set => id = value; }
        public HeroSubGroup SubGroupCluster { get => subGroupCluster; set => subGroupCluster = value; }
        public int Cluster { get => cluster; set => cluster = value; }
        public Gaussian Gaussian { get => gaussian; set => gaussian = value; }

        public HeroClusters(int id)
        {
            Id = id;
        }
    }

    [DataContract]
    public class Gaussian
    {
        [DataMember]
        private int cluster;
        [DataMember]
        private double[] probability;
        [DataMember]
        private double logLikelihoods;

        public int Cluster { get => cluster; set => cluster = value; }
        public double[] Probability { get => probability; set => probability = value; }
        public double LogLikelihoods { get => logLikelihoods; set => logLikelihoods = value; }
    }
}
