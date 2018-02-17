using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoTS_Service.Entity;

namespace HoTS_Service.Service
{
    public class HeroService : Service.IService<Hero>
    {
        Hero[] heroes;

        public int Count()
        {
            return heroes.Length;
        }

        public Hero Find(int id)
        {
            return heroes[id];
        }

        public Hero[] All()
        {
            return heroes;
        }

        public List<T> Select<T>(Func<Hero, T> callback)
        {
            return heroes.Select(callback).ToList();
        }

        public List<Hero> Where(Func<Hero, bool> callback)
        {
            return heroes.Where(callback).ToList();
        }

        public void Load(string file)
        {
            string json = System.IO.File.ReadAllText(file);
            heroes = (Hero[])Util.JSonParser.Load(json, typeof(Hero[]));
        }
    }
}
