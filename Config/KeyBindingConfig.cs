using System;
using System.Collections.Generic;
using ModSetting.Config.Data;
using ModSetting.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Config {
    public class KeyBindingConfig: IConfig {
        public string Key { get; }
        public string Description { get; }
        public KeyCode KeyCode { get; private set; }
        public KeyCode DefaultKeyCode { get; private set; }
        public event Action<KeyCode> OnValueChange;

        public KeyBindingConfig(string key, string description,KeyCode keyCode,KeyCode defaultKeyCode) {
            Key = key;
            Description = description;
            KeyCode = keyCode;
            DefaultKeyCode = defaultKeyCode;
        }

        public T GetValue<T>() {
            if (typeof(T) == typeof(KeyCode)) {
                return (T)(object)KeyCode;
            }
            if (typeof(T)==typeof(Key)) {
                return (T)(object)KeyCode.ToKey();
            }
            Logger.Error($"按键绑定获取失败,不支持的类型:{typeof(T)},key:{Key}");
            return default;
        }

        public void SetValue(object keyCode) {
            if (!IsTypeMatch(keyCode.GetType())) {
                Logger.Error($"类型不匹配:{keyCode.GetType()},无法赋值给:{GetTypesString()}");
                return;
            }
            if (keyCode.GetType() == typeof(Key)) {
                Key key= (Key)keyCode;
                SetValue(key.ToKeyCode());
                return;
            }
            if (keyCode.GetType() == typeof(KeyCode)) {
                SetValue((KeyCode)keyCode);
                return;
            }
            Logger.Error($"按键绑定设置值失败,类型不支持:{keyCode.GetType()},key:{Key}");
        }

        public bool IsTypeMatch(Type type) => GetValidTypes().Contains(type);

        public List<Type> GetValidTypes() => new List<Type>() { typeof(KeyCode),typeof(Key) };

        public string GetTypesString() => string.Join(",", GetValidTypes());

        public IConfigData GetConfigData() {
            return new KeyBindingConfigData(Key, Description, KeyCode);
        }

        public void SetValue(KeyCode keyCode) {
            KeyCode = keyCode;
            OnValueChange?.Invoke(keyCode);
        }
    }
}