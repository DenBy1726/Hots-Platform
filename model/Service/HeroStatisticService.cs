using HoTS_Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HoTS_Service.Service
{
    public class HeroStatisticService
    {
        HeroStatisticItemAvg[] heroAvg;

        HeroStatisticItemMin[] heroMin;

        HeroStatisticItemMax[] heroMax;

  

        public int Count()
        {
            return heroAvg.Length;
        }

        public List<T> Select<T>(Func<HeroStatisticItemAvg, T> callback)
        {
            return heroAvg.Select(callback).ToList();
        }

        public List<HeroStatisticItemAvg> Where(Func<HeroStatisticItemAvg, bool> callback)
        {
            return heroAvg.Where(callback).ToList();
        }

        public Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], HeroStatisticItemMax[]> All()
        {
            return new Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], HeroStatisticItemMax[]>
                (heroAvg,heroMin,heroMax);
        }


        public Tuple<HeroStatisticItemAvg, HeroStatisticItemMin, HeroStatisticItemMax> Find(int id)
        {
            return new Tuple<HeroStatisticItemAvg, HeroStatisticItemMin, HeroStatisticItemMax>(heroAvg[id], heroMin[id], heroMax[id]);
        }

        public void Load(string file)
        {
            string json = System.IO.File.ReadAllText(file);
            var stats = (Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], HeroStatisticItemMax[]>)
                Util.JSonParser.Load(json, 
                typeof(Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], HeroStatisticItemMax[]>));
            heroAvg = stats.Item1;
            heroMin = stats.Item2;
            heroMax = stats.Item3;

        }
    }
}
