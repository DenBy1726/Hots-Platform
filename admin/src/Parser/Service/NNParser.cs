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
    class NNParser : CLIParser<string[]>
    {
        string input, output,guidInput;
        bool addition = false;

        CSVParser parser;

        Dictionary<string, int> guidMapper;

        public override void Run(string[] args)
        {
            base.Run(args);
            Validate();
            guidMapper = (Dictionary<string, int>)JSonParser.Load(File.ReadAllText(guidInput), typeof(Dictionary<string, int>));
            HashSet<string[]> nnDataset = new HashSet<string[]>();
            OpenSource(input);
                        
            object data;

            if(addition)
                Console.WriteLine("Датасет будет дополнен обратными записями");

            while ((data = ReadData()) != null)
            {
                string[] line = ParseData(data);
                if (line == null)
                    continue;
                nnDataset.Add(line);
                if (addition == true)
                {
                    string[] reverseLine = ParseLoseData(data);
                    reverseLine[6] = (1 - Int32.Parse(reverseLine[6])).ToString();

                    nnDataset.Add(reverseLine);
                }

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

        protected override string[] ParseData(object obj)
        {
            string[] data = obj as string[];
            if (data.Length < 7)
            {
                Console.WriteLine("Неверный формат " + string.Join(" ",data));
                return null;
            }

            string[] result = new string[7];

            for (int i = 0; i < 5; i++)
            {
                result[i] = guidMapper["Hero#" + data[i]].ToString();
            }

       /*     for (int i = 0; i < 5; i++)
            {
                result[i + 5] = guidMapper["Hero#" + data[i + 5] + "#1"].ToString();
            }*/

            result[5] = guidMapper["Map#" + data[10]].ToString();
            result[6] = data[11];
           
            return result;
        }

        protected string[] ParseLoseData(object obj)
        {
            string[] data = obj as string[];
            if (data.Length < 7)
            {
                Console.WriteLine("Неверный формат " + string.Join(" ", data));
                return null;
            }

            string[] result = new string[7];

            for (int i = 0; i < 5; i++)
            {
                result[i] = guidMapper["Hero#" + data[i+5]].ToString();
            }

            /*     for (int i = 0; i < 5; i++)
                 {
                     result[i + 5] = guidMapper["Hero#" + data[i + 5] + "#1"].ToString();
                 }*/

            result[5] = guidMapper["Map#" + data[10]].ToString();
            result[6] = data[11];

            return result;
        }

        protected override object ReadData()
        {
            return parser.Next();
        }

        protected override void Save(string name,object obj, Type t)
        {
            List<string[]> sObj = obj as List<string[]>;
            parser.Save(name,sObj);
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
            if (NamedParam.ContainsKey("o") == false)
            {
                string input = NamedParam["i"];
                string fDir = Path.GetDirectoryName(input);
                string fName = Path.GetFileNameWithoutExtension(input);
                string fExt = Path.GetExtension(input);
                NamedParam["o"] = Path.Combine(fDir, String.Concat(fName, "_o", fExt));
            }
            if(Keys.IndexOf("-add") != -1)
            {
                addition = true;
            }

           
            input = NamedParam["i"];
            output = NamedParam["o"];
            guidInput = NamedParam["gi"];
        }
    }
}
