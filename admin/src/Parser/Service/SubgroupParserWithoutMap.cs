using HoTS_Service.Entity;
using HoTS_Service.Entity.Enum;
using HoTS_Service.Service;
using HoTS_Service.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Service
{

    //на вход: {гуиды героев команды 1} {гуиды героев команды 2} {вероятность победы команды 1}
    //пример: 69,11,67,65,63,23,29,58,49,69,1
    //на выход: {вектор ролей команды 1} {вектор ролей команды 2} {вектор идов карты}
    // {вероятность победы команды 1}
    class SubgroupParserWithoutMap : CLIParser<Tuple<sbyte[],double>>
    {
        string input, output,guidInput,heroInput;
        bool filter = false;
        const int HERO_SUBGROUP_COUNT = 9;
        int heroCount = 0;

        CSVParser parser;

        Dictionary<string, int> guidMapper;
        HeroService heroService;

        public override void Run(string[] args)
        {
            base.Run(args);
            Validate();

            if (filter == true)
            {
                Console.WriteLine("Фильтр для образов в диапазоне [0.4,0.6] включен");
                Console.WriteLine("Фильтр для случайных образов включен");
            }

            Console.WriteLine($"HERO_SUBGROUP_COUNT = {HERO_SUBGROUP_COUNT}");

            guidMapper = (Dictionary<string, int>)JSonParser.Load(File.ReadAllText(guidInput), typeof(Dictionary<string, int>));

            foreach(var it in guidMapper)
            {
                if(it.Key.Split('#')[0] == "Hero")
                {
                    heroCount++;
                }
            }

            Console.WriteLine($"HERO_COUNT = {heroCount}");

            heroService = new HeroService();
            heroService.Load(heroInput);

            Dictionary<SByte[], Tuple<double,int>> nnDataset = new Dictionary<SByte[], Tuple<double, int>>(new ArrayEqualityComparer());
            OpenSource(input);
                        
            object data;

            bool wasPrint = false;

            while ((data = ReadData()) != null)
            {
                var line = ParseData(data);
                if (line == null)
                    continue;

                if (nnDataset.ContainsKey(line.Item1))
                {
                    var prev = nnDataset[line.Item1];
                    nnDataset[line.Item1] = new Tuple<double, int>(prev.Item1 + line.Item2,prev.Item2 + 1);
                }
                else
                {
                    nnDataset[line.Item1] = new Tuple<double, int>(line.Item2,1);
                }


                if (nnDataset.Count % 100000 == 0 && wasPrint == false)
                {
                    Console.WriteLine("Уже обработано " + nnDataset.Count);
                    wasPrint = true;
                }
                if (nnDataset.Count % 100000 == 1)
                    wasPrint = false;
            }

            Save(output, nnDataset,null);
        }

        protected override void OpenSource(string path)
        {
            parser = new CSVParser();
            parser.Load(path);
        }

        protected override Tuple<sbyte[], double> ParseData(object obj)
        {
            string[] data = obj as string[];
            SByte[] result = Enumerable.Repeat<SByte>(0, HERO_SUBGROUP_COUNT*2).ToArray();
            if (data.Length < 12)
            {
                Console.WriteLine("Неверный формат " + string.Join(" ",data));
                return null;
            }

            for (int i = 0; i < 5; i++)
            {
                //получаем гуид героя
                int guid = Int32.Parse(data[i]);
                //получаем ид героя
                int id = guidMapper[$"Hero#{guid+1}"];
                //получаем индекс роли(иды идут с еденицы а индексы с нуля)
                int index = (sbyte)(heroService.Find(id + 1).SubGroup - 1);
                result[index]++;
            }


            for (int i = 0; i < 5; i++)
            {
                int guid = Int32.Parse(data[i + 5]);
                int id = guidMapper[$"Hero#{guid+1}"];
                int index = (sbyte)(heroService.Find(id + 1).SubGroup - 1 + HERO_SUBGROUP_COUNT);
                result[index]++;
            }


            double win = SByte.Parse(data[11]);
            //result[result.Length-1] = SByte.Parse(data[6]);


            return new Tuple<sbyte[], double>(result,win);
        }

        protected override object ReadData()
        {
            return parser.Next();
        }

        protected override void Save(string name, object obj, Type t)
        {
            Dictionary<SByte[], Tuple<double,int>> bObj = obj as Dictionary<SByte[], Tuple<double, int>>;
            Console.WriteLine("Сохранение...(может занять много времени)");

            using (var file = new StreamWriter(name))
            {
                
                foreach (var it in bObj)
                {
                    double prob = it.Value.Item1 / it.Value.Item2;
                    if (filter == true)
                    {
                        if (prob >= 0.4 && prob <= 0.6)
                            continue;
                        if (it.Value.Item2 <= 3 )
                            continue;
                    }
                    string oneItem = "";
                    for (int i = 0; i < it.Key.Length; i++)
                        oneItem += it.Key[i] + ",";
                    if (prob >= 1)
                        oneItem += 1;
                    else if (prob <= 0)
                        oneItem += 0;
                    else
                        oneItem += prob.ToString().Replace(",",".");
                    file.WriteLine(oneItem);
                }
            }
        }

        public override void Validate()
        {
            if(NamedParam.ContainsKey("i") == false)
            {
                throw new Exception("Не инициализирован обязательный параметр i");
            }
            if (File.Exists(NamedParam["i"]) == false)
            {
                throw new FileNotFoundException($"{NamedParam["i"]} not found");
            }
            if (NamedParam.ContainsKey("gi") == false)
            {
                throw new Exception("Не инициализирован обязательный параметр gi");
            }
            if (File.Exists(NamedParam["gi"]) == false)
            {
                throw new FileNotFoundException($"{NamedParam["gi"]} not found");
            }
            if (NamedParam.ContainsKey("h") == false)
            {
                throw new Exception("Не инициализирован обязательный параметр h");
            }
            if (File.Exists(NamedParam["h"]) == false)
            {
                throw new FileNotFoundException($"{NamedParam["h"]} not found");
            }
            if (NamedParam.ContainsKey("o") == false)
            {
                string input = NamedParam["o"];
                string fDir = Path.GetDirectoryName(input);
                string fName = Path.GetFileNameWithoutExtension(input);
                string fExt = Path.GetExtension(input);
                NamedParam["o"] = Path.Combine(fDir, String.Concat(fName, "_o", fExt));
            }

            if (Keys.IndexOf("-filter") != -1)
                filter = true;

            input = NamedParam["i"];
            output = NamedParam["o"];
            guidInput = NamedParam["gi"];
            heroInput = NamedParam["h"];
        }
    }
}
