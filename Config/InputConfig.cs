using System;
using ModSetting.Config.Data;
using UnityEngine;

namespace ModSetting.Config {
    public class InputConfig : IConfig {
        public string Key { get; }

        public string Description { get; }

        public Type ValueType { get; }

        public string Value { get; private set; }
        public int CharacterLimit { get;}
        
        public event Action<string> OnValueChange;

        public InputConfig(string key, string description, string value, int characterLimit) {
            Key = key;
            Description = description;
            Value = value;
            CharacterLimit = characterLimit;
            ValueType = typeof(string);
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

        public IConfigData GetConfigData() {
            return new InputConfigData(Key, Description, Value);
        }
    }
}