using System;
using Com.EnjoyCodes.SharpSerializer;

namespace WindowsShade.Models
{
    /// <summary>
    /// 显示器
    /// </summary>
    public class Monitor
    {
        private byte _alpha = 128;
        private int? _globalAlphaOffset;

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
        public byte Alpha
        {
            get => this._alpha;
            set
            {
                this._alpha = value;
                this._globalAlphaOffset = null;
            }
        }

        /// <summary>
        /// 随总亮度调整透明度，并保留逐屏偏移以便从上下限恢复。
        /// </summary>
        internal void ApplyGlobalAlpha(byte previous, byte current)
        {
            var offset = this._globalAlphaOffset ?? (this._alpha - previous);
            this._globalAlphaOffset = offset;
            this._alpha = (byte)Math.Max(byte.MinValue, Math.Min(byte.MaxValue, current + offset));
        }
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
