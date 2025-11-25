using System;
using ModSetting.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Config.Data {
    [Serializable]
    public class KeyBindingConfigData : IConfigData {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public UIType UIType => UIType.按键绑定;
        public int Liveness { get; set; } = 1;
        public T GetValue<T>() {
            if (typeof(T) == typeof(KeyCode)) {
                return (T)(object)Enum.Parse<KeyCode>(KeyCode);
            }
            if (typeof(T)==typeof(Key)) {
                return (T)(object)Enum.Parse<KeyCode>(KeyCode).ToKey();
            }
            Logger.Error($"按键绑定不支持获取此类型的值:{typeof(T)},key:{Key}");
            return default;
        }

        public string KeyCode { get; private set; }

        public KeyBindingConfigData(string key, string description, KeyCode keyCode) {
            Key = key;
            Description = description;
            KeyCode = keyCode.ToString();
        }
    }
}