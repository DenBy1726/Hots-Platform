using HoTS_Service.Entity;
using HoTS_Service.Entity.Enum;
using HoTS_Service.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HoTS_Service.Util.Logger;
using static HoTS_Service.Util.ConsoleProgress;


namespace Parser.Service
{
    /// <summary>
    /// Парсер, который автоматически обработает данные
    /// </summary>
    public class AutoParser : CLIParser<object>
    {

        ///класс для хранения путей к входным файлам
        public class InputFileParam
        {
            string inputFolder;

            public InputFileParam(string input)
            {
                inputFolder = input;
                Validate();
            }

            public string InputFolder { get => inputFolder;}

            void Validate()
            {
                for (int i = 0; i < Count; i++)
                {
                    CheckFileExist(inputFolder + Path[i], Name[i]);
                    log("debug", $"Входной файл {this[i]} найден");
                }
            }

            string[] path = new string[]
            {
                "\\Hero\\",
                "\\Map\\",
                "\\Mapper\\Replay\\",
                "\\Replay\\"
            };

            string[] name = new string[]
            {
                "Hero.csv",
                "Map.csv",
                "Replays.csv",
                "*.csv"
            };

            public string this[int i]
            {
                get
                {
                    return inputFolder + Path[i] + Name[i];
                }
            }

            public int Count
            {
                get
                {
                    return Name.Length;
                }
            }

            public string[] Path { get => path; set => path = value; }
            public string[] Name { get => name; set => name = value; }

            /// <summary>
            /// проверяет существует ли файл в папке. Если нет, то выбрасывает исключение.
            /// Если да, то возвращает путь к файлу
            /// Если имя не задано, ищет все файлы 
            /// </summary>
            /// <param name="folder"></param>
            /// <param name="name"></param>
            /// <returns></returns>
            public static string CheckFileExist(string folder, string name)
            {

                if (Directory.GetFiles(folder, name).Length == 0)
                {
                    log("Error", "");
                    throw new FileNotFoundException($"Не найден файл {name} по пути {folder} .");
                }
                return folder;

            }
        }

        ///класс для хранения путей к выходным файлам         
        public class OutputFileParam
        {
            string outputFolder;
            
            public OutputFileParam(string output)
            {
                OutputFolder = output;
                CreateFolder();
            }

            public void CreateFolder()
            {
                for (int i = 0; i < Count; i++)
                {
                    if (Directory.Exists(OutputFolder + Path[i]) == false)
                    {
                        Directory.CreateDirectory(OutputFolder + Path[i]);
                        log("debug", $"Выходной каталог { OutputFolder + Path[i]} найден");
                    }
                   
                }
            }

            string[] path = new string[]
            {
                "\\Hero\\",
                "\\Map\\",
                "\\Replay\\",
                "\\Replay\\",
                "\\Replay\\",
                "\\Replay\\",
                "\\",
                "\\Hero\\",
                "\\Map\\",
                "\\Replay\\",
                "\\Replay\\",
                "\\Replay\\"
            };

            string[] name = new string[]
            {
                "Hero_temp.json",
                "Map_temp.json",
                "Replays.csv",
                "Statistic_temp.json",
                "Statistic_o.csv",
                "Statistic_sho_temp.json",
                "NNData2.csv",
                "Hero.json",
                "Map.json",
                "Statistic_sho.json",
                "Statistic.json",
                "MatchupTable.json"
                
            };

            public string this[int i]
            {
                get
                {
                    return OutputFolder + Path[i] + Name[i];
                }
            }

            public int Count
            {
                get
                {
                    return Name.Length;
                }
            }

            public string[] Path { get => path; set => path = value; }
            public string[] Name { get => name; set => name = value; }
            public string OutputFolder { get => outputFolder; set => outputFolder = value; }
        }

        public class ParserKeys
        {
            public bool useOlderData = false;
            public bool removeNoiseData = true;
            public bool removeNonDeterminate = true;
        }

        public class HeroParser : CLIParser<Hero>
        {
            public string Input, Output;
            public List<Hero> Hero = new List<Hero>();
            public int GroupCount = Enum.GetValues(typeof(HeroGroup)).Length;
            public int SubGroupCount = Enum.GetValues(typeof(HeroSubGroup)).Length;

            public HeroParser(string input,string output)
            {
                Input = input;
                Output = output;
            }

            public void Run()
            {
                OpenSource(Input);

                object data;

                int i = 0;
                while ((data = ReadData()) != null)
                {
                    Hero h = ParseData(data);
                    if (h == null)
                    {
                        log("debug", $"Строка {i+1}");
                        continue;
                    }
                    Hero.Add(h);
                    log("info",h.ToString());
                    i++;
                }

                CSVParser.Dispose();

                log("debug", "Сохранение данных о героях начато");
                Save(Output, Hero, Hero.GetType());
                log("succes", "Сохранение данных о героях завершено");
            }
            

            public override void Validate()
            {
                
            }

            protected override void OpenSource(string path)
            {
                log("debug", "Инициализация парсера героев начата");
                CSVParser.Load(path);
                log("succes", "Инициализация парсера героев завершена");
            }

            protected override Hero ParseData(object obj)
            {
                string[] data = obj as string[];
                if (data == null)
                    return null;
                if (data.Length != 4 && data.Length != 2)
                {
                    log("warng", "Неверный формат строки героя");
                    return null;
                }

                int id = Int32.Parse(data[0]);
                string name = data[1];

                HeroGroup group;
                HeroSubGroup subgroup;

                if (data.Length == 2)
                {
                    group = HeroGroup.Unknown;
                    subgroup = HeroSubGroup.Unknown;
                }
                else
                {

                    bool parse = Enum.TryParse(data[2], true, out group);
                    if (parse == false || (int)group >= GroupCount)
                    {
                        log("warng", $"Неизвестная группа героя {data[2]}");
                        group = HeroGroup.Unknown;
                    }

                    parse = Enum.TryParse(data[3].Replace(' ', '_'), true, out subgroup);
                    if (parse == false || (int)subgroup >= SubGroupCount)
                    {
                        log("warng", $"Неизвестная подгруппа героя {data[3]}");
                        subgroup = HeroSubGroup.Unknown;
                    }
                }

                Hero temp = new Hero(id, name, group, subgroup);
                return temp;
            }

            protected override object ReadData()
            {
                return CSVParser.Next();
            }

            protected override void Save(string name, object obj, Type t)
            {
                File.WriteAllText(name, JSonParser.Save(obj, t));
            }
        }

        public class MapParser :CLIParser<Map>
        {
            public string Input, Output;
            public List<Map> Map = new List<Map>();

            public MapParser(string input,string output)
            {
                Input = input;
                Output = output;
            }

            
            public void Run()
            {
                OpenSource(Input);

                object data;

                while ((data = ReadData()) != null)
                {
                    Map h = ParseData(data);
                    if (h == null)
                        continue;
                    Map.Add(h);
                    log("info",h.ToString());
                }

                CSVParser.Dispose();

                Save(Output, Map, Map.GetType());
            }

            public override void Validate()
            {
                
            }

            protected override void OpenSource(string path)
            {
                log("debug", "Инициализация парсера карт начата");
                CSVParser.Load(path);
                log("succes", "Инициализация парсера карт завершена");
            }

            protected override Map ParseData(object obj)
            {
                string[] data = obj as string[];
                if (data == null)
                    return null;
                if (data.Length != 2 && data.Length != 4)
                {
                    log("warng", "Неверный формат строки карты");
                    return null;
                }

                int id = Int32.Parse(data[0]);
                if (id > 1000)
                {
                    log("warng", $"Приведение абсолютного ИД к относительному {id} -> {id%1000}");
                    id = id % 1000;
                }
                string name = data[1];

                Map temp = new Map(id, name);
                return temp;
            }

            protected override object ReadData()
            {
                return CSVParser.Next();
            }

            protected override void Save(string name, object obj, Type t)
            {
                File.WriteAllText(name, JSonParser.Save(obj, t));
            }
        }

        public class ReplaySchemaParser: CLIParser<ReplaySchema>
        {
            public string InputSchema,InputData,Output,Extension,StatisticOutput,
                StatisticHeroOutput;
            ///размер пакета для сохранения в файл.
            public int BatchCount;
            public int GameModeCount = Enum.GetValues(typeof(HeroGroup)).Length;

            public Statistic[] Stat;

            public HeroStatisticItemAvg[] AvgStat;
            public HeroStatisticItemMin[] MinStat;
            public HeroStatisticItemMax[] MaxStat;
            

            public ReplaySchemaParser(string inputSchema, string inputData,
                string extension,string output,string statisticOutput, string statisticHeroOutput,
                int batchCount = 10000 )
            {
                InputSchema = inputSchema;
                InputData = inputData;
                Output = output;
                Extension = extension;
                BatchCount = batchCount;
                StatisticOutput = statisticOutput;
                StatisticHeroOutput = statisticHeroOutput;

                //для юнит тестов
                if (HParser == null)
                {
                    HParser = new HeroParser("", "")
                    {
                        Hero = Enumerable.Repeat<Hero>(null, 1000).ToList()
                    };
                }

                AvgStat = new HeroStatisticItemAvg[HParser.Hero.Count];
                MinStat = new HeroStatisticItemMin[HParser.Hero.Count];
                MaxStat = new HeroStatisticItemMax[HParser.Hero.Count];

                Stat = new Statistic[1000];
            }


            public void Run()
            {
                //очищаем выходной файл если он уже существовал
                File.WriteAllText(Output, "");
                log("debug", $"Выходной файл был очищен");

                log("debug", "Инициализация статистики героев начата");
                InitializeHeroStat(AvgStat, MinStat, MaxStat);
                log("succes", "Инициализация статистики героев завершена");

                OpenSource(InputSchema);
                OpenBatchSource(InputData,Extension);
                AnimatedBar bar = new AnimatedBar();

                for (int j = 0; j < Stat.Length; j++)
                {
                    Stat[j] = new Statistic()
                    {
                        Statictic = new StatisticItem()
                        {
                            Matches = new int[HParser.Hero.Count + 1],
                            Wins = new int[HParser.Hero.Count + 1]
                        }
                    };
                }

                object data;
                int i = 0;
                int parsed = 0;
                int mapCount = 0;
                int replayId = 0;
                List<HeroStatisticItem> replayData = new List<HeroStatisticItem>();
                List<Match> batch = new List<Match>();
                log("debug", $"Датасет будет дополнен обратными записями");
                while ((data = ReadData()) != null)
                {
                    var r = ParseData(data);
                    if (r == null)
                    {
                        log("debug", $"Строка {i + 1}");
                        continue;
                    }
                    if (r.mapId > mapCount)
                        mapCount = r.mapId;

                    Stat[r.mapId].Statictic.Ammount++;

                    replayData.Clear();
                    do
                    {
                        data = ReadBatchData();
                        var replay = ParseReplayData(data);

                        if (replay == null)
                        {
                            log("debug", $"Строка {i + 1}");
                            continue;
                        }

                        //в csv при делении файла на части последняя строка первого файла
                        //и первой строка второго файла может повтоярться.Делаем проверку
                        if (replay.replayId == replayId && replayData.Count == 0)
                        {
                            continue;
                        }
                        replayId = replay.replayId;
                        replay.sec = r.length;
                        replayData.Add(replay);
                    } while (replayData.Count < 10);
                    if (replayData.Count < 10)
                    {
                        log("warng", $"Недостаточно данных для повтора с индексом " +
                              $"{r.id} .Ожидалось 10, а удалось считать {replayData.Count}");
                    }
                    else
                    {
                        if (replayData.Count > 10)
                        {
                            log("warng", $"Лишние данные для повтора с индексом " +
                                $"{r.id} .Ожидалось 10, а считано {replayData.Count}" +
                                $"лишние записи будут проигнорированы");
                            replayData = replayData.GetRange(0, 10);
                        }

                        var m = MakeMatchData(replayData, r);
                        var rev_m = MakeReverseMatchData(replayData, r);

                        if (m == null || rev_m == null)
                        {
                            log("debug", $"Строка {i + 1}");
                            continue;
                        }

                        batch.Add(m);
                        batch.Add(rev_m);

                        for (int j = 0; j < replayData.Count; j++)
                        {
                            UpdateHeroStat(AvgStat, MinStat, MaxStat, replayData[j]);
                        }
                        for (int j=0;j<m.YourTeam.Length;j++)
                        {
                            Stat[r.mapId].Statictic.Matches[m.YourTeam[j]]++;
                            Stat[r.mapId].Statictic.Wins[m.YourTeam[j]]++;
                        }

                        for (int j = 0; j < m.EnemyTeam.Length; j++)
                        {
                            Stat[r.mapId].Statictic.Matches[m.EnemyTeam[j]]++;
                        }

                        parsed += 1;
                        if (parsed % 10000 == 0)
                        {
                            bar.Step(parsed);
                        }

                        

                        if(batch.Count == BatchCount)
                        {
                            Save(Output, batch, typeof(List<Match>));
                            batch.Clear();
                        }
                    }
                    i++;
                    
                }
                //если остались не сохраненные данные
                if (batch.Count > 0)
                {
                    Save(Output, batch, typeof(List<Match>));
                    batch.Clear();
                }

                ComputeAverage(AvgStat);

                Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], HeroStatisticItemMax[]>
                    heroStat = new Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], 
                    HeroStatisticItemMax[]>(AvgStat, MinStat, MaxStat);

                File.WriteAllText(StatisticHeroOutput, JSonParser.Save(heroStat, 
                    typeof(Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], 
                    HeroStatisticItemMax[]>)), Encoding.Default);

                File.WriteAllText(StatisticOutput, JSonParser.Save(Stat, typeof(Statistic[])), Encoding.Default);
            }

            public Match MakeMatchData(List<HeroStatisticItem> data, ReplaySchema sch)
            {
                ///ошибка тут. Нужно учитывать .winrate при раскидывании идов героев.
                Match m = new Match();
                int m_teamCount = 0;
                int e_teamCount = 0;
                try
                {
                    foreach (var it in data)
                    {
                        if (it.winrate == 1)
                            m.YourTeam[m_teamCount++] = it.heroId;
                        else
                            m.EnemyTeam[e_teamCount++] = it.heroId;
                    }
                    m.Map = sch.mapId;
                    m.ProbabilityToWin = 1;
                    if (m_teamCount != 5 || e_teamCount != 5)
                    {
                        log("warng", "Неверный формат данных. Число игроков в каждой команде должно быть равно 5 ");
                        return null;
                    }
                }
                catch
                {
                    log("warng", "Неверный формат данных. Число игроков в каждой команде должно быть равно 5 ");
                    return null;
                }
                return m;
            }

            public Match MakeReverseMatchData(List<HeroStatisticItem> data, ReplaySchema sch)
            {
                Match m = new Match();
                int m_teamCount = 0;
                int e_teamCount = 0;
                try
                {
                    foreach (var it in data)
                    {
                        if (it.winrate == 0)
                            m.YourTeam[m_teamCount++] = it.heroId;
                        else
                            m.EnemyTeam[e_teamCount++] = it.heroId;
                    }
                    m.Map = sch.mapId;
                    m.ProbabilityToWin = 0;
                }
                catch
                {
                    log("warng", "Неверный формат данных. Число игроков в каждой команде должно быть равно 5 ");
                    return null;
                }
                return m;
            }

            public override void Validate()
            {

            }

            protected override void OpenSource(string path)
            {
                log("debug", "Инициализация парсера схемы повторов начата");
                CSVParser.Load(path);
                log("succes", "Инициализация парсера схемы повторов завершена");
            }

            protected void OpenBatchSource(string path,string ext)
            {
                log("debug", "Инициализация парсера схемы повторов начата");
                
                CSVBParser.Load(Directory.GetFiles(path, ext));
                log("succes", "Инициализация парсера схемы повторов завершена");
            }

            protected override ReplaySchema ParseData(object obj)
            {
                string[] data = obj as string[];
                if (data == null)
                    return null;
                if (data.Length != 5)
                {
                    log("warng", "Неверный формат строки схемы повторов");
                    return null;
                }
                ReplaySchema sch = new ReplaySchema();
                try
                {
                    sch.id = Int32.Parse(data[0]);
                    sch.mapId = Int32.Parse(data[2]);
                    if (sch.mapId > 1000)
                        sch.mapId = sch.mapId % 1000;
                    var dt = DateTime.Parse(data[3]);
                    sch.length = dt.Second + dt.Minute * 60;
                    bool parse = Enum.TryParse(data[1],out sch.gameMode);
                    sch.gameMode -= 2;
                    if (parse == false || (int)sch.gameMode >= GameModeCount)
                    {
                        log("warng", $"Неизвестный режим игры {data[1]}");
                        sch.gameMode = GameMode.Unknown;
                    }

                    return sch;
                }
                catch (Exception e)
                {
                    log("warng", "Неверный формат строки схемы повтора");
                    return null;
                }
                
            }

            protected HeroStatisticItem ParseReplayData(object obj)
            {
                string[] data = obj as string[];
                if (data == null)
                    return null;
                if (data.Length != 20)
                {
                    log("warng", "Неверный формат строки повторов");
                    return null;
                }
                HeroStatisticItem stat = new HeroStatisticItem();
                try
                {
                    stat.replayId = Int32.Parse(data[0]);
                    stat.heroId = Int32.Parse(data[2]);
                    stat.winrate = Int32.Parse(data[4]);
                    stat.killPerSec = Int32.Parse(data[8]);
                    stat.assistPerSec = Int32.Parse(data[9]);
                    stat.deathPerSec = Int32.Parse(data[10]);
                    stat.dps = Int32.Parse(data[12]);
                    stat.sdps = Int32.Parse(data[13]);
                    if (data[14] != "")
                        stat.hps = Int32.Parse(data[14]);
                    else
                        stat.hps = 0;

                    if (data[16] != "")
                        stat.damageTakenPerSec = Int32.Parse(data[16]);
                    else
                        stat.damageTakenPerSec = 0;
                    stat.expPerSec = Int32.Parse(data[17]);
                    stat.campTakenPerSec = Int32.Parse(data[19]);
                    return stat;
                }
                catch
                {
                    log("warng", "Неверный формат строки повтора");
                    return null;
                }
            }

            protected override object ReadData()
            {
                return CSVParser.Next();
            }

            protected object ReadBatchData()
            {
                return CSVBParser.Next();
            }

            private void InitializeHeroStat(HeroStatisticItemAvg[] avgStat, HeroStatisticItemMin[] minStat, HeroStatisticItemMax[] maxStat)
            {
                for (int i = 0; i < minStat.Length; i++)
                {
                    minStat[i] = new HeroStatisticItemMin();
                    avgStat[i] = new HeroStatisticItemAvg();
                    minStat[i].assistPerSec = Double.MaxValue;
                    minStat[i].campTakenPerSec = Double.MaxValue;
                    minStat[i].damageTakenPerSec = Double.MaxValue;
                    minStat[i].deathPerSec = Double.MaxValue;
                    minStat[i].dps = Double.MaxValue;
                    minStat[i].expPerSec = Double.MaxValue;
                    minStat[i].hps = Double.MaxValue;
                    minStat[i].killPerSec = Double.MaxValue;
                    minStat[i].sdps = Double.MaxValue;
                    minStat[i].sec = Double.MaxValue;
                    minStat[i].winrate = 0;
                }

                for (int i = 0; i < maxStat.Length; i++)
                {
                    maxStat[i] = new HeroStatisticItemMax();
                    maxStat[i].assistPerSec = Double.MinValue;
                    maxStat[i].campTakenPerSec = Double.MinValue;
                    maxStat[i].damageTakenPerSec = Double.MinValue;
                    maxStat[i].deathPerSec = Double.MinValue;
                    maxStat[i].dps = Double.MinValue;
                    maxStat[i].expPerSec = Double.MinValue;
                    maxStat[i].hps = Double.MinValue;
                    maxStat[i].killPerSec = Double.MinValue;
                    maxStat[i].sdps = Double.MinValue;
                    maxStat[i].sec = Double.MinValue;
                    maxStat[i].winrate = 0;
                }
            }

            private void UpdateHeroStat(HeroStatisticItemAvg[] avgStat, HeroStatisticItemMin[] minStat, HeroStatisticItemMax[] maxStat, HeroStatisticItem r)
            {
                int id = r.heroId;
                double time = r.sec;
                // обновление статистики
                minStat[id].assistPerSec = Min(minStat[id].assistPerSec, r.assistPerSec / time);
                minStat[id].campTakenPerSec = Min(minStat[id].campTakenPerSec, r.campTakenPerSec / time);
                minStat[id].damageTakenPerSec = Min(minStat[id].damageTakenPerSec, r.damageTakenPerSec / time);
                minStat[id].deathPerSec = Min(minStat[id].deathPerSec, r.deathPerSec / time);
                minStat[id].dps = Min(minStat[id].dps, r.dps / time);
                minStat[id].expPerSec = Min(minStat[id].expPerSec, r.expPerSec / time);
                minStat[id].hps = Min(minStat[id].hps, r.hps / time);
                minStat[id].killPerSec = Min(minStat[id].killPerSec, r.killPerSec / time);
                minStat[id].sdps = Min(minStat[id].sdps, r.sdps / time);
                minStat[id].sec = Min(minStat[id].sec, r.sec);

                maxStat[id].assistPerSec = Max(maxStat[id].assistPerSec, r.assistPerSec / time);
                maxStat[id].campTakenPerSec = Max(maxStat[id].campTakenPerSec, r.campTakenPerSec / time);
                maxStat[id].damageTakenPerSec = Max(maxStat[id].damageTakenPerSec, r.damageTakenPerSec / time);
                maxStat[id].deathPerSec = Max(maxStat[id].deathPerSec, r.deathPerSec / time);
                maxStat[id].dps = Max(maxStat[id].dps, r.dps / time);
                maxStat[id].expPerSec = Max(maxStat[id].expPerSec, r.expPerSec / time);
                maxStat[id].hps = Max(maxStat[id].hps, r.hps / time);
                maxStat[id].killPerSec = Max(maxStat[id].killPerSec, r.killPerSec / time);
                maxStat[id].sdps = Max(maxStat[id].sdps, r.sdps / time);
                maxStat[id].sec = Max(maxStat[id].sec, r.sec);

                avgStat[id].assistPerSec += r.assistPerSec / time;
                avgStat[id].campTakenPerSec += r.campTakenPerSec / time;
                avgStat[id].damageTakenPerSec += r.damageTakenPerSec / time;
                avgStat[id].deathPerSec += r.deathPerSec / time;
                avgStat[id].dps += r.dps / time;
                avgStat[id].expPerSec += r.expPerSec / time;
                avgStat[id].hps += r.hps / time;
                avgStat[id].killPerSec += r.killPerSec / time;
                avgStat[id].sdps += r.sdps / time;
                avgStat[id].sec += r.sec;
                avgStat[id].winrate += r.winrate;

                avgStat[id].count++;

            }

            private void ComputeAverage(HeroStatisticItemAvg[] avgStat)
            {
                for (int i = 0; i < avgStat.Length; i++)
                {
                    if (avgStat[i].count == 0)
                        continue;
                    avgStat[i].assistPerSec /= avgStat[i].count;//support
                    avgStat[i].campTakenPerSec /= avgStat[i].count;//specialist
                    avgStat[i].damageTakenPerSec /= avgStat[i].count;//tank
                    avgStat[i].deathPerSec /= avgStat[i].count;//tank'
                    avgStat[i].dps /= avgStat[i].count;//assasin
                    avgStat[i].expPerSec /= avgStat[i].count;//specialist
                    avgStat[i].hps /= avgStat[i].count;//support
                    avgStat[i].killPerSec /= avgStat[i].count;//assasin
                    avgStat[i].sdps /= avgStat[i].count;//specialist
                    avgStat[i].sec /= avgStat[i].count;
                    avgStat[i].winrate /= avgStat[i].count;
                }

                var temp = GetExtremumValues(avgStat);
                var minStat = temp.Item1;
                var maxStat = temp.Item2;

                for (int i = 0; i < avgStat.Length; i++)
                {
                    avgStat[i].assassinRating = ComputeAssasinValue(avgStat[i], minStat, maxStat);
                    avgStat[i].warriorRating = ComputeWarriorValue(avgStat[i], minStat, maxStat);
                    avgStat[i].specialistRating = ComputeSpecValue(avgStat[i], minStat, maxStat);
                    avgStat[i].supportRating = ComputeSupportValue(avgStat[i], minStat, maxStat);
                }

            }

            /// <summary>
            ///возвращает среднее минимальное и среднее макимальное рассчитанное из выборки всех героев
            /// </summary>
            /// <returns></returns>
            private Tuple<HeroStatisticItemMin, HeroStatisticItemMax> GetExtremumValues(HeroStatisticItemAvg[] avgStat)
            {
                HeroStatisticItemMin minStat = new HeroStatisticItemMin();
                HeroStatisticItemMax maxStat = new HeroStatisticItemMax();

                {
                    minStat.assistPerSec = Double.MaxValue;
                    minStat.campTakenPerSec = Double.MaxValue;
                    minStat.damageTakenPerSec = Double.MaxValue;
                    minStat.deathPerSec = Double.MaxValue;
                    minStat.dps = Double.MaxValue;
                    minStat.expPerSec = Double.MaxValue;
                    minStat.hps = Double.MaxValue;
                    minStat.killPerSec = Double.MaxValue;
                    minStat.sdps = Double.MaxValue;
                    minStat.sec = Double.MaxValue;
                    minStat.winrate = 0;
                }


                {
                    maxStat.assistPerSec = Double.MinValue;
                    maxStat.campTakenPerSec = Double.MinValue;
                    maxStat.damageTakenPerSec = Double.MinValue;
                    maxStat.deathPerSec = Double.MinValue;
                    maxStat.dps = Double.MinValue;
                    maxStat.expPerSec = Double.MinValue;
                    maxStat.hps = Double.MinValue;
                    maxStat.killPerSec = Double.MinValue;
                    maxStat.sdps = Double.MinValue;
                    maxStat.sec = Double.MinValue;
                    maxStat.winrate = 0;
                }

                for (int i = 0; i < avgStat.Length; i++)
                {
                    if (avgStat[i].count == 0)
                        continue;
                    minStat.assistPerSec = Min(minStat.assistPerSec, avgStat[i].assistPerSec);
                    minStat.campTakenPerSec = Min(minStat.campTakenPerSec, avgStat[i].campTakenPerSec);
                    minStat.damageTakenPerSec = Min(minStat.damageTakenPerSec, avgStat[i].damageTakenPerSec);
                    minStat.deathPerSec = Min(minStat.deathPerSec, avgStat[i].deathPerSec);
                    minStat.dps = Min(minStat.dps, avgStat[i].dps);
                    minStat.expPerSec = Min(minStat.expPerSec, avgStat[i].expPerSec);
                    minStat.hps = Min(minStat.hps, avgStat[i].hps);
                    minStat.killPerSec = Min(minStat.killPerSec, avgStat[i].killPerSec);
                    minStat.sdps = Min(minStat.sdps, avgStat[i].sdps);
                    minStat.sec = Min(minStat.sec, avgStat[i].sec);

                    maxStat.assistPerSec = Max(maxStat.assistPerSec, avgStat[i].assistPerSec);
                    maxStat.campTakenPerSec = Max(maxStat.campTakenPerSec, avgStat[i].campTakenPerSec);
                    maxStat.damageTakenPerSec = Max(maxStat.damageTakenPerSec, avgStat[i].damageTakenPerSec);
                    maxStat.deathPerSec = Max(maxStat.deathPerSec, avgStat[i].deathPerSec);
                    maxStat.dps = Max(maxStat.dps, avgStat[i].dps);
                    maxStat.expPerSec = Max(maxStat.expPerSec, avgStat[i].expPerSec);
                    maxStat.hps = Max(maxStat.hps, avgStat[i].hps);
                    maxStat.killPerSec = Max(maxStat.killPerSec, avgStat[i].killPerSec);
                    maxStat.sdps = Max(maxStat.sdps, avgStat[i].sdps);
                    maxStat.sec = Max(maxStat.sec, avgStat[i].sec);
                }

                return new Tuple<HeroStatisticItemMin, HeroStatisticItemMax>(minStat, maxStat);

            }


            private double ComputeAssasinValue(HeroStatisticItemAvg avg, HeroStatisticItemMin min, HeroStatisticItemMax max)
            {
                double dpsComponent = (avg.dps - min.dps) / (max.dps - min.dps);
                double killComponent = (avg.killPerSec - min.killPerSec) / (max.killPerSec - min.killPerSec);
                return dpsComponent * 0.7 + killComponent * 0.3;
            }

            private double ComputeWarriorValue(HeroStatisticItemAvg avg, HeroStatisticItemMin min, HeroStatisticItemMax max)
            {
                double dTakenComponent = (avg.damageTakenPerSec - min.damageTakenPerSec) /
                    (max.damageTakenPerSec - min.damageTakenPerSec);
                double deathComponent = 1 - (avg.deathPerSec - min.deathPerSec) /
                    (max.deathPerSec - min.deathPerSec);
                return dTakenComponent * 0.7 + (1 - deathComponent) * 0.3;
            }

            private double ComputeSpecValue(HeroStatisticItemAvg avg, HeroStatisticItemMin min, HeroStatisticItemMax max)
            {
                double campTakenComponent = (avg.campTakenPerSec - min.campTakenPerSec) /
                    (max.campTakenPerSec - min.campTakenPerSec);
                double expComponent = (avg.expPerSec - min.expPerSec) /
                    (max.expPerSec - min.expPerSec);
                double sdpsComponent = (avg.sdps - min.sdps) /
                    (max.sdps - min.sdps);
                return expComponent * 0.4 + sdpsComponent * 0.4 + campTakenComponent * 0.2;
            }

            private double ComputeSupportValue(HeroStatisticItemAvg avg, HeroStatisticItemMin min, HeroStatisticItemMax max)
            {
                double assistComponent = (avg.assistPerSec - min.assistPerSec) /
                    (max.assistPerSec - min.assistPerSec);
                double hpsComponent = (avg.hps - min.hps) /
                    (max.hps - min.hps);
                return assistComponent * 0.4 + hpsComponent * 0.6;
            }

            private double Min(double v1, double v2)
            {
                return v1 < v2 ? v1 : v2;
            }

            private double Max(double v1, double v2)
            {
                return v1 > v2 ? v1 : v2;
            }


            protected override void Save(string name, object obj, Type t)
            {
                List<Match> m = obj as List<Match>;
                int ind = 0;
                using (var f = File.Open(name,FileMode.Append))
                {
                    using (var file = new StreamWriter(f))
                    {
                        foreach (var it in m)
                        {
                            StringBuilder oneItem = new StringBuilder("");
                            for (int i = 0; i < 5; i++)
                            {
                                oneItem.Append(it.YourTeam[i]);
                                oneItem.Append(",");
                            }
                            for (int i = 0; i < 5; i++)
                            {
                                oneItem.Append(it.EnemyTeam[i]);
                                oneItem.Append(",");
                            }
                            oneItem.Append(it.Map + ",");
                            oneItem.Append(it.ProbabilityToWin);
                            ind++;
                            file.WriteLine(oneItem.ToString());
                        }
                    }
                }
                
            }
        }

        public class DeleterParser : CLIParser<object>
        {
            public string HeroInput, MapInput,HeroStatisticInput, MapStatisticInput;
            public string HeroOutput, MapOutput, HeroStatisticOutput, MapStatisticOutput;

            public Dictionary<int, int> HeroMapper = new Dictionary<int, int>();
            public Dictionary<int, int> MapMapper = new Dictionary<int, int>();

            public DeleterParser(string hero,string map,string mapStatistic,
                string heroStatistic,string heroOutput,string mapOutput,
                string heroStatisticOutput,string mapStatisticOutput)
            {
                HeroInput = hero;
                MapInput = map;
                HeroStatisticInput = heroStatistic;
                MapStatisticInput = mapStatistic;

                HeroOutput = heroOutput;
                MapOutput = mapOutput;
                HeroStatisticOutput = heroStatisticOutput;
                MapStatisticOutput = mapStatisticOutput;
            }

      

            public void Run()
            {
                //чистим героев
                var tempHeroAvg = RSParser.AvgStat.ToList();
                var tempHeroMin = RSParser.MinStat.ToList();
                var tempHeroMax = RSParser.MaxStat.ToList();
                var tempRSStat = RSParser.Stat.ToList();

                List<int> unused = GetUnusedHero(tempHeroAvg);
                RemoveUnusedHero(tempHeroAvg, tempHeroMin, tempHeroMax, unused);

                log("debug", "Индексация героев");
                IndexateHeroes(tempHeroAvg, tempHeroMin, tempHeroMax);

                //чистим карты
                unused = GetUnusedMap(tempRSStat);

                RemoveUnusedMap(tempRSStat, unused);

                log("debug", "Индексация карт");
                IndexateMap(tempRSStat);

                log("debug", "Индексация статистики");
                for (int i=0;i< tempRSStat.Count;i++)
                {
                    tempRSStat[i].Statictic.Matches = 
                        tempRSStat[i].Statictic.Matches.ToList().Where((x) => x != 0)
                        .ToArray();
                    tempRSStat[i].Statictic.Wins =
                       tempRSStat[i].Statictic.Wins.ToList().Where((x) => x != 0)
                       .ToArray();
                }

                //применение изменений и сохранение
                RSParser.AvgStat = tempHeroAvg.ToArray();
                RSParser.MinStat = tempHeroMin.ToArray();
                RSParser.MaxStat = tempHeroMax.ToArray();
                RSParser.Stat = tempRSStat.ToArray();

                var heroJson = JSonParser.Save(HParser.Hero, HParser.Hero.GetType());
                var mapJson = JSonParser.Save(MParser.Map, MParser.Map.GetType());
                var statJson = JSonParser.Save(RSParser.Stat, RSParser.Stat.GetType());
                var heroStat = new Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[],
                    HeroStatisticItemMax[]>(RSParser.AvgStat, RSParser.MinStat,
                    RSParser.MaxStat);
                var heroStatJson = JSonParser.Save(heroStat, heroStat.GetType());

                File.WriteAllText(HeroOutput, heroJson);
                File.WriteAllText(MapOutput, mapJson);
                File.WriteAllText(MapStatisticOutput, statJson);
                File.WriteAllText(HeroStatisticOutput, heroStatJson);
            }

            protected void IndexateMap(List<Statistic> tempRSStat)
            {
                for (int i = 0; i < tempRSStat.Count; i++)
                {
                    MapMapper[MParser.Map[i].Id-1] = i;
                    MParser.Map[i] = new Map(i, MParser.Map[i].Name);
                }
            }

            protected void RemoveUnusedMap(List<Statistic> tempRSStat, List<int> unused)
            {
                for (int i = 0; i < unused.Count; i++)
                {
                    tempRSStat.RemoveAt(unused[i]);
                    if (MParser.Map.Count > unused[i])
                        MParser.Map.RemoveAt(unused[i]);
                }
            }

            protected List<int> GetUnusedMap(List<Statistic> tempRSStat)
            {
                int len = tempRSStat.Count;
                List<int> unused = new List<int>();
                for (int i = 0; i < len; i++)
                {
                    if (tempRSStat[i].Statictic.Ammount == 0)
                    {
                        unused.Add(i);
                    }
                }
                unused.Reverse();
                return unused;
            }

            /// <summary>
            /// Переиндексация всех героев
            /// </summary>
            /// <param name="tempHeroAvg">средняя информация</param>
            /// <param name="tempHeroMin">минимум</param>
            /// <param name="tempHeroMax">максимум</param>
            protected void IndexateHeroes(List<HeroStatisticItemAvg> tempHeroAvg,
                List<HeroStatisticItemMin> tempHeroMin, List<HeroStatisticItemMax> 
                tempHeroMax)
            {
                for (int i = 0; i < tempHeroAvg.Count; i++)
                {
                    HeroMapper[HParser.Hero[i].Id] = i;
                    HParser.Hero[i] = new Hero(i, HParser.Hero[i].Name,
                        HParser.Hero[i].Group, HParser.Hero[i].SubGroup);
                    tempHeroAvg[i].heroId = i;
                    tempHeroMin[i].heroId = i;
                    tempHeroMax[i].heroId = i;
                }
            }

            /// <summary>
            /// Находит пустые записи в статистике героев
            /// </summary>
            /// <param name="avg">статистика героев</param>
            /// <returns>возвращает массив пустых записей в порядке убывания индексов</returns>
            protected List<int> GetUnusedHero(List<HeroStatisticItemAvg> avg)
            {
                int len = avg.Count;
                List<int> unused = new List<int>();
                for (int i = 0; i < len; i++)
                {
                    if (avg[i].count == 0)
                    {
                        unused.Add(i);
                    }
                }
                unused.Reverse();
                return unused;
            }

            /// <summary>
            /// Удаляет информацию о неиспользованных героях
            /// </summary>
            /// <param name="tempHeroAvg">средняя информация</param>
            /// <param name="tempHeroMin">минимум</param>
            /// <param name="tempHeroMax">максимум</param>
            /// <param name="unused">список неиспользуемых героев</param>
            protected void RemoveUnusedHero(List<HeroStatisticItemAvg> tempHeroAvg, 
                List<HeroStatisticItemMin> tempHeroMin, List<HeroStatisticItemMax> 
                tempHeroMax, List<int> unused)
            {
                for (int i = 0; i < unused.Count; i++)
                {
                    if (HParser.Hero.Count > unused[i])
                        HParser.Hero.RemoveAt(unused[i]);
                    tempHeroAvg.RemoveAt(unused[i]);
                    tempHeroMin.RemoveAt(unused[i]);
                    tempHeroMax.RemoveAt(unused[i]);
                }
            }

            public override void Validate()
            {
                throw new NotImplementedException();
            }

            protected override void OpenSource(string path)
            {
                throw new NotImplementedException();
            }

            protected override object ParseData(object data)
            {
                throw new NotImplementedException();
            }

            protected override object ReadData()
            {
                throw new NotImplementedException();
            }

            protected override void Save(string name, object obj, Type t)
            {
                throw new NotImplementedException();
            }
        }

        public class ModelParser : CLIParser<Match>
        {
            string Input, Output,MatchUpOutput;
            
            SubGroupMatchHasher hasher = new SubGroupMatchHasher();

            public MatchupTable MatchupTable;

            private KeyValuePair<int, int>[,] winWith = new KeyValuePair<int, int>
                [HParser.Hero.Count, HParser.Hero.Count];

            private KeyValuePair<int, int>[,] winAgainst = new KeyValuePair<int, int>
               [HParser.Hero.Count, HParser.Hero.Count];

            public class SubGroupMatchHasher
            {
                List<bool> bits;
                public Int64 Hash(SubGroupMatch m)
                {
                    bits = new List<bool>();
                    for (int i = 0; i < m.YourTeam.Length; i++)
                    {
                        if (m.YourTeam[i] == 0)
                        {
                            bits.AddRange(new bool[] {false,false,false });
                        }
                        else
                        {
                            ToBits(m.YourTeam[i]);
                            while (bits.Count % 3 != 0)
                                bits.Add(false);
                        }
                    }
                    for (int i = 0; i < m.EnemyTeam.Length; i++)
                    {
                        if (m.EnemyTeam[i] == 0)
                        {
                            bits.AddRange(new bool[] { false, false, false });
                        }
                        else
                        {
                            ToBits(m.EnemyTeam[i]);
                            while (bits.Count % 3 != 0)
                                bits.Add(false);
                        }
                    }
                    while (bits.Count < 64)
                        bits.Add(false);
                    BitArray arr = new BitArray(bits.ToArray());
                    Int32[] array = new Int32[2];
                    arr.CopyTo(array, 0);
                    return ((Int64)array[0]) << 32 | array[1];
                }

                public SubGroupMatch Restore(Int64 hash)
                {
                    SubGroupMatch m = new SubGroupMatch();
                    BitArray b = new BitArray(new Int32[] {
                    (Int32)(hash >> 32),
                        (Int32)(hash & 0x00000000FFFFFFFFL) });
                    bool[] arr = new bool[64];
                    b.CopyTo(arr, 0);
                    for (int i = 0; i < m.YourTeam.Length; i++)
                        m.YourTeam[i] = FromBits(arr.Skip(3*i).Take(3).ToArray());
                    for (int i = 0; i < m.EnemyTeam.Length; i++)
                        m.EnemyTeam[i] = FromBits(arr.Skip((m.YourTeam.Length)*3 + 3 * i).Take(3).ToArray());
                    return m;

                }

                protected void ToBits(int value)
                {
                    while (value != 0)
                    {
                        bits.Add(value % 2 == 1);
                        value /= 2;
                    }
                }
                protected sbyte FromBits(bool[] value)
                {
                    sbyte rez = 0;
                    for(int i = 0; i < value.Length; i++)
                    {
                        rez += (sbyte)((value[i] == true ? 1 : 0) * Math.Pow(2, i));
                    }
                    return rez;
                }
            }

            public ModelParser(string Input,string Output,string MatchUpOutput)
            {
                this.Input = Input;
                this.Output = Output;
                this.MatchUpOutput = MatchUpOutput;
            }

            public SubGroupMatch SubGroupsFromMatch(Match m)
            {
                SubGroupMatch subGroupMatch = new SubGroupMatch();
                
                for (int i = 0; i < m.YourTeam.Length; i++)
                {
                    subGroupMatch.YourTeam[(int)HParser.Hero[m.YourTeam[i]].SubGroup - 1]++;
                }
                for (int i = 0; i < m.EnemyTeam.Length; i++)
                {
                    subGroupMatch.EnemyTeam[(int)HParser.Hero[m.EnemyTeam[i]].SubGroup - 1]++;
                }
                return subGroupMatch;
            }

            public void Run()
            {
                OpenSource(Input);
                object data = null;
                SubGroupMatchHasher hasher = new SubGroupMatchHasher();
                MatchupTable = new MatchupTable(HParser.Hero.Count);
                Dictionary<Int64, Tuple<short, short>> HashTable = new Dictionary
                    <long, Tuple<short, short>>();
                int i = 0;

                string fDir = Path.GetDirectoryName(Input);
                string fName = Path.GetFileNameWithoutExtension(Input);
                string fExt = Path.GetExtension(Input);
                //для того чтобы сохранить новую версию повторов
                using (var file = new StreamWriter(File.Create($"{fDir}/{fName}_new{fExt}")))
                {
                    while ((data = ReadData()) != null)
                    {
                        var match = ParseData(data);

                        if (match == null)
                        {
                            log("debug", $"Строка {i + 1}");
                            continue;
                        }

                        {
                            string text = String.Join(",",match.YourTeam.Select(x => x.ToString()).ToArray()) +',';
                            text += String.Join(",", match.EnemyTeam.Select(x => x.ToString()).ToArray()) + ',';
                            text += match.Map + "," + match.ProbabilityToWin;                      
                            file.WriteLine(text);
                        }

                        UpdateMatchupTable(match);

                        var subMatch = SubGroupsFromMatch(match);
                        var hash = hasher.Hash(subMatch);
                        var assert = hasher.Restore(hash);

                        if (subMatch.Equals(assert) == false)
                        {
                            log("warng", "Ошибка хэш функции. Парсинг продолжается");
                            continue;
                        }

                        if (HashTable.ContainsKey(hash) == false)
                        {
                            HashTable.Add(hash, new Tuple<short, short>
                                ((short)match.ProbabilityToWin, 1));
                        }
                        else
                        {
                            var curItem = HashTable[hash];
                            HashTable[hash] = new Tuple<short, short>
                               ((short)(curItem.Item1 + (short)match.ProbabilityToWin), (short)(curItem.Item2 + 1));
                        }


                        i++;
                        if (i % 100000 == 0)
                        {
                            drawTextProgressBar(i, RSParser.Stat.Sum((x) => x.Statictic.Ammount * 2));
                            // log("time", "Уже обработано " + i);
                        }
                    }
                }
                ComputeMatchupTable();
                Console.WriteLine();
                //все данные расчитаны
                Save(Output, HashTable, HashTable.GetType());
                File.WriteAllText(MatchUpOutput, JSonParser.Save(MatchupTable, typeof(MatchupTable)));
                
            }

            private void UpdateMatchupTable(Match m)
            {
                for(int i = 0; i < m.YourTeam.Length; i++)
                {
                    for (int j = 0; j < m.YourTeam.Length; j++)
                    {
                        int h1 = m.YourTeam[i];
                        int h2 = m.YourTeam[j];
                        int played = winWith[h1, h2].Value + 1;
                        int win = winWith[h1, h2].Key;
                        if (i != j && m.ProbabilityToWin == 1)
                        {
                            win++;
                        }
                        winWith[h1, h2] = new KeyValuePair<int, int>(win, played);

                    }
                }

                for (int i = 0; i < m.EnemyTeam.Length; i++)
                {
                    for (int j = 0; j < m.EnemyTeam.Length; j++)
                    {
                        int h1 = m.EnemyTeam[i];
                        int h2 = m.EnemyTeam[j];
                        int played = winWith[h1, h2].Value + 1;
                        int win = winWith[h1, h2].Key;
                        if (i != j && m.ProbabilityToWin == 0)
                        {
                            win++;
                        }
                        winWith[h1, h2] = new KeyValuePair<int, int>(win, played);

                    }
                }

                for (int i = 0; i < m.YourTeam.Length; i++)
                {
                    for (int j = 0; j < m.EnemyTeam.Length; j++)
                    {
                        int h1 = m.YourTeam[i];
                        int h2 = m.EnemyTeam[j];
                        
                        int played = winAgainst[h1, h2].Value + 1;
                        int win = winAgainst[h1, h2].Key;
                        if (m.ProbabilityToWin == 1)
                            win++;
                        winAgainst[h1, h2] = new KeyValuePair<int, int>(win, played);
                    }
                }

                for (int i = 0; i < m.EnemyTeam.Length; i++)
                {
                    for (int j = 0; j < m.YourTeam.Length; j++)
                    {
                        int h1 = m.EnemyTeam[i];
                        int h2 = m.YourTeam[j];

                        int played = winAgainst[h1, h2].Value + 1;
                        int win = winAgainst[h1, h2].Key;
                        if (m.ProbabilityToWin == 0)
                            win++;
                        winAgainst[h1, h2] = new KeyValuePair<int, int>(win, played);
                    }
                }

            }

            private void ComputeMatchupTable()
            {
                for(int i = 0; i < winWith.GetLength(0); i++)
                {
                    for(int j = 0; j < winWith.GetLength(1); j++)
                    {
                        MatchupTable.WinWith[i][j] = (double)winWith[i, j].Key 
                            / (double)winWith[i, j].Value;
                        MatchupTable.WinAgainst[i][j] = (double)winAgainst[i, j].Key
                            / (double)winAgainst[i, j].Value;
                    }
                }
            }

            public override void Validate()
            {
                throw new NotImplementedException();
            }

            protected override void OpenSource(string path)
            {
                log("debug", "Инициализация парсера модели начата");
                CSVParser.Load(path);
                log("succes", "Инициализация парсера модели завершена");
            }

            protected override Match ParseData(object obj)
            {
                var data = obj as string[];
                if (data == null)
                    return null;
                if (data.Length != 12)
                {
                    log("warng", "Неверный формат строки схемы повторов");
                    return null;
                }
                Match m = new Match();
                try
                {
                    //индексация в считанных данных и модели отличается,
                    //используем маппер для приведения индексов считанных
                    //данных к индексам модели
                    for (int i = 0; i < 5; i++)
                        m.YourTeam[i] = DParser.HeroMapper[Int32.Parse(data[i])];
                    for (int i = 0; i < 5; i++)
                        m.EnemyTeam[i] = DParser.HeroMapper[Int32.Parse(data[i+5])];
                    m.Map = DParser.MapMapper[Int32.Parse(data[10])];
                    m.ProbabilityToWin = Double.Parse(data[11]);

                }
                catch
                {
                    log("warng", "Неверный формат строки схемы повтора");
                    return null;
                }
                return m;
            }


            protected override object ReadData()
            {
                return CSVParser.Next();
            }

            protected override void Save(string name, object obj, Type t)
            {
                Dictionary<Int64, Tuple<short, short>> arr = obj as Dictionary<Int64, Tuple<short, short>>;
                using (var file = CSVParser.Save(name))
                {
                    foreach (var it in arr)
                    {
                        if (KeysParam.removeNoiseData == true)
                        {
                            if (it.Value.Item2 <= 2)
                                continue;
                        }
                        double prob = (double)it.Value.Item1 / (double)it.Value.Item2;

                        if (KeysParam.removeNonDeterminate == true)
                        {
                            if (prob >= 0.3 && prob <= 0.7)
                                continue;
                        }
                        var subgr = hasher.Restore(it.Key);
                        var yourTeam = string.Join(",", subgr.YourTeam);
                        var enemyTeam = string.Join(",", subgr.EnemyTeam);
                        file.WriteLine(yourTeam + "," + enemyTeam + "," + 
                              prob.ToString().Replace(",", "."));
                    }
                }
            }
        }

        public InputFileParam Input;
        public OutputFileParam Output;
        public static ParserKeys KeysParam;
        public static HeroParser HParser;
        public static MapParser MParser;
        public static ReplaySchemaParser RSParser;
        public static CSVParser CSVParser;
        public static CSVBatchParser CSVBParser;
        public static DeleterParser DParser;
        public static ModelParser MDParser;
        

        public override void Run(string[] args)
        {
            int nBufferWidth = Console.BufferWidth;
            Console.SetBufferSize(nBufferWidth, 1000);

            base.Run(args);
            log("debug", "Валидация входных данных начата");
            Validate();
            log("succes", "Валидация входных данных завершена");

            log("debug", "Инициализация CSV парсера начата");
            CSVParser = new CSVParser();
            log("succes", "Инициализация CSV парсера завершена");

            log("debug", "Инициализация CSV Batch парсера начата");
            CSVBParser = new CSVBatchParser();
            log("succes", "Инициализация CSV Batch парсера завершена");

            //считываем данные для героя, сохраняем в память, парсим в json
            log("debug", "Парсинг героев начат");
            HParser = new HeroParser(Input[0], Output[0]);
            HParser.Run();
            log("succes", "Парсинг героев завершен");

            //считываем данные для карты, сохраняем в память, парсим в json
            log("debug", "Парсинг карт начат");
            MParser = new MapParser(Input[1], Output[1]);
            MParser.Run();
            log("succes", "Парсинг карт завершен");

            //считываем данные схемы повторов, сохраняем в память
            if (ExistAll(Output[2],Output[3], Output[5])== false)
            {
                log("debug", "Парсинг повторов начат");
                RSParser = new ReplaySchemaParser(Input[2], Input.InputFolder + Input.Path[3],
                    Input.Name[3], Output[2], Output[3], Output[5]);
                RSParser.Run();
                log("succes", "Парсинг повторов завершен");
            }
            else
            {
                log("debug", "Загрузка статистики начата");
                RSParser = new ReplaySchemaParser(Input[2], Input.InputFolder + Input.Path[3],
                    Input.Name[3], Output[2], Output[3], Output[5]);
                string json = System.IO.File.ReadAllText(Output[5]);
                var stats = (Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], 
                    HeroStatisticItemMax[]>)
                    JSonParser.Load(json,
                    typeof(Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], 
                    HeroStatisticItemMax[]>));
                RSParser.AvgStat = stats.Item1;
                RSParser.MinStat = stats.Item2;
                RSParser.MaxStat = stats.Item3;

                json = System.IO.File.ReadAllText(Output[3]);
                RSParser.Stat = (Statistic[])JSonParser.Load(json, typeof(Statistic[]));
                log("succes", "Загрузка статистики зввершена");

            }

            //удаляем пустые записи
            log("debug", "Очистка данных от пустых записей начата");
            DParser = new DeleterParser(Output[0], Output[1], Output[3], Output[5],
                Output[7], Output[8], Output[9], Output[10]);
            DParser.Run();
            log("succes", "Очистка данных от пустых записей завершен");

            //создаем конечную модель
            log("debug", "Формирование данных для модели начато");
            if (ExistAll(Output[6], Output[11]) == false)
            {
                MDParser = new ModelParser(Output[2], Output[6], Output[11]);
                MDParser.Run();
            }
            else
            {
                log("info", "Найдены старые данные для модели. " +
                    "Процесс формирования данных для модели прекоащен.");
            }
            log("succes", "Формирование данных для модели завершено");
        }

        private bool ExistAll(params string[] files)
        {
            return files.All(x => File.Exists(x));
        }

        public override void Validate()
        {
            if (NamedParam.ContainsKey("i") == false)
            {
                throw new Exception("Не инициализирован обязательный параметр i." +
                    " Укажите входным параметром папку,в которой содержатся исходные файлы." +
                    "Пример: i=\"D:\\temp\\\"");
            }

            string inpPath = NamedParam["i"];

            log("succes", $"Входной каталог найден");
            log("debug", $"{inpPath}");
            //если хотя бы один из файлов не найден, выкидаывется исключение
            Input = new InputFileParam(inpPath);

            log("succes", $"Входные файлы найдены");

            string outPath = "";
            if (NamedParam.ContainsKey("o") == false)
            {
                log("warng", $"Выходные каталог не задан, применяется значение по умолчанию");
                outPath = Directory.GetCurrentDirectory();
                log("debug", $"{outPath}");
            }
            else
            {
                outPath = NamedParam["o"];
            }

            Output = new OutputFileParam(outPath);

            log("succes", $"Выходной каталог найден");
            log("debug", $"{outPath}");

            KeysParam = new ParserKeys();

            if (Keys.Contains("-olds"))
            {
                KeysParam.useOlderData = true;
            }

        }



        protected override void OpenSource(string path)
        {
            
        }

        protected override object ParseData(object data)
        {
            return null;
        }

        protected override object ReadData()
        {
            return null;
        }

        protected override void Save(string name, object obj, Type t)
        {
           
        }

       


      
    }
 
}
