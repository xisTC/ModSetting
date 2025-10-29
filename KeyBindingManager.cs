using System.Collections.Generic;
using System.Linq;
using Duckov.Modding;
using ModSetting.UI;
using UnityEngine;

namespace ModSetting {
    public class KeyBindingManager {
        private readonly Dictionary<KeyCode,List<KeyBindingUI>> allKeyCodes = new Dictionary<KeyCode, List<KeyBindingUI>>();
        private readonly Dictionary<ulong, ModKeyBinding> modKeyBindingDic = new Dictionary<ulong, ModKeyBinding>();
        public bool IsRebinding { get; private set; }

        public void StartBinding() {
            IsRebinding = true;
        }

        public void StopBinding() {
            IsRebinding = false;
        }

        public void AddKeyCode(KeyCode newKeyCode,KeyBindingUI keyBindingUI) {
            if (allKeyCodes.TryGetValue(newKeyCode,out var uis)) {
                uis.Add(keyBindingUI);
                foreach (KeyBindingUI bindingUI in uis) {
                    bindingUI.UpdateDisplay();
                }
            } else {
                allKeyCodes.Add(newKeyCode,new List<KeyBindingUI>{keyBindingUI});
                keyBindingUI.UpdateDisplay();
            }
        }

        public void RemoveKeyCode(KeyCode keyCode, KeyBindingUI keyBindingUI) {
            if (allKeyCodes.TryGetValue(keyCode, out var uis)) {
                uis.Remove(keyBindingUI);
                foreach (KeyBindingUI bindingUI in uis) {
                    bindingUI.UpdateDisplay();
                }
            } else {
                Debug.LogError("找不到此keycode:"+keyCode);
            }
        }

        public void AddModKeyBinding(ModInfo modInfo, string key, KeyBindingUI keyBindingUI) {
            if (modKeyBindingDic.TryGetValue(modInfo.publishedFileId,out var modKeyBinding)) {
                modKeyBinding.AddKeyBindingUI(key, keyBindingUI);
            } else {
                ModKeyBinding keyBinding = new ModKeyBinding(modInfo);
                keyBinding.AddKeyBindingUI(key, keyBindingUI);
                modKeyBindingDic.Add(modInfo.publishedFileId,keyBinding);
            }
        }

        public void RemoveModKeyBinding(ModInfo modInfo, string key) {
            if (!modKeyBindingDic.TryGetValue(modInfo.publishedFileId, out var modKeyBinding)) return;
            KeyBindingUI keyBindingUI=modKeyBinding.GetKeyBindingUI(key);
            if(keyBindingUI==null)return;
            RemoveKeyCode(keyBindingUI.CurrentKey,keyBindingUI);
            modKeyBinding.RemoveKeyBindingUI(key);
            if (!modKeyBinding.HasKeyBindingUI()) {
                modKeyBindingDic.Remove(modInfo.publishedFileId);
                Debug.Log("按键绑定已经没了，移除绑定");
            }
        }
        public void RemoveModKeyBinding(ModInfo modInfo) {
            if (!modKeyBindingDic.Remove(modInfo.publishedFileId, out var modKeyBinding)) {
                Debug.LogError("同步错误/没有绑定ui");
                return;
            }
            List<KeyBindingUI> keyBindingUIs=modKeyBinding.GetKeyBindingUIs();
            foreach (KeyBindingUI keyBindingUI in keyBindingUIs) {
                RemoveKeyCode(keyBindingUI.CurrentKey,keyBindingUI);
            }
            modKeyBinding.Clear();
        }

        public Color GetButtonColor(KeyCode keyCode) {
            if (allKeyCodes.TryGetValue(keyCode,out var uis)) {
                if (uis.Count==1) return Color.clear;
                if (uis.Count==2) return Color.yellow;
            } else {
                Debug.LogError("此keyCode没有加入字典中");
            }
            return Color.red;
        }
    }

    public class ModKeyBinding {
        private ModInfo modInfo;
        private readonly Dictionary<string, KeyBindingUI> keyBindingUiDic = new Dictionary<string, KeyBindingUI>();

        public ModKeyBinding(ModInfo modInfo) {
            this.modInfo = modInfo;
        }

        public void AddKeyBindingUI(string key, KeyBindingUI keyBindingUI) {
            if (keyBindingUiDic.TryGetValue(key,out _)) {
                Debug.LogError("已经添加过此key,key:"+key);
            } else {
                keyBindingUiDic.Add(key,keyBindingUI);
            }
        }
        public void RemoveKeyBindingUI(string key) {
            if (!keyBindingUiDic.Remove(key)) Debug.LogError("删除失败:"+key);
        }

        public bool HasKeyBindingUI() => keyBindingUiDic.Values.Count > 0;

        public KeyBindingUI GetKeyBindingUI(string key) {
            keyBindingUiDic.TryGetValue(key, out var keyBindingUI);
            return keyBindingUI;
        }

        public List<KeyBindingUI> GetKeyBindingUIs() {
            return keyBindingUiDic.Values.ToList();
        }

        public void Clear() {
            keyBindingUiDic.Clear();
        }
    }
}