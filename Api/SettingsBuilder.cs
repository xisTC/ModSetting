using System;
using System.Collections.Generic;
using Duckov.Modding;
using ModSetting.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Api {
    public class SettingsBuilder {
        private ModInfo modInfo;
        private static readonly Dictionary<string, SettingsBuilder> allSetting = new();
        private SettingsBuilder(ModInfo modInfo) {
            this.modInfo = modInfo;
        }
        public static SettingsBuilder Create(ModInfo modInfo) {
            if (modInfo.IsEmpty()) {
                Logger.Error("SettingsBuilder初始化失败，不能使用空的info进行初始化");
                return null;
            }
            if (allSetting.TryGetValue(modInfo.GetModId(),out var builder)) return builder;
            SettingsBuilder settingsBuilder = new SettingsBuilder(modInfo);
            allSetting.Add(modInfo.GetModId(),settingsBuilder);
            return settingsBuilder;
        }
        public SettingsBuilder AddDropdownList(string key, string description,
            List<string> options, string defaultValue, Action<string> onValueChange = null) {
            if (Available(key)) ModBehaviour.AddDropDownList(modInfo,key, description, options, defaultValue, onValueChange);
            return this;
        }
        public SettingsBuilder AddSlider(string key, string description,
            float defaultValue, Vector2 sliderRange, Action<float> onValueChange = null, int decimalPlaces = 1,
            int characterLimit = 5) {
            if (Available(key)) ModBehaviour.AddSlider(modInfo,key, description, defaultValue, sliderRange, onValueChange, decimalPlaces, characterLimit);
            return this;
        }
        public SettingsBuilder AddSlider(string key, string description,
            int defaultValue, int minValue, int maxValue, Action<int> onValueChange = null, int characterLimit = 5) {
            if (Available(key)) ModBehaviour.AddSlider(modInfo,key, description, defaultValue, minValue, maxValue, onValueChange, characterLimit);
            return this;
        }
        public SettingsBuilder AddToggle(string key, string description,
            bool enable, Action<bool> onValueChange = null) {
            if (Available(key)) ModBehaviour.AddToggle(modInfo,key, description, enable, onValueChange);
            return this;
        }
        public SettingsBuilder AddKeybinding(string key, string description,
            KeyCode keyCode, KeyCode defaultKeyCode=KeyCode.None, Action<KeyCode> onValueChange = null) {
            if (Available(key)) ModBehaviour.AddKeybindingWithDefault(modInfo,key, description, keyCode, defaultKeyCode, onValueChange);
            return this;
        }

        public SettingsBuilder AddKeybinding(string key, string description,
            Key currentKey, Key defaultKey = Key.None, Action<Key> onValueChange = null) {
            if(Available(key))ModBehaviour.AddKeybindingWithKey(modInfo,key, description, currentKey, defaultKey, onValueChange);
            return this;
        }

        public SettingsBuilder AddInput(string key, string description,
            string defaultValue, int characterLimit = 40, Action<string> onValueChange = null) {
            if (Available(key)) ModBehaviour.AddInput(modInfo,key, description, defaultValue, characterLimit, onValueChange);
            return this;
        }
        public SettingsBuilder AddButton(string key, string description,
            string buttonText = "按钮", Action onClickButton = null) {
            if (Available(key)) ModBehaviour.AddButton(modInfo,key, description, buttonText, onClickButton);
            return this;
        }
        public SettingsBuilder AddGroup(string key, string description, List<string> keys,
            float scale = 0.7f, bool topInsert = false, bool open = false) {
            if (Available(key)) ModBehaviour.AddGroup(modInfo,key, description, keys, scale, topInsert, open);
            return this;
        }
        public SettingsBuilder GetValue<T>(string key, Action<T> callback = null) {
            if (Available(key)) ModBehaviour.GetValue(modInfo,key, callback);
            return this;
        }
        public SettingsBuilder SetValue<T>(string key, T value, Action<bool> callback = null) {
            if (Available(key)) ModBehaviour.SetValue(modInfo,key, value, callback);
            return this;
        }
        public bool HasConfig() {
            if (!Available()) return false;
            return ModBehaviour.HasConfig(modInfo);
        }
        public bool GetSavedValue<T>(string key, out T value) {
            value = default;
            if (!Available(key)) return false;
            return ModBehaviour.GetSavedValue(modInfo, key, out value);
        }
        public SettingsBuilder RemoveUI(string key, Action<bool> callback = null) {
            if (Available(key)) ModBehaviour.RemoveUI(modInfo,key, callback);
            return this;
        }
        public SettingsBuilder RemoveMod(Action<bool> callback = null) {
            if (Available()) ModBehaviour.RemoveMod(modInfo,callback);
            return this;
        }
        public SettingsBuilder Clear(Action<bool> callback = null) {
            if (Available()) ModBehaviour.Clear(modInfo,callback);
            return this;
        }
        private bool Available() {
            if (!ModBehaviour.Enable) {
                Logger.Error($"(Mod:{modInfo.displayName})modSetting未启用,添加失败");
                return false;
            }
            return !modInfo.IsEmpty();
        }

        private bool Available(string key) => Available() && key != null;
    }
}