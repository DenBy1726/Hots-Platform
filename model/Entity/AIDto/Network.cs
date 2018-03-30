using Accord.Neuro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Entity.AIDto
{
    public class Network
    {
        static BindingFlags flags = System.Reflection.BindingFlags.NonPublic
| System.Reflection.BindingFlags.Instance;

        public int InputsCount { get; set; }
        public Layer[] Layers { get; set; }
        public double[] Output { get; set; }
        public static implicit operator ActivationNetwork(Network l)
        {
            ActivationNetwork network = new ActivationNetwork(null, 0, new int[1]);
            Type type = network.GetType();

            type.GetField("inputsCount", flags).SetValue(network, l.InputsCount);
            type.GetField("layers", flags).SetValue(network, l.Layers.Select(x => (ActivationLayer)x).ToArray());
            type.GetField("output", flags).SetValue(network, l.Output);
            type.GetField("layersCount", flags).SetValue(network, l.Layers.Length);

            return network;
        }
    }
}
