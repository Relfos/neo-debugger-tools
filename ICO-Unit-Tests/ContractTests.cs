using System;
using System.IO;
using LunarParser;
using LunarParser.JSON;
using Neo.Emulator;
using NUnit.Framework;

namespace ICO_Unit_Tests
{
    [TestFixture]
    public class ContractTests
    {
        private static NeoEmulator emulator; 

        [OneTimeSetUp]
        public void Setup()
        {
            var path = TestContext.CurrentContext.TestDirectory.Replace("ICO-Unit-Tests", "ICO-Template");
            Directory.SetCurrentDirectory(path);
            var avmBytes = File.ReadAllBytes("ICOContract.avm");
            emulator = new NeoEmulator(avmBytes);
        }

        [Test]
        public void TestSymbol()
        {
            var inputs = DataNode.CreateArray();
            inputs.AddValue("symbol");
            inputs.AddValue(null);

            emulator.LoadInputs(inputs);
            emulator.Reset();

            emulator.Run();

            var result = emulator.GetResult();
            Assert.NotNull(result);

            var symbol = result.GetString();
            Assert.IsTrue(symbol.Equals("DEMO"));
        }
    }   
}
