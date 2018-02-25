using System;
using System.Collections.Generic;
using HoTS_Service.Entity;
using HoTS_Service.Util;
using System.IO;

namespace Parser
{
    class Program
    {
        static CSVParser parser;
        static string[] args;



        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = SetArgsByDefault();

            Program.args = args;

           
            if (Program.args.Length > 0 && args[0].ToLower() == "help")
            {
                Console.WriteLine("Модули:");
                Console.WriteLine("parsehero:\t загружает информацию о героях");
                Console.WriteLine("<i=источник> [o=результат]");
                Console.WriteLine("parseherowithalias:\t загружает информацию о героях и псевдонимах");
                Console.WriteLine("<источник> <источник псевдонимов> [результат] [результат псевдонимов]");
                return;
            }


            switch (args[0].ToLower())
            {
                case "parsehero":

                    new Service.HeroParser().Run(args);
                    break;
                case "parseherowithalias":
                    new Service.HeroParserWithMapper().Run(args);
                    break;
                case "parsemap":
                    new Service.MapParser().Run(args);
                    break;
                case "parsemapwithalias":
                    new Service.MapParserWithAlias().Run(args);
                    break;
                case "parsereplay":
                    var x = new Service.ReplayParser();
                    x.Run(args);
                    break;
                case "parsematches":
                    new Service.MatchParserStat().Run(args);
                    break;
                case "stattoexcel":
                    SaveTOExcelCSVFormat();
                    break;
                case "makeguid":
                    new Service.GuidMaker().Run(args);
                    break;
                case "nnparser":
                    new Service.NNParser().Run(args);
                    break;
                case "nnparser2":
                    new Service.NNParser2().Run(args);
                    break;
                case "subgroupparser":
                    new Service.SubgroupParser().Run(args);
                    break;
                case "nnparserboth":
                    new Service.NNParserBoth().Run(args);
                    break;
                case "subgroupparserwithoutmap":
                    new Service.SubgroupParserWithoutMap().Run(args);
                    break;
                case "autoparser":
                    new Service.AutoParser().Run(args);
                    break;
            }
        }

        private static string[] SetArgsByDefault()
        {
            Program.args = new string[]
            {
                @"autoparser",
                @"i=.\Input",
                @"o=.\Source"
            };
            return args;
        }


        public static void SaveTOExcelCSVFormat()
        {
            List<string> Keys = new List<string>();
            Dictionary<string, string> NamedParam = new Dictionary<string, string>();

            foreach (string arg in args)
            {
                string[] tryParse = arg.Split('=');
                if (tryParse.Length == 2)
                    NamedParam[tryParse[0]] = tryParse[1];
                else
                    Keys.Add(arg);
            }

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

            Statistic[] statistic = (Statistic[])JSonParser.Load(File.ReadAllText(NamedParam["i"]), typeof(Statistic[]));

            SaveTOExcelCSVFormat(statistic, NamedParam["o"]);
        }

        private static void SaveTOExcelCSVFormat(Statistic[] statistic, string file)
        {

            string result = "";
            int i = 0;

            result += "id" + '\t' + "matches" + "\r\n";
            foreach (var it in statistic)
            {
                result += i++ + "\t" + it.Statictic.Ammount + "\r\n";
            }

            i = 0;
            foreach (var it in statistic)
            {
                result += "id" + "\t" + "total" + "\t" + "wins" + "\t" + "for " + i + " map \r\n";
                for (int j = 0; j < it.Statictic.Matches.Length; j++)
                {
                    result += j + "\t" + it.Statictic.Matches[j] + "\t" + it.Statictic.Wins[j] + "\r\n";
                }
                i++;
            }

            File.WriteAllText(file, result);
        }
    }

     
    }

