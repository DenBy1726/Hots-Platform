using HoTS_Service.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Service.ModelParser.Tests
{
    [TestClass()]
    public class MatchHasherTests : AutoParser.ModelParser
    {
        static string Input = @"E:\Кабинет\Курс 4\Курсач\Source Dataset2\Replay\Replays.csv";
        public MatchHasherTests()
            :base(Input,"","")
        {

        }
        [TestMethod()]
        public void HashTest()
        {
            SubGroupMatch m = new SubGroupMatch()
            {
                YourTeam = new sbyte[] { 1, 2, 0, 1, 1 ,0,0,0,0},
                EnemyTeam = new sbyte[] { 0, 0, 2, 2, 1,0,0,0,0 },
            };
            AutoParser.ModelParser.SubGroupMatchHasher hasher = new 
                AutoParser.ModelParser.SubGroupMatchHasher();
            var hash = hasher.Hash(m);
            var restored = hasher.Restore(hash);

            Assert.AreEqual(m, restored);

        }

    }
}