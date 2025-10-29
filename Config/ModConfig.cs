using System;
using System.Collections.Generic;
using Duckov.Modding;
using ModSetting.Config.Data;
using UnityEngine;

//TODO 尝试使用多态Config
namespace ModSetting.Config {
    public class ModConfig {
        public ModInfo ModInfo { get; private set; }
        private readonly Dictionary<string, DropDownConfig> dropDownConfigs = new Dictionary<string, DropDownConfig>();
        private readonly Dictionary<string, SliderConfig> sliderConfigs = new Dictionary<string, SliderConfig>();
        private readonly Dictionary<string, ToggleConfig> toggleConfigs = new Dictionary<string, ToggleConfig>();
        private readonly Dictionary<string, KeyBindingConfig> keyBindingConfigs =
            new Dictionary<string, KeyBindingConfig>();

        public ModConfig(ModInfo modInfo) {
            ModInfo = modInfo;
        }

        public void AddDropDownList(DropDownConfig dropDownConfig) {
            if (dropDownConfigs.TryGetValue(dropDownConfig.Key,out _)) {
                Debug.LogError(ModInfo.displayName+":已经有此key，无法添加新dropDownConfig");
            } else {
                dropDownConfigs.Add(dropDownConfig.Key,dropDownConfig);
            }
        }

        public void AddSlider(SliderConfig sliderConfig) {
            if (sliderConfigs.TryGetValue(sliderConfig.Key,out _)) {
                Debug.LogError(ModInfo.displayName+":已经有此key，无法添加新sliderConfig");
            } else {
                sliderConfigs.Add(sliderConfig.Key,sliderConfig);
            }
        }

        public void AddToggle(ToggleConfig toggleConfig) {
            if (toggleConfigs.TryGetValue(toggleConfig.Key,out _)) {
                Debug.LogError(ModInfo.displayName+":已经有此key，无法添加新toggleConfig");
            } else {
                toggleConfigs.Add(toggleConfig.Key,toggleConfig);
            }
        }

        public void AddKeybinding(KeyBindingConfig keyBindingConfig) {
            if (keyBindingConfigs.TryGetValue(keyBindingConfig.Key,out _)) {
                Debug.LogError(ModInfo.displayName+":已经有此key，无法添加新keyBindingConfig");
            } else {
                keyBindingConfigs.Add(keyBindingConfig.Key,keyBindingConfig);
            }
        }

        public T GetValue<T>(string key) {
            Type type = typeof(T);
            if (type == typeof(string)) {
                if (dropDownConfigs.TryGetValue(key,out var downConfig)) {
                    return (T)(object)downConfig.DefaultValue;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            else if (type==typeof(float)) {
                if (sliderConfigs.TryGetValue(key, out var sliderConfig)) {
                    return (T)(object)sliderConfig.DefaultValue;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            else if (type==typeof(bool)) {
                if (toggleConfigs.TryGetValue(key, out var toggleConfig)) {
                    return (T)(object)toggleConfig.Enable;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            else if (type==typeof(KeyCode)) {
                if (keyBindingConfigs.TryGetValue(key, out var keyBindingConfig)) {
                    return (T)(object)keyBindingConfig.KeyCode;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            Debug.LogError("不支持类型:"+type);
            return default;
        }

        public bool SetValue<T>(string key,T value) {
            Type type = typeof(T);
            if (type == typeof(string)) {
                if (dropDownConfigs.TryGetValue(key,out var downConfig)) {
                    downConfig.SetValue((string)(object)value);
                    return true;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            else if (type==typeof(float)) {
                if (sliderConfigs.TryGetValue(key, out var sliderConfig)) {
                    sliderConfig.SetValue((float)(object)value);
                    return true;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            else if (type==typeof(bool)) {
                if (toggleConfigs.TryGetValue(key, out var toggleConfig)) {
                    toggleConfig.SetEnable((bool)(object)value);
                    return true;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            else if (type==typeof(KeyCode)) {
                if (keyBindingConfigs.TryGetValue(key, out var keyBindingConfig)) {
                    keyBindingConfig.SetKeyCode((KeyCode)(object)value);
                    return true;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            Debug.LogError("不支持类型:"+type);
            return false;
        }

        public bool RemoveUI<T>(string key) {
            Type type = typeof(T);
            if (type == typeof(string)) {
                if (dropDownConfigs.Remove(key)) {
                    return true;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            else if (type==typeof(float)) {
                if (sliderConfigs.Remove(key)) {
                    return true;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            else if (type==typeof(bool)) {
                if (toggleConfigs.Remove(key)) {
                    return true;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            else if (type==typeof(KeyCode)) {
                if (keyBindingConfigs.Remove(key)) {
                    return true;
                }
                Debug.LogError("找不到对应key的值,key:"+key);
            }
            Debug.LogError("不支持类型:"+type);
            return false;
        }

        public List<DropDownConfigData> GetDropDownConfigDatas() {
            List<DropDownConfigData> dropDownConfigDatas = new List<DropDownConfigData>();
            foreach (DropDownConfig dropDownConfig in dropDownConfigs.Values) {
                DropDownConfigData dropDownConfigData = new DropDownConfigData(
                    dropDownConfig.Key,
                    dropDownConfig.Description,
                    dropDownConfig.DefaultValue,
                    dropDownConfig.Options);
                dropDownConfigDatas.Add(dropDownConfigData);
            }
            return dropDownConfigDatas;
        }

        public List<SliderConfigData> GetSliderConfigDatas() {
            List<SliderConfigData> sliderConfigDatas = new List<SliderConfigData>();
            foreach (SliderConfig sliderConfig in sliderConfigs.Values) {
                SliderConfigData sliderConfigData = new SliderConfigData(
                    sliderConfig.Key,
                    sliderConfig.Description,
                    sliderConfig.DefaultValue,
                    sliderConfig.SliderRange);
                sliderConfigDatas.Add(sliderConfigData);
            }
            return sliderConfigDatas;
        }

        public List<ToggleConfigData> GetToggleConfigDatas() {
            List<ToggleConfigData> toggleConfigDatas = new List<ToggleConfigData>();
            foreach (ToggleConfig toggleConfig in toggleConfigs.Values) {
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
            foreach (KeyBindingConfig keyBindingConfig in keyBindingConfigs.Values) {
                KeyBindingConfigData keyBindingConfigData = new KeyBindingConfigData(
                    keyBindingConfig.Key,
                    keyBindingConfig.Description,
                    keyBindingConfig.KeyCode);
                keyBindingConfigDatas.Add(keyBindingConfigData);
            }
            return keyBindingConfigDatas;
        }
    }
}