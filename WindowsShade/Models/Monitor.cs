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
