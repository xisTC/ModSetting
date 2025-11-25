using System;
using System.Collections.Generic;
using ModSetting.Config.Data;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Config {
    public class DropDownConfig: IConfig {
        public string Key { get; }
        public string Description { get; }
        public string Value { get; private set; }
        public List<string> Options { get; private set; }
        public event Action<string> OnValueChange;
        public DropDownConfig(string key, string description, List<string> options, string value) {
            Key = key;
            Description = description;
            Options = options;
            Value = value;
        }

        public T GetValue<T>() => (T)(object)Value;

        public void SetValue(object value) {
            if (!IsTypeMatch(value.GetType())) {
                Logger.Error($"类型不匹配:{value.GetType()},无法赋值给:{GetTypesString()}");
                return;
            }
            if (!Options.Contains((string)value)) {
                Logger.Error($"DropDown设置值失败,值不在Options范围无法设置值,value:{value}");
                return;
            }
            Value = (string)value;
            OnValueChange?.Invoke(Value);
        }

        public bool IsTypeMatch(Type type) => GetValidTypes().Contains(type);

        public List<Type> GetValidTypes() => new List<Type>(){ typeof(string) };

        public string GetTypesString() => string.Join(",", GetValidTypes());

        public IConfigData GetConfigData() {
            return new DropDownConfigData(Key, Description, Value, Options);
        }
    }
}