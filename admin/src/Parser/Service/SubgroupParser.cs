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

    //на вход: {гуиды героев команды 1} {гуиды героев команды 2} {ид карты} {вероятность победы команды 1}
    //пример: 69,11,67,65,63,23,29,58,49,69,16,1
    //на выход: {вектор ролей команды 1} {вектор ролей команды 2} {вектор идов карты}
    // {вероятность победы команды 1}
    class SubgroupParser : CLIParser<SByte[]>
    {
        string input, output,guidInput,heroInput;
        const int HERO_SUBGROUP_COUNT = 9;
        const int MAP_COUNT = 13;
        int heroCount = 0;

        CSVParser parser;

        Dictionary<string, int> guidMapper;
        HeroService heroService;

        public override void Run(string[] args)
        {
            base.Run(args);
            Validate();

            Console.WriteLine($"HERO_SUBGROUP_COUNT = {HERO_SUBGROUP_COUNT}");
            Console.WriteLine($"MAP_COUNT = {MAP_COUNT}");

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

            List<SByte[]> nnDataset = new List<SByte[]>();
            OpenSource(input);
                        
            object data;

            while ((data = ReadData()) != null)
            {
                var line = ParseData(data);
                if (line == null)
                    continue;

                nnDataset.Add(line);

                if (nnDataset.Count % 100000 == 0)
                    Console.WriteLine("Уже обработано " + nnDataset.Count);
            }

            Save(output, nnDataset.ToList(),null);
        }

        protected override void OpenSource(string path)
        {
            parser = new CSVParser();
            parser.Load(path);
        }

        protected override SByte[] ParseData(object obj)
        {
            string[] data = obj as string[];
            SByte[] result = Enumerable.Repeat<SByte>(0, HERO_SUBGROUP_COUNT * 2 + MAP_COUNT + 1).ToArray();
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

            int mapid = Int32.Parse(data[10]);
            int m_index = mapid - heroCount + 2 * HERO_SUBGROUP_COUNT;
            result[m_index] = 1;
            result[result.Length-1] = SByte.Parse(data[11]);
            //result[result.Length-1] = SByte.Parse(data[6]);


            return result;
        }

        protected override object ReadData()
        {
            return parser.Next();
        }

        protected override void Save(string name, object obj, Type t)
        {
            List<SByte[]> bObj = obj as List<SByte[]>;

            Console.WriteLine("Сохранение...(может занять много времени)");
            parser.Save(name,bObj);
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
         

            input = NamedParam["i"];
            output = NamedParam["o"];
            guidInput = NamedParam["gi"];
            heroInput = NamedParam["h"];
        }
    }
}
