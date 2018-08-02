namespace WindowsShade.Models
{
    /// <summary>
    /// 分辨率
    /// </summary>
    public class Resolution
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Resolution(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
