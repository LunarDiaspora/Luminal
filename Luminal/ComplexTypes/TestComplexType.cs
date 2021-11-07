using Luminal.Assets.Complex;
using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luminal.ComplexTypes
{
    [ComplexType("TestComplex")]
    public class TestComplexType : ComplexAsset
    {
        struct TestComplexMeta
        {
            public string Text { get; set; }
        }

        public string Text;

        public override void OnConstruct(string rootPath, string metaFileContents)
        {
            var f = JsonSerializer.Deserialize<TestComplexMeta>(metaFileContents);
            Text = f.Text;
        }

        public void SayText()
        {
            Log.Debug($"TestComplexType: {Text}");
        }
    }
}
