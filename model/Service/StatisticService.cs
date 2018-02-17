using HoTS_Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Service
{
    public class StatisticService : IService<Statistic>
    {
        Statistic[] stats;

        public int Count()
        {
            return stats.Length;
        }

        public Statistic Find(int id)
        {
            return stats[id];
        }

        public Statistic[] All()
        {
            return stats;
        }

        public List<T> Select<T>(Func<Statistic, T> callback)
        {
            return stats.Select(callback).ToList();
        }

        public List<Statistic> Where(Func<Statistic, bool> callback)
        {
            return stats.Where(callback).ToList();
        }

        public void Load(string file)
        {
            string json = System.IO.File.ReadAllText(file);
            stats = (Statistic[])Util.JSonParser.Load(json, typeof(Statistic[]));
        }
    }
}
