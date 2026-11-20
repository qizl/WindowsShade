using System;
using Com.EnjoyCodes.SharpSerializer;

namespace WindowsShade.Models
{
    /// <summary>
    /// 显示器
    /// </summary>
    public class Monitor
    {
        [Obsolete]
        public int No { get; set; }
        public bool Enabled { get; set; }
        /// <summary>
        /// Windows 显示设备名。旧配置首次加载时按屏幕顺序补齐。
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 透明度
        ///     0~255
        /// </summary>
        public byte Alpha { get; set; } = 128;
        /// <summary>
        /// 是否主显
        /// </summary>
        [ExcludeFromSerialization]
        public bool Primary { get; set; }
        [ExcludeFromSerialization]
        public int Height { get; set; }
        [ExcludeFromSerialization]
        public int Width { get; set; }
        [ExcludeFromSerialization]
        public int Y { get; set; }
        [ExcludeFromSerialization]
        public int X { get; set; }

        public Monitor() { }

        public Monitor(int no, int width, int height, int x, int y, bool enabled = true, bool primary = false)
        {
            this.No = no;
            this.Width = width;
            this.Height = height;
            this.X = x;
            this.Y = y;
            this.Enabled = enabled;
            this.Primary = primary;
        }
    }
}
