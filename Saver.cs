using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModSetting.Config;
using ModSetting.Config.Data;
using ModSetting.Extensions;
using Newtonsoft.Json;
using UnityEngine;

namespace ModSetting {
    public static class Saver {
        private static readonly JsonSerializer jsonSerializer = new JsonSerializer() {
            Formatting = Formatting.Indented,
            Converters = { new ConfigDataReadConverter() }
        };

        private static readonly Dictionary<string, ModConfigData> saveConfigs = new Dictionary<string, ModConfigData>();
        private const string CONFIG_FOLDER = "ModSetting";
        private const string CONFIG_FILE_NAME = "ModSetting.json";

        public static void Load() {
            string configPath = GetConfigPath(CONFIG_FILE_NAME);
            string oldConfigPath = GetOldConfigPath("config.json");
            if (!File.Exists(configPath) && File.Exists(oldConfigPath)) {
                string json = File.ReadAllText(oldConfigPath);
                File.WriteAllText(configPath, json);
                File.Delete(oldConfigPath);
                ConfigData data;
                try {
                    data = jsonSerializer.Deserialize<ConfigData>(new JsonTextReader(new StringReader(json)));
                } catch (Exception e) {
                    Debug.Log("异常:" + e);
                    throw;
                }
                List<ModConfigData> modConfigDatas = data.configDatas;
                if (modConfigDatas != null && modConfigDatas.Count != 0) {
                    foreach (ModConfigData modConfigData in modConfigDatas) {
                        if (saveConfigs.TryGetValue(modConfigData.modId, out var configData)) {
                            Debug.LogError("配置文件出现相同modId，异常");
                        } else {
                            saveConfigs.Add(modConfigData.modId, modConfigData);
                        }
                    }

                    return;
                }
            }

            Debug.Log("加载配置文件:" + configPath);
            if (File.Exists(configPath)) {
                string json = File.ReadAllText(configPath);
                ConfigData data;
                try {
                    data = jsonSerializer.Deserialize<ConfigData>(new JsonTextReader(new StringReader(json)));
                } catch (Exception e) {
                    Debug.Log("异常:" + e);
                    throw;
                }

                List<ModConfigData> modConfigDatas = data.configDatas;
                if (modConfigDatas != null && modConfigDatas.Count != 0) {
                    foreach (ModConfigData modConfigData in modConfigDatas) {
                        if (saveConfigs.TryGetValue(modConfigData.modId, out var configData)) {
                            Debug.LogError("配置文件出现相同modId，异常");
                        } else {
                            saveConfigs.Add(modConfigData.modId, modConfigData);
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
            string configPath = GetConfigPath(CONFIG_FILE_NAME);
            Debug.Log("创建配置文件:" + configPath);
            foreach (ModConfig modConfig in ConfigManager.GetConfigs()) {
                if (saveConfigs.TryGetValue(modConfig.ModInfo.GetModId(), out _)) {
                    saveConfigs[modConfig.ModInfo.GetModId()] = modConfig.ToModConfigData();
                } else {
                    saveConfigs.Add(modConfig.ModInfo.GetModId(), modConfig.ToModConfigData());
                }
            }

            ConfigData configData = new ConfigData(saveConfigs.Values.ToList());
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
            directory= Path.Combine(Application.persistentDataPath, CONFIG_FOLDER);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory,CONFIG_FILE_NAME), json);
            Debug.Log("创建完成");
        }

        private static string GetConfigPath(string fileName) {
            string assemblyLocation = typeof(ModBehaviour).Assembly.Location;
            string directory = Path.GetDirectoryName(assemblyLocation);
            if (string.IsNullOrEmpty(directory)) {
                directory = Application.persistentDataPath ?? ".";
            }

            return Path.Combine(directory, CONFIG_FOLDER, fileName);
        }

        private static string GetOldConfigPath(string fileName) {
            string assemblyLocation = typeof(ModBehaviour).Assembly.Location;
            string directory = Path.GetDirectoryName(assemblyLocation);
            if (string.IsNullOrEmpty(directory)) {
                directory = Application.persistentDataPath ?? ".";
            }

            return Path.Combine(directory,fileName);
        }

        public static void Clear() => saveConfigs.Clear();

        public static bool HasValue(string modId, string key) {
            return saveConfigs.ContainsKey(modId) && saveConfigs[modId].HasValue(key);
        }

        public static bool HasValue(string modId) => saveConfigs.ContainsKey(modId);

        public static T GetValue<T>(string modId, string key) {
            if (HasValue(modId, key)) {
                return saveConfigs[modId].GetValue<T>(key);
            }

            return default;
        }


        public static void UpdateConfig(ModConfig modConfig) {
            if (modConfig == null) return;
            if (saveConfigs.TryGetValue(modConfig.ModInfo.GetModId(), out _)) {
                saveConfigs[modConfig.ModInfo.GetModId()] = modConfig.ToModConfigData();
            } else {
                saveConfigs.Add(modConfig.ModInfo.GetModId(), modConfig.ToModConfigData());
            }
        }
    }
}