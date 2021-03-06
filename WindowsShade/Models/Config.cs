﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Com.EnjoyCodes.SharpSerializer;

namespace WindowsShade.Models
{
    public class Config
    {
        /// <summary>
        /// 第一次启动，配置
        /// </summary>
        public readonly bool IsFirtConfig = false;

        /// <summary>
        /// 显示器配置
        /// </summary>
        public List<Monitor> Monitors { get; set; } = new List<Monitor>();
        /// <summary>
        /// 透明度
        ///     0~255
        /// </summary>
        public byte Alpha { get; set; } = 128;
        /// <summary>
        /// 软件启动自动调整亮度
        /// </summary>
        public bool AutoShowShade { get; set; }
        /// <summary>
        /// 软件启动自动隐藏主窗体
        /// </summary>
        public bool AutoHidden { get; set; }

        /// <summary>
        /// 自动调整亮度
        /// </summary>
        public bool AutoAdjust { get; set; } = true;
        /// <summary>
        /// 自动调整屏幕亮度间隔（min）
        /// </summary>
        public int AutoAdjustInterval { get; set; } = 10;
        /// <summary>
        /// 上次生成亮度数据时间
        /// </summary>
        public DateTime LastGenerateDataTime { get; set; } = new DateTime();
        /// <summary>
        /// 亮度数据生成间隔（day）
        /// </summary>
        public int GenerateDataInterval { get; set; } = 7;
        /// <summary>
        /// 亮度数据文件路径
        /// </summary>
        public string BrightnessDataPath { get; set; }

        public string ServerUrl { get; set; } = @"http://btapi.qizl.cn";
        /// <summary>
        /// 重连服务端间隔（min）
        /// </summary>
        public int ReconnectInterval { get; set; } = 30;

        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; }

        public Config() { }

        /// <summary/>
        /// <param name="isFirstConfig">是否第一次加载配置</param>
        public Config(bool isFirstConfig) { IsFirtConfig = true; }

        public static Config Load(string path)
        {
            Config config = null;
            try
            {
                var serializer = new SharpSerializer();
                config = serializer.Deserialize(path) as Config;
            }
            catch { }
            return config;
        }

        public bool Save() => this.Save(Common.ConfigPath);
        public bool Save(string path)
        {
            try
            {
                var serializer = new SharpSerializer();
                serializer.Serialize(this, path);
            }
            catch { return false; }
            return true;
        }

        public object Clone()
        {
            Object obj = null;
            using (var stream = new MemoryStream())
            {
                try
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    obj = formatter.Deserialize(stream);
                }
                catch { }
            }
            return obj;
        }
    }
}