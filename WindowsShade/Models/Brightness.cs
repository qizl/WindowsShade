using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Com.EnjoyCodes.SharpSerializer;

namespace WindowsShade.Models
{
    /// <summary>
    /// 屏幕亮度
    /// </summary>
    public class Brightness
    {
        /// <summary>
        /// 0~255
        /// </summary>
        public byte Value { get; private set; }
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
}