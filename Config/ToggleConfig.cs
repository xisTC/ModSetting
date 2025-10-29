using System;

namespace ModSetting.Config {
    public class ToggleConfig {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public bool Enable { get; private set; }
        public event Action<bool> onValueChange;

        public ToggleConfig(string key, string description, bool enable) {
            Key = key;
            Description = description;
            Enable = enable;
        }
        public void SetEnable(bool enable) {
            Enable = enable;
            onValueChange?.Invoke(enable);
        }
    }
}