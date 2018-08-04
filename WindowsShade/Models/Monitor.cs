namespace WindowsShade.Models
{
    /// <summary>
    /// 显示器
    /// </summary>
    public class Monitor
    {
        public byte No { get; set; }
        public Resolution Resolution { get; set; } = new Resolution(1920, 1080);
        public bool Enabled { get; set; }
        /// <summary>
        /// 是否主显
        /// </summary>
        public bool Primary { get; set; }

        public Monitor() { }

        public Monitor(byte no, int resolutionX, int resolutionY, bool enabled = true, bool isMain = false)
        {
            this.No = no;
            this.Resolution = new Resolution(resolutionX, resolutionY);
            this.Enabled = enabled;
            this.Primary = isMain;
        }
    }
}
