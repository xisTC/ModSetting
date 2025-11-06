using System;
using System.Collections.Generic;
using ModSetting.Config.Data;
using UnityEngine;

namespace ModSetting.Config {
    public class KeyBindingConfig: IConfig {
        public string Key { get; }
        public string Description { get; }
        public KeyCode KeyCode { get; private set; }
        public KeyCode DefaultKeyCode { get; private set; }
        public event Action<KeyCode> OnValueChange;

        public KeyBindingConfig(string key, string description,KeyCode keyCode,KeyCode defaultKeyCode) {
            Key = key;
            Description = description;
            KeyCode = keyCode;
            DefaultKeyCode = defaultKeyCode;
        }

        public T GetValue<T>() => (T)(object)KeyCode;
        public void SetValue(object keyCode) {
            if (!IsTypeMatch(keyCode.GetType())) {
                Debug.LogError($"类型不匹配:{keyCode.GetType()},无法赋值给:{GetTypesString()}");
                return;
            }
            SetValue((KeyCode)keyCode);
        }

        public bool IsTypeMatch(Type type) => GetValidTypes().Contains(type);

        public List<Type> GetValidTypes() => new List<Type>() { typeof(KeyCode) };

        public string GetTypesString() => string.Join(",", GetValidTypes());

        public IConfigData GetConfigData() {
            return new KeyBindingConfigData(Key, Description, KeyCode);
        }

        public void SetValue(KeyCode keyCode) {
            KeyCode = keyCode;
            OnValueChange?.Invoke(keyCode);
        }
    }
}