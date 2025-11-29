using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Modding;
using Duckov.Options.UI;
using Duckov.Utilities;
using ModSetting.Config;
using ModSetting.Config.Data;
using ModSetting.Extensions;
using ModSetting.Pool;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.UI {
    public abstract class PanelUI : MonoBehaviour {
        protected OptionsPanel_TabButton modTabButton;
        protected GameObject modContent;
        protected OptionsPanel optionsPanel;
        protected readonly Dictionary<string, TitleUI> titleUiDic = new();
        private KeyBindingManager keyBindingManager;
        private TextMeshProUGUI tabName;
        protected static float uiLenght = 1416.03f;
        public bool IsInit { get; protected set; }
        public abstract void Init();
        protected void InitTab() {
            if (optionsPanel == null) throw new Exception("找不到optionsPanel");
            List<OptionsPanel_TabButton> tabButtons = optionsPanel.GetInstanceField<List<OptionsPanel_TabButton>>("tabButtons");
            if (tabButtons == null) {
                Logger.Error($"反射获取tabButtons失败,找不到字段tabButtons");
                return;
            }
            OptionsPanel_TabButton tabButton = tabButtons
                .Where(button=>button!=optionsPanel.GetSelection())
                .FirstOrDefault(item => item != null);
            if (tabButton == null) {
                Logger.Error($"找不到不为null的tab");
                return;
            }
            // 复制一个tabButton的游戏对象
            GameObject tabButtonGameObjectClone =
                Instantiate(tabButton.gameObject, tabButton.transform.parent);
            modTabButton = tabButtonGameObjectClone.GetComponent<OptionsPanel_TabButton>();
            if (modTabButton == null) {
                Logger.Error($"无法获取克隆的GameObject的OptionsPanel_TabButton组件");
                DestroySafely(tabButtonGameObjectClone);
                return;
            }
            modTabButton.name = "modTab";
            // 获取原始tab并克隆
            var tab = modTabButton.GetInstanceField<GameObject>("tab");
            if (tab == null) {
                Logger.Error($"反射获取tabButton的tab成员失败");
                DestroySafely(tabButtonGameObjectClone);
                return;
            }
            modContent = Instantiate(tab, tab.transform.parent);
            modContent.name = "modContent";
            modContent.transform.DestroyAllChildren();
            // 设置克隆的tab到tabButton            
            bool result = modTabButton.SetInstanceField("tab", modContent);
            if (!result) {
                Logger.Error($"反射修改tab成员失败!");
                DestroySafely(tabButtonGameObjectClone);
                DestroySafely(modContent);
                return;
            }
            // 添加到tabButtons列表
            tabButtons.Add(modTabButton);
            // 调用Setup更新UI
            optionsPanel.InvokeInstanceMethod("Setup");
            tabName = modTabButton.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tabName != null) {
                // 移除本地化组件, 保证文本设置正常
                TextLocalizor localizor = modTabButton.GetComponentInChildren<TextLocalizor>(true);
                if (localizor != null)
                    Destroy(localizor);
                string tabNameText = ModLocalizationManager.GetText(ModLocalizationManager.TAB_NAME);
                tabName.text = tabNameText;
                ModLocalizationManager.onLanguageChanged += OnLanguageChanged;
            }
        }

        #region 添加组件
        public bool AddDropDownList(ModInfo modInfo, DropDownConfig dropDownConfig,
            Action<string> onValueChange = null) {
            if (modContent == null) return false;
            DropDownUI dropDownUI = UIPrefabFactory.Spawn<DropDownUI>(modContent.transform);
            dropDownUI.name += $"[{modInfo.name}]:{dropDownConfig.Key}";
            dropDownUI.Setup(dropDownConfig);
            dropDownUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, dropDownConfig.Key, dropDownUI);
            return true;
        }

        public bool AddSlider(ModInfo modInfo, SliderConfig sliderConfig, Action<float> onValueChange = null) {
            if (modContent == null) return false;
            SliderUI sliderUI = UIPrefabFactory.Spawn<SliderUI>(modContent.transform);
            sliderUI.name += $"[{modInfo.name}]:{sliderConfig.Key}";
            sliderUI.Setup(sliderConfig);
            sliderUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, sliderConfig.Key, sliderUI);
            return true;
        }

        public bool AddToggle(ModInfo modInfo, ToggleConfig toggleConfig, Action<bool> onValueChange = null) {
            if (modContent == null) return false;
            ToggleUI toggleUI = UIPrefabFactory.Spawn<ToggleUI>(modContent.transform);
            toggleUI.name += $"[{modInfo.name}]:{toggleConfig.Key}";
            toggleUI.Setup(toggleConfig);
            toggleUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, toggleConfig.Key, toggleUI);
            return true;
        }

        public bool AddKeybinding(ModInfo modInfo, KeyBindingConfig keyBindingConfig,List<KeyCode> validKeyCodes,
            Action<KeyCode> onValueChange = null) {
            if (modContent == null) return false;
            KeyBindingUI keyBindingUI = UIPrefabFactory.Spawn<KeyBindingUI>(modContent.transform);
            keyBindingUI.name += $"[{modInfo.name}]:{keyBindingConfig.Key}";
            keyBindingUI.Setup(keyBindingConfig, keyBindingManager,validKeyCodes);
            keyBindingManager.AddModKeyBinding(modInfo, keyBindingConfig.Key, keyBindingUI);
            keyBindingUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, keyBindingConfig.Key, keyBindingUI);
            return true;
        }
        public bool AddInput(ModInfo modInfo, InputConfig inputConfig, Action<string> onValueChange) {
            if (modContent == null) return false;
            InputUI inputUI = UIPrefabFactory.Spawn<InputUI>(modContent.transform);
            inputUI.name += $"[{modInfo.name}]:{inputConfig.Key}";
            inputUI.Setup(inputConfig);
            inputUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, inputConfig.Key, inputUI);
            return true;
        }

        public bool AddButton(ModInfo modInfo,string key,string description, string buttonText,Action onClickButton) {
            if (modContent == null) return false;
            ButtonUI buttonUI = UIPrefabFactory.Spawn<ButtonUI>(modContent.transform);
            buttonUI.name += $"[{modInfo.name}]:{key}";
            buttonUI.Setup(description, buttonText);
            buttonUI.onClickButton += onClickButton;
            AddUnderTheTitle(modInfo, key, buttonUI);
            return true;
        }

        public bool AddGroup(ModInfo modInfo, string key, string description, List<string> keys, float scale,bool top,bool open) {
            if (modContent == null) return false;
            if (keys.Contains(key)) return false;
            TitleUI titleUI = AddOrGetTitle(modInfo);
            GroupUI groupUI = UIPrefabFactory.Spawn<GroupUI>(modContent.transform);
            groupUI.name += $"[{modInfo.name}]:{key}";
            groupUI.Setup(modInfo,description,keys,scale,open);
            titleUI.AddGroup(key,groupUI,keys,top);
            return true;
        }
        public bool RemoveUI(ModInfo modInfo, string key) {
            if (titleUiDic.TryGetValue(modInfo.GetModId(), out var titleUI)) {
                return titleUI.RemoveUI(key);
            }
            return false;
        }

        public bool RemoveTitle(ModInfo info) {
            if (titleUiDic.Remove(info.GetModId(), out var titleUI)) {
                keyBindingManager.RemoveModKeyBinding(info);
                titleUI.Clear();
                UIPrefabFactory.ReturnToPool(titleUI);
                return true;
            }
            return false;
        }

        public bool HasTitle(ModInfo info) => titleUiDic.ContainsKey(info.GetModId());

        private void AddUnderTheTitle(ModInfo modInfo, string key, PoolableBehaviour uiGo) {
            TitleUI titleUI = AddOrGetTitle(modInfo);
            titleUI.Add(key, uiGo);
        }

        protected abstract TitleUI AddOrGetTitle(ModInfo modInfo);
        #endregion
        private void OnEnable() {
            Init();
            keyBindingManager = new KeyBindingManager();
            Setting.OnTitleSpaceChanged += Setting_OnTitleSpaceChanged;
            ChildOnEnable();
        }

        private void OnDisable() {
            //还要移除panel中的引用
            List<OptionsPanel_TabButton> tabButtons =
                optionsPanel.GetInstanceField<List<OptionsPanel_TabButton>>("tabButtons");
            tabButtons.Remove(modTabButton);
            OptionsPanel_TabButton firstTabButton = tabButtons.FirstOrDefault(tab=>tab!=null);
            if (firstTabButton!=null)optionsPanel.SetSelection(firstTabButton);
            ModLocalizationManager.onLanguageChanged -= OnLanguageChanged;
            Setting.OnTitleSpaceChanged -= Setting_OnTitleSpaceChanged;
            DestroySafely(modTabButton);
            DestroySafely(modContent);
            ChildOnDisable();
        }

        protected virtual void ChildOnEnable() {
        }

        protected virtual void ChildOnDisable() {
        }

        protected void DestroySafely(GameObject go) {
            if (go != null) {
                Destroy(go);
            }
        }

        protected void DestroySafely(Component component) {
            if (component != null) {
                DestroySafely(component.gameObject);
            }
        }
        
        private void OnLanguageChanged(SystemLanguage obj) {
            tabName.text =  ModLocalizationManager.GetText(ModLocalizationManager.TAB_NAME);
        }
        private void Setting_OnTitleSpaceChanged(float obj) {
            foreach (TitleUI titleUI in titleUiDic.Values) {
                titleUI.UpdateSpace(obj);
            }
        }
    }
}