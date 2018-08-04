using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Com.EnjoyCodes.SharpSerializer;

namespace WindowsShade.Models
{
    public class Config
    {
        /// <summary>
        /// 屏幕分辨率
        /// </summary>
        public Resolution[] Resolutions { get; set; } = new Resolution[] {
            new Resolution(1920,1080),
            new Resolution(1600,900),
            new Resolution(1440,900),
            new Resolution(1366,768)
        };
        /// <summary>
        /// 显示器配置
        /// </summary>
        public List<Monitor> Monitors { get; set; }
        /// <summary>
        /// 透明度
        ///     0~255
        /// </summary>
        public byte Alpha { get; set; } = 128;
        /// <summary>
        /// 软件启动自动调整亮度
        /// </summary>
        public bool AutoShowShade { get; set; }
        /// <summary>
        /// 软件启动自动隐藏主窗体
        /// </summary>
        public bool AutoHidden { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; }

        public Config()
        {
            // 获取显示器配置
            this.Monitors = (
                from screen in Screen.AllScreens
                select new Monitor(
                    (byte)Array.IndexOf(Screen.AllScreens, screen),
                    screen.Bounds.Width,
                    screen.Bounds.Height,
                    true,
                    screen.Primary)
                ).ToList();
        }

        public static Config Load(string path)
        {
            Config config = null;
            try
            {
                var serializer = new SharpSerializer();
                config = serializer.Deserialize(path) as Config;
            }
            catch (Exception e) { }
            return config;
        }

        public bool Save() => this.Save(Common.ConfigPath);
        public bool Save(string path)
        {
            try
            {
                var serializer = new SharpSerializer();
                serializer.Serialize(this, path);
            }
            catch { return false; }
            return true;
        }

        public object Clone() => this.MemberwiseClone();
    }
}