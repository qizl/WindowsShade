using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using WindowsShade.Models;

namespace WindowsShade.Task
{
    /// <summary>
    /// 数据驱动
    ///     1.定期更新亮度
    ///     2.定期分析亮度数据，根据用户习惯生成亮度数据
    /// </summary>
    public class DataDriver
    {
        public event AdjustBrightnessHandler AdjustBrightness;
        public event GenerateBrightnessHandler BrightnessGenerated;

        private Thread _tdMain;

        private bool _isRunning = false;

        private int _interval = 1000;

        private BrightnessTrain _brightnessTrain = new BrightnessTrain();

        public void Start()
        {
            if (!this._isRunning)
            {
                this._isRunning = true;
                this._tdMain = new Thread(this.mainThread);
                this._tdMain.Start();
            }
        }

        public void Stop()
        {
            if (this._isRunning)
            {
                this._isRunning = false;
                this._tdMain.Abort();
            }
        }

        private void mainThread()
        {
            var lastGetNewBrightnessTime = new DateTime(); // 上次获取屏幕亮度时间
            var lastGenerateDataTime = Common.Config.LastGenerateDataTime; // 上次自动生成屏幕亮度数据时间
            var lastConnectServerTime = new DateTime(); // 上次服务端离线后的连接时间

            while (this._isRunning)
            {
                Thread.Sleep(this._interval);

                var now = DateTime.Now;

                // 0.判断服务端是否上线
                if (!Common.IsServerOnline && (now - lastConnectServerTime).TotalMinutes >= Common.Config.ReconnectInterval)
                {
                    lastConnectServerTime = now;

                    Common.IsServerOnline = this._brightnessTrain.HealthCheck().Result;
                }

                // 1.更新亮度
                if (Common.Config.AutoAdjust)
                    if ((now - lastGetNewBrightnessTime).TotalMinutes >= Common.Config.AutoAdjustInterval)
                    {
                        lastGetNewBrightnessTime = now;

                        this.AdjustBrightness?.Invoke(this, new AdjustBrightnessEventArgs()
                        {
                            Alpha = this.getNewBrightness(now.ToString("HHmm").Substring(0, 3).PadRight(4, '0')),
                            Time = now
                        });
                    }

                // 2.生成数据
                if (Common.IsServerOnline)
                    if ((now - lastGenerateDataTime).TotalDays >= Common.Config.GenerateDataInterval)
                    {
                        lastGenerateDataTime = now;

                        this.BrightnessGenerated?.Invoke(this, new GenerateBrightnessEventArgs()
                        {
                            Datas = this.generateData(),
                            Time = now
                        });
                    }
            }
        }

        /// <summary>
        /// 获取新的屏幕亮度数据
        /// </summary>
        /// <param name="time">HHm0</param>
        /// <returns></returns>
        private byte getNewBrightness(string time)
        {
            if (Common.BrightnessDatas != null)
            {
                var b = Common.BrightnessDatas.FirstOrDefault(m => m.Time == time);
                if (b != null)
                    return b.Value;
            }
            return 0;
        }

        /// <summary>
        /// 生成屏幕亮度数据
        /// </summary>
        /// <returns></returns>
        private List<BrightnessData> generateData()
        {
            var brightnessData = new BrightnessData();
            var src = brightnessData.Load();
            return brightnessData.Train(src);
        }
    }
}
