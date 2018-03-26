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
        public double values;
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
            heroes.Load("./Source/Hero/HeroClusters.json");

            MapService maps = new MapService();
            maps.Load("./Source/Map/Map.json");

            StatisticService stats = new StatisticService();
            stats.Load("./Source/Replay/Statistic.json");

            HeroStatisticService hstats = new HeroStatisticService();
            hstats.Load("./Source/Replay/Statistic_sho.json");

            MatchupService matchups = new MatchupService();
            matchups.Load("./Source/Replay/MatchupTable.json");

            PostegresConverter converter = new PostegresConverter();

            string[] enumsTable = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsEnum && t.Namespace == "HoTS_Service.Entity.Enum")
                       .Select(_enum => converter.CreateDictionary(_enum.Name))
                       .ToArray();

            Dictionary<string, string> tables = new Dictionary<string, string>();
            tables["heroesTable"] = converter.CreateTable("Hero", typeof(Hero), "Id",
                new List<Foreign> {
                    new Foreign()
                    {
                        DataTable = "HeroGroup",
                        Type = ForeignType.OneToMany,
                        Key = "_group",
                        ForeignKey = "id"
                    },
                     new Foreign()
                    {
                        DataTable = "HeroSubGroup",
                        Type = ForeignType.OneToMany,
                        Key = "subgroup",
                        ForeignKey = "id"
                    }
                });
            tables["detailsTable"] = converter.CreateTable("HeroDetails",
                typeof(HeroDetails), "id", new List<Foreign> {
                    new Foreign()
                    {
                        DataTable = "Hero",
                        Type = ForeignType.OneToOne,
                        Key = "id",
                        ForeignKey = "id"
                    },
                     new Foreign()
                    {
                        DataTable = "Difficulty",
                        Type = ForeignType.OneToMany,
                        Key = "difficulty",
                        ForeignKey = "id"
                    },
                       new Foreign()
                    {
                        DataTable = "Franchise",
                        Type = ForeignType.OneToMany,
                        Key = "franchise",
                        ForeignKey = "id"
                    },
                         new Foreign()
                    {
                        DataTable = "ResourceType",
                        Type = ForeignType.OneToMany,
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
                        Type = ForeignType.OneToOne,
                        Key = "min_id",
                        ForeignKey = "id"
                    },
                    new Foreign()
                    {
                        DataTable = "StatisticHeroesAvg",
                        Type = ForeignType.OneToOne,
                        Key = "avg_id",
                        ForeignKey = "id"
                    },
                    new Foreign()
                    {
                        DataTable = "StatisticHeroesMax",
                        Type = ForeignType.OneToOne,
                        Key = "max_id",
                        ForeignKey = "id"
                    },
                    new Foreign()
                    {
                        DataTable = "Hero",
                        Type = ForeignType.OneToOne,
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
                        Type = ForeignType.OneToOne,
                        Key = "gaussian_id",
                        ForeignKey = "id"
                    }
                });

            tables["heroClusters"] = converter.CreateTable("HeroClusters", typeof(HeroClusters)
                , "id", new List<Foreign> {
                    new Foreign()
                    {
                        DataTable = "Hero",
                        Type = ForeignType.OneToOne,
                        Key = "id",
                        ForeignKey = "id"
                    },
                    new Foreign()
                    {
                        DataTable = "Gaussian",
                        Type = ForeignType.OneToOne,
                        Key = "gaussian",
                        ForeignKey = "id"
                    }
                });
            
            File.WriteAllText("./Database/create.sql", 
                string.Join("\n", enumsTable) + string.Join("\n",tables.Select(x=>x.Value).ToArray()));
        }

        static string StatisticSchema()
        {
            return "CREATE TABLE IF NOT EXISTS STATISTIC \n" +
                "( \n" +
                "id SERIAL PRIMARY KEY,\n" +
                "matches INT, \n" +
                "wins INT, \n" +
                "map_id INT,\n" +
                "hero_id INT\n" +
                ");\n";
        }

        static string MatchupTableSchema()
        {
            return "CREATE TABLE IF NOT EXISTS MatchupTable \n" +
                "( \n" +
                "id SERIAL PRIMARY KEY,\n" +
                "win_with FLOAT8, \n" +
                "win_against FLOAT8, \n" +
                "map_id INT,\n" +
                "hero_id INT\n" +
                ");\n";
        }


    }
}
