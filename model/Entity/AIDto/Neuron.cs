using Accord.Neuro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity.AIDto
{
    public class Neuron
    {
        static BindingFlags flags = System.Reflection.BindingFlags.NonPublic
| System.Reflection.BindingFlags.Instance;

        public double Threshold { get; set; }
        public ActivationFunction ActivationFunction { get; set; }
        public RandGenerator RandGenerator { get; set; }
        public int InputsCount { get; set; }
        public double Output { get; set; }
        public double[] Weights { get; set; }

        public static implicit operator ActivationNeuron(Neuron l)
        {
            ActivationNeuron neuron = new ActivationNeuron(0, null);
            Type type = neuron.GetType();

            type.GetField("threshold", flags).SetValue(neuron, l.Threshold);
            type.GetField("function", flags).SetValue(neuron, (SigmoidFunction)l.ActivationFunction);
            type.GetField("inputsCount", flags).SetValue(neuron, l.InputsCount);
            type.GetField("output", flags).SetValue(neuron, l.Output);
            type.GetField("weights", flags).SetValue(neuron, l.Weights);

            return neuron;

        }
    }
}
