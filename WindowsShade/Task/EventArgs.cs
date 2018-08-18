using System;
using System.Collections.Generic;
using WindowsShade.Models;

namespace WindowsShade.Task
{
    public class AdjustBrightnessEventArgs : EventArgs
    {
        public byte Alpha { get; set; }
        public DateTime Time { get; set; }
    }

    public class GenerateBrightnessEventArgs : EventArgs
    {
        public List<BrightnessData> Datas { get; set; }
        public DateTime Time { get; set; }
    }
}
