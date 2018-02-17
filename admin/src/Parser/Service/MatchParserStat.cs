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

/// <summary>
/// Разбирает датасет с информацией о матчах, преобразует в другой вид и собирает статистику
/// </summary>
    class MatchParserStat : CLIParser<Tuple<int, int, HeroStatisticItem>>
    {
        string inputFolder,output,statisticOutput, statisticHeroOutput;

        CSVBatchParser parser;

        ReplayParserStat rParser = new ReplayParserStat();

        bool convertToExcel = true;

        public override void Run(string[] args)
        {
            base.Run(args);
            Validate();
            rParser.Run(args);

            Statistic[] stat = new Statistic[rParser.MapCount + 1];

            HeroStatisticItemAvg[] avgStat = new HeroStatisticItemAvg[1000];
            HeroStatisticItemMin[] minStat = new HeroStatisticItemMin[1000];
            HeroStatisticItemMax[] maxStat = new HeroStatisticItemMax[1000];

            InitializeHeroStat(avgStat, minStat, maxStat);

            OpenSource(inputFolder);

            object data;
            int i = 0;
            List<Tuple<int, int, HeroStatisticItem>> temps = new List<Tuple<int, int, HeroStatisticItem>>();
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

                temps.Add(r);
                i++;

                r.Item3.sec = rParser.ReplayResult[temps[0].Item1].Item2;

                UpdateHeroStat(avgStat, minStat, maxStat, r);

                //собираем части воедино
                if (i == 10)
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
                    m.Map = rParser.ReplayResult[temps[0].Item1].Item1;
                    m.ProbabilityToWin = 1;
                    int first_i = 0, second_i = 0;
                    for(int j=0;j<temps.Count;j++)
                    {
                        if(temps[j].Item3.winrate == 1)
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

            ClearEmptyStat(ref avgStat,ref minStat,ref maxStat);
            ComputeAverage(avgStat);

            Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], HeroStatisticItemMax[]> heroStat =
                new Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], HeroStatisticItemMax[]>(avgStat,minStat,maxStat);
            Save(output, result, typeof(Dictionary<Match, double>));

            File.WriteAllText(statisticHeroOutput, JSonParser.Save(heroStat, typeof(Tuple<HeroStatisticItemAvg[], HeroStatisticItemMin[], HeroStatisticItemMax[]>)), Encoding.Default);

            Array.Resize(ref avgStat, 0);
            Array.Resize(ref minStat, 0);
            Array.Resize(ref maxStat, 0);

            for (int j=0;j<stat.Length;j++)
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
                stat[it.Value.Item1].Statictic.Ammount++;
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

        private void InitializeHeroStat(HeroStatisticItemAvg[] avgStat, HeroStatisticItemMin[] minStat, HeroStatisticItemMax[] maxStat)
        {
            for(int i=0;i<minStat.Length;i++)
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

        private void UpdateHeroStat(HeroStatisticItemAvg[] avgStat, HeroStatisticItemMin[] minStat, HeroStatisticItemMax[] maxStat,Tuple<int, int, HeroStatisticItem> r)
        {
            int id = r.Item2;
            double time = r.Item3.sec;
            // обновление статистики
            minStat[id].assistPerSec = Min(minStat[id].assistPerSec, r.Item3.assistPerSec/time);
            minStat[id].campTakenPerSec = Min(minStat[id].campTakenPerSec, r.Item3.campTakenPerSec / time);
            minStat[id].damageTakenPerSec = Min(minStat[id].damageTakenPerSec, r.Item3.damageTakenPerSec / time);
            minStat[id].deathPerSec = Min(minStat[id].deathPerSec, r.Item3.deathPerSec / time);
            minStat[id].dps = Min(minStat[id].dps, r.Item3.dps / time);
            minStat[id].expPerSec = Min(minStat[id].expPerSec, r.Item3.expPerSec / time);
            minStat[id].hps = Min(minStat[id].hps, r.Item3.hps / time);
            minStat[id].killPerSec = Min(minStat[id].killPerSec, r.Item3.killPerSec / time);
            minStat[id].sdps = Min(minStat[id].sdps, r.Item3.sdps / time);
            minStat[id].sec = Min(minStat[id].sec, r.Item3.sec);

            maxStat[id].assistPerSec = Max(maxStat[id].assistPerSec, r.Item3.assistPerSec / time);
            maxStat[id].campTakenPerSec = Max(maxStat[id].campTakenPerSec, r.Item3.campTakenPerSec / time);
            maxStat[id].damageTakenPerSec = Max(maxStat[id].damageTakenPerSec, r.Item3.damageTakenPerSec / time);
            maxStat[id].deathPerSec = Max(maxStat[id].deathPerSec, r.Item3.deathPerSec / time);
            maxStat[id].dps = Max(maxStat[id].dps, r.Item3.dps / time);
            maxStat[id].expPerSec = Max(maxStat[id].expPerSec, r.Item3.expPerSec / time);
            maxStat[id].hps = Max(maxStat[id].hps, r.Item3.hps / time);
            maxStat[id].killPerSec = Max(maxStat[id].killPerSec, r.Item3.killPerSec / time);
            maxStat[id].sdps = Max(maxStat[id].sdps, r.Item3.sdps / time);
            maxStat[id].sec = Max(maxStat[id].sec, r.Item3.sec);

            avgStat[id].assistPerSec += r.Item3.assistPerSec / time;
            avgStat[id].campTakenPerSec += r.Item3.campTakenPerSec / time;
            avgStat[id].damageTakenPerSec += r.Item3.damageTakenPerSec / time;
            avgStat[id].deathPerSec += r.Item3.deathPerSec / time;
            avgStat[id].dps += r.Item3.dps / time;
            avgStat[id].expPerSec += r.Item3.expPerSec / time;
            avgStat[id].hps += r.Item3.hps / time;
            avgStat[id].killPerSec += r.Item3.killPerSec / time;
            avgStat[id].sdps += r.Item3.sdps / time;
            avgStat[id].sec += r.Item3.sec;
            avgStat[id].winrate += r.Item3.winrate;

            avgStat[id].count++;

        }

        private void ComputeAverage(HeroStatisticItemAvg[] avgStat)
        {
            for(int i=0;i<avgStat.Length;i++)
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

            for(int i=0;i<avgStat.Length;i++)
            {
                avgStat[i].assassinRating = ComputeAssasinValue(avgStat[i], minStat, maxStat);
                avgStat[i].warriorRating = ComputeWarriorValue(avgStat[i], minStat, maxStat);
                avgStat[i].specialistRating = ComputeSpecValue(avgStat[i], minStat, maxStat);
                avgStat[i].supportRating = ComputeSupportValue(avgStat[i], minStat, maxStat);
            }

        }

        private double ComputeAssasinValue(HeroStatisticItemAvg avg,HeroStatisticItemMin min,HeroStatisticItemMax max)
        {
            double dpsComponent = (avg.dps - min.dps) / (max.dps - min.dps);
            double killComponent = (avg.killPerSec - min.killPerSec)/ (max.killPerSec - min.killPerSec);
            return dpsComponent * 0.7 + killComponent * 0.3;
        }

        private double ComputeWarriorValue(HeroStatisticItemAvg avg, HeroStatisticItemMin min, HeroStatisticItemMax max)
        {
            double dTakenComponent = (avg.damageTakenPerSec - min.damageTakenPerSec) / 
                (max.damageTakenPerSec - min.damageTakenPerSec);
            double deathComponent = 1 - (avg.deathPerSec - min.deathPerSec) /
                (max.deathPerSec - min.deathPerSec);
            return dTakenComponent * 0.7 + (1 - deathComponent)*0.3;
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
    

        private void ClearEmptyStat(ref HeroStatisticItemAvg[] avgStat,ref HeroStatisticItemMin[] minStat, ref HeroStatisticItemMax[] maxStat)
        {
            var avgList = avgStat.ToList();
            avgList.RemoveAll((x) => x.sec == 0);
            avgStat = avgList.ToArray();

            var minList = minStat.ToList();
            minList.RemoveAll((x) => x.sec == 0 || x.sec == Double.MaxValue);
            minStat = minList.ToArray();

            var maxList = maxStat.ToList();
            maxList.RemoveAll((x) => x.sec == 0 || x.sec == Double.MinValue);
            maxStat = maxList.ToArray();
        }

        private double Min(double v1,double v2)
        {
            return v1 < v2 ? v1 : v2;
        }

        private double Max(double v1, double v2)
        {
            return v1 > v2 ? v1 : v2;
        }

        protected override void OpenSource(string path)
        {
            parser = new CSVBatchParser();
            string[] files = Directory.GetFiles(path, "*.csv");
            parser.Load(files);
        }

        protected override Tuple<int, int, HeroStatisticItem> ParseData(object obj)
        {
            string[] data = obj as string[];
            HeroStatisticItem stat = new HeroStatisticItem();
            try
            { 

                int replayId = Int32.Parse(data[0]);
                int heroId = Int32.Parse(data[2]);
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


                return new Tuple<int, int, HeroStatisticItem>(replayId, heroId, stat);
            }
            catch(Exception e)
            {
                Console.Write("Входная строка имела неверный формат. Парсинг продолжается");
            }
            return new Tuple<int, int, HeroStatisticItem>(-1, -1, null);

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

            if (NamedParam.ContainsKey("sho") == false)
            {
                string input = NamedParam["so"];
                string fDir = Path.GetDirectoryName(input);
                string fName = Path.GetFileNameWithoutExtension(input);
                string fExt = Path.GetExtension(input);
                NamedParam["sho"] = Path.Combine(fDir, String.Concat(fName, "_sho", fExt));
            }


            inputFolder = NamedParam["if"];
            output = NamedParam["o"];
            statisticOutput = NamedParam["so"];

            statisticHeroOutput = NamedParam["sho"];
        }
    }
}
