using System;
using System.Collections.Generic;

namespace ModSetting.Config {
    public class DropDownConfig {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public string DefaultValue { get; private set; }
        public List<string> Options { get; private set; }
        public event Action<string> onValueChange;
        public DropDownConfig(string key, string description, List<string> options, string defaultValue) {
            Key = key;
            Description = description;
            Options = options;
            DefaultValue = defaultValue;
        }
        public void SetValue(string value) {
            DefaultValue = value;
            onValueChange?.Invoke(value);
        }
    }
}