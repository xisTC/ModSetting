using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Modding;
using Duckov.Options.UI;
using Duckov.Utilities;
using ModSetting.Config;
using ModSetting.Extensions;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModSetting.UI {
    public abstract class PanelUI : MonoBehaviour {
        protected OptionsPanel_TabButton modTabButton;
        protected GameObject modContent;
        protected OptionsPanel optionsPanel;
        private DropDownUI dropDownPrefab;
        private SliderUI sliderPrefab;
        private ToggleUI togglePrefab;
        private KeyBindingUI keyBindEntryPrefab;
        private TitleUI titlePrefab;
        private InputUI inputPrefab;
        private ButtonUI buttonPrefab;
        private GroupUI groupPrefab;
        public float TitleHeight { get; protected set; }
        private readonly Dictionary<string, TitleUI> titleUiDic = new();
        private KeyBindingManager keyBindingManager;
        private TextMeshProUGUI tabName;
        public bool IsInit { get; protected set; }
        public abstract void Init();
        protected void InitTab() {
            List<OptionsPanel_TabButton> tabButtons = optionsPanel.GetInstanceField<List<OptionsPanel_TabButton>>("tabButtons");
            if (tabButtons == null) {
                Debug.LogError("反射获取tabButtons失败");
                return;
            }
            OptionsPanel_TabButton tabButton = tabButtons
                .Where(button=>button!=optionsPanel.GetSelection())
                .FirstOrDefault(item => item != null);
            if (tabButton == null) {
                Debug.LogError("找不到不为null的tab");
                return;
            }
            // 复制一个tabButton的游戏对象
            GameObject tabButtonGameObjectClone =
                Instantiate(tabButton.gameObject, tabButton.transform.parent);
            modTabButton = tabButtonGameObjectClone.GetComponent<OptionsPanel_TabButton>();
            modTabButton.name = "modTab";
            if (modTabButton == null) {
                Debug.LogError("无法获取克隆的GameObject的OptionsPanel_TabButton组件");
                DestroySafely(tabButtonGameObjectClone);
                return;
            }
            // 获取原始tab并克隆
            var tab = modTabButton.GetInstanceField<GameObject>("tab");
            if (tab == null) {
                Debug.LogError("无法反射获取modTabButton的tab成员");
                DestroySafely(tabButtonGameObjectClone);
                return;
            }
            modContent = Instantiate(tab, tab.transform.parent);
            modContent.name = "modContent";
            modContent.transform.DestroyAllChildren();
            // 设置克隆的tab到tabButton            
            bool result = modTabButton.SetInstanceField("tab", modContent);
            if (!result) {
                Debug.LogError("反射修改tab成员失败!!");
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

        #region 初始化预制件

        protected void InitPrefab() {
            OptionsUIEntry_Dropdown optionDropDown = optionsPanel.gameObject
                .GetComponentsInChildren<OptionsUIEntry_Dropdown>(true)
                .Skip(1)
                .FirstOrDefault(item => item != null);
            CreateDropDownPrefab(optionDropDown);
            CreateTitlePrefab(optionDropDown);
            CreateGroupPrefab(optionDropDown);
            OptionsUIEntry_Slider optionSlider = optionsPanel.gameObject
                .GetComponentsInChildren<OptionsUIEntry_Slider>(true)
                .FirstOrDefault(item => item != null);
            CreateSliderPrefab(optionSlider);
            CreateInputPrefab(optionSlider);
            UIKeybindingEntry keybindingEntry = optionsPanel.gameObject.GetComponentsInChildren<UIKeybindingEntry>(true)
                .FirstOrDefault(item => item != null);
            CreateKeyBindPrefab(keybindingEntry);
            CreateTogglePrefab(keybindingEntry);
            CreateButtonPrefab(keybindingEntry);
            DontDestroyOnLoad(dropDownPrefab);
            DontDestroyOnLoad(sliderPrefab);
            DontDestroyOnLoad(keyBindEntryPrefab);
            DontDestroyOnLoad(togglePrefab);
            DontDestroyOnLoad(titlePrefab);
            DontDestroyOnLoad(inputPrefab);
            DontDestroyOnLoad(buttonPrefab);
            DontDestroyOnLoad(groupPrefab);
        }

        private void CreateTitlePrefab(OptionsUIEntry_Dropdown optionDropDown) {
            if (optionDropDown == null) return;
            OptionsUIEntry_Dropdown titleClone = Instantiate(optionDropDown);
            if (titleClone != null) {
                titleClone.transform.DestroyAllChildren();
                GameObject titleCloneGameObject = titleClone.gameObject;
                DestroyImmediate(titleClone);
                titlePrefab = titleCloneGameObject.AddComponent<TitleUI>();
                titlePrefab.Init();
                titlePrefab.name = "标题";
                Debug.Log("成功创建titlePrefab预制体");
            }
        }

        private void CreateGroupPrefab(OptionsUIEntry_Dropdown optionDropDown) {
            if (optionDropDown == null) return;
            OptionsUIEntry_Dropdown groupClone = Instantiate(optionDropDown);
            if (groupClone != null) {
                groupClone.transform.DestroyAllChildren();
                GameObject groupCloneGameObject = groupClone.gameObject;
                DestroyImmediate(groupClone);
                groupPrefab = groupCloneGameObject.AddComponent<GroupUI>();
                groupPrefab.Init();
                groupPrefab.name = "分组";
                Debug.Log("成功创建groupPrefab预制体");
            }
        }
        private void CreateTogglePrefab(UIKeybindingEntry keybindingEntry) {
            if (keybindingEntry == null) return;
            UIKeybindingEntry toggleClone = Instantiate(keybindingEntry);
            if (toggleClone != null) {
                TextMeshProUGUI label = toggleClone.GetInstanceField<TextMeshProUGUI>("label");
                Button rebindButton = toggleClone.GetInstanceField<Button>("rebindButton");
                Button clearButton = toggleClone.GetInstanceField<Button>("clearButton");
                InputIndicator indicator = toggleClone.GetInstanceField<InputIndicator>("indicator");
                TextMeshProUGUI text = indicator.GetInstanceField<TextMeshProUGUI>("text");
                GameObject toggleCloneGameObject = toggleClone.gameObject;
                DestroyImmediate(clearButton.image);
                DestroyImmediate(indicator);
                DestroyImmediate(clearButton);
                DestroyImmediate(toggleClone);
                togglePrefab = toggleCloneGameObject.AddComponent<ToggleUI>();
                togglePrefab.Init(label, rebindButton, text, "toggle默认文本");
                togglePrefab.name = "开关";
                Debug.Log("成功创建togglePrefab预制体");
            }
        }

        private void CreateButtonPrefab(UIKeybindingEntry keybindingEntry) {
            if (keybindingEntry == null) return;
            UIKeybindingEntry buttonClone = Instantiate(keybindingEntry);
            if (buttonClone != null) {
                TextMeshProUGUI label = buttonClone.GetInstanceField<TextMeshProUGUI>("label");
                Button rebindButton = buttonClone.GetInstanceField<Button>("rebindButton");
                Button clearButton = buttonClone.GetInstanceField<Button>("clearButton");
                InputIndicator indicator = buttonClone.GetInstanceField<InputIndicator>("indicator");
                TextMeshProUGUI text = indicator.GetInstanceField<TextMeshProUGUI>("text");
                GameObject toggleCloneGameObject = buttonClone.gameObject;
                DestroyImmediate(clearButton.image);
                DestroyImmediate(indicator);
                DestroyImmediate(clearButton);
                DestroyImmediate(buttonClone);
                buttonPrefab = toggleCloneGameObject.AddComponent<ButtonUI>();
                buttonPrefab.Init(label, rebindButton, text, "button默认文本","按钮");
                buttonPrefab.name = "按钮";
                Debug.Log("成功创建buttonPrefab预制体");
            }
        }
        private void CreateKeyBindPrefab(UIKeybindingEntry keybindingEntry) {
            if (keybindingEntry == null) return;
            UIKeybindingEntry keybinding = Instantiate(keybindingEntry);
            if (keybinding != null) {
                TextMeshProUGUI label = keybinding.GetInstanceField<TextMeshProUGUI>("label");
                Button rebindButton = keybinding.GetInstanceField<Button>("rebindButton");
                Button clearButton = keybinding.GetInstanceField<Button>("clearButton");
                InputIndicator indicator = keybinding.GetInstanceField<InputIndicator>("indicator");
                TextMeshProUGUI text = indicator.GetInstanceField<TextMeshProUGUI>("text");
                GameObject keybindingGameObject = keybinding.gameObject;
                DestroyImmediate(indicator);
                DestroyImmediate(keybinding);
                keyBindEntryPrefab = keybindingGameObject.AddComponent<KeyBindingUI>();
                keyBindEntryPrefab.Init(label, rebindButton,clearButton,text, "按键绑定默认文本", KeyCode.None);
                keyBindEntryPrefab.name = "按键绑定";
                Debug.Log("成功创建keyBindingPrefab预制体");
            }
        }

        private void CreateInputPrefab(OptionsUIEntry_Slider optionSlider) {
            if (optionSlider == null) return;
            OptionsUIEntry_Slider sliderClone = Instantiate(optionSlider);
            if (sliderClone != null) {
                TextMeshProUGUI label = sliderClone.GetInstanceField<TextMeshProUGUI>("label");
                Slider slider = sliderClone.GetInstanceField<Slider>("slider");
                TMP_InputField inputField = sliderClone.GetInstanceField<TMP_InputField>("valueField");
                GameObject sliderGameObject = sliderClone.gameObject;
                DestroyImmediate(sliderClone);
                DestroyImmediate(slider.gameObject);
                inputPrefab = sliderGameObject.AddComponent<InputUI>();
                inputPrefab.Init(label,inputField,"默认输入文本","默认值");
                inputPrefab.name = "输入框";
            }
        }

        private void CreateSliderPrefab(OptionsUIEntry_Slider optionSlider) {
            if (optionSlider == null) return;
            OptionsUIEntry_Slider entrySlider = Instantiate(optionSlider);
            if (entrySlider != null) {
                TextMeshProUGUI label = entrySlider.GetInstanceField<TextMeshProUGUI>("label");
                Slider slider = entrySlider.GetInstanceField<Slider>("slider");
                TMP_InputField inputField = entrySlider.GetInstanceField<TMP_InputField>("valueField");
                GameObject sliderGameObject = entrySlider.gameObject;
                DestroyImmediate(entrySlider);
                sliderPrefab = sliderGameObject.AddComponent<SliderUI>();
                sliderPrefab.Init(label, slider, inputField, "slider默认文本", 0, 0, 100);
                sliderPrefab.name = "滑块";
                Debug.Log("成功创建slider预制体");
            }
        }
        private void CreateDropDownPrefab(OptionsUIEntry_Dropdown optionDropDown) {
            if (optionDropDown == null) return;
            OptionsUIEntry_Dropdown dropDown = Instantiate(optionDropDown);
            if (dropDown != null) {
                TextMeshProUGUI label = dropDown.GetInstanceField<TextMeshProUGUI>("label");
                TMP_Dropdown dropdown = dropDown.GetInstanceField<TMP_Dropdown>("dropdown");
                GameObject dropDownGameObject = dropDown.gameObject;
                DestroyImmediate(dropDown);
                dropDownPrefab = dropDownGameObject.AddComponent<DropDownUI>();
                dropDownPrefab.Init(label, dropdown,
                    "默认文本", new List<string>() { "选项1", "选项2", "选项3", "选项4" }, "选项2");
                dropDownPrefab.name ="下拉列表";
                Debug.Log("成功创建下拉列表预制体");
            }
        }

        #endregion

        #region 添加组件
        public bool AddDropDownList(ModInfo modInfo, DropDownConfig dropDownConfig,
            Action<string> onValueChange = null) {
            if (modContent == null || dropDownPrefab == null) return false;
            DropDownUI dropDownUI = Instantiate(dropDownPrefab, modContent.transform);
            dropDownUI.name += dropDownConfig.Key;
            dropDownUI.Setup(dropDownConfig);
            dropDownUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, dropDownConfig.Key, dropDownUI.gameObject);
            return true;
        }

        public bool AddSlider(ModInfo modInfo, SliderConfig sliderConfig, Action<float> onValueChange = null) {
            if (modContent == null || sliderPrefab == null) return false;
            SliderUI sliderUI = Instantiate(sliderPrefab, modContent.transform);
            sliderUI.name += sliderConfig.Key;
            sliderUI.Setup(sliderConfig);
            sliderUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, sliderConfig.Key, sliderUI.gameObject);
            return true;
        }

        public bool AddToggle(ModInfo modInfo, ToggleConfig toggleConfig, Action<bool> onValueChange = null) {
            if (modContent == null || togglePrefab == null) return false;
            ToggleUI toggleUI = Instantiate(togglePrefab, modContent.transform);
            toggleUI.name += toggleConfig.Key;
            toggleUI.Setup(toggleConfig);
            toggleUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, toggleConfig.Key, toggleUI.gameObject);
            return true;
        }

        public bool AddKeybinding(ModInfo modInfo, KeyBindingConfig keyBindingConfig,
            Action<KeyCode> onValueChange = null) {
            if (modContent == null || keyBindEntryPrefab == null) return false;
            KeyBindingUI keyBindingUI = Instantiate(keyBindEntryPrefab, modContent.transform);
            keyBindingUI.name += keyBindingConfig.Key;
            keyBindingUI.Setup(keyBindingConfig, keyBindingManager);
            keyBindingManager.AddModKeyBinding(modInfo, keyBindingConfig.Key, keyBindingUI);
            keyBindingUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, keyBindingConfig.Key, keyBindingUI.gameObject);
            return true;
        }
        public bool AddInput(ModInfo modInfo, InputConfig inputConfig, Action<string> onValueChange) {
            if (modContent == null || inputPrefab == null) return false;
            InputUI inputUI = Instantiate(inputPrefab, modContent.transform);
            inputUI.name += inputConfig.Key;
            inputUI.Setup(inputConfig);
            inputUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, inputConfig.Key, inputUI.gameObject);
            return true;
        }

        public bool AddButton(ModInfo modInfo,string key,string description, string buttonText,Action onClickButton) {
            if (modContent == null || buttonPrefab == null) return false;
            ButtonUI buttonUI = Instantiate(buttonPrefab, modContent.transform);
            buttonUI.name += key;
            buttonUI.Setup(description, buttonText);
            buttonUI.onClickButton += onClickButton;
            AddUnderTheTitle(modInfo, key, buttonUI.gameObject);
            return true;
        }

        public bool AddGroup(ModInfo modInfo, string key, string description, List<string> keys, float height,bool top,bool open) {
            if (modContent == null || groupPrefab == null) return false;
            if (keys.Contains(key)) return false;
            TitleUI titleUI = AddOrGetTitle(modInfo);
            if (titleUI.HasNest(keys)) {
                Debug.LogWarning($"暂不支持Group嵌套");
                ConfigManager.RemoveUI(modInfo, key);
                return false;
            }
            GroupUI groupUI = Instantiate(groupPrefab, modContent.transform);
            groupUI.name += key;
            groupUI.Setup(modInfo,description,keys,height,open);
            titleUI.AddGroup(key,groupUI,keys,top);
            return true;
        }
        public bool RemoveUI(ModInfo modInfo, string key) {
            if (titleUiDic.TryGetValue(modInfo.GetModId(), out var titleUI)) {
                keyBindingManager.RemoveModKeyBinding(modInfo, key);
                return titleUI.RemoveUI(key);
            }
            return false;
        }

        public bool RemoveTitle(ModInfo info) {
            if (titleUiDic.Remove(info.GetModId(), out var titleUI)) {
                keyBindingManager.RemoveModKeyBinding(info);
                titleUI.Clear();
                DestroySafely(titleUI);
                return true;
            }
            return false;
        }

        public bool HasTitle(ModInfo info) => titleUiDic.ContainsKey(info.GetModId());

        private void AddUnderTheTitle(ModInfo modInfo, string key, GameObject uiGo) {
            TitleUI titleUI = AddOrGetTitle(modInfo);
            titleUI.Add(key, uiGo);
        }

        private TitleUI AddOrGetTitle(ModInfo modInfo) {
            return AddTitle(modInfo);
        }

        private TitleUI AddTitle(ModInfo modInfo) {
            if (titleUiDic.TryGetValue(modInfo.GetModId(), out var title)) return title;
            if (modContent == null || titlePrefab == null) return null;
            TitleUI titleUI = Instantiate(titlePrefab, modContent.transform);
            titleUI.name += modInfo.name;
            titleUI.Setup(modInfo.preview, modInfo.displayName, TitleHeight);
            titleUiDic.Add(modInfo.GetModId(), titleUI);
            return titleUI;
        }
        #endregion
        private void OnEnable() {
            Init();
            keyBindingManager = new KeyBindingManager();
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
            DestroySafely(modTabButton);
            DestroySafely(modContent);
            DestroySafely(dropDownPrefab);
            DestroySafely(sliderPrefab);
            DestroySafely(togglePrefab);
            DestroySafely(keyBindEntryPrefab);
            DestroySafely(titlePrefab);
            DestroySafely(inputPrefab);
            DestroySafely(buttonPrefab);
            DestroySafely(groupPrefab);
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
    }
}