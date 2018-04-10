using Classifier;
using HoTS_Service.Entity;
using HoTS_Service.Entity.AIDto;
using HoTS_Service.Service;
using HoTS_Service.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using static HoTS_Service.Util.Logger;

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

    class Dataset
    {
        public string fileName;
        public DateTime date;
        public long id;
    }

    class Network
    {
        public long dataset_id;
        public long state_id;
        public long meta_id;
        public long id;
        public Json data;
        public bool isBest;

    }

    class Json
    {
        public string data;
        public Json(string data)
        {
            this.data = data;
        }

        public Json()
        {

        }
    }

    class NetworkTuple
    {
        public Json Network;
        public TrainMeta Meta;
    }

    class Program
    {
        static void Main(string[] args)
        {
            log("debug", "Процесс миграции схемы запущен");
            log("debug", "Дамп будет создан для базы данных PostegreSQL");
            if (!Directory.Exists("./Database"))
                Directory.CreateDirectory("./Database");
            if (!Directory.Exists("./Dataset"))
                Directory.CreateDirectory("./Dataset");

            HeroService heroes = new HeroService();
            log("debug", "Загрузка Hero.json");
            heroes.Load("./Source/Hero/Hero.json");
            log("succes", "Hero.json Загружен");

            HeroDetailsService details = new HeroDetailsService();
            log("debug", "Загрузка HeroDetails.json");
            details.Load("./Source/Hero/HeroDetails.json");
            log("succes", "HeroDetails.json Загружен");

            HeroClustersSevice clusters = new HeroClustersSevice();
            log("debug", "Загрузка HeroClusters.json");
            clusters.Load("./Source/Hero/HeroClusters.json");
            log("succes", "HeroClusters.json Загружен");

            MapService maps = new MapService();
            log("debug", "Загрузка Map.json");
            maps.Load("./Source/Map/Map.json");
            log("succes", "Map.json Загружен");

            StatisticService stats = new StatisticService();
            log("debug", "Загрузка Statistic.json");
            stats.Load("./Source/Replay/Statistic.json");
            log("succes", "Statistic.json Загружен");

            HeroStatisticService hstats = new HeroStatisticService();
            log("debug", "Загрузка Statistic_sho.json");
            hstats.Load("./Source/Replay/Statistic_sho.json");
            log("succes", "Statistic_sho.json Загружен");

            MatchupService matchups = new MatchupService();
            log("debug", "Загрузка MatchupTable.json");
            matchups.Load("./Source/Replay/MatchupTable.json");
            log("succes", "MatchupTable.json Загружен");

            log("debug", "Формирование датасета начато");
            Dataset set = MakeDataset();
            log("succes", "Датасета сформирован");

            log("debug", "Инициализация PostegreSQL Converter ORM");
            PostegresConverter converter = new PostegresConverter();
            log("info", "========Конвертация ключей===============");
            log("info", "group => _group");
            log("info", "min_id => id_min");
            log("info", "max_id => id_max");
            log("info", "avg_id => id_avg");
            converter.CustomNameMapper["group"] = "_group";
            converter.CustomNameMapper["min_id"] = "id_min";
            converter.CustomNameMapper["max_id"] = "id_max";
            converter.CustomNameMapper["avg_id"] = "id_avg";
            log("info", "=========================================");
            log("succes", "PostegreSQL Converter ORM инициалирован");
            log("debug", "Генерация секвенсеров");

            string sequensers = @"
alter sequence gaussian_id_seq minvalue 0 start with 0;
select setval('gaussian_id_seq', 0, false);
alter sequence heroclusters_id_seq minvalue 0 start with 0;
select setval('heroclusters_id_seq', 0, false);
alter sequence statisticheroesmax_id_seq minvalue 0 start with 0;
select setval('statisticheroesmax_id_seq', 0, false);
alter sequence statisticheroesmin_id_seq minvalue 0 start with 0;
select setval('statisticheroesmin_id_seq', 0, false);
alter sequence statisticheroesavg_id_seq minvalue 0 start with 0;
select setval('statisticheroesavg_id_seq', 0, false);";

            log("succes", "Cеквенсеры сгенерированы");

            log("debug", "Процесс генерации схемы начат");
            log("info", "========Генерация словарей===============");

            string[] enumsTable = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsEnum && t.Namespace == "HoTS_Service.Entity.Enum")
                       .Select(_enum =>
                       {
                           log("info", _enum.FullName);
                           return converter.CreateDictionary(_enum.Name);
                       })
                       .ToArray();

            log("info", "=========================================");
            log("info", "========Генерация таблиц=================");
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
            log("info", "Hero");

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
            log("info", "HeroDetails");

            tables["mapTable"] = converter.CreateTable("Map", typeof(Map), "id", null);
            log("info", "Map");

            tables["statisticTable"] = StatisticSchema();
            log("info", "Statistic");

            tables["statisticShoMin"] = converter.CreateTable("StatisticHeroesMin",
               typeof(HeroStatisticItemMin), "id", null);
            log("info", "StatitsticHeroesMin");

            tables["statisticShoMax"] = converter.CreateTable("StatisticHeroesMax",
               typeof(HeroStatisticItemMax), "id", null);
            log("info", "StatitsticHeroesMax");

            tables["statisticShoAvg"] = converter.CreateTable("StatisticHeroesAvg",
               typeof(HeroStatisticItemAvg), "id", null);
            log("info", "StatitsticHeroesAvg");

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
            log("info", "StatitsticHeroes");

            tables["matchupTable"] = MatchupTableSchema();
            log("info", "MatchupTable");

            tables["gaussian"] = converter.CreateTable("Gaussian", typeof(Gaussian)
               , "id", null);
            log("info", "Gaussian");

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
            log("info", "GaussianProbabilities");

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
            log("info", "HeroClusters");

            tables["dataset"] = converter.CreateTable("Dataset", typeof(Dataset), "id", null);
            log("info", "Dataset");

            tables["trainingState"] = converter.CreateTable("TrainingState", typeof(LogInfo), "id", null);
            log("info", "TrainingState");

            tables["trainMeta"] = converter.CreateTable("TrainingMeta", typeof(TrainMeta), "id", null);
            log("info", "TrainingMeta");

            tables["network"] = converter.CreateTable("Network", typeof(Network)
                , "id", new List<Foreign> {
                    new Foreign()
                    {
                        DataTable = "Dataset",
                        Key = "dataset_id",
                        ForeignKey = "id"
                    },
                    new Foreign()
                    {
                        DataTable = "TrainingState",
                        Key = "state_id",
                        ForeignKey = "id"
                    },
                     new Foreign()
                    {
                        DataTable = "TrainingMeta",
                        Key = "meta_id",
                        ForeignKey = "id"
                    },
                });

            log("info", "Network");

            log("info", "=========================================");
            log("succes", "Схема успешно сгенерирована");

            log("info", "========Генерация дампа данных===========");
            string[] enumsData = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsEnum && t.Namespace == "HoTS_Service.Entity.Enum")
                       .Select(_enum => converter.InsertDictionary(_enum))
                       .ToArray();


            data["heroesTable"] = converter.Insert("Hero", heroes.All());
            log("info", "Hero");

            data["detailsTable"] = converter.Insert("HeroDetails", details.All());
            log("info", "HeroDetails");

            data["mapTable"] = converter.Insert("Map", maps.All());
            log("info", "Map");

            data["statisticTable"] = StatisticData(stats);
            log("info", "Statistic");

            data["statisticShoMin"] = converter.Insert("StatisticHeroesMin", hstats.All().Item2);
            log("info", "StatisticHeroesMin");

            data["statisticShoMax"] = converter.Insert("StatisticHeroesMax", hstats.All().Item3);
            log("info", "StatisticHeroesMax");

            data["statisticShoAvg"] = converter.Insert("StatisticHeroesAvg", hstats.All().Item1);
            log("info", "StatisticHeroesAvg");

            data["statisticSho"] = HeroesStatisticData(hstats);
            log("info", "HeroesStatistic");

            data["matchupTable"] = MatchupData(matchups, heroes.Count());
            int probId = 0;

            data["gaussian"] = converter.Insert("Gaussian", clusters.Select(x => x.Gaussian));
            log("info", "Gaussian");

            data["probabilities"] = converter.Insert("GaussianProbabilities",
               clusters.
               Select(x => x.Gaussian.Probability.
                   Select(y => new Probabilities()
                   {
                       id = probId++,
                       value = y,
                       gaussian_id = x.Id,
                   })).SelectMany(z => z));
            log("info", "GaussianProbabilities");

            data["heroClusters"] = converter.Insert("HeroClusters", clusters.All());
            log("info", "HeroClusters");

            data["dataset"] = converter.Insert("Dataset", set);
            log("info", "Dataset");


            string[] trainingStatesAll = Directory
                .GetFiles("./Source/Network", "*.json", SearchOption.AllDirectories);

            string[] trainingStates = trainingStatesAll
                .Where(x => !x.Contains("Best") && x.Contains("\\Report\\"))
                .ToArray();

            string[] trainingStatesBest = trainingStatesAll
                .Where(x => x.Contains("Best") && x.Contains("\\Report\\"))
                .ToArray();

            long[] traingingStateIds = trainingStates
                .Select(file => File.GetCreationTime(file).Ticks)
                .ToArray();

            long[] traingingStateIdsBest = trainingStatesBest
                .Select(file => File.GetCreationTime(file).Ticks)
                .ToArray();


            var trainigsStateData = trainingStates
                .Select(x => File.ReadAllText(x))
                .Select(json => ((Dictionary<string, dynamic>)JSONWebParser.Load(json)))
                .Select((obj, index) => new
                {
                    id = traingingStateIds[index],
                    error = (double)obj["error"],
                    iteration = (int)obj["iteration"],
                    percent = (double)obj["percent"],
                    validError = (double)obj["validError"],
                    validPercent = (double)obj["validPercent"]
                })
                .Concat(
                    trainingStatesBest
                    .Select(x => File.ReadAllText(x))
                    .Select(json => ((Dictionary<string, dynamic>)JSONWebParser.Load(json)))
                    .Select((obj, index) => new
                    {
                        id = traingingStateIdsBest[index],
                        error = (double)obj["error"],
                        iteration = (int)obj["iteration"],
                        percent = (double)obj["percent"],
                        validError = (double)obj["validError"],
                        validPercent = (double)obj["validPercent"]
                    }));

            data["trainingState"] = converter.Insert("TrainingState", trainigsStateData);
            log("info", "TrainingState");

            string[] networksAll = Directory
                .GetFiles("./Source/Network", "*.json", SearchOption.AllDirectories)
                .ToArray();

            string[] networks = networksAll
                .Where(x => !x.Contains("Best") && !x.Contains("\\Report\\"))
                .ToArray();

            string[] networksBest = networksAll
                .Where(x => x.Contains("Best") && !x.Contains("\\Report\\"))
                .ToArray();

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var networksMeta = networks
                .Select(x => File.ReadAllText(x))
                .Select(json => serializer.Deserialize<NetworkTuple>(json).Meta)
                .Select((obj, index) => new
                {
                    Alias = obj.Alias,
                    ClusterPath = obj.ClusterPath,
                    Name = obj.Name,
                    Description = obj.Description,
                    Id = traingingStateIds[index]
                }
                )
                .Concat(
                    networksBest
                    .Select(x => File.ReadAllText(x))
                    .Select(json => serializer.Deserialize<NetworkTuple>(json).Meta)
                    .Select((obj, index) => new
                    {
                        Alias = obj.Alias,
                        ClusterPath = obj.ClusterPath,
                        Name = obj.Name,
                        Description = obj.Description,
                        Id = traingingStateIdsBest[index]
                    }));

            data["trainingMeta"] = converter.Insert("TrainingMeta", networksMeta);
            log("info", "TrainingMeta");

            var networksData = networks
                .Select(x => File.ReadAllText(x))
                .Select(json => serializer.Deserialize<NetworkTuple>(json).Network)
                .Select((x, index) => new Network()
                {
                    dataset_id = set.id,
                    data = new Json(File.ReadAllText(networks[index])),
                    state_id = traingingStateIds[index],
                    meta_id = traingingStateIds[index],
                    isBest = false,
                    id = traingingStateIds[index]
                })
                .Concat(
                    networksBest
                    .Select(x => File.ReadAllText(x))
                    .Select(json => serializer.Deserialize<NetworkTuple>(json).Network)
                    .Select((x, index) => new Network()
                    {
                        dataset_id = set.id,
                        data = new Json(File.ReadAllText(networksBest[index])),
                        state_id = traingingStateIdsBest[index],
                        meta_id = traingingStateIdsBest[index],
                        isBest = true,
                        id = traingingStateIdsBest[index]
                    }));

            data["network"] = converter.Insert("Network", networksData);
            log("info", "Network");
            log("info", "=========================================");
            log("succes", "Дамп данных успешно сгенерирован");
            log("succes", "Схема успешно сгенерирована");

            File.WriteAllText("./Database/create.sql",
                string.Join("\n", enumsTable) +
                string.Join("\n", tables.Select(x => x.Value).ToArray()));
            File.WriteAllText("./Database/insert.sql",
                sequensers + "\n\n" +
                string.Join("\n", enumsData) +
                string.Join("\n", data.Select(x => x.Value).ToArray()));
        }

        private static Dataset MakeDataset()
        {
            Directory.CreateDirectory("./Dataset/Data");
            Directory.CreateDirectory("./Dataset/Data/AI");
            Directory.CreateDirectory("./Dataset/UI/");
            Directory.CreateDirectory("./Dataset/UI/Images");
            Directory.CreateDirectory("./Dataset/UI/Icons");
            Directory.CreateDirectory("./Archive/");
            string[] jsonFiles = Directory
                .GetFiles("./Source/", "*.json",SearchOption.AllDirectories)
                .Where(x=>!x.EndsWith("_temp.json") && !x.Contains("Network"))
                .ToArray();

            log("debug", "Исключение временных файлов");

            string[] networkFiles = Directory
                .GetFiles("./Source/Network", "*.json", SearchOption.AllDirectories)
                .Where(x=>x.Contains("Best") && !x.Contains("\\Report\\"))
                .ToArray();

            log("debug", "Исключение отчетов обучения моделей");
            log("debug", "Выборка лучших моделей");

            string[] icons = Directory
                .GetFiles("./Source/Icons", "*.png");
            string[] images = Directory
                .GetFiles("./Source/Images", "*.png");
            foreach (var file in jsonFiles)
            {
                File.Copy(file, $"./Dataset/Data/{Path.GetFileName(file)}",true);
            }
            int num = 0;
            foreach (var file in networkFiles)
            {
                File.Copy(file, $"./Dataset/Data/AI/" +
                    $"{Path.GetFileNameWithoutExtension(file)}_" +
                    $"{++num}" +
                    $"{Path.GetExtension(file)}", true);
            }
            foreach(var icon in icons)
            {
                File.Copy(icon, $"./Dataset/UI/Icons/{Path.GetFileName(icon)}", true);
            }
            foreach (var image in images)
            {
                File.Copy(image, $"./Dataset/UI/Images/{Path.GetFileName(image)}", true);
            }

            log("debug", "Архивирование датасета");

            DateTime date = File.GetCreationTime("./Dataset/Data/Hero.json");
            string fileName = $"dataset_{date.ToFileTime()}.rar";
            File.Delete( $"./Archive/dataset_{date.ToFileTime()}.rar");
            ZipFile.CreateFromDirectory("./Dataset/", $"./Archive/dataset_{date.ToFileTime()}.rar",CompressionLevel.Optimal,false);
            Directory.Delete($"./Dataset/",true);
            return new Dataset() { date = date, fileName = fileName, id = date.ToFileTime() };
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
