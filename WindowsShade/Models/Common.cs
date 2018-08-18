using System;
using System.Collections.Generic;
using System.IO;

namespace WindowsShade.Models
{
    public class Common
    {
        public static Config Config { get; set; }
        public static string ConfigPath = Path.Combine(Environment.CurrentDirectory, "Config.xml");
        /// <summary>
        /// 原始亮度数据文件夹
        /// </summary>
        public static string BrightnessFolder = Path.Combine(Environment.CurrentDirectory, "Brightness");
        public static string GetBrightnessFileName() => Path.Combine(BrightnessFolder, $"{DateTime.Now.ToString("yyyyMMdd")}.xml");
        /// <summary>
        /// ML生成数据文件夹
        /// </summary>
        public static string BrightnessTrainedFolder = Path.Combine(Environment.CurrentDirectory, "BrightnessTrained");
        public static string GetBrightnessTrainedFileName() => Path.Combine(BrightnessTrainedFolder, $"{DateTime.Now.ToString("yyyyMMdd")}.xml");

        /// <summary>
        /// 亮度数据
        /// </summary>
        public static List<BrightnessData> BrightnessDatas { get; set; }

        public static bool IsServerOnline { get; set; }
    }
}
