using System;
using UnityEngine;

namespace ModSetting.Config {
    public class KeyBindingConfig {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public KeyCode KeyCode { get; private set; }
        public event Action<KeyCode> onValueChange;
        public KeyBindingConfig(string key, string description, KeyCode keyCode) {
            Key = key;
            Description = description;
            KeyCode = keyCode;
        }

        public void SetKeyCode(KeyCode keyCode) {
            KeyCode = keyCode;
            onValueChange?.Invoke(keyCode);
        } 
    }
}