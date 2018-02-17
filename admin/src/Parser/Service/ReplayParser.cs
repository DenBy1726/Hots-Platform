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

    class ReplayParser : CLIParser<KeyValuePair<int,int>>
    {
        string input;

        CSVParser parser;

        public int MapCount = 0;

        private Dictionary<int,int> replayResult = new Dictionary<int, int>();

        public Dictionary<int, int> ReplayResult { get => replayResult; protected set => replayResult = value; }

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
                if (r.Key == -1 && r.Value == -1)
                    continue;
                if (r.Value > MapCount)
                    MapCount = r.Value;
                replayResult[r.Key] = r.Value;
            }
        }

        protected override void OpenSource(string path)
        {
            parser = new CSVParser();
            parser.Load(path);
        }

        protected override KeyValuePair<int,int> ParseData(object obj)
        {
            string[] data = obj as string[];

            try
            {
                int id = Int32.Parse(data[0]);
                int map = Int32.Parse(data[2]);
                if (map > 1000)
                    map = map % 1000;
                return new KeyValuePair<int, int>(id, map);
            }
            catch
            {
                Console.Write("Входная строка имела неверный формат. Парсинг продолжается");
            }
            return new KeyValuePair<int, int>(-1, -1);
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
