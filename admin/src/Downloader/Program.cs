using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static HoTS_Service.Util.ConsoleProgress;
using static HoTS_Service.Util.Logger;
using System.IO.Compression;
using Parser;
using System.Threading;
using HoTS_Service.Util;

namespace Downloader
{
    class Program
    {
        const string defaultDatasetURL = 
            "https://d1i1jxrdh2kvwy.cloudfront.net/Data/HOTSLogs%20Data%20Export%20Current.zip";

        const string zip = "./Download/Current.zip";
        const string heroAndMapPath = "./Download/Unzip/HeroIDAndMapID.csv";
        const string unzipPath = "./Download/Unzip";
        const string replaysOldPath = "./Download/Unzip/Replays.csv";
        const string replaysCharacter = "./Download/Unzip/ReplayCharacters.csv";

        const string heroPath = "./Input/Hero/Hero.csv";
        const string mapPath = "./Input/Map/Map.csv";
        const string replaysPath = "./Input/Mapper/Replay/Replays.csv";
        const string replayBatch = "./Input/Replay/";

        const string downloadPath = "./Download";

        static CSVParser parser = new CSVParser();
        static Mutex mutexObj = new Mutex();
        static AnimatedBar bar = new AnimatedBar();

        static void Main(string[] args)
        {
            string datasetURL = defaultDatasetURL;
            if (args.Length != 0)
                datasetURL = args[0];
            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);
            if (!Directory.Exists(unzipPath))
                Directory.CreateDirectory(unzipPath);
            if (!Directory.Exists("./Input"))
                Directory.CreateDirectory("./Input");
            if (!Directory.Exists("./Input/Hero"))
                Directory.CreateDirectory("./Input/Hero");
            if (!Directory.Exists("./Input/Map"))
                Directory.CreateDirectory("./Input/Map");
            if (!Directory.Exists("./Input/Mapper/Replay"))
                Directory.CreateDirectory("./Input/Mapper/Replay");
            if (!Directory.Exists("./Input/Replay"))
                Directory.CreateDirectory("./Input/Replay");


            WebClient client = new WebClient();
            client.Proxy = null;
            ServicePointManager.DefaultConnectionLimit = 100000000;
            WebRequest.DefaultWebProxy = null;



            if (File.Exists(zip))
            {
                log("debug", "Датасет уже загружен в директорию");
            }
            else
            {

                log("debug", "Загрузка датасета начата");
                client.DownloadProgressChanged += DownloadProgressChanged;
                client.DownloadFileCompleted += DownloadFileCompleted;
                client.DownloadFileTaskAsync(new Uri(datasetURL), zip).Wait();
            }

            log("debug", "Распаковка архива начата");
            string[] files = Directory.GetFiles(unzipPath);
            if (files.Length > 0)
            {
                log("debug", "Директория для распаковки не пуста." +
                    "Возможно вы уже распаковывали архив ранее?");
                log("succes", "Распаковка архива прервана");
            }
            else
            {
                UnzipWithProgress(zip, unzipPath);
                log("succes", "Распаковка архива завершена");
            }

            log("debug", "Разбиение HeroIDAndMapID");

            if (!File.Exists(heroAndMapPath))
            {
                log("error", "HeroIDAndMapID не найден");
            }
            else
            {
                SplitHeroMap();
                log("succes", "Разбиение HeroIDAndMapID успешно");
            }

           
            if (File.Exists(replaysOldPath) && File.Exists(replaysPath) == false)
            {
                log("debug", "Перемещение Replays");
                File.Copy(replaysOldPath, replaysPath);
                log("succes", "Перемещение Replays успешно");
            }

            log("debug", "Разбиение ReplayCharacters на пакеты");

            SplitReplay();

            log("succes", "Разбиение ReplayCharacters на пакеты завершено");



        }


        private static void UnzipWithProgress(string zipFile, string outFolder)
        {
            using (ZipArchive zip = ZipFile.OpenRead(zipFile))
            {
                log("debug", "Количество файлов: " + zip.Entries.Count);
                foreach(var entry in zip.Entries)
                {
                    log("debug", $"{entry.Name} {entry.Length} Bytes");
                    entry.ExtractToFile(outFolder + "//" + entry.Name);
                    log("succes", $"{entry.Name} Распакован ");
                }

            }
        }

        private static void SplitHeroMap()
        {
            using (parser = parser.Load(heroAndMapPath))
            {
                using (var hero = new StreamWriter(heroPath))
                {
                    using (var map = new StreamWriter(mapPath))
                    {
                        string[] data = null;
                        //пропускаем строку со схемой
                        parser.Next();
                        while ((data = parser.Next()) != null)
                        {
                            if (data[0].Length < 3)
                                hero.WriteLine(String.Join(",",data));
                            else
                                map.WriteLine(String.Join(",", data));
                        }
                    }
                }
            }
            parser.Dispose();
            
           
        }

        private static void SplitReplay()
        {
            int batchId = 0;
            string data;
            using(var replays = new StreamReader(replaysCharacter))
            {  
                //пропускаем строку со схемой
                replays.ReadLine();
                
                long length = new FileInfo(replaysCharacter).Length * 1000 / 1024;
                long readBytes = 0;

                //читаем весь файл
                while (true)
                {
                    using (var batch = new StreamWriter(replayBatch + "ReplayCharacters_" +
                        batchId + ".csv"))
                    {
                        for (int i = 0; i < 10000; i++)
                        {
                            data = replays.ReadLine();
                            //если строка null, файл закончен
                            if (data == null)
                                return;
                            batch.WriteLine(String.Join(",", data));
                            readBytes += data.Length;
                        }
                        batchId++;
                        drawTextProgressBar(readBytes ,length);
                    }
                }
            }
        }

        private static void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
                log("succes", "Загрузка датасета завершена");
            else
            {
                log("error", e.Error.Message);
                File.Delete(zip);
            }
        }

        private static void DownloadProgressChanged(object sender, 
            DownloadProgressChangedEventArgs e)
        {
            mutexObj.WaitOne();
            drawTextProgressBar((int)e.BytesReceived, (int)e.TotalBytesToReceive);
            mutexObj.ReleaseMutex();
        }
    }
}
