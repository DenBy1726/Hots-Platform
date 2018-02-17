using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Service.Tests
{
    [TestClass()]
    public class AutoParserTests
    {
        [TestMethod()]
        [TestCategory("validation")]
        public void CheckFileExistTest()
        {
            string path = @"E:\Кабинет\Курс 4\Курсач\Source Dataset";

            new AutoParser.InputFileParam(path);



            Assert.ThrowsException<DirectoryNotFoundException>(()=>
            {
                new AutoParser.InputFileParam(path + "0");

            });

        }
    }
}