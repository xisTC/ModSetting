using System;
using UnityEngine;

namespace ModSetting.Config.Data {
    [Serializable]
    public class KeyBindingConfigData : IConfigData {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public UIType UIType => UIType.按键绑定;
        public int Liveness { get; set; } = 1;
        public T GetValue<T>() => (T)(object)Enum.Parse<KeyCode>(KeyCode);

        public string KeyCode { get; private set; }

        public KeyBindingConfigData(string key, string description, KeyCode keyCode) {
            Key = key;
            Description = description;
            KeyCode = keyCode.ToString();
        }
    }
}