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
        [ExcludeFromSerialization]
        public Resolution Resolution { get; set; }
        /// <summary>
        /// 是否主显
        /// </summary>
        [ExcludeFromSerialization]
        public bool Primary { get; set; }

        public Monitor() { }

        public Monitor(int no, int resolutionX, int resolutionY, bool enabled = true, bool primary = false)
        {
            this.No = no;
            this.Resolution = new Resolution(resolutionX, resolutionY);
            this.Enabled = enabled;
            this.Primary = primary;
        }
    }
}
