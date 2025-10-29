using System;
using ModSetting.Config.Data;
using UnityEngine;

namespace ModSetting.Config {
    public class ToggleConfig : IConfig{
        public string Key { get; }
        public string Description { get; }
        public Type ValueType { get; }
        public Type ConfigDataType { get; }

        public bool Enable { get; private set; }
        public event Action<bool> OnValueChange;

        public ToggleConfig(string key, string description, bool enable) {
            Key = key;
            Description = description;
            Enable = enable;
            ValueType = enable.GetType();
            ConfigDataType = typeof(ToggleConfigData);
        }

        public object GetValue() => Enable;

        public void SetValue(object value) {
            if (value.GetType() != ValueType) {
                Debug.LogError($"类型不匹配:{ValueType}和{value.GetType()},无法赋值");
                return;
            }
            SetValue((bool)value);
        }

        public void SetValue(bool value) {
            Enable = value;
            OnValueChange?.Invoke(value);
        }
    }
}