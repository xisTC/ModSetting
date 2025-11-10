using System;
using System.Collections.Generic;
using Duckov.Modding;
using ModSetting.Config.Data;
using ModSetting.Extensions;
using UnityEngine;


namespace ModSetting.Config {
    public class ModConfig {
        public ModInfo ModInfo { get; private set; }
        private readonly Dictionary<string, IConfig> allConfigs = new();
        //添加操作对activeKey进行检查,删除操作对activeKey移除,注意一些没有config的key也会加入到activeKey中
        private readonly HashSet<string> activeKey = new();
        public ModConfig(ModInfo modInfo) {
            ModInfo = modInfo;
        }
        public void AddConfig(IConfig config) {
            if (HasKey(config.Key)) {
                Debug.LogError("已经有此key无法添加,key:"+config.Key);
                return;
            }
            if (allConfigs.TryGetValue(config.Key,out IConfig oldConfig)) {
                allConfigs[config.Key] = config;
            } else {
                allConfigs.Add(config.Key,config);
            }
            AddKey(config.Key);
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

        public void AddKey(string key) {
            if (HasKey(key)) {
                Debug.LogError("已经存在此key,key:"+key);
                return;
            }
            activeKey.Add(key);
        }

        public bool HasKey(string key) => activeKey.Contains(key);
        public bool RemoveKey(string key) => activeKey.Remove(key);

        public bool Clear() {
            activeKey.Clear();
            return true;
        }
    }
}