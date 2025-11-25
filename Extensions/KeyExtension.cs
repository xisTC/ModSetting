using UnityEngine;
using UnityEngine.InputSystem;

namespace ModSetting.Extensions {
    public static class KeyExtension {
        public static KeyCode ToKeyCode(this Key key) => KeyCodeConverter.ConverterToKeyCode(key);
    }
}