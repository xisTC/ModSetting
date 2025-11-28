using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Options.UI;
using Duckov.Utilities;
using ModSetting.Config.Data;
using ModSetting.Extensions;
using ModSetting.Pool;
using ModSetting.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = ModSetting.Log.Logger;
using Object = UnityEngine.Object;

namespace ModSetting {
    public static class UIPrefabFactory {
        public static bool IsInit;
        public static PoolableBehaviour Spawn(UIType uiType, Transform parent) => UIPool.Spawn(uiType, parent);
        public static T Spawn<T>(Transform parent) where T : PoolableBehaviour {
            UIType uiType =MapToUIType<T>();
            return (T)UIPool.Spawn(uiType, parent);
        }

        public static void ReturnToPool(PoolableBehaviour poolableBehaviour) => UIPool.ReturnToPool(poolableBehaviour);

        public static void Init() {
            OptionsPanel optionsPanel = Object.FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "MainMenu");
            CreatePrefabs(optionsPanel);
            IsInit = true;
        }

        private static void CreatePrefabs(OptionsPanel optionsPanel) {
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
            UIKeybindingEntry keybindingEntry = optionsPanel.gameObject
                .GetComponentsInChildren<UIKeybindingEntry>(true)
                .FirstOrDefault(item => item != null);
            CreateKeyBindPrefab(keybindingEntry);
            CreateTogglePrefab(keybindingEntry);
            CreateButtonPrefab(keybindingEntry);
        }

        private static void CreateTitlePrefab(OptionsUIEntry_Dropdown optionDropDown) {
            if (optionDropDown == null) return;
            OptionsUIEntry_Dropdown titleClone = Instantiate(optionDropDown);
            if (titleClone != null) {
                titleClone.transform.DestroyAllChildren();
                GameObject titleCloneGameObject = titleClone.gameObject;
                DestroyImmediate(titleClone);
                DontDestroyOnLoad(titleCloneGameObject);
                TitleUI titleUI = titleCloneGameObject.AddComponent<TitleUI>();
                titleUI.Init();
                titleUI.name = "标题";
                UIPool.AddSetting(new PoolableSetting<TitleUI>(UIType.标题, titleCloneGameObject));
                Logger.Info($"成功创建titlePrefab预制体");
            }
        }

        private static void CreateGroupPrefab(OptionsUIEntry_Dropdown optionDropDown) {
            if (optionDropDown == null) return;
            OptionsUIEntry_Dropdown groupClone = Instantiate(optionDropDown);
            if (groupClone != null) {
                groupClone.transform.DestroyAllChildren();
                GameObject groupCloneGameObject = groupClone.gameObject;
                DestroyImmediate(groupClone);
                DontDestroyOnLoad(groupCloneGameObject);
                GroupUI groupUI = groupCloneGameObject.AddComponent<GroupUI>();
                groupUI.Init();
                groupUI.name = "分组";
                UIPool.AddSetting(new PoolableSetting<GroupUI>(UIType.分组, groupCloneGameObject));
                Logger.Info($"成功创建groupPrefab预制体");
            }
        }

        private static void CreateTogglePrefab(UIKeybindingEntry keybindingEntry) {
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
                DontDestroyOnLoad(toggleCloneGameObject);
                ToggleUI toggleUI = toggleCloneGameObject.AddComponent<ToggleUI>();
                toggleUI.Init(label, rebindButton, text, "toggle默认文本");
                toggleUI.name = "开关";
                UIPool.AddSetting(new PoolableSetting<ToggleUI>(UIType.开关, toggleCloneGameObject));
                Logger.Info($"成功创建togglePrefab预制体");
            }
        }

        private static void CreateButtonPrefab(UIKeybindingEntry keybindingEntry) {
            if (keybindingEntry == null) return;
            UIKeybindingEntry buttonClone = Instantiate(keybindingEntry);
            if (buttonClone != null) {
                TextMeshProUGUI label = buttonClone.GetInstanceField<TextMeshProUGUI>("label");
                Button rebindButton = buttonClone.GetInstanceField<Button>("rebindButton");
                Button clearButton = buttonClone.GetInstanceField<Button>("clearButton");
                InputIndicator indicator = buttonClone.GetInstanceField<InputIndicator>("indicator");
                TextMeshProUGUI text = indicator.GetInstanceField<TextMeshProUGUI>("text");
                GameObject buttonCloneGameObject = buttonClone.gameObject;
                DestroyImmediate(clearButton.image);
                DestroyImmediate(indicator);
                DestroyImmediate(clearButton);
                DestroyImmediate(buttonClone);
                DontDestroyOnLoad(buttonCloneGameObject);
                ButtonUI buttonUI = buttonCloneGameObject.AddComponent<ButtonUI>();
                buttonUI.Init(label, rebindButton, text, "button默认文本", "按钮");
                buttonUI.name = "按钮";
                UIPool.AddSetting(new PoolableSetting<ButtonUI>(UIType.按钮, buttonCloneGameObject));
                Logger.Info($"成功创建buttonPrefab预制体");
            }
        }

        private static void CreateKeyBindPrefab(UIKeybindingEntry keybindingEntry) {
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
                DontDestroyOnLoad(keybindingGameObject);
                KeyBindingUI keyBindingUI = keybindingGameObject.AddComponent<KeyBindingUI>();
                keyBindingUI.Init(label, rebindButton, clearButton, text, "按键绑定默认文本", KeyCode.None);
                keyBindingUI.name = "按键绑定";
                UIPool.AddSetting(new PoolableSetting<KeyBindingUI>(UIType.按键绑定, keybindingGameObject));
                Logger.Info($"成功创建keyBindingPrefab预制体");
            }
        }

        private static void CreateInputPrefab(OptionsUIEntry_Slider optionSlider) {
            if (optionSlider == null) return;
            OptionsUIEntry_Slider sliderClone = Instantiate(optionSlider);
            if (sliderClone != null) {
                TextMeshProUGUI label = sliderClone.GetInstanceField<TextMeshProUGUI>("label");
                Slider slider = sliderClone.GetInstanceField<Slider>("slider");
                TMP_InputField inputField = sliderClone.GetInstanceField<TMP_InputField>("valueField");
                GameObject sliderGameObject = sliderClone.gameObject;
                DestroyImmediate(sliderClone);
                DestroyImmediate(slider.gameObject);
                DontDestroyOnLoad(sliderGameObject);
                InputUI inputUI = sliderGameObject.AddComponent<InputUI>();
                inputUI.Init(label, inputField, "默认输入文本", "默认值");
                inputUI.name = "输入框";
                UIPool.AddSetting(new PoolableSetting<InputUI>(UIType.输入框, sliderGameObject));
                Logger.Info($"成功创建输入框预制体");
            }
        }

        private static void CreateSliderPrefab(OptionsUIEntry_Slider optionSlider) {
            if (optionSlider == null) return;
            OptionsUIEntry_Slider entrySlider = Instantiate(optionSlider);
            if (entrySlider != null) {
                TextMeshProUGUI label = entrySlider.GetInstanceField<TextMeshProUGUI>("label");
                Slider slider = entrySlider.GetInstanceField<Slider>("slider");
                TMP_InputField inputField = entrySlider.GetInstanceField<TMP_InputField>("valueField");
                GameObject sliderGameObject = entrySlider.gameObject;
                DestroyImmediate(entrySlider);
                DontDestroyOnLoad(sliderGameObject);
                SliderUI sliderUI = sliderGameObject.AddComponent<SliderUI>();
                sliderUI.Init(label, slider, inputField, "slider默认文本", 0, 0, 100);
                sliderUI.name = "滑块";
                UIPool.AddSetting(new PoolableSetting<SliderUI>(UIType.滑块, sliderGameObject));
                Logger.Info($"成功创建slider预制体");
            }
        }

        private static void CreateDropDownPrefab(OptionsUIEntry_Dropdown optionDropDown) {
            if (optionDropDown == null) return;
            OptionsUIEntry_Dropdown dropDown = Instantiate(optionDropDown);
            if (dropDown != null) {
                TextMeshProUGUI label = dropDown.GetInstanceField<TextMeshProUGUI>("label");
                TMP_Dropdown dropdown = dropDown.GetInstanceField<TMP_Dropdown>("dropdown");
                GameObject dropDownGameObject = dropDown.gameObject;
                DestroyImmediate(dropDown);
                DontDestroyOnLoad(dropDownGameObject);
                DropDownUI dropDownUI = dropDownGameObject.AddComponent<DropDownUI>();
                dropDownUI.Init(label, dropdown,
                    "默认文本", new List<string>() { "选项1", "选项2", "选项3", "选项4" }, "选项2");
                dropDownUI.name = "下拉列表";
                UIPool.AddSetting(new PoolableSetting<DropDownUI>(UIType.下拉列表, dropDownGameObject));
                Logger.Info($"成功创建下拉列表预制体");
            }
        }
        private static UIType MapToUIType<T>() where T : PoolableBehaviour {
            return typeof(T) switch {
                Type t when t == typeof(ButtonUI) => UIType.按钮,
                Type t when t == typeof(ToggleUI) => UIType.开关,
                Type t when t == typeof(SliderUI) => UIType.滑块,
                Type t when t == typeof(DropDownUI) => UIType.下拉列表,
                Type t when t == typeof(InputUI) => UIType.输入框,
                Type t when t == typeof(KeyBindingUI) => UIType.按键绑定,
                Type t when t == typeof(TitleUI) => UIType.标题,
                Type t when t == typeof(GroupUI) => UIType.分组,
                _ => throw new ArgumentException($"未支持的类型: {typeof(T)}")
            };
        }
        private static void DontDestroyOnLoad(GameObject gameObject) => Object.DontDestroyOnLoad(gameObject);

        private static void DestroyImmediate(Object obj) => Object.DestroyImmediate(obj);

        private static T Instantiate<T>(T obj) where T : Object => Object.Instantiate(obj);
    }
}