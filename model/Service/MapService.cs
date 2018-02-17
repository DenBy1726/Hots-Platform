using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoTS_Service.Entity;

namespace HoTS_Service.Service
{
    public class MapService : Service.IService<Map>
    {
        Map[] maps;

        public int Count()
        {
            return maps.Length;
        }

        public Map Find(int id)
        {
            return maps[id];
        }

        public Map[] All()
        {
            return maps;
        }

        public List<T> Select<T>(Func<Map, T> callback)
        {
            return maps.Select(callback).ToList();
        }

        public List<Map> Where(Func<Map, bool> callback)
        {
            return maps.Where(callback).ToList();
        }

        public void Load(string file)
        {
            string json = System.IO.File.ReadAllText(file);
            maps = (Map[])Util.JSonParser.Load(json, typeof(Map[]));
        }
    }
}
