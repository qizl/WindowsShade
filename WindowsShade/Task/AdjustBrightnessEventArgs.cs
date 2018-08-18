using System;

namespace WindowsShade.Task
{
    public class AdjustBrightnessEventArgs : EventArgs
    {
        public byte Alpha { get; set; }
        public DateTime Time { get; set; }
    }
}
