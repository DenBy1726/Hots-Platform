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

    class ReplayParserStat : CLIParser<Tuple<int,int,int>>
    {
        string input;

        CSVParser parser;

        public int MapCount = 0;

        private Dictionary<int,Tuple<int,int>> replayResult = new Dictionary<int, Tuple<int, int>>();

        public Dictionary<int, Tuple<int, int>> ReplayResult { get => replayResult; protected set => replayResult = value; }

        public override void Run(string[] args)
        {
            base.Run(args);
            Validate();
            OpenSource(input);

            object data;
            Console.WriteLine("Разбор схемы повторов");
            while ((data = ReadData()) != null)
            {
                var r = ParseData(data);
                if (r.Item1 == -1 && r.Item2 == -1)
                    continue;
                if (r.Item2 > MapCount)
                    MapCount = r.Item2;
                replayResult[r.Item1] = new Tuple<int,int>(r.Item2,r.Item3);
            }
        }

        protected override void OpenSource(string path)
        {
            parser = new CSVParser();
            parser.Load(path);
        }

        protected override Tuple<int,int,int> ParseData(object obj)
        {
            string[] data = obj as string[];

            try
            {
                int id = Int32.Parse(data[0]);
                int map = Int32.Parse(data[2]);
                DateTime dt = DateTime.Parse(data[3]);
                int time = dt.Second + dt.Minute*60;
                if (map > 1000)
                    map = map % 1000;
                return new Tuple<int, int,int>(id, map,time);
            }
            catch
            {
                Console.Write("Входная строка имела неверный формат. Парсинг продолжается");
            }
            return new Tuple<int, int,int>(-1, -1,-1);
        }

        protected override object ReadData()
        {
            return parser.Next();
        }

        protected override void Save(string name,object obj, Type t)
        {
            File.WriteAllText(name, JSonParser.Save(obj,t));
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



            input = NamedParam["i"];
        }
    }
}
