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

namespace Classifier
{

    class Program
    {
        struct LogInfo
        {
            public double error;
            public int iteration;
            public double validError;
            public double percent;
            public double validPercent;

            public void WriteTop()
            {
                Console.SetCursorPosition(0, 0);
                log(iteration, error, validError, percent, validPercent);
                Console.WriteLine("________________________________________");
            }

        }

        static void Main(string[] args)
        {
            if (Directory.Exists("./Source/Network") == false)
                Directory.CreateDirectory("./Source/Network");
            Config cfg = new Config("Classifier.config");
            //File.WriteAllText("Classifier.config", JSonParser.Save(cfg, typeof(Config)));

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;



            // Convert the DataTable to input and output vectors
            foreach (var file in cfg.Input)
            {
                using (CSVParser parser = new CSVParser().Load(file))
                {
                    List<Tuple<double[], double>> dataset =
                       new List<Tuple<double[], double>>();
                    string[] buff;
                    double[] inputBuff;
                    double outputBuff;
                    while ((buff = parser.Next()) != null)
                    {
                        inputBuff = buff.Take(buff.Length - 1).ToArray().ToDouble();
                        outputBuff = Double.Parse(buff.Skip(buff.Length - 1).ToArray()[0]);
                        dataset.Add(new Tuple<double[], double>(inputBuff, outputBuff));
                    }

                    dataset = dataset
                        .Where(x => cfg.Filter.IsInside(x.Item2))
                        .Take(cfg.Filter.Max)
                        .ToList();
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
                            network.Save("./Source/Network/Best_" + Path.GetFileName(file));
                        }
                        better.WriteTop();
                        
                    } while (true);
                    network.Save("./Source/Network/" + Path.GetFileName(file));
                }

            }

        }
    }
}