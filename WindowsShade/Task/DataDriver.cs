using System;
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

        private Thread _tdMain;

        private bool _isRunning = false;

        private int _interval = 1000;

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

            while (this._isRunning)
            {
                Thread.Sleep(this._interval);

                // 1.更新亮度
                if ((DateTime.Now - lastGetNewBrightnessTime).TotalMinutes >= Common.Config.AutoAdjustInterval)
                {
                    lastGetNewBrightnessTime = DateTime.Now;

                    this.AdjustBrightness?.Invoke(this, new AdjustBrightnessEventArgs()
                    {
                        Alpha = this.getNewBrightness(),
                        Time = DateTime.Now
                    });
                }

                // 2.生成数据
                if ((DateTime.Now - lastGenerateDataTime).TotalDays >= Common.Config.GenerateDataInterval)
                {
                    lastGenerateDataTime = DateTime.Now;

                    //var path = this.generateData();
                    //Common.Config.BrightnessDataPath = path;
                    //Common.Config.LastGenerateDataTime = DateTime.Now;
                    //Common.Config.Save();
                }
            }
        }

        /// <summary>
        /// 获取新的屏幕亮度数据
        /// </summary>
        /// <returns></returns>
        private byte getNewBrightness()
        {
            return 10;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 生成屏幕亮度数据
        /// </summary>
        /// <returns></returns>
        private string generateData()
        {
            return string.Empty;
            //throw new NotImplementedException();
        }
    }
}
