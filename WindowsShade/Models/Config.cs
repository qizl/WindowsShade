using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Com.EnjoyCodes.SharpSerializer;
using Com.EnjoyCodes.SharpSerializer.Advanced.Serializing;

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
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; }

        public Config() { }

        /// <summary/>
        /// <param name="isFirstConfig">是否第一次加载配置</param>
        public Config(bool isFirstConfig) { IsFirtConfig = isFirstConfig; }

        /// <summary>
        /// 加载配置并补齐缺失的屏幕配置。
        /// </summary>
        public static Config Load(string path)
        {
            try
            {
                var settings = new SharpSerializerXmlSettings();
                settings.AdvancedSettings.TypeNameConverter = new ConfigTypeNameConverter();
                var serializer = new SharpSerializer(settings);
                var config = serializer.Deserialize(path) as Config;
                if (config == null)
                    return null;

                // 逐项补齐空配置，保留其他屏幕的配置索引。
                for (int i = 0; i < config.Monitors.Count; i++)
                {
                    if (config.Monitors[i] == null)
                        config.Monitors[i] = new Monitor();
                }
                return config;
            }
            catch
            {
                return null;
            }
        }

        public bool Save() => this.Save(Common.ConfigPath);

        /// <summary>
        /// 保存配置，写入完成后再替换原文件。
        /// </summary>
        public bool Save(string path)
        {
            string temporaryPath = null;
            try
            {
                path = Path.GetFullPath(path);
                var directory = Path.GetDirectoryName(path);
                Directory.CreateDirectory(directory);

                // 临时文件与目标文件同目录，确保替换时位于同一卷。
                var pendingPath = Path.Combine(directory, Path.GetRandomFileName());
                using (var stream = new FileStream(pendingPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    temporaryPath = pendingPath;
                    var serializer = new SharpSerializer();
                    serializer.Serialize(this, stream);
                }

                if (File.Exists(path))
                    File.Replace(temporaryPath, path, null);
                else
                    File.Move(temporaryPath, path);
                temporaryPath = null;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (temporaryPath != null)
                {
                    try
                    {
                        File.Delete(temporaryPath);
                    }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
            }
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

        private sealed class ConfigTypeNameConverter : ITypeNameConverter
        {
            private static readonly Regex AssemblyMetadata = new Regex(
                @", (?:Version=[0-9]+(?:\.[0-9]+){3}|Culture=[\w-]+|PublicKeyToken=\w+)(?=,|\]|$)",
                RegexOptions.CultureInvariant);

            private static readonly Dictionary<string, Type> AllowedTypes = createAllowedTypes();

            public string ConvertToTypeName(Type type) => type?.AssemblyQualifiedName ?? string.Empty;

            public Type ConvertToType(string typeName)
            {
                if (string.IsNullOrEmpty(typeName))
                    return null;

                // 直接映射配置类型，兼容旧版本记录的程序集版本号。
                var name = AssemblyMetadata.Replace(typeName, string.Empty);
                if (AllowedTypes.TryGetValue(name, out var type))
                    return type;

                throw new InvalidDataException("配置包含不支持的类型。");
            }

            private static Dictionary<string, Type> createAllowedTypes()
            {
                var allowed = new Dictionary<string, Type>(StringComparer.Ordinal);
                var types = new[]
                {
                    typeof(Config), typeof(Monitor), typeof(List<Monitor>),
                    typeof(bool), typeof(byte), typeof(int), typeof(DateTime), typeof(string)
                };
                foreach (var type in types)
                {
                    allowed.Add(AssemblyMetadata.Replace(type.FullName, string.Empty), type);
                    allowed.Add(AssemblyMetadata.Replace(type.AssemblyQualifiedName, string.Empty), type);
                }
                return allowed;
            }
        }
    }
}
