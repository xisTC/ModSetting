using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Modding;
using Duckov.Options.UI;
using Duckov.Utilities;
using ModSetting.Config;
using SodaCraft.Localizations;
using Test;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// NOTE 保存为预制件关键，需要将字段设置为public或者使用SerializeField来修饰字段，这样实例化预制件的时候，内部才会有值，否则为null
namespace ModSetting.UI {
    public abstract class PanelUI : MonoBehaviour {
        protected OptionsPanel_TabButton modTabButton;
        protected GameObject modContent;
        protected OptionsPanel optionsPanel;
        protected DropDownUI dropDownPrefab;
        private SliderUI sliderPrefab;
        private ToggleUI togglePrefab;
        private KeyBindingUI keyBindEntryPrefab;
        private TitleUI titlePrefab;
        private Texture2D infoPreview;
        protected float titleHeight;
        protected Dictionary<ulong, TitleUI> titleUiDic = new Dictionary<ulong, TitleUI>();
        private KeyBindingManager keyBindingManager;
        public bool IsInit { get; protected set; }
        public abstract void Init();

        public void Setup(Texture2D infoPreview) {
            this.infoPreview = infoPreview;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.T)) {
                Debug.Log($"modContent状态:{modContent.activeSelf}");   
            }
        }

        protected void InitTab() {
            Debug.Log("开始创建Mod设置标签页:" + optionsPanel.gameObject.scene.name);
            List<OptionsPanel_TabButton> tabButtons =
                ReflectionExtension.GetInstanceField<List<OptionsPanel_TabButton>>(optionsPanel, "tabButtons");
            if (tabButtons == null) {
                Debug.LogError("反射获取tabButtons失败");
                return;
            }

            OptionsPanel_TabButton tabButton = tabButtons.FirstOrDefault(item => item != null);
            if (tabButton == null) {
                Debug.Log("找不到不为null的tab");
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
            var tab = ReflectionExtension.GetInstanceField<GameObject>(modTabButton, "tab");
            if (tab == null) {
                Debug.LogError("无法反射获取modTabButton的tab成员");
                DestroySafely(tabButtonGameObjectClone);
                return;
            }

            modContent = Instantiate(tab, tab.transform.parent);
            modContent.name = "modContent";
            modContent.transform.DestroyAllChildren();
            // 设置克隆的tab到tabButton            
            bool result = ReflectionExtension.SetInstanceField(modTabButton, "tab", modContent);
            if (!result) {
                Debug.LogError("反射修改tab成员失败!!");
                DestroySafely(tabButtonGameObjectClone);
                DestroySafely(modContent);
                return;
            }

            // 添加到tabButtons列表
            tabButtons.Add(modTabButton);
            // 调用Setup更新UI
            ReflectionExtension.InvokeInstanceMethod(optionsPanel, "Setup");
            // 修改标签页名称
            TextMeshProUGUI tabName = modTabButton.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tabName != null) {
                // 移除本地化组件, 保证文本设置正常
                TextLocalizor localizor = modTabButton.GetComponentInChildren<TextLocalizor>(true);
                if (localizor != null)
                    Destroy(localizor);
                tabName.SetText("Mod设置");
            }

            modTabButton.onClicked = (button, pointData) => {
                Debug.Log("点击mod设置");
                // optionsPanel.SetSelection(button);
            };
            Debug.Log($"初始化modTabButton:{modTabButton},初始化modContent:{modContent}");
        }

        #region InitPrefab

        protected void InitPrefab() {
            OptionsUIEntry_Dropdown optionDropDown = optionsPanel.gameObject
                .GetComponentsInChildren<OptionsUIEntry_Dropdown>(true)
                .Skip(1)
                .FirstOrDefault(item => item != null);
            CreateDropDownPrefab(optionDropDown);
            CreateSliderPrefab();
            UIKeybindingEntry keybindingEntry = optionsPanel.gameObject.GetComponentsInChildren<UIKeybindingEntry>(true)
                .FirstOrDefault(item => item != null);
            CreateKeyBindPrefab(keybindingEntry);
            CreateTogglePrefab(keybindingEntry);
            CreateTitlePrefab(optionDropDown);
            DontDestroyOnLoad(dropDownPrefab);
            DontDestroyOnLoad(sliderPrefab);
            DontDestroyOnLoad(keyBindEntryPrefab);
            DontDestroyOnLoad(togglePrefab);
            DontDestroyOnLoad(titlePrefab);
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
                Debug.Log("成功创建titlePrefab预制体");
            }
        }

        private void CreateTogglePrefab(UIKeybindingEntry keybindingEntry) {
            if (keybindingEntry == null) return;
            UIKeybindingEntry toggleClone = Instantiate(keybindingEntry);
            if (toggleClone != null) {
                TextMeshProUGUI label = ReflectionExtension.GetInstanceField<TextMeshProUGUI>(toggleClone, "label");
                Button rebindButton = ReflectionExtension.GetInstanceField<Button>(toggleClone, "rebindButton");
                InputIndicator indicator =
                    ReflectionExtension.GetInstanceField<InputIndicator>(toggleClone, "indicator");
                TextMeshProUGUI text = ReflectionExtension.GetInstanceField<TextMeshProUGUI>(indicator, "text");
                GameObject toggleCloneGameObject = toggleClone.gameObject;
                DestroyImmediate(indicator);
                DestroyImmediate(toggleClone);
                togglePrefab = toggleCloneGameObject.AddComponent<ToggleUI>();
                togglePrefab.Init(label, rebindButton, text, "toggle默认文本");
                Debug.Log("成功创建togglePrefab预制体");
            }
        }

        private void CreateKeyBindPrefab(UIKeybindingEntry keybindingEntry) {
            if (keybindingEntry == null) return;
            UIKeybindingEntry keybinding = Instantiate(keybindingEntry);
            if (keybinding != null) {
                TextMeshProUGUI label = ReflectionExtension.GetInstanceField<TextMeshProUGUI>(keybinding, "label");
                Button rebindButton = ReflectionExtension.GetInstanceField<Button>(keybinding, "rebindButton");
                InputIndicator indicator =
                    ReflectionExtension.GetInstanceField<InputIndicator>(keybinding, "indicator");
                TextMeshProUGUI text = ReflectionExtension.GetInstanceField<TextMeshProUGUI>(indicator, "text");
                GameObject keybindingGameObject = keybinding.gameObject;
                DestroyImmediate(indicator);
                DestroyImmediate(keybinding);
                keyBindEntryPrefab = keybindingGameObject.AddComponent<KeyBindingUI>();
                keyBindEntryPrefab.Init(label, rebindButton, text, "按键绑定默认文本", KeyCode.None);
                Debug.Log("成功创建keyBindingPrefab预制体");
            }
        }

        private void CreateSliderPrefab() {
            OptionsUIEntry_Slider optionSlider = optionsPanel.gameObject
                .GetComponentsInChildren<OptionsUIEntry_Slider>(true)
                .FirstOrDefault(item => item != null);
            if (optionSlider == null) return;
            OptionsUIEntry_Slider entrySlider = Instantiate(optionSlider);
            if (entrySlider != null) {
                TextMeshProUGUI label = ReflectionExtension.GetInstanceField<TextMeshProUGUI>(entrySlider, "label");
                Slider slider = ReflectionExtension.GetInstanceField<Slider>(entrySlider, "slider");
                TMP_InputField inputField =
                    ReflectionExtension.GetInstanceField<TMP_InputField>(entrySlider, "valueField");
                GameObject sliderGameObject = entrySlider.gameObject;
                DestroyImmediate(entrySlider);
                sliderPrefab = sliderGameObject.AddComponent<SliderUI>();
                sliderPrefab.Init(label, slider, inputField, "slider默认文本", 0, 0, 100);
                Debug.Log("成功创建slider预制体");
            }
        }

        private void CreateDropDownPrefab(OptionsUIEntry_Dropdown optionDropDown) {
            if (optionDropDown == null) return;
            OptionsUIEntry_Dropdown dropDown = Instantiate(optionDropDown);
            if (dropDown != null) {
                TextMeshProUGUI label = ReflectionExtension.GetInstanceField<TextMeshProUGUI>(dropDown, "label");
                TMP_Dropdown dropdown = ReflectionExtension.GetInstanceField<TMP_Dropdown>(dropDown, "dropdown");
                GameObject dropDownGameObject = dropDown.gameObject;
                DestroyImmediate(dropDown);
                dropDownPrefab = dropDownGameObject.AddComponent<DropDownUI>();
                dropDownPrefab.Init(label, dropdown,
                    "默认文本", new List<string>() { "选项1", "选项2", "选项3", "选项4" }, "选项2");
                Debug.Log("成功创建下拉列表预制体");
            }
        }

        #endregion

        public bool AddDropDownList(ModInfo modInfo, DropDownConfig dropDownConfig,
            Action<string> onValueChange = null) {
            if (modContent == null || dropDownPrefab == null) return false;
            DropDownUI dropDownUI = Instantiate(dropDownPrefab, modContent.transform);
            dropDownUI.Setup(dropDownConfig);
            dropDownUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, dropDownConfig.Key, dropDownUI.gameObject);
            return true;
        }

        public bool AddSlider(ModInfo modInfo, SliderConfig sliderConfig, Action<float> onValueChange = null) {
            if (modContent == null || sliderPrefab == null) return false;
            SliderUI sliderUI = Instantiate(sliderPrefab, modContent.transform);
            sliderUI.Setup(sliderConfig);
            sliderUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, sliderConfig.Key, sliderUI.gameObject);
            return true;
        }

        public bool AddToggle(ModInfo modInfo, ToggleConfig toggleConfig, Action<bool> onValueChange = null) {
            if (modContent == null || togglePrefab == null) return false;
            ToggleUI toggleUI = Instantiate(togglePrefab, modContent.transform);
            toggleUI.Setup(toggleConfig);
            toggleUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, toggleConfig.Key, toggleUI.gameObject);
            return true;
        }

        public bool AddKeybinding(ModInfo modInfo, KeyBindingConfig keyBindingConfig,
            Action<KeyCode> onValueChange = null) {
            if (modContent == null || keyBindEntryPrefab == null) return false;
            KeyBindingUI keyBindingUI = Instantiate(keyBindEntryPrefab, modContent.transform);
            keyBindingUI.Setup(keyBindingConfig, keyBindingManager);
            keyBindingManager.AddModKeyBinding(modInfo, keyBindingConfig.Key, keyBindingUI);
            keyBindingUI.onValueChange += onValueChange;
            AddUnderTheTitle(modInfo, keyBindingConfig.Key, keyBindingUI.gameObject);
            return true;
        }

        public bool RemoveUI(ModInfo modInfo, string key) {
            if (titleUiDic.TryGetValue(modInfo.publishedFileId, out var titleUI)) {
                keyBindingManager.RemoveModKeyBinding(modInfo, key);
                return titleUI.RemoveUI(key);
            }

            return false;
        }

        public bool RemoveTitle(ModInfo info) {
            if (titleUiDic.Remove(info.publishedFileId, out var titleUI)) {
                keyBindingManager.RemoveModKeyBinding(info);
                titleUI.Clear();
                DestroySafely(titleUI);
                return true;
            }

            return false;
        }

        private void AddUnderTheTitle(ModInfo modInfo, string key, GameObject uiGo) {
            TitleUI titleUI = AddOrGetTitle(modInfo);
            titleUI.Add(key, uiGo);
        }

        private TitleUI AddOrGetTitle(ModInfo modInfo) {
            return AddTitle(modInfo);
        }

        private TitleUI AddTitle(ModInfo modInfo) {
            if (titleUiDic.TryGetValue(modInfo.publishedFileId, out var title)) return title;
            if (modContent == null || titlePrefab == null) return null;
            TitleUI titleUI = Instantiate(titlePrefab, modContent.transform);
            titleUI.Setup(modInfo.preview, modInfo.displayName, titleHeight);
            titleUiDic.Add(modInfo.publishedFileId, titleUI);
            return titleUI;
        }

        private void OnEnable() {
            Init();
            keyBindingManager = new KeyBindingManager();
            ChildOnEnable();
        }

        private void OnDisable() {
            //还要移除panel中的引用
            List<OptionsPanel_TabButton> tabButtons =
                ReflectionExtension.GetInstanceField<List<OptionsPanel_TabButton>>(optionsPanel, "tabButtons");
            tabButtons.Remove(modTabButton);
            ReflectionExtension.InvokeInstanceMethod(optionsPanel, "Setup");
            DestroySafely(modTabButton);
            DestroySafely(modContent);
            Debug.Log(
                $"销毁modTabButton:{modTabButton},销毁modContent:{modContent},是否成功:{modTabButton == null && modContent == null}");
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
    }

    public class GlobalPanelUI : PanelUI {
        public override void Init() {
            titleHeight = 50f;
            optionsPanel = FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "DontDestroyOnLoad");
            InitTab();
            InitPrefab();
            IsInit = true;
        }
    }

    public class MainMenuPanelUI : PanelUI {
        private GameObject save;

        public override void Init() {
            optionsPanel = FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "MainMenu");
            if (!IsInit) {
                save = new GameObject("save");
                DontDestroyOnLoad(save);
                titleHeight = 100f;
                InitTab();
                InitPrefab();
                IsInit = true;
            } else {
                ResetTab();
            }
        }

        private void ResetTab() {
            modTabButton.onClicked = null;
            List<OptionsPanel_TabButton> tabButtons =
                ReflectionExtension.GetInstanceField<List<OptionsPanel_TabButton>>(optionsPanel, "tabButtons");
            if (tabButtons == null) {
                Debug.LogError("反射获取tabButtons失败");
                return;
            }

            OptionsPanel_TabButton tabButton = tabButtons.FirstOrDefault(item => item != null);
            if (tabButton == null) {
                Debug.Log("找不到不为null的tab");
                return;
            }

            modTabButton.transform.SetParent(tabButton.transform.parent);
            // 添加到tabButtons列表
            tabButtons.Add(modTabButton);
            // 调用Setup更新UI
            ReflectionExtension.InvokeInstanceMethod(optionsPanel, "Setup");
            var tab = ReflectionExtension.GetInstanceField<GameObject>(tabButton, "tab");
            if (tab == null) {
                Debug.LogError("无法反射获取tabButton的tab成员");
                return;
            }

            modContent.transform.SetParent(tab.transform.parent);
        }
        protected override void ChildOnEnable() {
            SceneLoader.onStartedLoadingScene += OnStartedLoadingScene;
        }

        protected override void ChildOnDisable() {
            SceneLoader.onStartedLoadingScene -= OnStartedLoadingScene;
        }

        private void OnStartedLoadingScene(SceneLoadingContext sceneLoadingContext) {
            if (SceneManager.GetActiveScene().name == "MainMenu") {
                modContent.transform.SetParent(save.transform);
                modTabButton.transform.SetParent(save.transform);
                Debug.Log("保存组件");
            }
        }
    }
}