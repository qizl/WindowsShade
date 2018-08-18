using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WindowsShade.Models;

namespace WindowsShade.Tests
{
    [TestClass]
    public class BrightDataHandler
    {
        [TestMethod]
        public void LoadDatas()
        {
            var folders = @"F:\Docs\PL\Github\WindowsShade\WindowsShade\bin\Debug\Brightness";
            //var folders = @"D:\PL\MyProjects\GitHub\WindowsShade\WindowsShade\bin\Debug\Brightness"; 
            var d = new BrightnessData().Load(folders);
            var sb = new StringBuilder();
            d.ToList().ForEach(m =>
            {
                sb.AppendLine($"{m.Time},{m.Value}");
            });
            Debug.Write(sb.ToString());
        }
    }
}