using System;
using System.Collections.Generic;
using ModSetting.Config.Data;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Config {
    public class InputConfig : IConfig {
        public string Key { get; }

        public string Description { get; }

        public string Value { get; private set; }
        public int CharacterLimit { get;}
        
        public event Action<string> OnValueChange;

        public InputConfig(string key, string description, string value, int characterLimit) {
            Key = key;
            Description = description;
            Value = value;
            CharacterLimit = characterLimit;
        }

        public T GetValue<T>() => (T)(object)Value;

        public void SetValue(object value) {
            if (!IsTypeMatch(value.GetType())) {
                Logger.Error($"类型不匹配:{value.GetType()},无法赋值给:{GetTypesString()}");
                return;
            }
            Value = (string)value;
            OnValueChange?.Invoke(Value);
        }

        public bool IsTypeMatch(Type type) => GetValidTypes().Contains(type);

        public List<Type> GetValidTypes() => new List<Type>() { typeof(string) };

        public string GetTypesString() => string.Join(",", GetValidTypes());

        public IConfigData GetConfigData() {
            return new InputConfigData(Key, Description, Value);
        }
    }
}