using HoTS_Service.Entity;
using HoTS_Service.Entity.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Service.Tests
{
    [TestClass()]
    public class DeleterParserTests : AutoParser.DeleterParser
    {
        public DeleterParserTests()
            :base("","","","","","","","")
        {

        }
        [TestMethod()]
        [TestCategory("parser")]
        public void DeleterHeroTest()
        {
            List<Hero> heroesInput = new List<Hero>()
            {
                new Hero(0,"Unknown",0,0),
                new Hero(1,"FirstHero",(HeroGroup)3,(HeroSubGroup)2),
                new Hero(2,"WhoItIs?",(HeroGroup)(-1),(HeroSubGroup)2),
                new Hero(3,"Artanis",(HeroGroup)4,(HeroSubGroup)2)
            };

            List<Hero> heroesTarget = new List<Hero>()
            {
                new Hero(0,"FirstHero",(HeroGroup)3,(HeroSubGroup)2),
                new Hero(1,"Artanis",(HeroGroup)4,(HeroSubGroup)2)
            };

            HeroStatisticItemAvg[] heroesAvg = new HeroStatisticItemAvg[]
            {
                new HeroStatisticItemAvg()
                {
                    count = 0
                },
                 new HeroStatisticItemAvg()
                {
                    count = 1
                },
                  new HeroStatisticItemAvg()
                {
                    count = 0
                },
                   new HeroStatisticItemAvg()
                {
                    count = 124124
                }
            };

            HeroStatisticItemMin[] heroesMin = new HeroStatisticItemMin[]
            {
                new HeroStatisticItemMin()
                {
                    assistPerSec = 0
                },
                 new HeroStatisticItemMin()
                {
                     assistPerSec = 0
                },
                  new HeroStatisticItemMin()
                {
                     assistPerSec = 0
                },
                   new HeroStatisticItemMin()
                {
                     assistPerSec = 0
                }
            };

            HeroStatisticItemMax[] heroesMax = new HeroStatisticItemMax[]
            {
                new HeroStatisticItemMax()
                {
                    assistPerSec = 0
                },
                 new HeroStatisticItemMax()
                {
                     assistPerSec = 0
                },
                  new HeroStatisticItemMax()
                {
                     assistPerSec = 0
                },
                   new HeroStatisticItemMax()
                {
                     assistPerSec = 0
                }
            };

            List<int> unusedTarget = new List<int>() { 2, 0 };

            AutoParser.HParser = new AutoParser.HeroParser("", "")
            {
                Hero = heroesInput
            };
            AutoParser.RSParser = new AutoParser.ReplaySchemaParser("", "", "", "", "", "")
            {
                AvgStat = heroesAvg,
                MinStat = heroesMin,
                MaxStat = heroesMax
            };

            List<HeroStatisticItemAvg> avgList = heroesAvg.ToList();
            List<HeroStatisticItemMin> minList = heroesMin.ToList();
            List<HeroStatisticItemMax> maxList = heroesMax.ToList();

            var unusedOutput = GetUnusedHero(avgList);

            CollectionAssert.AreEqual(unusedTarget, unusedOutput);

            RemoveUnusedHero(avgList, minList, maxList, unusedOutput);

            Assert.AreEqual(avgList.Count, 2);
            Assert.AreEqual(minList.Count, 2);
            Assert.AreEqual(maxList.Count, 2);

            IndexateHeroes(avgList, minList, maxList);

            CollectionAssert.AreEqual(heroesInput, heroesTarget);
        }

        [TestMethod()]
        [TestCategory("parser")]
        public void DeleterMapTest()
        {
            List<Map> mapInput = new List<Map>()
            {
                new Map(0,"Unknown"),
                new Map(1,"Hanamura"),
                new Map(2,"WhatItIs?"),
                new Map(3,"Filed")
            };

            List<Map> mapTarget = new List<Map>()
            {
                new Map(0,"Hanamura"),
                new Map(1,"Filed")
            };



            List<int> unusedTarget = new List<int>() { 2, 0 };

            AutoParser.MParser = new AutoParser.MapParser("", "")
            {
                Map = mapInput
            };

            List<Statistic> stat = new List<Statistic>()
            {
                new Statistic()
                {
                    Statictic = new StatisticItem()
                    {
                        Ammount = 0,
                        Matches = null,
                        Wins = null
                    }
                },
                 new Statistic()
                {
                    Statictic = new StatisticItem()
                    {
                        Ammount = 10,
                        Matches = null,
                        Wins = null
                    }
                },
                  new Statistic()
                {
                    Statictic = new StatisticItem()
                    {
                        Ammount = 0,
                        Matches = null,
                        Wins = null
                    }
                },
                   new Statistic()
                {
                    Statictic = new StatisticItem()
                    {
                        Ammount = 10,
                        Matches = null,
                        Wins = null
                    }
                }
        };

            var unusedOutput = GetUnusedMap(stat);

            CollectionAssert.AreEqual(unusedTarget, unusedOutput);

            RemoveUnusedMap(stat, unusedOutput);

            Assert.AreEqual(stat.Count,2);

            IndexateMap(stat);

            CollectionAssert.AreEqual(mapInput,mapTarget);
        }
    }
}