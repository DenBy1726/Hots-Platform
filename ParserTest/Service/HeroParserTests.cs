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
    public class HeroParserTests : AutoParser.HeroParser
    {
        public HeroParserTests() : base("", "")
        {
        }

        [TestMethod()]
        [TestCategory("parser")]
        public void ParseHero()
        {
            object[] inputs = new object[7];
            object[] outputs = new object[7];
            object[] targets = new object[7];

            inputs[0] = new string[] { "0", "Hero1", "1", "1" } as object;
            inputs[1] = new string[] { "124", "Hero1", "1241", "124" } as object;
            inputs[2] = new string[] { "124", "Hero1", "1241", "124","53" } as object;
            inputs[3] = new string[] { "124", "Hero1", "1241" } as object;
            inputs[4] = null;
            inputs[5] = new string[] { "0", "Hero1", "1", "1" } as object;
            inputs[6] = new string[] { "0", "Hero1" } as object;

            targets[0] = new Hero(0, "Hero1",HoTS_Service.Entity.Enum.HeroGroup.Assassin,HoTS_Service.Entity.Enum.HeroSubGroup.Tank);
            targets[1] = new Hero(124, "Hero1", HoTS_Service.Entity.Enum.HeroGroup.Unknown, HoTS_Service.Entity.Enum.HeroSubGroup.Unknown);
            targets[2] = null;
            targets[3] = null;
            targets[4] = null;
            targets[5] = new Hero(0, "Hero1", HoTS_Service.Entity.Enum.HeroGroup.Assassin, HoTS_Service.Entity.Enum.HeroSubGroup.Tank);
            targets[6] = new Hero(0, "Hero1", HoTS_Service.Entity.Enum.HeroGroup.Unknown, HoTS_Service.Entity.Enum.HeroSubGroup.Unknown);

            for (int i=0;i<inputs.Length;i++)
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