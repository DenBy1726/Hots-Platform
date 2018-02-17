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
    class GuidMaker : CLIParser<object>
    {
        string input, output,heroInput,mapInput;

        public List<Map> MapResult = new List<Map>();
        public override void Run(string[] args)
        {
            base.Run(args);
            Validate();
            OpenSource(input);

            Statistic[] data = (Statistic[])ReadData();
            List<Hero> hData = (List<Hero>)JSonParser.Load(File.ReadAllText(heroInput,Encoding.Default), typeof(List<Hero>));
            List<Map> mData = (List<Map>)JSonParser.Load(File.ReadAllText(mapInput,Encoding.Default), typeof(List<Map>));

            Dictionary<string, int> mapper = new Dictionary<string, int>();

            int id = 0;

            ///Сперва добавляем только тех героев, которые участвовали в матче
            ///формат записи: Hero#ид героя#номер команды => глобальный идентификатор
            for(int i=0;i < data[0].Statictic.Matches.Length;i++)
            {
                if(data.Sum((x) => x.Statictic.Matches[i]) > 0)
                {
                    mapper.Add($"Hero#{i}", id++);
                    Console.WriteLine(hData[i] + "    =>  " + id + ";" + (id + 1));
                   // mapper.Add($"Hero#{i}#1", id++);
                }
                else
                {
                    Console.WriteLine(hData[i] + " отсутствует в датасете");
                }
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Statictic.Ammount > 0)
                {
                    mapper.Add($"Map#{i}", id++);
                    Console.WriteLine(mData[i] + "    =>  " + id);
                }
                else
                {
                    Console.WriteLine(mData[i] + " отсутствует в датасете");
                }
            }

            Save(output, mapper, mapper.GetType());

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
            if (NamedParam.ContainsKey("hi") == false)
            {
                throw new Exception("Не инициализирован обязательный параметр hi");
            }
            if (File.Exists(NamedParam["hi"]) == false)
            {
                throw new FileNotFoundException($"{NamedParam["hi"]} not found");
            }
            if (NamedParam.ContainsKey("mi") == false)
            {
                throw new Exception("Не инициализирован обязательный параметр mi");
            }
            if (File.Exists(NamedParam["mi"]) == false)
            {
                throw new FileNotFoundException($"{NamedParam["mi"]} not found");
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
            heroInput = NamedParam["hi"];
            mapInput = NamedParam["mi"];
        }

        protected override void OpenSource(string path)
        {
            
        }

        protected override object ParseData(object obj)
        {
            return null;
        }

        protected override object ReadData()
        {
            return JSonParser.Load(File.ReadAllText(NamedParam["i"],Encoding.Default), typeof(Statistic[]));
        }

        protected override void Save(string name, object obj, Type t)
        {
            File.WriteAllText(name, JSonParser.Save(obj, t));
        }
    }
}
