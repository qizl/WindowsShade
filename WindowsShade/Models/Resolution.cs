namespace WindowsShade.Models
{
    /// <summary>
    /// 分辨率
    /// </summary>
    public class Resolution
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Resolution() { }

        public Resolution(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
