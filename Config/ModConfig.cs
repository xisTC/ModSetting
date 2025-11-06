using System;
using System.Collections.Generic;
using Duckov.Modding;
using ModSetting.Config.Data;
using ModSetting.Extensions;
using UnityEngine;


namespace ModSetting.Config {
    public class ModConfig {
        public ModInfo ModInfo { get; private set; }
        private readonly Dictionary<string, IConfig> allConfigs = new Dictionary<string, IConfig>();
        public ModConfig(ModInfo modInfo) {
            ModInfo = modInfo;
        }
        public void AddConfig(IConfig config) {
            if (allConfigs.TryGetValue(config.Key,out IConfig oldConfig)) {
                allConfigs[config.Key] = config;
            } else {
                allConfigs.Add(config.Key,config);
            }
        }

        public T GetValue<T>(string key) {
            if (allConfigs.TryGetValue(key,out var config)) {
                if (config.IsTypeMatch(typeof(T))) {
                    return config.GetValue<T>();
                }
                Debug.LogError($"key和类型不匹配,key:{key};type:{typeof(T)}。类型应该为:{config.GetTypesString()}");
            } else {
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            return default;
        }

        public bool SetValue<T>(string key,T value) {
            if (allConfigs.TryGetValue(key, out var config)) {
                if (config.IsTypeMatch(typeof(T))) {
                    config.SetValue(value);
                    return true;
                }
                Debug.LogError($"key和类型不匹配,key:{key};type:{typeof(T)}。类型应该为:{config.GetTypesString()}");
                return false;
            }
            Debug.LogError("找不到对应key的值,key:"+key);
            return false;
        }

        public ModConfigData ToModConfigData() {
            List<IConfigData> configDatas = new List<IConfigData>();
            foreach (IConfig config in allConfigs.Values) {
                configDatas.Add(config.GetConfigData());
            }
            return new ModConfigData(ModInfo.GetModId(),configDatas);
        }
    }
}