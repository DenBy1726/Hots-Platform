using HoTS_Service.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Service.Tests
{
    [TestClass()]
    public class MapParserTests : AutoParser.MapParser
    {
        public MapParserTests() : base("", "")
        {
        }

        [TestMethod()]
        [TestCategory("parser")]
        public void ParseMap()
        {
            object[] inputs = new object[8];
            object[] outputs = new object[8];
            object[] targets = new object[8];

            inputs[0] = new string[] { "1", "Map1", "1", "1" } as object;
            inputs[1] = new string[] { "1001", "Map1", "1", "1" } as object;
            inputs[2] = new string[] { "10001", "Map1", "1", "1"} as object;
            inputs[3] = new string[] { "1", "Map1", "1", "1","1" } as object;
            inputs[4] = null;
            inputs[5] = new string[] { "0", "Map1", "1", "1" } as object;
            inputs[6] = new string[] { "0", "Map1", "","" } as object;
            inputs[7] = new string[] { "0", "Map1" } as object;

            targets[0] = new Map(1,"Map1");
            targets[1] = new Map(1, "Map1");
            targets[2] = new Map(1, "Map1");
            targets[3] = null;
            targets[4] = null;
            targets[5] = new Map(0, "Map1");
            targets[6] = new Map(0, "Map1");
            targets[7] = new Map(0, "Map1");

            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = base.ParseData(inputs[i]);
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                Assert.AreEqual(outputs[i], targets[i]);

            }


        }
    }
}