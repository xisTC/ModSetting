using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Modding;
using Duckov.Options.UI;
using ModSetting.Extensions;
using ModSetting.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.UI {
    public class MainMenuPanelUI : PanelUI {
        private GameObject save;

        public override void Init() {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (IsInit) return;
            optionsPanel = FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "MainMenu");
            save = new GameObject("save");
            save.transform.SetParent(Setting.Parent);
            InitTab();
            RectTransform scrollRectTransform = optionsPanel.GetComponentsInChildren<ScrollRect>(true)
                .Select(scrollRect => scrollRect.GetComponent<RectTransform>())
                .FirstOrDefault(item => item.name == "ScrollView");
            VerticalLayoutGroup verticalLayoutGroup = modContent.transform.parent?.GetComponent<VerticalLayoutGroup>();
            if (scrollRectTransform == null||verticalLayoutGroup==null) {
                uiLenght = 1416.03f;
                Logger.Warning($"找不到scrollRect/verticalLayoutGroup,使用备用长度{uiLenght}");
            } else {
                uiLenght=scrollRectTransform.rect.width-(verticalLayoutGroup.padding.left + verticalLayoutGroup.padding.right);
            }
            IsInit = true;
            Logger.Info($"主菜单设置初始化完毕,使用时间: {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()-timestamp}ms");
        }

        public void ResetTab() {
            modTabButton.onClicked = null;
            optionsPanel = FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "MainMenu");
            List<OptionsPanel_TabButton> tabButtons =
                optionsPanel.GetInstanceField<List<OptionsPanel_TabButton>>("tabButtons");
            if (tabButtons == null) {
                Logger.Error($"反射获取tabButtons失败,找不到字段tabButtons");
                return;
            }

            OptionsPanel_TabButton tabButton = tabButtons.FirstOrDefault(item => item != null);
            if (tabButton == null) {
                Logger.Error($"找不到不为null的tab");
                return;
            }

            modTabButton.transform.SetParent(tabButton.transform.parent, false);
            // 添加到tabButtons列表
            tabButtons.Add(modTabButton);
            // 调用Setup更新UI
            optionsPanel.InvokeInstanceMethod("Setup");
            var tab = tabButton.GetInstanceField<GameObject>("tab");
            if (tab == null) {
                Logger.Error($"反射获取tabButton的tab成员失败");
                return;
            }

            modContent.transform.SetParent(tab.transform.parent, false);
        }

        protected override TitleUI AddOrGetTitle(ModInfo modInfo) {
            if (titleUiDic.TryGetValue(modInfo.GetModId(), out var title)) return title;
            if (modContent == null) return null;
            TitleUI titleUI = UIPrefabFactory.Spawn<TitleUI>(modContent.transform);
            titleUI.name += modInfo.name;
            titleUI.Setup(modInfo.preview, modInfo.displayName, Setting.MainMenuTitleFontSize,Setting.MainMenuImageLength,uiLenght);
            titleUiDic.Add(modInfo.GetModId(), titleUI);
            return titleUI;
        }

        protected override void ChildOnEnable() {
            SceneLoader.onStartedLoadingScene += OnStartedLoadingScene;
            Setting.OnMainMenuTitleFontSizeChanged+= Setting_OnMainMenuTitleFontSizeChanged;
            Setting.OnMainMenuImageLengthChanged+= Setting_OnMainMenuImageLengthChanged;
        }

        protected override void ChildOnDisable() {
            SceneLoader.onStartedLoadingScene -= OnStartedLoadingScene;
            Setting.OnMainMenuTitleFontSizeChanged-= Setting_OnMainMenuTitleFontSizeChanged;
            Setting.OnMainMenuImageLengthChanged-= Setting_OnMainMenuImageLengthChanged;
            DestroySafely(save);
        }

        private void Setting_OnMainMenuImageLengthChanged(float obj) {
            foreach (TitleUI titleUI in titleUiDic.Values) {
                titleUI.UpdateImageLength(obj);
            }
        }

        private void Setting_OnMainMenuTitleFontSizeChanged(float obj) {
            foreach (TitleUI titleUI in titleUiDic.Values) {
                titleUI.UpdateFontSize(obj);
            }
        }

        private void OnStartedLoadingScene(SceneLoadingContext sceneLoadingContext) {
            if (SceneManager.GetActiveScene().name == "MainMenu") {
                modContent.transform.SetParent(save.transform, false);
                modTabButton.transform.SetParent(save.transform, false);
                Logger.Info($"保存组件");
            }
        }
    }
}