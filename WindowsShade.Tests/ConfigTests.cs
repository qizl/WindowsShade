using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsShade.Models;

namespace WindowsShade.Tests
{
    [TestClass]
    public class ConfigTests
    {
        private string _directory;
        private string _configPath;

        [TestInitialize]
        public void CreateTestDirectory()
        {
            this._directory = Path.Combine(Path.GetTempPath(), "WindowsShade.Tests-" + Guid.NewGuid().ToString("N"));
            this._configPath = Path.Combine(this._directory, "Config.xml");
        }

        [TestCleanup]
        public void RemoveTestDirectory()
        {
            if (!Directory.Exists(this._directory))
                return;

            foreach (var file in Directory.GetFiles(this._directory))
                File.Delete(file);
            Directory.Delete(this._directory);
        }

        [TestMethod]
        public void SaveReplacesExistingConfigAndLoadPreservesMonitorPositions()
        {
            Assert.IsTrue(new Config { Alpha = 200 }.Save(this._configPath));
            var config = new Config { Alpha = 50, AutoHidden = true };
            config.Monitors.Add(null);
            config.Monitors.Add(new Monitor { DeviceName = @"\\.\DISPLAY1", Enabled = true, Alpha = 80 });
            Assert.IsTrue(config.Save(this._configPath));

            var loaded = Config.Load(this._configPath);
            Assert.IsNotNull(loaded);
            Assert.AreEqual((byte)50, loaded.Alpha);
            Assert.IsTrue(loaded.AutoHidden);
            Assert.AreEqual(2, loaded.Monitors.Count);
            Assert.IsFalse(loaded.Monitors[0].Enabled);
            Assert.IsTrue(loaded.Monitors[1].Enabled);
            Assert.AreEqual((byte)80, loaded.Monitors[1].Alpha);
            Assert.AreEqual(@"\\.\DISPLAY1", loaded.Monitors[1].DeviceName);
        }

        [TestMethod]
        public void LockedDestinationKeepsOriginalConfig()
        {
            Assert.IsTrue(new Config { Alpha = 200 }.Save(this._configPath));
            using (File.Open(this._configPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Assert.IsFalse(new Config { Alpha = 50 }.Save(this._configPath));
                Assert.AreEqual((byte)200, Config.Load(this._configPath).Alpha);
            }
            CollectionAssert.AreEqual(new[] { this._configPath }, Directory.GetFiles(this._directory));
        }

        [TestMethod]
        public void LoadsOlderAssemblyVersionAndRejectsUnexpectedTypes()
        {
            Assert.IsTrue(new Config { Alpha = 73 }.Save(this._configPath));
            var xml = File.ReadAllText(this._configPath);
            var legacyXml = Regex.Replace(xml, @"WindowsShade, Version=[\d.]+", "WindowsShade, Version=1.0.1.1");
            Assert.AreNotEqual(xml, legacyXml);
            File.WriteAllText(this._configPath, legacyXml);
            Assert.AreEqual((byte)73, Config.Load(this._configPath).Alpha);

            var unexpectedTypeXml = legacyXml.Replace("name=\"Alpha\"", "name=\"Alpha\" type=\"System.UInt64, mscorlib\"");
            Assert.AreNotEqual(legacyXml, unexpectedTypeXml);
            File.WriteAllText(this._configPath, unexpectedTypeXml);
            Assert.IsNull(Config.Load(this._configPath));
        }
    }
}
