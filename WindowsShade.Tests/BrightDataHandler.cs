using Com.EnjoyCodes.SharpSerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WindowsShade.Models;

namespace WindowsShade.Tests
{
    [TestClass]
    public class BrightDataHandler
    {
        private class B
        {
            public string Time { get; set; }
            public byte Alpha { get; set; }
        }
        private class BCompare : IEqualityComparer<B>
        {
            public bool Equals(B x, B y) => x.Time == y.Time && x.Alpha == y.Alpha;

            public int GetHashCode(B obj) => obj == null ? 0 : obj.ToString().GetHashCode();
        }

        [TestMethod]
        public void LoadDatas()
        {
            var folders = @"C:\Program Files\qizl\WindowsShade\Brightness";
            var files = Directory.GetFiles(folders);

            // 1.加载日志数据
            var brightnesses = new List<Brightness>();
            var serializer = new SharpSerializer();

            foreach (var item in files)
            {
                var r1 = serializer.Deserialize(item) as List<Brightness>;
                if (r1 != null)
                    brightnesses.AddRange(r1);
            }

            // 2.日志数据优化

            // 3.转化为ML数据
            var d = brightnesses
                .Select(m => new B() { Time = m.Time.ToString("HHmm"), Alpha = m.Value })
                .OrderBy(m => m.Time)
                .Distinct(new BCompare())
                .ToList();
            var sb = new StringBuilder();
            d.ForEach(m =>
            {
                sb.AppendLine($"{m.Time},{m.Alpha}");
            });
            Debug.Write(sb.ToString());
        }
    }
}