using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoTS_Service.Entity;

namespace HoTS_Service.Service
{
    public class HeroClustersSevice : IService<Entity.HeroClusters>
    {
        HeroClusters[] clusters;

        public int Count()
        {
            return clusters.Length;
        }

        public HeroClusters Find(int id)
        {
            return clusters[id];
        }

        public HeroClusters[] All()
        {
            return clusters;
        }

        public List<T> Select<T>(Func<HeroClusters, T> callback)
        {
            return clusters.Select(callback).ToList();
        }

        public List<HeroClusters> Where(Func<HeroClusters, bool> callback)
        {
            return clusters.Where(callback).ToList();
        }

        public void Load(string file)
        {
            string json = System.IO.File.ReadAllText(file);
            clusters = (HeroClusters[])Util.JSonParser.Load(json, typeof(HeroClusters[]));
        }
    }
}
