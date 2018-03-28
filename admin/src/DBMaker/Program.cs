using HoTS_Service.Entity;
using HoTS_Service.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DBMaker
{
    class HeroStatistic
    {
        public int id;
        public int min_id;
        public int max_id;
        public int avg_id;
    }

    class Probabilities
    {
        public int id;
        public int gaussian_id;
        public double value;
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (!Directory.Exists("./Database"))
                Directory.CreateDirectory("./Database");

            HeroService heroes = new HeroService();
            heroes.Load("./Source/Hero/Hero.json");

            HeroDetailsService details = new HeroDetailsService();
            details.Load("./Source/Hero/HeroDetails.json");

            HeroClustersSevice clusters = new HeroClustersSevice();
            clusters.Load("./Source/Hero/HeroClusters.json");

            MapService maps = new MapService();
            maps.Load("./Source/Map/Map.json");

            StatisticService stats = new StatisticService();
            stats.Load("./Source/Replay/Statistic.json");

            HeroStatisticService hstats = new HeroStatisticService();
            hstats.Load("./Source/Replay/Statistic_sho.json");

            MatchupService matchups = new MatchupService();
            matchups.Load("./Source/Replay/MatchupTable.json");

            PostegresConverter converter = new PostegresConverter();
            converter.CustomNameMapper["group"] = "_group";
            converter.CustomNameMapper["min_id"] = "id_min";
            converter.CustomNameMapper["max_id"] = "id_max";
            converter.CustomNameMapper["avg_id"] = "id_avg";

            string sequensers = @"alter sequence gaussian_id_seq minvalue 0 start with 0;
select setval('gaussian_id_seq', 0, false);
alter sequence heroclusters_id_seq minvalue 0 start with 0;
select setval('heroclusters_id_seq', 0, false);
alter sequence statisticheroesmax_id_seq minvalue 0 start with 0;
select setval('statisticheroesmax_id_seq', 0, false);
alter sequence statisticheroesmin_id_seq minvalue 0 start with 0;
select setval('statisticheroesmin_id_seq', 0, false);
alter sequence statisticheroesavg_id_seq minvalue 0 start with 0;
select setval('statisticheroesavg_id_seq', 0, false);";

            string[] enumsTable = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsEnum && t.Namespace == "HoTS_Service.Entity.Enum")
                       .Select(_enum => converter.CreateDictionary(_enum.Name))
                       .ToArray();

            Dictionary<string, string> tables = new Dictionary<string, string>();
            Dictionary<string, string> data = new Dictionary<string, string>();
            tables["heroesTable"] = converter.CreateTable("Hero", typeof(Hero), "Id",
                new List<Foreign> {
                    new Foreign()
                    {
                        DataTable = "HeroGroup",
                        Key = "_group",
                        ForeignKey = "id"
                    },
                     new Foreign()
                    {
                        DataTable = "HeroSubGroup",
                        Key = "subgroup",
                        ForeignKey = "id"
                    }
                });
            tables["detailsTable"] = converter.CreateTable("HeroDetails",
                typeof(HeroDetails), "id", new List<Foreign> {
                    new Foreign()
                    {
                        DataTable = "Hero",
                        Key = "id",
                        ForeignKey = "id"
                    },
                     new Foreign()
                    {
                        DataTable = "Difficulty",
                        Key = "difficulty",
                        ForeignKey = "id"
                    },
                       new Foreign()
                    {
                        DataTable = "Franchise",
                        Key = "franchise",
                        ForeignKey = "id"
                    },
                         new Foreign()
                    {
                        DataTable = "ResourceType",
                        Key = "resourcetype",
                        ForeignKey = "id"
                    }
                });
            tables["mapTable"] = converter.CreateTable("Map", typeof(Map), "id", null);

            tables["statisticTable"] = StatisticSchema();

            tables["statisticShoMin"] = converter.CreateTable("StatisticHeroesMin",
               typeof(HeroStatisticItemMin), "id", null);
            tables["statisticShoMax"] = converter.CreateTable("StatisticHeroesMax",
               typeof(HeroStatisticItemMax), "id", null);
            tables["statisticShoAvg"] = converter.CreateTable("StatisticHeroesAvg",
               typeof(HeroStatisticItemAvg), "id", null);

            tables["statisticSho"] = converter.CreateTable("StatisticHeroes",
                typeof(HeroStatistic), "id", new List<Foreign> {
                    new Foreign()
                    {
                        DataTable = "StatisticHeroesMin",
                        Key = "id_min",
                        ForeignKey = "id"
                    },
                    new Foreign()
                    {
                        DataTable = "StatisticHeroesAvg",
                        Key = "id_avg",
                        ForeignKey = "id"
                    },
                    new Foreign()
                    {
                        DataTable = "StatisticHeroesMax",
                        Key = "id_max",
                        ForeignKey = "id"
                    },
                    new Foreign()
                    {
                        DataTable = "Hero",
                        Key = "id",
                        ForeignKey = "id"
                    }
                });


            tables["matchupTable"] = MatchupTableSchema();

            tables["gaussian"] = converter.CreateTable("Gaussian", typeof(Gaussian)
               , "id", null);

            tables["probabilities"] = converter.CreateTable("GaussianProbabilities",
            typeof(Probabilities)
            , "id", new List<Foreign> {
                    new Foreign()
                    {
                        DataTable = "Gaussian",
                        Key = "gaussian_id",
                        ForeignKey = "id"
                    }
            });

           

            tables["heroClusters"] = converter.CreateTable("HeroClusters", typeof(HeroClusters)
                , "id", new List<Foreign> {
                    new Foreign()
                    {
                        DataTable = "Hero",
                        Key = "id",
                        ForeignKey = "id"
                    },
                    new Foreign()
                    {
                        DataTable = "Gaussian",
                        Key = "gaussian",
                        ForeignKey = "id"
                    },
                    new Foreign()
                    {
                        DataTable = "HeroSubGroup",
                        Key = "subgroupcluster",
                        ForeignKey = "id"
                    }
                });

            string[] enumsData = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsEnum && t.Namespace == "HoTS_Service.Entity.Enum")
                       .Select(_enum => converter.InsertDictionary(_enum))
                       .ToArray();

            data["heroesTable"] = converter.Insert("Hero", heroes.All());
            data["detailsTable"] = converter.Insert("HeroDetails", details.All());
            data["mapTable"] = converter.Insert("Map", maps.All());
            data["statisticTable"] = StatisticData(stats);
            data["statisticShoMin"] = converter.Insert("StatisticHeroesMin", hstats.All().Item2);
            data["statisticShoMax"] = converter.Insert("StatisticHeroesMax", hstats.All().Item3);
            data["statisticShoAvg"] = converter.Insert("StatisticHeroesAvg", hstats.All().Item1);
            data["statisticSho"] = HeroesStatisticData(hstats);
            data["matchupTable"] = MatchupData(matchups, heroes.Count());
            int probId = 0;
            data["gaussian"] = converter.Insert("Gaussian", clusters.Select(x => x.Gaussian));
            data["probabilities"] = converter.Insert("GaussianProbabilities",
               clusters.
               Select(x => x.Gaussian.Probability.
                   Select(y => new Probabilities()
                   {
                       id = probId++,
                       value = y,
                       gaussian_id = x.Id,
                   })).SelectMany(z => z));
            data["heroClusters"] = converter.Insert("HeroClusters", clusters.All());
           

            File.WriteAllText("./Database/create.sql",
                string.Join("\n", enumsTable) + 
                string.Join("\n", tables.Select(x => x.Value).ToArray()));
            File.WriteAllText("./Database/insert.sql",
                sequensers + "\n\n" + 
                string.Join("\n", enumsData) + 
                string.Join("\n", data.Select(x => x.Value).ToArray()));
        }

        static string StatisticSchema()
        {
            return "CREATE TABLE IF NOT EXISTS Statistic \n" +
                "( \n" +
                "id SERIAL UNIQUE PRIMARY KEY,\n" +
                "matches INT, \n" +
                "wins INT, \n" +
                "map_id INT,\n" +
                "hero_id INT,\n" +
                "FOREIGN KEY (map_id) REFERENCES Map(id),\n" +
                "FOREIGN KEY (hero_id) REFERENCES Hero(id)\n" +
                ");\n";
        }

        static string StatisticData(StatisticService statService)
        {
            StatisticItem[] stat = statService.All().Select(x=>x.Statictic).ToArray();
            int id = 0;
            string result = "";
            string result1 = "INSERT INTO Statistic(id,matches,wins,map_id,hero_id)\n";
            string result2 = "VALUES (";
            for (int i = 0; i < stat.Length; i++)
            {
                for(int j = 0; j < stat[i].Matches.Length; j++,id++)
                {
                    int matches = stat[i].Matches[j];
                    int wins = stat[i].Wins[j];
                    int mapId = i;
                    int heroId = j;
                    result += result1 + result2 + 
                    $"{id},{matches},{wins},{mapId},{heroId}" + ");\n";
                }   
            }

            return result;
        }

        static string HeroesStatisticData(HeroStatisticService hstatService)
        {
            string result = "";
            string result1 = "INSERT INTO StatisticHeroes(id,id_min,id_max,id_avg)\n";
            string result2 = "VALUES (";
            var stat = hstatService.All();
            for(int i = 0; i < stat.Item1.Length; i++)
            {
                result += result1 + result2 +
                    $"{i},{i},{i},{i}" + ");\n";
            }
            return result;
        }

        static string MatchupData(MatchupService matchup,int count)
        {
            string result = "";
            string result1 = "INSERT INTO MatchupTable(id,win_with,win_against,hero_id1,hero_id2)\n";
            string result2 = "VALUES (";
            int id = 0;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++,id++)
                {
                    result += result1 + result2 +
                   $"{id}," +
                   $"{matchup.With(i,j).ToString().Replace(",",".")}," +
                   $"{matchup.Against(i,j).ToString().Replace(",", ".")},{i},{j}" + ");\n";
                }
            }
            return result;
        }

        static string MatchupTableSchema()
        {
            return "CREATE TABLE IF NOT EXISTS MatchupTable \n" +
                "( \n" +
                "id SERIAL UNIQUE PRIMARY KEY,\n" +
                "win_with FLOAT8, \n" +
                "win_against FLOAT8, \n" +
                "hero_id1 INT,\n" +
                "hero_id2 INT,\n" +
                "FOREIGN KEY (hero_id1) REFERENCES Hero(id),\n" +
                "FOREIGN KEY (hero_id2) REFERENCES Hero(id)\n" +
                ");\n";
        }


    }
}
