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
    class HeroParser : CLIParser<Hero>
    {
        string input, output;

        CSVParser parser;

        private List<Hero> heroResult = new List<Hero>();

        public List<Hero> HeroResult { get => heroResult; protected set => heroResult = value; }

        public override void Run(string[] args)
        {
                base.Run(args);
                Validate();
                OpenSource(input);

                object data;
               
                while ((data = ReadData()) != null)
                {
                    Hero h = ParseData(data);
                    if (h == null)
                        continue;
                    HeroResult.Add(h);
                    Console.WriteLine(h.ToString());
                }

                Save(output, HeroResult, HeroResult.GetType());

       


        }

        protected override void OpenSource(string path)
        {
            parser = new CSVParser();
            parser.Load(path);
        }

        protected override Hero ParseData(object obj)
        {
            string[] data = obj as string[];
            if (data.Length < 2)
            {
                Console.WriteLine("Пустая строка");
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
                if (parse == false)
                {
                    Console.WriteLine("Не могу распарсить {0}", data[2]);
                    group = HeroGroup.Unknown;
                }
                parse = Enum.TryParse(data[3].Replace(' ', '_'), true, out subgroup);
                if (parse == false)
                {
                    Console.WriteLine("Не могу распарсить {0}", data[3]);
                    group = HeroGroup.Unknown;
                }
            }

            Hero temp = new Hero(id, name, group, subgroup);
            return temp;
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
    }
}
