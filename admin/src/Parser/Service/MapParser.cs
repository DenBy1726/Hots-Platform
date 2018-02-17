using HoTS_Service.Entity;
using HoTS_Service.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Service
{
    class MapParser : CLIParser<Map>
    {
        string input, output;
        CSVParser parser;

        public List<Map> MapResult = new List<Map>();
        public override void Run(string[] args)
        {
            base.Run(args);
            Validate();
            OpenSource(input);

            object data;

            while ((data = ReadData()) != null)
            {
                Map h = ParseData(data);
                if (h == null)
                    continue;
                MapResult.Add(h);
                Console.WriteLine(h.ToString());
            }

            Save(output, MapResult, MapResult.GetType());
        }

        public override void Validate()
        {
            if (NamedParam.ContainsKey("i") == false)
            {
                throw new Exception("Не инициализирован обязательный параметр i");
            }
            if (File.Exists(NamedParam["i"]) == false)
            {
                throw new FileNotFoundException($"{NamedParam["i"]} not found");
            }
            if (NamedParam.ContainsKey("o") == false)
            {
                string input = NamedParam["i"];
                string fDir = Path.GetDirectoryName(input);
                string fName = Path.GetFileNameWithoutExtension(input);
                string fExt = Path.GetExtension(input);
                NamedParam["o"] = Path.Combine(fDir, String.Concat(fName, "_o", fExt));
            }


            input = NamedParam["i"];
            output = NamedParam["o"];
        }

        protected override void OpenSource(string path)
        {
            parser = new CSVParser();
            parser.Load(path);
        }

        protected override Map ParseData(object obj)
        {
            string[] data = obj as string[];
            if (data.Length < 2)
            {
                Console.WriteLine("Пустая строка");
                return null;
            }

            int id = Int32.Parse(data[0]);
            if (id > 1000)
                id = id % 1000;
            string name = data[1];

            Map temp = new Map(id, name);
            return temp;
        }

        protected override object ReadData()
        {
            return parser.Next();
        }

        protected override void Save(string name, object obj, Type t)
        {
            File.WriteAllText(name, JSonParser.Save(obj, t));
        }
    }
}
