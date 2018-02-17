using HoTS_Service.Entity;
using HoTS_Service.Entity.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Service.Tests
{
    [TestClass()]
    public class ReplaySchemaParserTests : AutoParser.ReplaySchemaParser
    {
        
        public ReplaySchemaParserTests()
            :base(Directory.GetCurrentDirectory() + "\\Replay\\Schema1.txt",
                 Directory.GetCurrentDirectory() + "\\Replay\\Match\\", "*.txt",
                 Directory.GetCurrentDirectory() + "\\Replay\\Output1.txt","","",2)
        {
            AutoParser.CSVParser = new CSVParser();
            AutoParser.CSVBParser = new CSVBatchParser();
        }

        [TestMethod()]
        [TestCategory("parser")]
        public void ParseReplay()
        {
            object[] inputs = new object[7];
            object[] outputs = new object[7];
            object[] targets = new object[7];

            inputs[0] = new string[] { "117363378", "3", "1016", "00:19:29",
                "00:19:29,7/14/2017 2:48:30 PM" } as object;
            inputs[1] = new string[] {"117363378", "3", "1016", "00:19:29",
                "00:19:29,7/14/2017 2:48:30 PM","gg"} as object;
            inputs[2] = new string[] {"117363378", "3", "1016", "00:19:29"
               } as object;
            inputs[3] = new string[] {"1173dsg63378", "3", "1016", "00:19:29",
                "00:19:29"} as object;
            inputs[4] = new string[] {"117363378", "333", "1016", "00:19:29",
                "00:19:29"} as object;
            inputs[5] = new string[] {"117363378", "3", "16", "00:19:29",
                "00:19:29"} as object;
            inputs[6] = new string[] {"117363378", "3", "16", "00:19:29",
                "LOL"} as object;

            targets[0] = new ReplaySchema()
            {
                gameMode = (GameMode)1,
                id = 117363378,
                mapId = 16,
                length = 19 * 60 + 29
            };
            targets[1] = null;
            targets[2] = null;
            targets[3] = null;
            targets[4] = new ReplaySchema()
            {
                gameMode = (GameMode)0,
                id = 117363378,
                mapId = 16,
                length = 19 * 60 + 29
            };
            targets[5] = new ReplaySchema()
            {
                gameMode = (GameMode)1,
                id = 117363378,
                mapId = 16,
                length = 19 * 60 + 29
            };
            targets[6] = new ReplaySchema()
            {
                gameMode = (GameMode)1,
                id = 117363378,
                mapId = 16,
                length = 19 * 60 + 29
            };

            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = base.ParseData(inputs[i]);
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                Assert.AreEqual(outputs[i], targets[i]);

            }

        }

        [TestMethod()]
        [TestCategory("parser")]
        public void ParseReplayData()
        {
            object[] inputs = new object[7];
            object[] outputs = new object[7];
            object[] targets = new object[7];

            inputs[0] = new string[]{"117363378","0","23","20","0","2081","18","12","7",
                "5","4","6","39023","832","7393","0","","8571","00:02:19","5" } as object;
            inputs[1] = new string[]{"117363378","0","23","20","0","2081","18","12","7",
                "5","4","6","39023","832","7393","0","","8571","00:02:19","5","h" } as object;
            inputs[2] = new string[]{"117363378","0","23","20","0","2081","18","12","7",
                "5","4","6","39023","832","7393","0","","8571","00:02:19" } as object;
            inputs[3] = new string[]{"1173sdg63378","0","23","20","0","2081","18","12","7",
                "5","4","6","39023","832","7393","0","","8571","00:02:19","5" } as object;
            inputs[4] = new string[]{"117363378","0","23","20","0sdf","2081","18","12","7",
                "5","4","6","39023","832","7393","0","","8571","00:02:19","5" } as object;
            inputs[5] = new string[]{"117363378","0","23","20","0","2081","18","12","7",
                "5","4","6","39023","832","","0","","8571","00:02:19","5" } as object;
            inputs[6] = new string[]{"117363378","0","23","20","0","2081","18","12","7",
                "5","4","6","39023","832","7393","0","123","8571","00:02:19","5" } as object;

            targets[0] = new HeroStatisticItem()
            {
                replayId = 117363378,
                heroId = 23,
                winrate = 0,
                killPerSec = 7,
                assistPerSec = 5,
                deathPerSec = 4,
                dps = 39023,
                sdps = 832,
                hps = 7393,
                damageTakenPerSec = 0,
                expPerSec = 8571,
                campTakenPerSec = 5
            };
            targets[1] = null;
            targets[2] = null;
            targets[3] = null;
            targets[4] = null;
            targets[5] = new HeroStatisticItem()
            {
                replayId = 117363378,
                heroId = 23,
                winrate = 0,
                killPerSec = 7,
                assistPerSec = 5,
                deathPerSec = 4,
                dps = 39023,
                sdps = 832,
                hps = 0,
                damageTakenPerSec = 0,
                expPerSec = 8571,
                campTakenPerSec = 5
            };
            targets[6] = new HeroStatisticItem()
            {
                replayId = 117363378,
                heroId = 23,
                winrate = 0,
                killPerSec = 7,
                assistPerSec = 5,
                deathPerSec = 4,
                dps = 39023,
                sdps = 832,
                hps = 7393,
                damageTakenPerSec = 123,
                expPerSec = 8571,
                campTakenPerSec = 5
            };

            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = base.ParseReplayData(inputs[i]);
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                Assert.AreEqual(outputs[i], targets[i]);

            }

        }

    }
}