using Accord.Neuro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity.AIDto
{
    public class Layer
    {
        static BindingFlags flags = System.Reflection.BindingFlags.NonPublic
| System.Reflection.BindingFlags.Instance;

        public int InputsCount { get; set; }
        public Neuron[] Neurons { get; set; }
        public double[] Output { get; set; }

        public static implicit operator ActivationLayer(Layer l)
        {
            ActivationLayer layer = new ActivationLayer(0,
                0, null);
            Type type = layer.GetType();

            type.GetField("inputsCount", flags).SetValue(layer, l.InputsCount);
            type.GetField("neurons", flags).SetValue(layer, l.Neurons.Select(x => (ActivationNeuron)x).ToArray());
            type.GetField("output", flags).SetValue(layer, l.Output);
            type.GetField("neuronsCount", flags).SetValue(layer, l.Neurons.Length);

            return layer;
        }

    }
}
