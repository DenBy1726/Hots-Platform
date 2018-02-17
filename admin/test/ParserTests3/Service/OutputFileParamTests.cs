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
    public class OutputFileParamTests
    {
        [TestMethod()]
        [TestCategory("validation")]
        public void CreateFolderTest()
        {

            AutoParser.OutputFileParam output = new AutoParser.OutputFileParam
                (Directory.GetCurrentDirectory());

            for(int i=0;i<output.Count;i++)
            {
                Assert.IsTrue(Directory.Exists(output.OutputFolder + output.Path[i]));
            }
            


        }
    }
}