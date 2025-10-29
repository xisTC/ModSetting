using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModSetting.Config.Data {
    [Serializable]
    public struct ModConfigData {
        public string displayModName;
        public ulong modId;
        public List<DropDownConfigData> dropDownConfigDatas;
        public List<SliderConfigData> sliderConfigDatas;
        public List<ToggleConfigData> toggleConfigDatas;
        public List<KeyBindingConfigData> keyBindingConfigDatas;

        public ModConfigData(string displayModName, ulong modId, List<DropDownConfigData> dropDownConfigDatas, 
            List<SliderConfigData> sliderConfigDatas, List<ToggleConfigData> toggleConfigDatas,
            List<KeyBindingConfigData> keyBindingConfigDatas) {
            this.displayModName = displayModName;
            this.modId = modId;
            this.dropDownConfigDatas = dropDownConfigDatas;
            this.sliderConfigDatas = sliderConfigDatas;
            this.toggleConfigDatas = toggleConfigDatas;
            this.keyBindingConfigDatas = keyBindingConfigDatas;
        }
    }

    [Serializable]
    public struct DropDownConfigData {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public string DefaultValue { get; private set; }
        public List<string> Options { get; private set; }

        public DropDownConfigData(string key, string description, string defaultValue, List<string> options) {
            Key = key;
            Description = description;
            DefaultValue = defaultValue;
            Options = options;
        }
    }
    [Serializable]
    public struct SliderConfigData {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public float DefaultValue { get; private set; }
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public SliderConfigData(string key, string description, float defaultValue, Vector2 sliderRange) {
            Key = key;
            Description = description;
            DefaultValue = defaultValue;
            MinValue = sliderRange.x;
            MaxValue = sliderRange.y;
        }
    }
    [Serializable]
    public struct ToggleConfigData {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public bool Enable { get; private set; }

        public ToggleConfigData(string key, string description, bool enable) {
            Key = key;
            Description = description;
            Enable = enable;
        }
    }
    [Serializable]
    public struct KeyBindingConfigData {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public string KeyCode { get; private set; }

        public KeyBindingConfigData(string key, string description, KeyCode keyCode) {
            Key = key;
            Description = description;
            KeyCode = keyCode.ToString();
        }
    }
}