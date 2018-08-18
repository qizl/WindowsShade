using System;
using System.IO;

namespace BrightnessTrainAPI.Models
{
    public class Common
    {
        public static string BrightnessTrainedFolder { get; set; }
        public static string GetBrightnessTrainedFileName() => Path.Combine(BrightnessTrainedFolder, $"{DateTime.Now.ToString("yyyyMMddss")}.txt");
    }
}
