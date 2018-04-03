using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoTS_Service;
using HoTS_Service.Service;
using HoTS_Service.Entity;
using Accord.MachineLearning.Clustering;
using Accord.Math;
using Accord.Controls;
using Accord.MachineLearning;
using HoTS_Service.Entity.Enum;
using HoTS_Service.Util;
using System.IO;
using Parser;
using Accord.Neuro;
using Accord.Neuro.Learning;
using Accord.Statistics.Kernels;
using static HoTS_Service.Util.NNLoger;
using static HoTS_Service.Util.Logger;
using HoTS_Service.Entity.AIDto;

namespace Classifier
{
    public struct LogInfo
    {
        public double error;
        public int iteration;
        public double validError;
        public double percent;
        public double validPercent;

        public void WriteTop()
        {
            Console.Title = $"Best iteration: {iteration} " +
                $"percent {Math.Round(validPercent * 100, 2)}%";
        }
    }

    class Program
    {
        public static Config cfg;
        static void Main(string[] args)
        {
            if (Directory.Exists("./Source/Network") == false)
                Directory.CreateDirectory("./Source/Network");

            cfg = new Config("Classifier.config");

            log("info", "Конфиг:" + cfg.ToString());
            //File.WriteAllText("Classifier.config", JSonParser.Save(cfg, typeof(Config)));

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;



            // Convert the DataTable to input and output vectors
            foreach (var file in cfg.Input)
            {
                log("info", "Конфиг:" + cfg.ToString());
                List<Tuple<double[], double>> dataset =
                       new List<Tuple<double[], double>>();
                string root = Path.GetFileNameWithoutExtension(file);
                using (CSVParser parser = new CSVParser().Load(file))
                {
                    string[] buff;
                    double[] inputBuff;
                    double outputBuff;
                    while ((buff = parser.Next()) != null)
                    {
                        inputBuff = buff.Take(buff.Length - 1).ToArray().ToDouble();
                        outputBuff = Double.Parse(buff.Skip(buff.Length - 1).ToArray()[0]);
                        dataset.Add(new Tuple<double[], double>(inputBuff, outputBuff));
                    }
                }
                dataset = dataset
                    .Where(x => cfg.Filter.IsInside(x.Item2))
                    .Take(cfg.Filter.Max)
                    .ToList();
                log("info", "Конечный размер датасета:" + dataset.Count);
                if (cfg.Network.Shuffle)
                    dataset.Shuffle();
                var trainData = dataset
                    .Take((int)(dataset.Count * (1 - cfg.Network.ValidationPercent)))
                    .ToArray();
                var validData = dataset
                    .Skip((int)(dataset.Count * (1 - cfg.Network.ValidationPercent)))
                    .ToArray();

                var trainInput = trainData.Select(x => x.Item1).ToArray();
                var trainOutput = trainData.Select(x => new double[] { x.Item2 }).ToArray();
                var validInput = validData.Select(x => x.Item1).ToArray();
                var validOutput = validData.Select(x => new double[] { x.Item2 }).ToArray();
                var topology = new List<int>(cfg.Network.Layers)
                    {
                        1
                    };
                var network = new ActivationNetwork(
                    new SigmoidFunction(), trainInput[0].Length, topology.ToArray());
                var teacher = new ParallelResilientBackpropagationLearning(network);

                LogInfo current = new LogInfo()
                {
                    error = double.PositiveInfinity,
                    iteration = 0,
                    percent = 0,
                    validError = double.PositiveInfinity
                };

                LogInfo better = current;

                double previous;
                do
                {
                    previous = current.error;
                    current.error = teacher.RunEpoch(trainInput, trainOutput);

                    if (cfg.MoreInfoLog)
                    {
                        int[] answers =
                            validInput.Apply(network.Compute).GetColumn(0).
                            Apply(x => x > 0.5 ? 1 : 0);
                        current.validError = teacher.ComputeError(validInput, validOutput);
                        int[] outputs = validOutput.Apply(x => x[0] > 0.5 ? 1 : 0);
                        int pos = 0;
                        for (int j = 0; j < answers.Length; j++)
                        {
                            if (answers[j] == outputs[j])
                                pos++;
                        }
                        current.validPercent = (double)pos / (double)answers.Length;

                        answers =
                            trainInput.Apply(network.Compute).GetColumn(0).
                            Apply(x => x > 0.5 ? 1 : 0);

                        outputs = trainOutput.Apply(x => x[0] > 0.5 ? 1 : 0);

                        pos = 0;
                        for (int j = 0; j < answers.Length; j++)
                        {
                            if (answers[j] == outputs[j])
                                pos++;
                        }
                        current.percent = (double)pos / (double)answers.Length;

                        log(current.iteration, current.error, current.validError,
                            current.percent, current.validPercent);
                    }
                    else
                    {
                        smalllog(current.iteration, current.error);
                    }
                    if (current.error < cfg.Cancelation.Error)
                        break;
                    if (Math.Abs(previous - current.error) < cfg.Cancelation.Step)
                        break;
                    if (current.iteration == cfg.Cancelation.MaxEpoch)
                        break;
                    if (current.percent >= cfg.Validation.Percent)
                        break;
                    current.iteration++;

                    if (better.validPercent < current.validPercent)
                    {
                        better = current;
                        SaveNetwork($"Best_{root}", validInput,
                            validOutput, network, better, root);
                    }
                    better.WriteTop();

                } while (true);
                SaveNetwork(root, trainInput, trainOutput, network, current, root);
            }
        }

        private static void SaveNetwork(string root, double[][] Input,
            double[][] Output, ActivationNetwork network, LogInfo info, string type)
        {
            double[] output = Input.Apply(network.Compute).GetColumn(0);
            double[] target = Output.Select(x => x[0]).ToArray();
            double[] error = output.Subtract(target);
            if (Directory.Exists($"./Source/Network/{root}") == false)
                Directory.CreateDirectory($"./Source/Network/{root}/Report");
            if (Directory.Exists($"./Source/Network/{root}/Report") == false)
                Directory.CreateDirectory($"./Source/Network/{root}/Report");
            File.WriteAllText($"./Source/Network/{root}/Network.json", MakeNetworkJSON(network, type));
            File.WriteAllText($"./Source/Network/{root}/Report/TrainingState.json", JSONWebParser.Save(info));
            File.WriteAllLines($"./Source/Network/{root}/Report/Output.csv", output.Select(x => x.ToString()));
            File.WriteAllLines($"./Source/Network/{root}/Report/Target.csv", target.Select(x => x.ToString()));
            File.WriteAllLines($"./Source/Network/{root}/Report/Error.csv", error.Select(x => x.ToString()));
        }

        private static string MakeNetworkJSON(ActivationNetwork network, string type)
        {
            TrainMeta curMeta;
            if (cfg.Meta == null)
                curMeta = new TrainMeta() { Name = type };
            else
            {
                var found = cfg.Meta.ToList().Find(x => x.Name == type);
                if (found == null)
                    curMeta = new TrainMeta() { Name = type };
                else
                    curMeta = found;
            }
            return JSONWebParser.Save(new { Network = network, Meta = curMeta });
        }
    }
}