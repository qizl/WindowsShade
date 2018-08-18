using Com.EnjoyCodes.SharpSerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        private class b
        {
            public string Time { get; set; }
            public byte Alpha { get; set; }
        }
        private class bCompare : IEqualityComparer<b>
        {
            public bool Equals(b x, b y) => x.Time == y.Time && x.Alpha == y.Alpha;

            public int GetHashCode(b obj) => obj == null ? 0 : obj.ToString().GetHashCode();
        }
        private class bStatus : Brightness
        {
            public bool Valid { get; set; }
            public int Weight { get; set; }

            public bStatus(DateTime time, byte value) : base(value)
            {
                this.Time = time;
            }
        }

        private class bStatusCompare : IEqualityComparer<bStatus>
        {
            public bool Equals(bStatus x, bStatus y) => x.Time == y.Time && x.Value == y.Value;

            public int GetHashCode(bStatus obj) => obj == null ? 0 : obj.ToString().GetHashCode();
        }

        [TestMethod]
        public void LoadDatas()
        {
            var folders = @"C:\Program Files\qizl\WindowsShade\Brightness";
            //var folders = @"D:\PL\MyProjects\GitHub\WindowsShade\WindowsShade\bin\Debug\Brightness"; 
            var files = Directory.GetFiles(folders);

            // 1.加载日志数据
            var brightnesses = new List<bStatus>();
            var serializer = new SharpSerializer();

            foreach (var item in files)
            {
                var r = serializer.Deserialize(item) as IEnumerable<Brightness>;
                if (r == null) continue;

                // 2.优化当天数据
                var r1 = this.analyzePerDay(r);
                brightnesses.AddRange(r1);
            }

            // 3.优化合并后数据
            brightnesses.ForEach(m => m.Time = new DateTime(2018, 1, 1, m.Time.Hour, m.Time.Minute, m.Time.Second));
            var r2 = this.analyzeAll(brightnesses);

            // 3.转化为ML数据
            var d = r2
                .Select(m => new b() { Time = m.Time.ToString("HHmmss"), Alpha = m.Value })
                .OrderBy(m => m.Time)
                .ToList();
            var sb = new StringBuilder();
            d.ForEach(m =>
            {
                sb.AppendLine($"{m.Time},{m.Alpha}");
            });
            Debug.Write(sb.ToString());
        }

        /// <summary>
        /// 原始数据分析
        /// </summary>
        /// <param name="brightnesses">一天的原始数据</param>
        /// <returns></returns>
        private IEnumerable<bStatus> analyzePerDay(IEnumerable<Brightness> brightnesses)
        {
            var bs = brightnesses
                .Where(m => m.Value != 0)
                .Distinct(new BrightnessCompare())
                .Select(m => new bStatus(m.Time, m.Value))
                .OrderBy(m => m.Time)
                .ToArray();

            for (int i = 1; i < bs.Length; i++)
            {
                //bs[i].Valid = false;

                // 1.最小有效间隔时间为10min，小于10min，则认为前一条无效
                if ((bs[i].Time - bs[i - 1].Time).TotalMinutes >= 10)
                    bs[i].Valid = true;
                else
                    continue;

                // 2.滤波，与两侧共5个数的平均数差小于指定值，则认为该数有效
                if (i > 1 && i < bs.Length - 2) // 两侧数的数量大于4
                {
                    var avg = bs.Skip(i - 2).Take(5).Average(m => m.Value);
                    //var abs = Math.Abs(avg - bs[i].Brightness.Value);
                    if (bs[i].Value > avg / 2)
                        bs[i].Valid = true;
                    else
                        continue;
                }
            }

            return bs.Where(m => m.Valid);
        }
        private IEnumerable<bStatus> analyzeAll(IEnumerable<bStatus> brightnesses)
        {
            var bs = brightnesses
                .Distinct(new bStatusCompare())
                .OrderBy(m => m.Time)
                .ToArray();

            for (int i = 1; i < bs.Length; i++)
            {
                bs[i].Valid = false;

                // 1.滤波，与两侧共5个数的平均数小于指定值，则认为该数有效
                if (i > 1 && i < bs.Length - 2) // 两侧数的数量大于4
                {
                    var avg = bs.Skip(i - 2).Take(5).Average(m => m.Value);
                    //var abs = Math.Abs(avg - bs[i].Brightness.Value);
                    if (bs[i].Value > avg * 2 / 3)
                        bs[i].Valid = true;
                    else
                        continue;
                }
            }

            return bs.Where(m => m.Valid);
        }
    }
}