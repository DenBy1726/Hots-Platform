using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoTS_Service.Entity;

namespace HoTS_Service.Service
{
    public class HeroDetailsService : IService<Entity.HeroDetails>
    {
        HeroDetails[] details;

        public int Count()
        {
            return details.Length;
        }

        public HeroDetails Find(int id)
        {
            return details[id];
        }

        public HeroDetails[] All()
        {
            return details;
        }

        public List<T> Select<T>(Func<HeroDetails, T> callback)
        {
            return details.Select(callback).ToList();
        }

        public List<HeroDetails> Where(Func<HeroDetails, bool> callback)
        {
            return details.Where(callback).ToList();
        }

        public void Load(string file)
        {
            string json = System.IO.File.ReadAllText(file);
            details = (HeroDetails[])Util.JSonParser.Load(json, typeof(HeroDetails[]));
        }
    }
}
