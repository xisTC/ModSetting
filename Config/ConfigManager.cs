using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Duckov.Modding;
using ModSetting.Config.Data;
using Newtonsoft.Json;
using UnityEngine;
//todo ulong需要更改，因为没有上架的id为0
// todo 增加保存系统，优先从UI获取数据，如果没有，然后从保存系统中获取
namespace ModSetting.Config {
    public static class ConfigManager {
        private static readonly Dictionary<string, ModConfig> configs = new Dictionary<string, ModConfig>();

        public static void AddConfig(ModInfo modInfo, IConfig config) {
            if (configs.TryGetValue(modInfo.GetModId(), out var modConfig)) {
                modConfig.AddConfig(config);
            } else {
                modConfig = new ModConfig(modInfo);
                modConfig.AddConfig(config);
                configs.Add(modInfo.GetModId(), modConfig);
            }
        }

        public static T GetValue<T>(ModInfo info, string key) {
            if (configs.TryGetValue(info.GetModId(), out var modConfig))
                return modConfig.GetValue<T>(key);
            if (Saver.HasValue(info.GetModId(),key)) {
                return Saver.GetValue<T>(info.GetModId(),key);
            }
            Debug.LogError($"找不到此{info.displayName}的值,key:" + key);
            return default;
        }

        public static bool SetValue<T>(ModInfo info, string key, T value) {
            if (configs.TryGetValue(info.GetModId(), out var modConfig))
                return modConfig.SetValue(key, value);
            Debug.LogError($"找不到此{info.displayName}的配置,无法设置值");
            return false;
        }

        public static bool RemoveUI(ModInfo info, string key) {
            if (configs.TryGetValue(info.GetModId(), out var modConfig))
                return modConfig.RemoveUI(key);
            Debug.LogError($"找不到此{info.displayName}的UI,key:" + key);
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
        public static List<ModConfig> GetConfigs() => configs.Values.ToList();
    }
}