using HoTS_Service.Entity.Enum;
using HoTS_Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity
{
    public class Match : IComparable<Match>
    {
        int[] yourTeam = new int[5];
        int[] enemyTeam = new int[5];
        int map;
        double probabilityToWin;

        public double ProbabilityToWin { get => probabilityToWin; set => probabilityToWin = value; }
        public int[] YourTeam { get => yourTeam; set => yourTeam = value; }
        public int[] EnemyTeam { get => enemyTeam; set => enemyTeam = value; }
        public int Map { get => map; set => map = value; }

        public Match()
        {

        }

        public Match(string[] input)
        {

        }

        public int CompareTo(Match other)
        {
            for (int i = 0; i < 5; i++)
            {
                if (yourTeam[i] != enemyTeam[i])
                    break;
            }
            if (map == other.map)
                return 0;
            else
                return yourTeam.GetHashCode().CompareTo(enemyTeam.GetHashCode());
        }

        public override bool Equals(object o)
        {
            Match other = o as Match;
            if (other == null)
            {
                return false;
            }
            return (yourTeam.SequenceEqual(other.yourTeam)) && 
                (enemyTeam.SequenceEqual(other.enemyTeam)) && (map == other.map)
                && (probabilityToWin == other.probabilityToWin);
        }

        public override int GetHashCode()
        {
            return 37 * yourTeam.GetHashCode() ^ 33 * enemyTeam.GetHashCode() ^
                27 * map.GetHashCode() ^ 23 * probabilityToWin.GetHashCode();
        }

        public double[] ToArray()
        {
            List<double> array = new List<double>();
            for(int i = 0; i < yourTeam.Length; i++)
            {
                array.Add(yourTeam[i]);
            }
            for (int i = 0; i < yourTeam.Length; i++)
            {
                array.Add(enemyTeam[i]);
            }
            array.Add(map);
            array.Add(probabilityToWin);
            return array.ToArray();
        }

        public SubGroupMatch ToSubGroups(HeroService HParser)
        {
            SubGroupMatch subGroupMatch = new SubGroupMatch();

            for (int i = 0; i < YourTeam.Length; i++)
            {
                subGroupMatch.YourTeam[(int)HParser.Find(YourTeam[i]).SubGroup - 1]++;
            }
            for (int i = 0; i < EnemyTeam.Length; i++)
            {
                subGroupMatch.EnemyTeam[(int)HParser.Find(EnemyTeam[i]).SubGroup - 1]++;
            }
            return subGroupMatch;
        }


    }

    public class SubGroupMatch
    {
        sbyte[] yourTeam;
        sbyte[] enemyTeam;
        public SubGroupMatch()
        {
            var subgrLen = System.Enum.GetValues(typeof(HeroSubGroup)).Length-1;
            YourTeam = new sbyte[subgrLen];
            EnemyTeam = new sbyte[subgrLen];
        }

        public sbyte[] YourTeam { get => yourTeam; set => yourTeam = value; }
        public sbyte[] EnemyTeam { get => enemyTeam; set => enemyTeam = value; }

     

        public override bool Equals(object o)
        {
            SubGroupMatch other = o as SubGroupMatch;
            if (other == null)
            {
                return false;
            }
            return (yourTeam.SequenceEqual(other.yourTeam)) &&
                (enemyTeam.SequenceEqual(other.enemyTeam));
        }

        public override int GetHashCode()
        {
            return 37 * yourTeam.GetHashCode() ^ 33 * enemyTeam.GetHashCode();
        }

        public double[] ToArray()
        {
            List<double> array = new List<double>();
            for (int i = 0; i < yourTeam.Length; i++)
            {
                array.Add(yourTeam[i]);
            }
            for (int i = 0; i < yourTeam.Length; i++)
            {
                array.Add(enemyTeam[i]);
            }
            return array.ToArray();
        }
    }
}
