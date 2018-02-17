using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity
{
   public class Statistic
    {

        StatisticItem statictic;

        public StatisticItem Statictic { get => statictic; set => statictic = value; }

    }

    public class StatisticItem
    {
        /// список матчей на данной карте i героем
        public int[] Matches;
        /// список матчей на данной карте i героем с победой
        public int[] Wins;
        /// общее количество матчей на данной карте
        public int Ammount;
    }

    public class HeroStatisticItem
    {
        /// процент побед
        public double winrate;
        /// убийств в еденицу времени
        public double killPerSec;
        /// поддержки в еденицу времени
        public double assistPerSec;
        /// смертей в еденицу времени
        public double deathPerSec;
        /// урона в секунду
        public double dps;
        /// исцеления в секунду
        public double hps;
        /// урона по фортам в секунду
        public double sdps;
        /// получено урона в секунду
        public double damageTakenPerSec;
        /// опыта в секунду
        public double expPerSec;
        /// взято лагерей в секунду
        public double campTakenPerSec;
        /// средняя длинна матча
        public double sec;
        
        ///ид повтора
        public int replayId;

        ///ид героя
        public int heroId;

        public override bool Equals(object o)
        {
            HeroStatisticItem obj = o as HeroStatisticItem;
            if (obj == null)
            {
                return false;
            }
            return winrate.Equals(obj.winrate) && killPerSec.Equals(obj.killPerSec) &&
                assistPerSec.Equals(obj.assistPerSec) && deathPerSec.Equals(obj.deathPerSec) &&
                dps.Equals(obj.dps) && hps.Equals(obj.hps) && sdps.Equals(obj.sdps) &&
                damageTakenPerSec.Equals(obj.damageTakenPerSec) && expPerSec.Equals(obj.expPerSec) &&
                campTakenPerSec.Equals(obj.campTakenPerSec) && sec.Equals(obj.sec) &&
                replayId.Equals(obj.replayId) && heroId.Equals(obj.heroId);
        }

        public override int GetHashCode()
        {
            return heroId * 37 ^ replayId * 33 ^ (int)sec * 27;
        }
    }

    public class HeroStatisticItemAvg:HeroStatisticItem
    {
        public int count;
        public double assassinRating;
        public double warriorRating;
        public double supportRating;
        public double specialistRating;
    }

    public class HeroStatisticItemMax : HeroStatisticItem
    {

    }

    public class HeroStatisticItemMin : HeroStatisticItem
    {

    }

    [DataContract]
    public class MatchupTable
    {
        double[][] winWith;
        double[][] winAgainst;

        /// <summary>
        /// процент матчей когда i с j выйграли (союзники)
        /// </summary>
        [DataMember]
        public double[][] WinWith { get => winWith; set => winWith = value; }


        /// <summary>
        /// процент матчей когда i с j выйграли (противники)
        /// </summary>
        [DataMember]
        public double[][] WinAgainst { get => winAgainst; set => winAgainst = value; }

        public MatchupTable(int n)
        {
            winWith = new double[n][];
            winAgainst = new double[n][];
            for(int i = 0; i < n; i++)
            {
                winWith[i] = new double[n];
                winAgainst[i] = new double[n];
            }
            
        }
    }
}
