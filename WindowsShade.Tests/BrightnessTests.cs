using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsShade.Models;

namespace WindowsShade.Tests
{
    [TestClass]
    public class BrightnessTests
    {
        [TestMethod]
        public void GlobalAlphaRestoresMonitorOffsetAfterClipping()
        {
            var bright = new Monitor { Alpha = 240 };
            var dim = new Monitor { Alpha = 10 };

            bright.ApplyGlobalAlpha(128, 150);
            dim.ApplyGlobalAlpha(128, 100);

            Assert.AreEqual(byte.MaxValue, bright.Alpha);
            Assert.AreEqual(byte.MinValue, dim.Alpha);

            bright.ApplyGlobalAlpha(150, 128);
            dim.ApplyGlobalAlpha(100, 128);

            Assert.AreEqual((byte)240, bright.Alpha);
            Assert.AreEqual((byte)10, dim.Alpha);
        }

        [TestMethod]
        public void ManualAlphaReplacesOffset()
        {
            var monitor = new Monitor { Alpha = 240 };
            monitor.ApplyGlobalAlpha(128, 150);
            monitor.Alpha = 180;

            monitor.ApplyGlobalAlpha(150, 128);

            Assert.AreEqual((byte)158, monitor.Alpha);
        }

        [TestMethod]
        public void SystemBrightnessUsesNearestSupportedLevel()
        {
            Assert.AreEqual(0, ScreenBrightness.FindNearestLevel(new byte[] { 50 }, 60));
            Assert.AreEqual(1, ScreenBrightness.FindNearestLevel(new byte[] { 10, 30, 90 }, 35));
            Assert.AreEqual(2, ScreenBrightness.FindNearestLevel(new byte[] { 10, 30, 90 }, 90));
        }
    }
}
