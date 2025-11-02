using System;
using System.Collections.Generic;
using ModSetting.Config.Data;
using UnityEngine;

namespace ModSetting.Config {
    public class ToggleConfig : IConfig{
        public string Key { get; }
        public string Description { get; }

        public bool Enable { get; private set; }
        public event Action<bool> OnValueChange;

        public ToggleConfig(string key, string description, bool enable) {
            Key = key;
            Description = description;
            Enable = enable;
        }

        public T GetValue<T>() => (T)(object)Enable;

        public void SetValue(object value) {
            if (!IsTypeMatch(value.GetType())) {
                Debug.LogError($"类型不匹配:{value.GetType()},无法赋值给:{GetTypesString()}");
                return;
            }
            SetValue((bool)value);
        }

        public bool IsTypeMatch(Type type) => GetValidTypes().Contains(type);

        public List<Type> GetValidTypes() => new List<Type>() { typeof(bool) };

        public string GetTypesString() => string.Join(",", GetValidTypes());

        public IConfigData GetConfigData() {
            return new ToggleConfigData(Key, Description, Enable);
        }

        public void SetValue(bool value) {
            Enable = value;
            OnValueChange?.Invoke(value);
        }
    }
}