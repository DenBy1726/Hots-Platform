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
    class NNParser2 : CLIParser<SByte[]>
    {
        string input, output,guidInput;

        CSVParser parser;

        Dictionary<string, int> guidMapper;

        public override void Run(string[] args)
        {
            base.Run(args);
            Validate();
            guidMapper = (Dictionary<string, int>)JSonParser.Load(File.ReadAllText(guidInput), typeof(Dictionary<string, int>));
            List<SByte[]> nnDataset = new List<SByte[]>();
            OpenSource(input);
                        
            object data;

            while ((data = ReadData()) != null)
            {
                SByte[] line = ParseData(data);
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
            SByte[] result = Enumerable.Repeat<SByte>(-1, guidMapper.Count + 1).ToArray();

            if (result.Length < 7)
            {
                Console.WriteLine("Неверный формат " + string.Join(" ",data));
                return null;
            }

            for (int i = 0; i < 5; i++)
            {
                result[Int32.Parse(data[i])] = 1;
            }

            result[Int32.Parse(data[5])] = 1;
            result[result.Length-1] = SByte.Parse(data[6]);
           
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
            guidInput = NamedParam["gi"];
        }
    }
}
