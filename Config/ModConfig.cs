using System;
using System.Collections.Generic;
using Duckov.Modding;
using ModSetting.Config.Data;
using UnityEngine;

//TODO 尝试使用多态Config
namespace ModSetting.Config {
    public class ModConfig {
        public ModInfo ModInfo { get; private set; }
        private readonly Dictionary<string, IConfig> allConfigs = new Dictionary<string, IConfig>();
        public ModConfig(ModInfo modInfo) {
            ModInfo = modInfo;
        }
        public void AddConfig(IConfig config) {
            if (allConfigs.TryGetValue(config.Key,out _)) {
                Debug.LogError(ModInfo.displayName+":已经有此key，无法添加新config");
            } else {
                allConfigs.Add(config.Key,config);
            }
        }

        public T GetValue<T>(string key) {
            if (allConfigs.TryGetValue(key,out var config)) {
                if(typeof(T)==config.ValueType) return (T)config.GetValue();
                Debug.LogError($"key和类型不匹配,key:{key};type:{typeof(T)}。类型应该为:{config.ValueType}");
            } else {
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            return default;
        }

        public bool SetValue<T>(string key,T value) {
            if (allConfigs.TryGetValue(key, out var config)) {
                if (typeof(T) == config.ValueType) {
                    config.SetValue(value);
                    return true;
                }
                Debug.LogError($"key和类型不匹配,key:{key};type:{typeof(T)}。类型应该为:{config.ValueType}");
                return false;
            }
            Debug.LogError("找不到对应key的值,key:"+key);
            return false;
        }

        public bool RemoveUI(string key) {
            if (allConfigs.Remove(key, out _)) {
                return true;
            }
            Debug.LogError("删除UI失败,key:"+key);
            return false;
        }

        public List<DropDownConfigData> GetDropDownConfigDatas() {
            List<DropDownConfigData> dropDownConfigDatas = new List<DropDownConfigData>();
            List<DropDownConfig> dropDownConfigs = Filter<DropDownConfigData,DropDownConfig>();
            foreach (DropDownConfig dropDownConfig in dropDownConfigs) {
                DropDownConfigData dropDownConfigData = new DropDownConfigData(
                    dropDownConfig.Key,
                    dropDownConfig.Description,
                    dropDownConfig.Value,
                    dropDownConfig.Options);
                dropDownConfigDatas.Add(dropDownConfigData);
            }
            return dropDownConfigDatas;
        }

        public List<SliderConfigData> GetSliderConfigDatas() {
            List<SliderConfigData> sliderConfigDatas = new List<SliderConfigData>();
            List<SliderConfig> sliderConfigs = Filter<SliderConfigData,SliderConfig>();
            foreach (SliderConfig sliderConfig in sliderConfigs) {
                SliderConfigData sliderConfigData = new SliderConfigData(
                    sliderConfig.Key,
                    sliderConfig.Description,
                    sliderConfig.Value,
                    sliderConfig.SliderRange);
                sliderConfigDatas.Add(sliderConfigData);
            }
            return sliderConfigDatas;
        }

        public List<ToggleConfigData> GetToggleConfigDatas() {
            List<ToggleConfigData> toggleConfigDatas = new List<ToggleConfigData>();
            List<ToggleConfig> toggleConfigs = Filter<ToggleConfigData,ToggleConfig>();
            foreach (ToggleConfig toggleConfig in toggleConfigs) {
                ToggleConfigData toggleConfigData = new ToggleConfigData(
                    toggleConfig.Key,
                    toggleConfig.Description,
                    toggleConfig.Enable);
                toggleConfigDatas.Add(toggleConfigData);
            }
            return toggleConfigDatas;
        }

        public List<KeyBindingConfigData> GetKeyBindingConfigDatas() {
            List<KeyBindingConfigData> keyBindingConfigDatas = new List<KeyBindingConfigData>();
            List<KeyBindingConfig> keyBindingConfigs = Filter<KeyBindingConfigData,KeyBindingConfig>();
            foreach (KeyBindingConfig keyBindingConfig in keyBindingConfigs) {
                KeyBindingConfigData keyBindingConfigData = new KeyBindingConfigData(
                    keyBindingConfig.Key,
                    keyBindingConfig.Description,
                    keyBindingConfig.KeyCode);
                keyBindingConfigDatas.Add(keyBindingConfigData);
            }
            return keyBindingConfigDatas;
        }

        private List<K> Filter<T,K>() {
            List<K> list = new List<K>();
            foreach (IConfig config in allConfigs.Values) {
                if (config.ConfigDataType!=typeof(T)||
                    !(config is K k)) continue;
                list.Add(k);
            }
            return list;
        }
    }
}