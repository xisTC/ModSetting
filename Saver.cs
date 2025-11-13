using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ModSetting.Config;
using ModSetting.Config.Data;
using ModSetting.Extensions;
using Newtonsoft.Json;
using UnityEngine;

namespace ModSetting {
    public static class Saver {
        private static readonly JsonSerializer jsonSerializer = new() {
            Formatting = Formatting.Indented,
            Converters = { new ConfigDataReadConverter() }
        };

        private static readonly Dictionary<string, ModConfigData> saveConfigs = new();
        private const string CONFIG_FOLDER = "ModSetting";
        private const string CONFIG_FILE_NAME = "ModSetting.json";
        public static void Load() {
            string configPath = GetConfigPath(CONFIG_FILE_NAME);
            string backupPath = configPath + ".bak";
            Debug.Log("加载配置文件:" + configPath);
            if (File.Exists(configPath)) {
                try {
                    if (LoadData(configPath)) return;
                } catch (Exception e) {
                    Debug.LogError($"主配置文件损坏: {e}，尝试从备份加载");
                }
            }
            if (File.Exists(backupPath)) {
                try {
                    if (LoadData(backupPath)) {
                        Debug.Log("从备份文件加载成功");
                        return;
                    }
                } catch (Exception e) {
                    Debug.LogError($"备份配置文件也损坏: {e}");
                }
            }
            Debug.Log("未找到配置文件/文件为空,启用默认配置");
        }

        private static bool LoadData(string configPath) {
            string json = File.ReadAllText(configPath);
            ConfigData data;
            try {
                data = jsonSerializer.Deserialize<ConfigData>(new JsonTextReader(new StringReader(json)));
            } catch (Exception e) {
                Debug.LogError("配置文件解析异常，使用默认配置，异常信息:" + e);
                return true;
            }
            List<ModConfigData> modConfigDatas = data.configDatas;
            if (modConfigDatas != null && modConfigDatas.Count != 0) {
                saveConfigs.Clear();
                foreach (ModConfigData modConfigData in modConfigDatas) {
                    if (saveConfigs.TryGetValue(modConfigData.modId, out _)) {
                        Debug.LogError("配置文件出现相同modId，异常");
                    } else {
                        foreach (IConfigData configData in modConfigData.allConfigDatas) {
                            configData.Liveness--;
                        }
                        //如果活跃度小于-99可以进行移除
                        saveConfigs.Add(modConfigData.modId, modConfigData);
                    }
                }
                return true;
            }
            return false;
        }

        public static void Save() {
            CreateConfigFile();
        }

        private static void CreateConfigFile() {
            string configPath = GetConfigPath(CONFIG_FILE_NAME);
            string tempPath = configPath + ".tmp";
            string backupPath = configPath + ".bak";
            Debug.Log("创建配置文件:" + configPath);
            foreach (ModConfig modConfig in ConfigManager.GetConfigs()) {
                saveConfigs[modConfig.ModInfo.GetModId()] = modConfig.ToModConfigData();
            }
            ConfigData configData = new ConfigData(saveConfigs.Values.ToList());
            string directory = Path.GetDirectoryName(configPath);
            if (directory == null) {
                Debug.LogError("directory不能为null");
                return;
            }

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            try {
                using (StringWriter stringWriter = new StringWriter()) {
                    jsonSerializer.Serialize(stringWriter, configData);
                    string json = stringWriter.ToString();
                    File.WriteAllText(tempPath, json);
                }

                if (File.Exists(configPath)) {
                    if (File.Exists(backupPath)) File.Delete(backupPath);
                    File.Move(configPath, backupPath);
                }

                File.Move(tempPath, configPath);
                Debug.Log("创建完成");
            } catch (Exception e) {
                Debug.LogError($"保存配置文件失败: {e}");
                if (File.Exists(tempPath)) {
                    try {
                        File.Delete(tempPath);
                    } catch (Exception ex) {
                        Debug.LogError($"删除临时文件失败: {ex}");
                    }
                }
            }
        }

        private static string GetConfigPath(string fileName) {
            string directory = Application.persistentDataPath;
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory)) {
                directory = Directory.GetCurrentDirectory() ?? ".";
            }

            return Path.Combine(directory, CONFIG_FOLDER, fileName);
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