using System;
using System.Linq;

namespace WindowsShade.Models
{
    /// <summary>
    /// 系统屏幕亮度调节
    ///     参考：https://www.codeproject.com/Articles/236898/Screen-Brightness-Control-for-Laptops-and-Tablets
    /// </summary>
    public class ScreenBrightness
    {
        /// <summary>
        /// array of valid level values
        /// </summary>
        private byte[] _bLevels;

        public int Maximum { get; set; }

        public ScreenBrightness() => this.Initiazlie();

        public bool Initiazlie()
        {
            this._bLevels = this.getBrightnessLevels();
            if (this._bLevels.Count() == 0)
            {
                //throw new Exception($"\"WmiMonitorBrightness\" is not supported by the system");
                return false;
            }

            this.Maximum = this._bLevels.Count() - 1;
            return true;
        }

        /// <summary>
        /// array of valid brightness values in percent
        /// </summary>
        /// <returns></returns>
        private byte[] getBrightnessLevels()
        {
            var s = new System.Management.ManagementScope("root\\WMI"); //define scope (namespace)     
            var q = new System.Management.SelectQuery("WmiMonitorBrightness"); // define query         
            var mos = new System.Management.ManagementObjectSearcher(s, q); // output current brightness

            var brightnessLevels = new byte[0];

            try
            {
                var moc = mos.Get();
                foreach (System.Management.ManagementObject o in moc)
                {
                    brightnessLevels = (byte[])o.GetPropertyValue("Level");
                    break; //only work on the first object
                }

                moc.Dispose();
                mos.Dispose();
            }
            catch { }

            return brightnessLevels;
        }

        /// <summary>
        /// get the actual percentage of brightness
        /// </summary>
        /// <returns></returns>
        public int GetBrightness()
        {
            var s = new System.Management.ManagementScope("root\\WMI"); //define scope (namespace)     
            var q = new System.Management.SelectQuery("WmiMonitorBrightness"); // define query         
            var mos = new System.Management.ManagementObjectSearcher(s, q); // output current brightness
            var moc = mos.Get();

            byte curBrightness = 0;
            foreach (System.Management.ManagementObject o in moc)
            {
                curBrightness = (byte)o.GetPropertyValue("CurrentBrightness");
                break; //only work on the first object
            }

            moc.Dispose();
            mos.Dispose();

            var i = Array.IndexOf(this._bLevels, curBrightness);
            return i < 0 ? 1 : i;
        }

        public void SetBrightness(int value)
        {
            byte targetBrightness = this._bLevels[value];

            var s = new System.Management.ManagementScope("root\\WMI"); //define scope (namespace)     
            var q = new System.Management.SelectQuery("WmiMonitorBrightnessMethods"); // define query         
            var mos = new System.Management.ManagementObjectSearcher(s, q); // output current brightness
            var moc = mos.Get();

            foreach (System.Management.ManagementObject o in moc)
            {
                o.InvokeMethod("WmiSetBrightness", new Object[] { UInt32.MaxValue, targetBrightness }); // note the reversed order - won't work otherwise!
                break; //only work on the first object
            }

            moc.Dispose();
            mos.Dispose();
        }
    }
}