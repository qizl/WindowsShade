using System;
using System.IO;

namespace WindowsShade.Models
{
    public class Common
    {
        public static Config DefaultConfig = new Config()
        {
            ShadeType = ShadeTypes.D1920R,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now
        };
        public static Config Config { get; set; }
        public static string ConfigPath = Path.Combine(Environment.CurrentDirectory, "Config.xml");
    }
}
