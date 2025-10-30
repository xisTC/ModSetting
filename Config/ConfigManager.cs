using System;
using System.Collections.Generic;
using System.IO;
using Duckov.Modding;
using ModSetting.Config.Data;
using Newtonsoft.Json;
using UnityEngine;
//todo ulong需要更改，因为没有上架的id为0
// todo 增加保存系统，优先从UI获取数据，如果没有，然后从保存系统中获取
namespace ModSetting.Config {
    public static class ConfigManager {
        private static readonly Dictionary<string, ModConfig> configs = new Dictionary<string, ModConfig>();
        public static void AddConfig(ModInfo modInfo,IConfig config) {
            if (configs.TryGetValue(modInfo.GetModId(),out var modConfig)) {
                modConfig.AddConfig(config);
            } else {
                modConfig = new ModConfig(modInfo);
                modConfig.AddConfig(config);
                configs.Add(modInfo.GetModId(),modConfig);
            }
        }
        public static T GetValue<T>(ModInfo info,string key) {
            if (configs.TryGetValue(info.GetModId(),out var modConfig)) {
                return modConfig.GetValue<T>(key);
            } else {
                Debug.LogError($"找不到此{info.displayName}的值,key:"+key);
                return default;
            }
        }

        public static bool SetValue<T>(ModInfo info, string key,T value) {
            if (configs.TryGetValue(info.GetModId(),out var modConfig)) {
                return modConfig.SetValue(key,value);
            }
            Debug.LogError($"找不到此{info.displayName}的配置,无法设置值");
            return false;
        }

        public static bool RemoveUI(ModInfo info, string key) {
            if (configs.TryGetValue(info.GetModId(),out var modConfig)) {
                return modConfig.RemoveUI(key);
            }
            Debug.LogError($"找不到此{info.displayName}的UI,key:"+key);
            return false;
        }
        public static bool RemoveMod(ModInfo info) {
            if (configs.Remove(info.GetModId())) {
                return true;
            }
            Debug.LogError($"找不到此{info.displayName}的配置");
            return false;
        }

        public static void Clear() => configs.Clear();

        private static readonly JsonSerializer jsonSerializer = new JsonSerializer() {
            Formatting = Formatting.Indented
        };

        private const string CONFIG_FILE_NAME = "config.json";

        public static void Save() {
            CreateConfigFile();
        }

        private static void CreateConfigFile() {
            string configPath = GetConfigPath();
            Debug.Log("创建配置文件:" + configPath);
            List<ModConfigData> modConfigDatas = new List<ModConfigData>();
            foreach (ModConfig modConfig in configs.Values) {
                ModConfigData modConfigData = new ModConfigData(modConfig.ModInfo.displayName,
                    modConfig.ModInfo.publishedFileId,
                    modConfig.GetDropDownConfigDatas(),
                    modConfig.GetSliderConfigDatas(),
                    modConfig.GetToggleConfigDatas(),
                    modConfig.GetKeyBindingConfigDatas(),
                    modConfig.GetInputConfigDatas());
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
    }

    public static class ModInfoExtension {
        public static string GetModId(this ModInfo modInfo) {
            return $"displayName:{modInfo.displayName};name:{modInfo.name};publishedFileId:{modInfo.publishedFileId}";
        }
    }
}