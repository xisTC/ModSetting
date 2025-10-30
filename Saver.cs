using System;
using System.Collections.Generic;
using System.IO;
using ModSetting.Config;
using ModSetting.Config.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ModSetting {
    public class Saver {
         private static readonly JsonSerializer jsonSerializer = new JsonSerializer() {
            Formatting = Formatting.Indented,
            Converters = { new ConfigDataReadConverter() }
        };
        private static readonly Dictionary<string, ModConfigData> saveConfigs = new Dictionary<string, ModConfigData>();

        private const string CONFIG_FILE_NAME = "config.json";

        public static void Load() {
            string configPath = GetConfigPath();
            Debug.Log("加载配置文件:" + configPath);
            if (File.Exists(configPath)) {
                string json = File.ReadAllText(configPath);
                ConfigData data;
                try {
                    data = jsonSerializer.Deserialize<ConfigData>(new JsonTextReader(new StringReader(json)));
                } catch (Exception e) {
                    Debug.Log("异常:"+e);
                    throw;
                }
                data.Init();
                List<ModConfigData> modConfigDatas = data.configDatas;
                if (modConfigDatas!=null&& modConfigDatas.Count!=0) {
                    foreach (ModConfigData modConfigData in modConfigDatas) {
                        if (saveConfigs.TryGetValue(modConfigData.modId, out var configData)) {
                            Debug.LogError("配置文件出现相同modId，异常");
                        } else {
                            saveConfigs.Add(modConfigData.modId,modConfigData);
                        }
                    }
                    return;
                }
            }
            // 默认设置
            Debug.Log("未找到配置文件/文件为空");
        }

        public static void Save() {
            CreateConfigFile();
        }

        private static void CreateConfigFile() {
            string configPath = GetConfigPath();
            Debug.Log("创建配置文件:" + configPath);
            List<ModConfigData> modConfigDatas = new List<ModConfigData>();
            foreach (ModConfig modConfig in ConfigManager.GetConfigs()) {
                ModConfigData modConfigData =
                    new ModConfigData(modConfig.ModInfo.GetModId(), modConfig.GetConfigDatas());
                modConfigDatas.Add(modConfigData);
            }
            ConfigData configData = new ConfigData(modConfigDatas);
            string directory = Path.GetDirectoryName(configPath);
            if (directory == null) {
                Debug.LogError("directory不能为null");
                return;
            }
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            StringWriter stringWriter = new StringWriter();
            jsonSerializer.Serialize(stringWriter, configData);
            string json = stringWriter.ToString();
            File.WriteAllText(configPath, json);
            Debug.Log("创建完成");
        }

        private static string GetConfigPath() {
            string assemblyLocation = typeof(ModBehaviour).Assembly.Location;
            string directory = Path.GetDirectoryName(assemblyLocation);
            if (string.IsNullOrEmpty(directory)) {
                directory = AppContext.BaseDirectory ?? ".";
            }
            return Path.Combine(directory, CONFIG_FILE_NAME);
        }

        public static void Clear() => saveConfigs.Clear();

        public static bool HasValue(string modId,string key) {
            return saveConfigs.ContainsKey(modId)&& saveConfigs[modId].HasValue(key);
        }

        public static T GetValue<T>(string modId,string key) {
            if (HasValue(modId,key)) {
                return saveConfigs[modId].GetValue<T>(key);
            }
            return default;
        }
    }
}