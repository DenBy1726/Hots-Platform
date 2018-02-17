using HoTS_Service.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Service
{
    class MapParserWithAlias : CLIParser<KeyValuePair<string, string[]>>
    {
        MapParser mParser = new MapParser();

        protected bool merge;

        string aInput, aOutput;

        protected Dictionary<string, int> mapper = new Dictionary<string, int>();

        public override void Run(string[] args)
        {
            base.Run(args);
            Validate();
            mParser.Run(args);

            if (merge == true)
            {
                Console.WriteLine($"Читаю псевдонимы из {aOutput}");
                Dictionary<string,object> dict = JSONWebParser.Load(aOutput) as Dictionary<string, object>;
                foreach(var it in dict)
                {
                    AddValue(mapper, it.Key, (int)it.Value);
                }

                Console.WriteLine($"Закончил чтение псевдонимов из файла");
            }

            foreach(var it in mParser.MapResult)
            {
                AddValue(mapper, it.Name, it.Id);
            }


            object[] aliases = (object[])ReadData();
            foreach (var obj in aliases)
            {
                var pair = ParseData(obj);
                bool parse = mapper.TryGetValue(pair.Key, out int id);
                if (parse == false)
                {
                    Console.WriteLine("Неизвестная карта {0}",pair.Key);
                    continue;
                }

                foreach (string alias in pair.Value)
                {
                    AddValue(mapper, alias, id);
                }

            }          

            System.IO.File.WriteAllText(aOutput, JSonParser.Save(mapper, mapper.GetType()));
        }

        private static void AddValue(Dictionary<string, int> mapper, string key, int value)
        {
            if (mapper.ContainsKey(key) == false)
            {
                mapper[key] = value;
                Console.WriteLine("Добавлен псевдоним: [{0}] => [{1}]", key, value);
            }
        }

        protected override void OpenSource(string path)
        {
            throw new NotImplementedException();
        }

        protected override KeyValuePair<string, string[]> ParseData(object json)
        {
            Dictionary<string, object> data = json as Dictionary<string, object>;
            string name = data["PrimaryName"].ToString();
            string[] aliases = data["Translations"].ToString().Split(',');
            return new KeyValuePair<string, string[]>(name, aliases);
        }

        protected override object ReadData()
        {
            return JSONWebParser.Load(File.ReadAllText(aInput));
        }

        protected override void Save(string name, object obj, Type t)
        {
            throw new NotImplementedException();
        }

        public override void Validate()
        {

            if (NamedParam.ContainsKey("ai") == false)
            {
                throw new Exception("Не инициализирован обязательный параметр ai");
            }
            if (File.Exists(NamedParam["ai"]) == false)
            {
                throw new FileNotFoundException($"{NamedParam["ai"]} not found");
            }

            if (NamedParam.ContainsKey("ao") == false)
            {
                string input = NamedParam["ai"];
                string fDir = Path.GetDirectoryName(input);
                string fName = Path.GetFileNameWithoutExtension(input);
                string fExt = Path.GetExtension(input);
                NamedParam["ao"] = Path.Combine(fDir, String.Concat(fName, "_o", fExt));
            }

            if (Keys.IndexOf("merge") == -1)
                merge = false;
            else
            {
                if (File.Exists(NamedParam["ao"]) == true)
                    merge = true;
                else
                    merge = false;
            }

            aInput = NamedParam["ai"];
            aOutput = NamedParam["ao"];
                
        }
    }
}
