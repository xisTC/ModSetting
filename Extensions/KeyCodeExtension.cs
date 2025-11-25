using UnityEngine;
using UnityEngine.InputSystem;

namespace ModSetting.Extensions {
    public static class KeyCodeExtension {
        public static Key ToKey(this KeyCode keyCode) => KeyCodeConverter.ConverterToKey(keyCode);
    }
}