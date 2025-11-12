using System;
using System.Collections.Generic;
using Duckov.Modding;
using ModSetting.Config;
using ModSetting.Extensions;
using ModSetting.UI;
using SodaCraft.Localizations;
using UnityEngine;

namespace ModSetting {
    public class ModBehaviour : Duckov.Modding.ModBehaviour{
        private static bool isInit;
        private static GlobalPanelUI globalPanelUI;
        private static MainMenuPanelUI mainMenuPanelUI;
        private static readonly Queue<Action> actionQueue = new Queue<Action>();
        public static float Version= 0.4f;
        public static readonly Version VERSION = new Version(0, 4, 0);
        private const string MOD_NAME = "ModSetting";
        public static bool Enable { get; private set; }
        private void OnEnable() {
            Debug.Log("ModSetting:启用");
            Enable = true;
            MainMenu.OnMainMenuAwake += Init;
            ModManager.OnModWillBeDeactivated += OnModWillBeDeactivated;
            LocalizationManager.OnSetLanguage += ModLocalizationManager.OnLanguageChanged;
            ModLocalizationManager.Init();
            if(!isInit)Init();
            Saver.Load();
        }

        private void OnDisable() {
            Debug.Log("ModSetting:禁用");
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
        }

        private void Update() {
            ProcessPendingConfigs();
        }

        private void Init() {
            if (FindAnyObjectByType(typeof(MainMenu)) != null) {
                if (!isInit) {
                    Debug.Log("开始初始化mod设置");
                    globalPanelUI = gameObject.AddComponent<GlobalPanelUI>();
                    mainMenuPanelUI = gameObject.AddComponent<MainMenuPanelUI>();
                    isInit = true;
                } else {
                    mainMenuPanelUI.Init();
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
            Debug.Log("添加完毕");
        }

        private static void AddAction(ModInfo modInfo,Action addConfigAction) {
            if (modInfo.name == MOD_NAME) {
                Debug.LogError("不能使用ModSetting信息");
                return;
            }
            if (isInit&&globalPanelUI.IsInit && mainMenuPanelUI.IsInit) {
                try {
                    addConfigAction?.Invoke();
                    // Debug.Log($"{modInfo.name}执行添加UI");
                } catch (Exception e) {
                    Debug.LogError("添加UI异常:"+e.StackTrace);
                }
            } else {
                actionQueue.Enqueue(addConfigAction);
                Debug.Log($"{modInfo.name}加入队列，等待mod菜单初始化。当前队列长度: {actionQueue.Count}");
            }
        }

        public static void AddDropDownList(ModInfo modInfo,string key,string description,
            List<string> options, string defaultValue,Action<string> onValueChange=null) {
            if (!options.Contains(defaultValue)) {
                Debug.LogWarning("下拉列表options不包含此默认值,默认值:"+defaultValue);
                options.Add(defaultValue);
            }
            AddAction(modInfo,() => {
                if (ConfigManager.HasKey(modInfo, key)) {
                    Debug.LogWarning("已经有相同的key无法添加,key:"+key);
                    return;
                }
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
                if (ConfigManager.HasKey(modInfo, key)) {
                    Debug.LogWarning("已经有相同的key无法添加,key:"+key);
                    return;
                }
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
                if (ConfigManager.HasKey(modInfo, key)) {
                    Debug.LogWarning("已经有相同的key无法添加,key:"+key);
                    return;
                }
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
                if (ConfigManager.HasKey(modInfo, key)) {
                    Debug.LogWarning("已经有相同的key无法添加,key:"+key);
                    return;
                }
                ToggleConfig toggleConfig = new ToggleConfig(key, description, enable);
                ConfigManager.AddConfig(modInfo, toggleConfig);
                globalPanelUI.AddToggle(modInfo,toggleConfig,onValueChange);
                mainMenuPanelUI.AddToggle(modInfo,toggleConfig,onValueChange);
            });
        }

        public static void AddKeybindingWithDefault(ModInfo modInfo,string key,string description,
            KeyCode keyCode,KeyCode defaultKeyCode,Action<KeyCode> onValueChange=null) {
            AddAction(modInfo,() => {
                if (ConfigManager.HasKey(modInfo, key)) {
                    Debug.LogWarning("已经有相同的key无法添加,key:"+key);
                    return;
                }
                KeyBindingConfig keyBindingConfig = new KeyBindingConfig(key,description,keyCode,defaultKeyCode);
                ConfigManager.AddConfig(modInfo, keyBindingConfig);
                globalPanelUI.AddKeybinding(modInfo, keyBindingConfig,onValueChange);
                mainMenuPanelUI.AddKeybinding(modInfo, keyBindingConfig,onValueChange);
            });
        }

        public static void AddKeybinding(ModInfo modInfo, string key, string description,
            KeyCode keyCode, Action<KeyCode> onValueChange = null) {
            AddKeybindingWithDefault(modInfo, key, description, keyCode,KeyCode.None,onValueChange);
        }

        public static void AddInput(ModInfo modInfo,string key,string description,
            string defaultValue,int characterLimit=40,Action<string> onValueChange=null) {
            AddAction(modInfo,() => {
                if (ConfigManager.HasKey(modInfo, key)) {
                    Debug.LogWarning("已经有相同的key无法添加,key:"+key);
                    return;
                }
                InputConfig inputConfig = new InputConfig(key,description,defaultValue,characterLimit);
                ConfigManager.AddConfig(modInfo, inputConfig);
                globalPanelUI.AddInput(modInfo, inputConfig,onValueChange);
                mainMenuPanelUI.AddInput(modInfo, inputConfig,onValueChange);
            });
        }

        public static void AddButton(ModInfo modInfo, string key, string description,
            string buttonText="按钮", Action onClickButton = null) {
            AddAction(modInfo,() => {
                if (ConfigManager.HasKey(modInfo, key)) {
                    Debug.LogWarning("已经有相同的key无法添加,key:"+key);
                    return;
                }
                ConfigManager.AddKey(modInfo,key);
                globalPanelUI.AddButton(modInfo,key, description, buttonText, onClickButton);
                mainMenuPanelUI.AddButton(modInfo,key,description,buttonText,onClickButton);
            });
        }
        public static void AddGroup(ModInfo modInfo, string key, string description,
            List<string> keys, float scale=0.7f,bool topInsert=false,bool open=false) {
            if (keys == null || keys.Count == 0) {
                Debug.LogError("group的keys不能为空");
                return;
            }
            if (keys.Contains(key)) keys.Remove(key);
            AddAction(modInfo,() => {
                if (ConfigManager.HasKey(modInfo, key)) {
                    Debug.LogWarning("已经有相同的key无法添加,key:"+key);
                    return;
                }
                foreach (string otherKey in keys) {
                    if (!ConfigManager.HasKey(modInfo,otherKey)) {
                        Debug.LogError("不存在的key无法添加Group,key:"+otherKey);
                        return;
                    }
                }
                ConfigManager.AddKey(modInfo,key);
                scale = Math.Clamp(scale, 0f, 0.9f);
                globalPanelUI.AddGroup(modInfo,key, description, keys, scale,topInsert, open);
                mainMenuPanelUI.AddGroup(modInfo,key,description,keys, scale,topInsert, open);
            });
        }
        public static void GetValue<T>(ModInfo modInfo, string key,Action<T> callback=null) {
            AddAction(modInfo,() => {
                T value = ConfigManager.GetValue<T>(modInfo, key);
                callback?.Invoke(value);
            });
        }

        public static void SetValue<T>(ModInfo modInfo, string key,T value,Action<bool> callback=null) {
            AddAction(modInfo,() => {
                bool result = ConfigManager.SetValue<T>(modInfo, key, value);
                callback?.Invoke(result);
            });
        }

        public static bool HasConfig(ModInfo modInfo) => Saver.HasValue(modInfo.GetModId());

        public static bool GetSavedValue<T>(ModInfo modInfo, string key, out T value) {
            value = Saver.GetValue<T>(modInfo.GetModId(),key);
            return Saver.HasValue(modInfo.GetModId(), key);
        }

        public static void RemoveUI(ModInfo modInfo, string key,Action<bool> callback=null) {
            AddAction(modInfo,() => {
                if (globalPanelUI.RemoveUI(modInfo,key)) {
                    if (ConfigManager.RemoveUI(modInfo, key)&&mainMenuPanelUI.RemoveUI(modInfo,key)) {
                        callback?.Invoke(true);
                        return;
                    }
                    Debug.LogError("UI和ConfigManager不同步");
                }
                callback?.Invoke(false);
            });
        }

        public static void RemoveMod(ModInfo modInfo,Action<bool> callback=null) {
            AddAction(modInfo,() => {
                bool result = globalPanelUI.RemoveTitle(modInfo)&& mainMenuPanelUI.RemoveTitle(modInfo) &&ConfigManager.RemoveMod(modInfo);
                callback?.Invoke(result);
            });
        }


        private void OnModWillBeDeactivated(ModInfo arg1, Duckov.Modding.ModBehaviour arg2) {
            if (globalPanelUI.HasTitle(arg1)) {
                globalPanelUI.RemoveTitle(arg1);
                mainMenuPanelUI.RemoveTitle(arg1);
                ConfigManager.RemoveMod(arg1);
                Saver.UpdateConfig(ConfigManager.GetConfig(arg1));
            }
        }
    }
}