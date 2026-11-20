using System;
using System.Management;
using System.Runtime.InteropServices;

namespace WindowsShade.Models
{
    /// <summary>
    /// 通过 WMI 读取和设置系统支持的屏幕亮度档位。
    /// </summary>
    internal sealed class ScreenBrightness
    {
        private byte[] _levels = Array.Empty<byte>();

        public int Maximum => this._levels.Length - 1;

        /// <summary>
        /// 一次查询读取亮度档位和当前值。硬件或驱动不支持时返回 false。
        /// </summary>
        public bool TryInitialize(out int brightness)
        {
            this._levels = Array.Empty<byte>();
            brightness = 0;
            try
            {
                using (var searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT Level, CurrentBrightness FROM WmiMonitorBrightness"))
                using (var monitors = searcher.Get())
                {
                    foreach (ManagementObject monitor in monitors)
                    {
                        using (monitor)
                        {
                            var levels = monitor["Level"] as byte[];
                            if (levels == null || levels.Length == 0 || !(monitor["CurrentBrightness"] is byte current))
                                continue;

                            this._levels = levels;
                            brightness = FindNearestLevel(levels, current);
                            return true;
                        }
                    }
                }
            }
            catch (Exception e) when (e is ManagementException || e is COMException || e is UnauthorizedAccessException)
            {
                return false;
            }
            return false;
        }

        internal static int FindNearestLevel(byte[] levels, byte current)
        {
            var nearest = 0;
            for (int i = 1; i < levels.Length; i++)
            {
                if (Math.Abs(levels[i] - current) < Math.Abs(levels[nearest] - current))
                    nearest = i;
            }
            return nearest;
        }

        /// <summary>
        /// 将系统亮度设置为给定档位。
        /// </summary>
        public void SetBrightness(int value)
        {
            byte target = this._levels[value];
            using (var searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM WmiMonitorBrightnessMethods"))
            using (var monitors = searcher.Get())
            {
                foreach (ManagementObject monitor in monitors)
                {
                    using (monitor)
                    {
                        using (var parameters = monitor.GetMethodParameters("WmiSetBrightness"))
                        {
                            parameters["Timeout"] = uint.MaxValue;
                            parameters["Brightness"] = target;
                            // 部分驱动在写入成功时返回 null，调用失败由 WMI 抛出异常。
                            monitor.InvokeMethod("WmiSetBrightness", parameters, null)?.Dispose();
                        }
                        return;
                    }
                }
            }
            throw new InvalidOperationException("找不到支持系统亮度调节的显示器。");
        }
    }
}
