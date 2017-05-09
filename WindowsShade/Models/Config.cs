using System;
using Com.EnjoyCodes.SharpSerializer;

namespace WindowsShade.Models
{
    public class Config
    {
        public ShadeTypes ShadeType { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public static Config Load(string path)
        {
            Config config = null;
            try
            {
                SharpSerializer serializer = new SharpSerializer();
                config = serializer.Deserialize(path) as Config;
            }
            catch { }
            return config;
        }

        public bool Save() { return this.Save(Common.ConfigPath); }
        public bool Save(string path)
        {
            try
            {
                SharpSerializer serializer = new SharpSerializer();
                serializer.Serialize(this, path);
            }
            catch { return false; }
            return true;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}