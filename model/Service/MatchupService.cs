using HoTS_Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Service
{
    public class MatchupService
    {
        MatchupTable table;

        public int Count()
        {
            return table.WinWith.Length;
        }

        public double With(int i,int j)
        {
            return table.WinWith[i][j];
        }

        public double Against(int i, int j)
        {
            return table.WinAgainst[i][j];
        }

        public void Load(string file)
        {
            string json = System.IO.File.ReadAllText(file);
            table = (MatchupTable)Util.JSonParser.Load(json, typeof(MatchupTable));
        }
    }
}
