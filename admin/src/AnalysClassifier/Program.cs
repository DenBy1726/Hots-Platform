using Accord.Controls;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Analysis;
using Accord.Statistics.Visualizations;
using HoTS_Service.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord.Math;
using System.Text;
using System.Threading.Tasks;
using static HoTS_Service.Util.Logger;
using Accord.Extensions.BinaryTree;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Statistics.Filters;

namespace AnalysClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            int nBufferWidth = Console.BufferWidth;
            Console.SetBufferSize(nBufferWidth, 1000);

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            Config cfg = new Config("AnalysClassifier.config");

            log("info", "Конфиг:" + cfg.ToString());
            double[] output = File.ReadAllLines(cfg.OutputPath)
                .Select(x => double.Parse(x)).ToArray();
            double[] target = File.ReadAllLines(cfg.TargetPath)
                .Select(x => double.Parse(x)).ToArray();
            double[] error = File.ReadAllLines(cfg.ErrorPath)
               .Select(x => double.Parse(x)).ToArray();

            if (cfg.Plotroc == true)
            {
                Console.WriteLine("Модуль Plotroc");
                Console.WriteLine("Расчет ROC");
                var roc = new ReceiverOperatingCharacteristic(output, target);
                roc.Compute(100); // Compute a ROC curve with 100 cut-off points
                Console.WriteLine("ROC расчитана");
                ScatterplotBox.Show(roc.GetScatterplot(includeRandom: true))
                    .SetSymbolSize(0)      // do not display data points
                    .SetLinesVisible(true) // show lines connecting points
                    .SetScaleTight(true);   // tighten the scale to points
            }
            if(cfg.Plothist == true)
            {
                Console.WriteLine("Модуль Plothist");
                HistoframShow(error,"ошибок");
                HistoframShow(target, "эталонных выходов");
                HistoframShow(output, "выходов модели");

            }
            if (cfg.Plotconfucion == true)
            {
                Console.WriteLine("Модуль Plotconfucion");
                Console.WriteLine("Расчет ConfusionMatrix");
                var cm = new GeneralConfusionMatrix(classes: 2,
                    expected: output.Select(x=>x>0.5?1:0).ToArray(), 
                    predicted: target.Select(x => x > 0.5 ? 1 : 0).ToArray());
                Console.WriteLine("ConfusionMatrix расчитана");
                Console.WriteLine("Confusion Matrix:");
                string[][] outMat = cm.Matrix.
                    ToJagged().
                    Select(x => x.Select(y => IntToStringFormatted(y)).ToArray()).
                    ToArray();

                foreach(var it in cm.ColumnTotals)
                {
                    Console.Write($"{IntToStringFormatted(it)}");
                }
                Console.WriteLine("|");
                Console.WriteLine(new string('_', 9 * cm.ColumnTotals.Length));
                int i = 0;
                foreach (var it in outMat)
                {
                  
                    foreach(var it2 in it)
                    {
                        Console.Write(it2);
                        Console.Write(" ");
                    }
                    Console.Write($"| {cm.RowTotals[i++]}");
                    Console.WriteLine();
                }
                Console.WriteLine();

                // We can get more information about our problem as well:
                Console.WriteLine("Дополнительная информация:");
                Console.WriteLine($"Классов: {cm.NumberOfClasses}:");
                Console.WriteLine($"Примеров: {cm.NumberOfSamples}:");
                Console.WriteLine($"Точность: {cm.Accuracy}:");
                Console.WriteLine($"Ошибка: {cm.Error}:");
                Console.WriteLine($"chanceAgreement: {cm.ChanceAgreement}:");
                Console.WriteLine($"geommetricAgreement: {cm.Error}:");
                Console.WriteLine($"pearson: {cm.Pearson}:");
                Console.WriteLine($"kappa: {cm.Kappa}:");
                Console.WriteLine($"tau: {cm.Tau}:");
                Console.WriteLine($"chiSquare: {cm.ChiSquare}:");
                Console.WriteLine($"kappaStdErr: {cm.Kappa}:");
       
                
            }
        }

        private static void HistoframShow(double[] data,string label)
        {
            Console.WriteLine($"Расчет гистограммы {label}");
            Histogram hist = new Histogram();
            hist.Compute(data,numberOfBins:20);
            int min = hist.Values.Min();
            int max = hist.Values.Max();
            int range = max - min;
            int dashLength = range / 20;
            foreach (var it in hist.Bins)
            {
                Console.Write($"" +
                    $"[{DoubleToStringFormatted(it.Range.Min)}" +
                    $";{DoubleToStringFormatted(it.Range.Max)}]");
                int dashes = it.Value / dashLength;
                for (int i = 0; i < dashes; i++)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.Write(" ");
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();

            }

            Console.WriteLine($"Гистограмма {label} расчитана");
        }

        static string IntToStringFormatted(int value)
        {
            string s_value = value.ToString();
            while (s_value.Length < 8)
                s_value += " ";
            return s_value;
        }
        static string DoubleToStringFormatted(double value)
        {
            string s_value = Math.Round(value,2).ToString();
            while (s_value.Length < 5)
                s_value += " ";
            return s_value;
        }
    }
}


