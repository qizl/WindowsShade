using System;
using System.Collections.Generic;

namespace WindowsShade.Models
{
    /// <summary>
    /// 按设备名匹配显示器配置。
    /// </summary>
    internal sealed class MonitorConfiguration
    {
        private bool _legacyOrderBound;

        /// <summary>
        /// 将已连接屏幕按当前顺序放在列表前部，并保留后续可能重连的配置。
        /// </summary>
        public void Reconcile(List<Monitor> monitors, IEnumerable<string> deviceNames, byte defaultAlpha)
        {
            var byName = new Dictionary<string, Monitor>(StringComparer.OrdinalIgnoreCase);
            var legacyMonitors = new Queue<Monitor>();
            foreach (var monitor in monitors)
            {
                if (string.IsNullOrEmpty(monitor.DeviceName))
                    legacyMonitors.Enqueue(monitor);
                else
                    byName[monitor.DeviceName] = monitor;
            }

            var ordered = new List<Monitor>(monitors.Count);
            var connected = new HashSet<Monitor>();
            foreach (var deviceName in deviceNames)
            {
                if (!byName.TryGetValue(deviceName, out var monitor))
                {
                    // 旧配置缺少设备名，启动时按原顺序绑定尚未匹配的屏幕。
                    monitor = !this._legacyOrderBound && legacyMonitors.Count > 0
                        ? legacyMonitors.Dequeue()
                        : new Monitor { Alpha = defaultAlpha };
                    monitor.DeviceName = deviceName;
                }

                ordered.Add(monitor);
                connected.Add(monitor);
            }

            foreach (var monitor in monitors)
            {
                if (!connected.Contains(monitor))
                    ordered.Add(monitor);
            }

            this._legacyOrderBound |= connected.Count > 0;
            monitors.Clear();
            monitors.AddRange(ordered);
        }
    }
}
