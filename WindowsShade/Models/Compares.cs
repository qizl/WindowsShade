using System.Collections.Generic;

namespace WindowsShade.Models
{
    public class BrightnessDataCompare : IEqualityComparer<BrightnessData>
    {
        public bool Equals(BrightnessData x, BrightnessData y) => x.Time == y.Time && x.Value == y.Value;

        public int GetHashCode(BrightnessData obj) => obj == null ? 0 : obj.ToString().GetHashCode();
    }

    public class BrightnessCompare : IEqualityComparer<Brightness>
    {
        public bool Equals(Brightness x, Brightness y) => x.Time == y.Time && x.Value == y.Value;

        public int GetHashCode(Brightness obj) => obj == null ? 0 : obj.ToString().GetHashCode();
    }

    public class BrightnessWithStatusCompare : IEqualityComparer<BrightnessWithStatus>
    {
        public bool Equals(BrightnessWithStatus x, BrightnessWithStatus y) => x.Time == y.Time && x.Value == y.Value;

        public int GetHashCode(BrightnessWithStatus obj) => obj == null ? 0 : obj.ToString().GetHashCode();
    }
}
