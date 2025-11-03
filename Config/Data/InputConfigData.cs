using System;

namespace ModSetting.Config.Data {
    [Serializable]
    public class InputConfigData : IConfigData {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public UIType UIType => UIType.输入框;
        public T GetValue<T>() => (T)(object)Value;

        public string Value { get; private set; }

        public InputConfigData(string key, string description, string value) {
            Key = key;
            Description = description;
            Value = value;
        }
    }
}