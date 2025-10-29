using System;
using System.Collections.Generic;
using Duckov.Modding;
using ModSetting.Config;
using ModSetting.UI;
using UnityEngine;
/* TODO Init函数只有在OnMainMenuAwake时调用才能正常的创建tab，其余时候主动调用Init
        函数会出现，点击tabButton无反应的情况。
        目前想到的解决办法:
        1.将OptionsPanel_TabButton中的字段交给我们自己管理，并且需要管理
        OptionsPanel的字段private List<OptionsPanel_TabButton> tabButtons;
        2.直接使用内置的SetSelection函数，不需要将modTabButton设置到OptionsPanel
        的tabButtons中去
*/
namespace ModSetting {
    public class ModBehaviour : Duckov.Modding.ModBehaviour{
        private static bool isInit;
        private static GlobalPanelUI globalPanelUI;
        private static MainMenuPanelUI mainMenuPanelUI;
        private static readonly Queue<Action> actionQueue = new Queue<Action>();
        public static float Version { get; private set; } = 0.1f;
        private void OnEnable() {
            Debug.Log("ModSetting:启用");
            MainMenu.OnMainMenuAwake += Init;
            if(!isInit)Init();
        }
        private void OnDisable() {
            Debug.Log("ModSetting:禁用");
            MainMenu.OnMainMenuAwake -= Init;
            isInit = false;
            actionQueue.Clear();
            ConfigManager.Save();
            ConfigManager.Clear();
        }
        private void Update() {
            ProcessPendingConfigs();
        }
        private void Init() {
            if (FindAnyObjectByType(typeof(MainMenu)) != null) {
                Debug.Log("找到主菜单");
                if (!isInit) {
                    globalPanelUI = gameObject.AddComponent<GlobalPanelUI>();
                    mainMenuPanelUI = gameObject.AddComponent<MainMenuPanelUI>();
                    globalPanelUI.Setup(info.preview);
                    mainMenuPanelUI.Setup(info.preview);
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
            Debug.Log("配置完毕");
        }
         private static void AddAction(Action addConfigAction) {
            if (isInit&&globalPanelUI.IsInit && mainMenuPanelUI.IsInit) {
                addConfigAction?.Invoke();
            } else {
                actionQueue.Enqueue(addConfigAction);
                Debug.Log($"配置项已加入队列，等待mod菜单初始化。当前队列长度: {actionQueue.Count}");
            }
        }

        public static void AddDropDownList(ModInfo modInfo,string key,string description,
            List<string> options, string defaultValue,Action<string> onValueChange=null) {
            AddAction(() => {
                DropDownConfig dropDownConfig = new DropDownConfig(key, description, options, defaultValue);
                ConfigManager.AddDropDownList(modInfo, dropDownConfig);
                globalPanelUI.AddDropDownList(modInfo,dropDownConfig,onValueChange);
                mainMenuPanelUI.AddDropDownList(modInfo,dropDownConfig,onValueChange);
            });
        }

        public static void AddSlider(ModInfo modInfo,string key,string description,
            float defaultValue, Vector2 sliderRange,Action<float> onValueChange=null) {
            AddAction(() => {
                SliderConfig sliderConfig = new SliderConfig(key, description,defaultValue,sliderRange);
                ConfigManager.AddSlider(modInfo, sliderConfig);
                globalPanelUI.AddSlider(modInfo,sliderConfig,onValueChange);
                mainMenuPanelUI.AddSlider(modInfo,sliderConfig,onValueChange);
            });
        }

        public static void AddToggle(ModInfo modInfo,string key,string description,
            bool enable, Action<bool> onValueChange = null) {
            AddAction(() => {
                ToggleConfig toggleConfig = new ToggleConfig(key, description, enable);
                ConfigManager.AddToggle(modInfo, toggleConfig);
                globalPanelUI.AddToggle(modInfo,toggleConfig,onValueChange);
                mainMenuPanelUI.AddToggle(modInfo,toggleConfig,onValueChange);
            });
        }

        public static void AddKeybinding(ModInfo modInfo,string key,string description,
            KeyCode keyCode,Action<KeyCode> onValueChange=null) {
            AddAction(() => {
                KeyBindingConfig keyBindingConfig = new KeyBindingConfig(key, description, keyCode);
                ConfigManager.AddKeybinding(modInfo, keyBindingConfig);
                globalPanelUI.AddKeybinding(modInfo, keyBindingConfig,onValueChange);
                mainMenuPanelUI.AddKeybinding(modInfo, keyBindingConfig,onValueChange);
            });
        }

        public static T GetValue<T>(ModInfo info, string key) => ConfigManager.GetValue<T>(info, key);
        public static bool SetValue<T>(ModInfo info, string key,T value) => ConfigManager.SetValue<T>(info, key,value);
        public static bool RemoveUI<T>(ModInfo info, string key) {
            if (!isInit || !globalPanelUI.IsInit||!mainMenuPanelUI.IsInit) return false;
            if (globalPanelUI.RemoveUI(info,key)) {
                if (ConfigManager.RemoveUI<T>(info, key)&&mainMenuPanelUI.RemoveUI(info,key)) {
                    return true;
                }
                Debug.LogError("UI和ConfigManager不同步");
            }
            return false;
        }
        public static bool RemoveMod(ModInfo info) {
            if (!isInit || !globalPanelUI.IsInit||!mainMenuPanelUI.IsInit) return false;
            return globalPanelUI.RemoveTitle(info)&& mainMenuPanelUI.RemoveTitle(info) &&ConfigManager.RemoveMod(info);
        }
    }
}