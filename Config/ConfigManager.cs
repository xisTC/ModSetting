using System;
using System.Collections.Generic;
using System.IO;
using Duckov.Modding;
using ModSetting.Config.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace ModSetting.Config {
    // todo 做成保存文件的类，加载保存文件的数据接口
    public static class ConfigManager {
        private static readonly Dictionary<ulong, ModConfig> configs = new Dictionary<ulong, ModConfig>();
        public static void AddDropDownList(ModInfo modInfo,DropDownConfig dropDownConfig) {
            if (configs.TryGetValue(modInfo.publishedFileId,out var modConfig)) {
                modConfig.AddDropDownList(dropDownConfig);
            } else {
                ModConfig config = new ModConfig(modInfo);
                config.AddDropDownList(dropDownConfig);
                configs.Add(modInfo.publishedFileId,config);
            }
        }
        public static void AddSlider(ModInfo modInfo,SliderConfig sliderConfig) {
            if (configs.TryGetValue(modInfo.publishedFileId,out var modConfig)) {
                modConfig.AddSlider(sliderConfig);
            } else {
                ModConfig config = new ModConfig(modInfo);
                config.AddSlider(sliderConfig);
                configs.Add(modInfo.publishedFileId,config);
            }
        }
        public static void AddToggle(ModInfo modInfo,ToggleConfig toggleConfig) {
            if (configs.TryGetValue(modInfo.publishedFileId,out var modConfig)) {
                modConfig.AddToggle(toggleConfig);
            } else {
                ModConfig config = new ModConfig(modInfo);
                config.AddToggle(toggleConfig);
                configs.Add(modInfo.publishedFileId,config);
            }
        }
        public static void AddKeybinding(ModInfo modInfo,KeyBindingConfig keyBindingConfig) {
            if (configs.TryGetValue(modInfo.publishedFileId,out var modConfig)) {
                modConfig.AddKeybinding(keyBindingConfig);
            } else {
                ModConfig config = new ModConfig(modInfo);
                config.AddKeybinding(keyBindingConfig);
                configs.Add(modInfo.publishedFileId,config);
            }
        }
        public static T GetValue<T>(ModInfo info,string key) {
            if (configs.TryGetValue(info.publishedFileId,out var modConfig)) {
                return modConfig.GetValue<T>(key);
            } else {
                Debug.LogError($"找不到此{info.displayName}的配置");
                return default;
            }
        }

        public static bool SetValue<T>(ModInfo info, string key,T value) {
            if (configs.TryGetValue(info.publishedFileId,out var modConfig)) {
                return modConfig.SetValue(key,value);
            }
            Debug.LogError($"找不到此{info.displayName}的配置");
            return false;
        }

        public static bool RemoveUI<T>(ModInfo info, string key) {
            if (configs.TryGetValue(info.publishedFileId,out var modConfig)) {
                return modConfig.RemoveUI<T>(key);
            } else {
                Debug.LogError($"找不到此{info.displayName}的配置");
                return false;
            }
        }
        public static bool RemoveMod(ModInfo info) {
            if (configs.Remove(info.publishedFileId)) {
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
                    modConfig.GetKeyBindingConfigDatas());
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
            Debug.Log("创建完成：" + json);
        }

        private static string GetConfigPath() {
            string assemblyLocation = typeof(Duckov.Modding.ModBehaviour).Assembly.Location;
            string directory = Path.GetDirectoryName(assemblyLocation);
            if (string.IsNullOrEmpty(directory)) {
                directory = AppContext.BaseDirectory ?? ".";
            }

            return Path.Combine(directory, CONFIG_FILE_NAME);
        }
    }
}