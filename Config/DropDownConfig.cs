using System;
using System.Collections.Generic;
using ModSetting.Config.Data;
using UnityEngine;

namespace ModSetting.Config {
    public class DropDownConfig: IConfig {
        public string Key { get; }
        public string Description { get; }
        public Type ValueType { get; }
        public Type ConfigDataType { get; }
        public string Value { get; private set; }
        public List<string> Options { get; private set; }
        public event Action<string> OnValueChange;
        public DropDownConfig(string key, string description, List<string> options, string value) {
            Key = key;
            Description = description;
            Options = options;
            Value = value;
            ValueType = value.GetType();
            ConfigDataType = typeof(DropDownConfigData);
        }

        public object GetValue() => Value;

        public void SetValue(object value) {
            if (value.GetType() != ValueType) {
                Debug.LogError($"类型不匹配:{ValueType}和{value.GetType()},无法赋值");
                return;
            }
            Value = (string)value;
            OnValueChange?.Invoke(Value);
        }
    }
}