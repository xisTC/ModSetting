using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Modding;
using ModSetting.Config;
using ModSetting.Extensions;
using ModSetting.UI;
using SodaCraft.Localizations;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = ModSetting.Log.Logger;

namespace ModSetting {
    public class ModBehaviour : Duckov.Modding.ModBehaviour{
        private static bool isInit;
        private static GlobalPanelUI globalPanelUI;
        private static MainMenuPanelUI mainMenuPanelUI;
        private static readonly Queue<Action> actionQueue = new Queue<Action>();
        public static float Version= 0.5f;
        public static readonly Version VERSION = new Version(0, 5, 0);
        private const string MOD_NAME = "ModSetting";
        public static bool Enable { get; private set; }
        private void OnEnable() {
            Logger.Warning("ModSetting:启用");
            MainMenu.OnMainMenuAwake += Init;
            ModManager.OnModWillBeDeactivated += OnModWillBeDeactivated;
            LocalizationManager.OnSetLanguage += ModLocalizationManager.OnLanguageChanged;
            Enable = true;
        }

        private void OnDisable() {
            Logger.Info("ModSetting:禁用");
            Enable = false;
            MainMenu.OnMainMenuAwake -= Init;
            ModManager.OnModWillBeDeactivated -= OnModWillBeDeactivated;
            LocalizationManager.OnSetLanguage -= ModLocalizationManager.OnLanguageChanged;
            isInit = false;
            actionQueue.Clear();
            Saver.Save();
            Saver.Clear();
            ConfigManager.Clear();
            ModLocalizationManager.Clear();
            KeyCodeConverter.Clear();
            Setting.Clear();
        }

        private void Update() {
            ProcessPendingConfigs();
        }

        private void Init() {
            if (FindAnyObjectByType(typeof(MainMenu)) != null) {
                if (!isInit) {
                    Logger.Info($"开始初始化mod设置");
                    mainMenuPanelUI = gameObject.AddComponent<MainMenuPanelUI>();
                    globalPanelUI = gameObject.AddComponent<GlobalPanelUI>();
                    isInit = true;
                } else {
                    mainMenuPanelUI.ResetTab();
                }
            }
        }

        private void ProcessPendingConfigs() {
            if (!isInit ||!globalPanelUI.IsInit || !mainMenuPanelUI.IsInit) return;
            if (actionQueue.Count == 0) return;
            while (actionQueue.Count > 0) {
                Action action = actionQueue.Dequeue();
                action?.Invoke();
            }
            Logger.Info($"队列执行完毕");
        }

        private static void AddAction(ModInfo modInfo,Action addConfigAction) {
            if (isInit&&globalPanelUI.IsInit && mainMenuPanelUI.IsInit&&actionQueue.Count == 0) {
                try {
                    addConfigAction?.Invoke();
                } catch (Exception e) {
                    Logger.Exception("ModSetting异常",e);
                }
            } else {
                actionQueue.Enqueue(addConfigAction);
                Logger.Info($"{modInfo.name}加入队列，等待mod菜单初始化。当前队列长度: {actionQueue.Count}");
            }
        }

        public static void AddDropDownList(ModInfo modInfo,string key,string description,
            List<string> options, string defaultValue,Action<string> onValueChange=null) {
            if (!options.Contains(defaultValue)) {
                Logger.Warning($"下拉列表options不包含此默认值,默认值:{defaultValue}");
                options.Add(defaultValue);
            }
            AddAction(modInfo,() => {
                if (HasKey(modInfo, key)) return;
                Logger.Info($"(Mod:{modInfo.displayName})添加下拉列表,key:{key}");
                DropDownConfig dropDownConfig = new DropDownConfig(key, description, options, defaultValue);
                ConfigManager.AddConfig(modInfo, dropDownConfig);
                globalPanelUI.AddDropDownList(modInfo,dropDownConfig,onValueChange);
                mainMenuPanelUI.AddDropDownList(modInfo,dropDownConfig,onValueChange);
            });
        }

        public static void AddSlider(ModInfo modInfo,string key,string description,
            float defaultValue, Vector2 sliderRange,Action<float> onValueChange=null,
            int decimalPlaces=1,int characterLimit=5) {
            if (sliderRange.x > sliderRange.y) sliderRange = new Vector2(sliderRange.y, sliderRange.x);
            defaultValue = Math.Clamp(defaultValue, sliderRange.x, sliderRange.y);
            AddAction(modInfo,() => {
                if (HasKey(modInfo, key)) return;
                Logger.Info($"(Mod:{modInfo.displayName})添加滑块,key:{key}");
                SliderConfig sliderConfig = new SliderConfig(key, description,defaultValue,sliderRange,decimalPlaces,characterLimit);
                ConfigManager.AddConfig(modInfo, sliderConfig);
                globalPanelUI.AddSlider(modInfo,sliderConfig,onValueChange);
                mainMenuPanelUI.AddSlider(modInfo,sliderConfig,onValueChange);
            });
        }

        public static void AddSlider(ModInfo modInfo, string key, string description,
            int defaultValue,int minValue,int maxValue, Action<int> onValueChange = null,int characterLimit=5) {
            minValue = minValue < maxValue ? minValue : maxValue;
            maxValue = minValue < maxValue ? maxValue : minValue;
            defaultValue = Math.Clamp(defaultValue, minValue, maxValue);
            AddAction(modInfo,() => {
                if (HasKey(modInfo, key)) return;
                Logger.Info($"(Mod:{modInfo.displayName})添加滑块,key:{key}");
                SliderConfig sliderConfig = new SliderConfig(key, description,defaultValue,new Vector2(minValue,maxValue),0,characterLimit);
                ConfigManager.AddConfig(modInfo, sliderConfig);
                Action<float> floatCallback = onValueChange != null ? 
                    floatValue => onValueChange((int)floatValue) :
                    null;
                globalPanelUI.AddSlider(modInfo,sliderConfig,floatCallback);
                mainMenuPanelUI.AddSlider(modInfo,sliderConfig,floatCallback);
            });
        }

        public static void AddToggle(ModInfo modInfo,string key,string description,
            bool enable, Action<bool> onValueChange = null) {
            AddAction(modInfo,() => {
                if (HasKey(modInfo, key)) return;
                Logger.Info($"(Mod:{modInfo.displayName})添加开关,key:{key}");
                ToggleConfig toggleConfig = new ToggleConfig(key, description, enable);
                ConfigManager.AddConfig(modInfo, toggleConfig);
                globalPanelUI.AddToggle(modInfo,toggleConfig,onValueChange);
                mainMenuPanelUI.AddToggle(modInfo,toggleConfig,onValueChange);
            });
        }

        public static void AddKeybindingWithDefault(ModInfo modInfo,string key,string description,
            KeyCode keyCode,KeyCode defaultKeyCode,Action<KeyCode> onValueChange=null) {
            AddAction(modInfo,() => {
                if (HasKey(modInfo, key)) return;
                Logger.Info($"(Mod:{modInfo.displayName})添加按键绑定,key:{key}");
                KeyBindingConfig keyBindingConfig = new KeyBindingConfig(key,description,keyCode,defaultKeyCode);
                ConfigManager.AddConfig(modInfo, keyBindingConfig);
                List<KeyCode> keyCodes = ((KeyCode[])Enum.GetValues(typeof(KeyCode))).ToList();
                globalPanelUI.AddKeybinding(modInfo, keyBindingConfig,keyCodes,onValueChange);
                mainMenuPanelUI.AddKeybinding(modInfo, keyBindingConfig,keyCodes,onValueChange);
            });
        }

        public static void AddKeybinding(ModInfo modInfo, string key, string description,
            KeyCode keyCode, Action<KeyCode> onValueChange = null) {
            AddKeybindingWithDefault(modInfo, key, description, keyCode,KeyCode.None,onValueChange);
        }

        public static void AddKeybindingWithKey(ModInfo modInfo, string key, string description,
            Key currentKey,Key defaultKey=Key.None,Action<Key> onValueChange = null) {
            AddAction(modInfo,() => {
                if (HasKey(modInfo, key)) return;
                Logger.Info($"(Mod:{modInfo.displayName})添加按键绑定,key:{key}");
                KeyBindingConfig keyBindingConfig = new KeyBindingConfig(key,description,currentKey.ToKeyCode(),defaultKey.ToKeyCode());
                ConfigManager.AddConfig(modInfo, keyBindingConfig);
                List<KeyCode> keyCodes = ((Key[])Enum.GetValues(typeof(Key))).Select(item=>item.ToKeyCode()).ToList();
                globalPanelUI.AddKeybinding(modInfo, keyBindingConfig,keyCodes,keyCode=>onValueChange?.Invoke(keyCode.ToKey()));
                mainMenuPanelUI.AddKeybinding(modInfo, keyBindingConfig,keyCodes,keyCode=>onValueChange?.Invoke(keyCode.ToKey()));
            });
        }

        public static void AddInput(ModInfo modInfo,string key,string description,
            string defaultValue,int characterLimit=40,Action<string> onValueChange=null) {
            AddAction(modInfo,() => {
                if (HasKey(modInfo, key)) return;
                Logger.Info($"(Mod:{modInfo.displayName})添加输入框,key:{key}");
                InputConfig inputConfig = new InputConfig(key,description,defaultValue,characterLimit);
                ConfigManager.AddConfig(modInfo, inputConfig);
                globalPanelUI.AddInput(modInfo, inputConfig,onValueChange);
                mainMenuPanelUI.AddInput(modInfo, inputConfig,onValueChange);
            });
        }

        public static void AddButton(ModInfo modInfo, string key, string description,
            string buttonText="按钮", Action onClickButton = null) {
            AddAction(modInfo,() => {
                if (HasKey(modInfo, key)) return;
                Logger.Info($"(Mod:{modInfo.displayName})添加按钮,key:{key}");
                ConfigManager.AddKey(modInfo,key);
                globalPanelUI.AddButton(modInfo,key, description, buttonText, onClickButton);
                mainMenuPanelUI.AddButton(modInfo,key,description,buttonText,onClickButton);
            });
        }

        public static void AddGroup(ModInfo modInfo, string key, string description,
            List<string> keys, float scale=0.7f,bool topInsert=false,bool open=false) {
            if (keys == null || keys.Count == 0) {
                Logger.Error($"group的keys不能为空");
                return;
            }
            if (keys.Contains(key)) keys.Remove(key);
            AddAction(modInfo,() => {
                if (HasKey(modInfo, key)) return;
                foreach (string otherKey in keys) {
                    if (!ConfigManager.HasKey(modInfo,otherKey)) {
                        Logger.Error($"不存在的key无法添加Group,key:{otherKey}");
                        return;
                    }
                }
                Logger.Info($"(Mod:{modInfo.displayName})添加分组,key:{key}");
                ConfigManager.AddKey(modInfo,key);
                scale = Math.Clamp(scale, 0f, 0.9f);
                globalPanelUI.AddGroup(modInfo,key, description, keys, scale,topInsert, open);
                mainMenuPanelUI.AddGroup(modInfo,key,description,keys, scale,topInsert, open);
            });
        }

        public static void GetValue<T>(ModInfo modInfo, string key,Action<T> callback=null) {
            AddAction(modInfo,() => {
                Logger.Info($"(Mod:{modInfo.displayName})获取值,key:{key}");
                T value = ConfigManager.GetValue<T>(modInfo, key);
                callback?.Invoke(value);
            });
        }

        public static void SetValue<T>(ModInfo modInfo, string key,T value,Action<bool> callback=null) {
            AddAction(modInfo,() => {
                Logger.Info($"(Mod:{modInfo.displayName})设置值,key:{key};value:{value}");
                bool result = ConfigManager.SetValue<T>(modInfo, key, value);
                callback?.Invoke(result);
            });
        }

        public static bool HasConfig(ModInfo modInfo) => Saver.HasValue(modInfo.GetModId());

        public static bool GetSavedValue<T>(ModInfo modInfo, string key, out T value) {
            Logger.Info($"(Mod:{modInfo.displayName})获取保存值,key:{key}");
            value = Saver.GetValue<T>(modInfo.GetModId(),key);
            return Saver.HasValue(modInfo.GetModId(), key);
        }
        public static void RemoveUI(ModInfo modInfo, string key,Action<bool> callback=null) {
            AddAction(modInfo,() => {
                if (ConfigManager.RemoveUI(modInfo, key)) {
                    Logger.Info($"(Mod:{modInfo.displayName})移除ui,key:{key}");
                    if (globalPanelUI.RemoveUI(modInfo,key)&&mainMenuPanelUI.RemoveUI(modInfo,key)) {
                        callback?.Invoke(true);
                        return;
                    }
                    Logger.Error($"UI和ConfigManager不同步");
                }
                callback?.Invoke(false);
            });
        }

        public static void RemoveMod(ModInfo modInfo,Action<bool> callback=null) {
            AddAction(modInfo,() => {
                Logger.Info($"(Mod:{modInfo.displayName})移除mod设置");
                bool result = globalPanelUI.RemoveTitle(modInfo)&& mainMenuPanelUI.RemoveTitle(modInfo) &&ConfigManager.RemoveMod(modInfo);
                callback?.Invoke(result);
            });
        }

        public static void Clear(ModInfo modInfo,Action<bool> callback=null) {
            AddAction(modInfo,() => {
                Logger.Info($"(Mod:{modInfo.displayName})清空mod设置");
                ModConfig modConfig = ConfigManager.GetConfig(modInfo);
                if (modConfig == null) {
                    callback?.Invoke(false);
                    return;
                }
                HashSet<string> activeKeys = modConfig.GetActiveKeys();
                List<string> list = new List<string>();
                foreach (var key in activeKeys) list.Add(key);
                foreach (string activeKey in list) {
                    RemoveUI(modInfo,activeKey);
                }
                callback?.Invoke(true);
            });
        }

        private static bool HasKey(ModInfo modInfo, string key) {
            if (ConfigManager.HasKey(modInfo, key)) {
                Logger.Warning($"(Mod:{modInfo.displayName})已经有相同的key无法添加,key:{key}");
                return true;
            }
            return false;
        }

        protected override void OnAfterSetup() {
            Saver.Load();
            ModLocalizationManager.Init();
            Setting.Init(info);
            KeyCodeConverter.Init();
            if(!isInit)Init();
        }

        private void OnModWillBeDeactivated(ModInfo arg1, Duckov.Modding.ModBehaviour arg2) {
            if (globalPanelUI.HasTitle(arg1)) {
                globalPanelUI.RemoveTitle(arg1);
                mainMenuPanelUI.RemoveTitle(arg1);
                Saver.UpdateConfig(ConfigManager.GetConfig(arg1));
                ConfigManager.RemoveMod(arg1);
                Logger.Info($"(Mod:{arg1.displayName})禁用,移除UI");
            }
        }
    }
}