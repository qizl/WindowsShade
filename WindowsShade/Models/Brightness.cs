using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Com.EnjoyCodes.SharpSerializer;

namespace WindowsShade.Models
{
    /// <summary>
    /// 屏幕亮度对象
    /// </summary>
    public class Brightness
    {
        /// <summary>
        /// 0~255
        /// </summary>
        public byte Value { get; private set; }
        /// <summary>
        /// 天气
        ///     光照因素
        /// </summary>
        //public int Weather { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;

        private static List<Brightness> tmp = new List<Brightness>();

        public Brightness(byte value) => this.Value = value;

        public Brightness() { }

        public static void Save(byte value, bool saveImmediately = false) => new Brightness(value).save(Common.GetBrightnessFileName(), saveImmediately);

        private void save(string path, bool saveImmediately = false)
        {
            /*
             * 数据有效验证：
             *  1.tmp为空
             *  2.时间间隔大于30s
             */
            if (tmp.Count == 0 || (DateTime.Now - tmp.Last().Time).TotalSeconds > 30)
                Brightness.tmp.Add(this);
            else
                tmp.Last().Value = this.Value;

            /*
             * 保存验证
             *  1.saveImmediately is true
             *  2.缓存条数超过100条
             *  3.保存间隔超过30min
             */
            if (saveImmediately || tmp.Count >= 100 || (DateTime.Now - tmp.First().Time).TotalMinutes >= 30)
            {
                var serializer = new SharpSerializer();

                var brightness = File.Exists(path) ? (serializer.Deserialize(path) as List<Brightness>) : new List<Brightness>();
                if (brightness == null)
                    brightness = new List<Brightness>();
                brightness.AddRange(Brightness.tmp);

                serializer.Serialize(brightness, path);

                tmp.Clear();
                brightness.Clear();
            }
        }
    }

    /// <summary>
    /// 亮度数据分析对象
    /// </summary>
    public class BrightnessWithStatus : Brightness
    {
        public bool Valid { get; set; }
        public int Weight { get; set; }

        public BrightnessWithStatus(DateTime time, byte value) : base(value)
        {
            this.Time = time;
        }
    }

    /// <summary>
    /// ML保存数据对象
    /// </summary>
    public class BrightnessData
    {
        /// <summary>
        /// ML输入格式HHmsss，输出HHm0
        /// </summary>
        public string Time { get; set; }
        public byte Value { get; set; }

        /// <summary>
        /// 加载原始亮度数据
        /// </summary>
        /// <param name="fileFolders">原始数据文件夹</param>
        /// <returns>经过过滤的有效数据</returns>
        public IEnumerable<BrightnessData> Load(string fileFolders = null)
        {
            // 0.原始亮度数据文件
            var files = Directory.GetFiles(fileFolders ?? Common.BrightnessFolder);

            // 1.加载日志数据
            var brightnesses = new List<BrightnessWithStatus>();
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
                .Select(m => new BrightnessData() { Time = m.Time.ToString("HHmmss"), Value = m.Value })
                .OrderBy(m => m.Time);

            return d;
        }

        /// <summary>
        /// 训练数据
        /// </summary>
        /// <param name="src">经过过滤的原始数据</param>
        /// <returns>24小时的亮度数据</returns>
        public List<BrightnessData> Train(IEnumerable<BrightnessData> src) => new BrightnessTrain().GetBrightnessData(src).Result;

        /// <summary>
        /// 原始数据分析
        /// </summary>
        /// <param name="brightnesses">一天的原始数据</param>
        /// <returns></returns>
        private IEnumerable<BrightnessWithStatus> analyzePerDay(IEnumerable<Brightness> brightnesses)
        {
            var bs = brightnesses
                .Where(m => m.Value != 0)
                .Distinct(new BrightnessCompare())
                .Select(m => new BrightnessWithStatus(m.Time, m.Value))
                .OrderBy(m => m.Time)
                .ToArray();

            for (int i = 1; i < bs.Length; i++)
            {
                //bs[i].Valid = false;
                // 忽略第一条数据

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
        /// <summary>
        /// 原始数据分析
        /// </summary>
        /// <param name="brightnesses">经过逐天处理的所有数据</param>
        /// <returns></returns>
        private IEnumerable<BrightnessWithStatus> analyzeAll(IEnumerable<BrightnessWithStatus> brightnesses)
        {
            var bs = brightnesses
                .Distinct(new BrightnessWithStatusCompare())
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