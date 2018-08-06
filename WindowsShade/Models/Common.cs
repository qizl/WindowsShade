using System;
using System.IO;

namespace WindowsShade.Models
{
    public class Common
    {
        public static Config Config { get; set; }
        public static string ConfigPath = Path.Combine(Environment.CurrentDirectory, "Config.xml");
        public static string BrightnessFolder = Path.Combine(Environment.CurrentDirectory, "Brightness");
        public static string GetBrightnessFileName() => Path.Combine(BrightnessFolder, DateTime.Now.ToString("yyyyMMdd") + ".xml");
    }
}
