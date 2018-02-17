using HoTS_Service.Entity;
using HoTS_Service.Entity.Enum;
using HoTS_Service.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Service
{


    class MatchParser : CLIParser<Tuple<int, int, int>>
    {
        string inputFolder,output,statisticOutput;

        CSVBatchParser parser;

        ReplayParser rParser = new ReplayParser();

        bool convertToExcel = true;

        public override void Run(string[] args)
        {
            base.Run(args);
            Validate();
            rParser.Run(args);

            Statistic[] stat = new Statistic[rParser.MapCount + 1];

            OpenSource(inputFolder);

            object data;
            int i = 0;
            List<Tuple<int, int, int>> temps = new List<Tuple<int, int, int>>();
            Dictionary<Match,double> result = new Dictionary<Match, double>();
            Console.WriteLine("Парсинг матчей");
            int heroCount = 0;
            int lastMatchId = -1;
            while ((data = ReadData()) != null)
            {
                var r = ParseData(data);
                if (r.Item1 == -1)
                    continue;

                if (lastMatchId == r.Item1)
                    continue;

                temps.Add(ParseData(data));
                i++;

                //собираем части воедино
                if(i == 10)
                {
                    i = 0;
                    bool flag = false;
                    for(int j=0;j<9;j++)
                    {
                        if(temps[j].Item1 != temps[j+1].Item1)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        temps.Clear();
                        continue;
                    }
                    
                    Match m = new Match();
                    m.Map = rParser.ReplayResult[temps[0].Item1];
                    m.ProbabilityToWin = 1;
                    int first_i = 0, second_i = 0;
                    for(int j=0;j<temps.Count;j++)
                    {
                        if(temps[j].Item3 == 1)
                        {
                            m.YourTeam[first_i++] = temps[j].Item2;
                        }
                        else
                        {
                            m.EnemyTeam[second_i++] = temps[j].Item2;
                        }
                        if(temps[j].Item2 > heroCount)
                        {
                            heroCount = temps[j].Item2;
                        }
                    }
                    //обратное
                    Match temp_m = new Match()
                    {
                        YourTeam = m.EnemyTeam,
                        EnemyTeam = m.YourTeam,
                        Map = m.Map
                    };
                    //пробуем добавить
                    //если запись уже есть
                    if (result.ContainsKey(m))
                    {
                        //усредняем вероятность
                        double used = result[m];
                        double newer = m.ProbabilityToWin;
                        double rez = (used + newer) / 2;
                        result[m] = rez;
                        Console.WriteLine(m.ToString() + " Уже существует");
                    }
                    //если есть противоречие
                    else if(result.ContainsKey(temp_m))
                    {
                        //усредняем вероятность
                        double used = result[m];
                        double newer = 1 - m.ProbabilityToWin;
                        double rez = (used + newer) / 2;
                        result[m] = rez;
                    }
                    else
                    {
                        result.Add(m, m.ProbabilityToWin);
                        if (result.Count % 100000 == 0)
                            Console.WriteLine("Уже обработано " + result.Count);
                    }
                    lastMatchId = temps.Last().Item1;
                    temps.Clear();
                }
            }

            Save(output, result, typeof(Dictionary<Match, double>));

           

            for(int j=0;j<stat.Length;j++)
            {
                stat[j] = new Statistic()
                {
                    Statictic = new StatisticItem()
                    {
                        Matches = new int[heroCount + 1],
                        Wins = new int[heroCount + 1]
                    }
                };
            }

            Console.WriteLine("Расчет статистики матчей");
            ///считаем сколько матчей на каждой карте
            foreach (var it in rParser.ReplayResult)
            {
                stat[it.Value].Statictic.Ammount++;
            }

            //данные собраны,считаем статистику
            Console.WriteLine("Расчет статистики героев");

            foreach (var it in result)
            {
                Match cur = it.Key;
                for (int j = 0; j < 5; j++)
                {
                    stat[cur.Map].Statictic.Matches[cur.YourTeam[j]]++;
                    stat[cur.Map].Statictic.Wins[cur.YourTeam[j]]++;
                }
                for (int j = 0; j < 5; j++)
                {
                    stat[cur.Map].Statictic.Matches[cur.EnemyTeam[j]]++;
                }
            }

            File.WriteAllText(statisticOutput, JSonParser.Save(stat, typeof(Statistic[])), Encoding.Default);

            Console.WriteLine("Успешно спарсено " + result.Count + " записей");
        }

      

        protected override void OpenSource(string path)
        {
            parser = new CSVBatchParser();
            string[] files = Directory.GetFiles(path, "*.csv");
            parser.Load(files);
        }

        protected override Tuple<int, int, int> ParseData(object obj)
        {
            string[] data = obj as string[];
            try
            {
                int replayId = Int32.Parse(data[0]);
                int heroId = Int32.Parse(data[2]);
                int win = Int32.Parse(data[4]);
                return new Tuple<int, int, int>(replayId, heroId, win);
            }
            catch
            {
                Console.Write("Входная строка имела неверный формат. Парсинг продолжается");
            }
            return new Tuple<int, int, int>(-1, -1, -1);

        }

        protected override object ReadData()
        {
            return parser.Next();
        }

        protected override void Save(string name,object obj, Type t)
        {
            Dictionary<Match, double> results = obj as Dictionary<Match, double>;
            using (var file = new StreamWriter(name))
            {
                foreach (var it in results)
                {
                    string oneItem = "";
                    for (int i = 0; i < 5; i++)
                        oneItem += it.Key.YourTeam[i] + ",";
                    for (int i = 0; i < 5; i++)
                        oneItem += it.Key.EnemyTeam[i] + ",";
                    oneItem += it.Key.Map + ",";
                    if (it.Key.ProbabilityToWin >= 1)
                        oneItem += 1;
                    else if (it.Key.ProbabilityToWin <= 0)
                        oneItem += 0;
                    else
                        oneItem += it.Key.ProbabilityToWin;
                    file.WriteLine(oneItem);
                }
            }
        }

        public override void Validate()
        {
            if (NamedParam.ContainsKey("if") == false)
            {
                throw new Exception("Не инициализирован обязательный параметр if");
            }
            if (Directory.GetFiles(NamedParam["if"], "*.csv").Length == 0)
            {
                throw new FileNotFoundException($"{NamedParam["if"]} пустая");
            }

            if (NamedParam.ContainsKey("o") == false)
            {
                string input = NamedParam["i"];
                string fDir = Path.GetDirectoryName(input);
                string fName = Path.GetFileNameWithoutExtension(input);
                string fExt = Path.GetExtension(input);
                NamedParam["o"] = Path.Combine(fDir, String.Concat(fName, "_o", fExt));
            }


            if (NamedParam.ContainsKey("so") == false)
            {
                string input = NamedParam["i"];
                string fDir = Path.GetDirectoryName(input);
                string fName = Path.GetFileNameWithoutExtension(input);
                string fExt = Path.GetExtension(input);
                NamedParam["so"] = Path.Combine(fDir, String.Concat(fName, "_s", fExt));
            }


            inputFolder = NamedParam["if"];
            output = NamedParam["o"];
            statisticOutput = NamedParam["so"];
        }
    }
}
