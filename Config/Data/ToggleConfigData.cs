using System;

namespace ModSetting.Config.Data {
    [Serializable]
    public class ToggleConfigData  : IConfigData{
        public string Key { get; private set; }
        public string Description { get; private set; }
        public UIType UIType => UIType.按钮;
        public object GetValue() => Enable;

        public bool Enable { get; private set; }

        public ToggleConfigData(string key, string description, bool enable) {
            Key = key;
            Description = description;
            Enable = enable;
        }
    }
}