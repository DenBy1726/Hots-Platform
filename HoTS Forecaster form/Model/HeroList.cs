using System.Collections.Generic;
using System.Linq;
using HoTS_Service.Entity;
using HoTS_Service.Entity.Enum;
using System.Collections;

namespace HoTS_Forecaster_form
{

    /// <summary>
    /// список героев с возможностью конвеерной фильтрации
    /// </summary>
    public class HeroList : IEnumerable<HeroData>
    {
        List<HeroData> items = new List<HeroData>();

        public HeroList(List<Hero> items,List<HeroDetails> details,List<HeroClusters> clusters,List<HeroStatisticItemAvg> stat)
        {
            for(int i=0;i<items.Count;i++)
            {
                this.items.Add(new HeroData(items[i], details[i],clusters[i], stat[i], i));
            }
        }

        public HeroList(List<HeroData> items)
        {
            this.items = items.ToList();
        }

        public List<HeroData> Item { get => items;}

        public IEnumerator<HeroData> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public HeroList Select(HashSet<int> ids)
        {
            return new HeroList(items.Where((x) => ids.Contains(x.Hero.Id)).ToList());
        }

        public HeroList Select(HashSet<HeroGroup> groups)
        {
            return new HeroList(items.Where((x) => groups.Contains(x.Hero.Group)).ToList());
        }

        public HeroList Select(HashSet<HeroSubGroup> subgroups)
        {
            return new HeroList(items.Where((x) => subgroups.Contains(x.Hero.SubGroup)).ToList());
        }

        public HeroList Select(HashSet<Franchise> franchise)
        {
            return new HeroList(items.Where((x) => franchise.Contains(x.Details.Franchise)).ToList());
        }

        public HeroList Select(string text)
        {
            return new HeroList(items.Where((x) => x.Hero.Name.ToUpper().IndexOf(text.ToUpper()) != -1).ToList());
        }

        public HeroStatisticItemAvg Statistic(string text)
        {
            return Select(text).Item[0].Statistic;
        }

        public HeroData Select(int id)
        {
            return items.Where((x) => x.Id == id).First();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }

    
}
