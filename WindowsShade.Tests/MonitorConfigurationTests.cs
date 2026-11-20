using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsShade.Models;

namespace WindowsShade.Tests
{
    [TestClass]
    public class MonitorConfigurationTests
    {
        [TestMethod]
        public void ReconcileBindsLegacyScreensByPositionAndRetainsExtraRows()
        {
            var left = new Monitor { Enabled = true, Alpha = 200 };
            var right = new Monitor { Alpha = 50 };
            var detached = new Monitor { Alpha = 100 };
            var monitors = new List<Monitor> { left, right, detached };
            var configuration = new MonitorConfiguration();

            configuration.Reconcile(monitors, new[] { "DISPLAY2", "DISPLAY1" }, 128);

            CollectionAssert.AreEqual(new[] { left, right, detached }, monitors);
        }

        [TestMethod]
        public void ReconcileKeepsSettingsWithDevicesWhenReordered()
        {
            var first = new Monitor { Enabled = true, Alpha = 200 };
            var second = new Monitor { Alpha = 50 };
            var monitors = new List<Monitor> { first, second };
            var configuration = new MonitorConfiguration();
            configuration.Reconcile(monitors, new[] { "DISPLAY1", "DISPLAY2" }, 128);

            configuration.Reconcile(monitors, new[] { "display2", "DISPLAY1" }, 128);

            CollectionAssert.AreEqual(new[] { second, first }, monitors);
            Assert.IsFalse(monitors[0].Enabled);
            Assert.AreEqual((byte)200, monitors[1].Alpha);
        }

        [TestMethod]
        public void ReconcileRestoresDisconnectedScreensAndDefaultsNewScreens()
        {
            var first = new Monitor { Enabled = true, Alpha = 200 };
            var second = new Monitor { Alpha = 50 };
            var monitors = new List<Monitor> { first, second };
            var configuration = new MonitorConfiguration();
            configuration.Reconcile(monitors, new[] { "DISPLAY1", "DISPLAY2" }, 128);

            configuration.Reconcile(monitors, new[] { "DISPLAY2" }, 128);
            CollectionAssert.AreEqual(new[] { second, first }, monitors);

            configuration.Reconcile(monitors, new[] { "DISPLAY3", "DISPLAY2" }, 160);
            var newMonitor = monitors[0];
            Assert.IsFalse(newMonitor.Enabled);
            Assert.AreEqual((byte)160, newMonitor.Alpha);
            CollectionAssert.AreEqual(new[] { newMonitor, second, first }, monitors);

            configuration.Reconcile(monitors, new[] { "DISPLAY1", "DISPLAY3", "DISPLAY2" }, 140);
            CollectionAssert.AreEqual(new[] { first, newMonitor, second }, monitors);
            Assert.IsTrue(monitors[0].Enabled);
            Assert.AreEqual((byte)200, monitors[0].Alpha);
        }

        [TestMethod]
        public void ReconcileUsesSavedDeviceNamesAfterRestart()
        {
            var disconnected = new Monitor { DeviceName = "DISPLAY1", Enabled = true, Alpha = 200 };
            var connected = new Monitor { DeviceName = "DISPLAY2", Alpha = 50 };
            var monitors = new List<Monitor> { disconnected, connected };

            new MonitorConfiguration().Reconcile(monitors, new[] { "DISPLAY2" }, 128);

            CollectionAssert.AreEqual(new[] { connected, disconnected }, monitors);
            Assert.IsFalse(monitors[0].Enabled);
            Assert.IsTrue(monitors[1].Enabled);
        }
    }
}
