using System;
using ModSetting.Config.Data;
using UnityEngine;

namespace ModSetting.Config {
    public class KeyBindingConfig: IConfig {
        public string Key { get; }
        public string Description { get; }
        public KeyCode KeyCode { get; private set; }
        public Type ValueType { get; }
        public Type ConfigDataType { get; }
        public event Action<KeyCode> OnValueChange;

        public KeyBindingConfig(string key, string description, KeyCode keyCode) {
            Key = key;
            Description = description;
            KeyCode = keyCode;
            ValueType = keyCode.GetType();
            ConfigDataType = typeof(KeyBindingConfigData);
        }

        public object GetValue() => KeyCode;
        public void SetValue(object keyCode) {
            if (keyCode.GetType() != ValueType) {
                Debug.LogError($"类型不匹配:{ValueType}和{keyCode.GetType()},无法赋值");
                return;
            }
            SetValue((KeyCode)keyCode);
        }
        public void SetValue(KeyCode keyCode) {
            KeyCode = keyCode;
            OnValueChange?.Invoke(keyCode);
        }
    }
}