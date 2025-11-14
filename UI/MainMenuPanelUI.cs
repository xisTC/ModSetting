using System.Collections.Generic;
using System.Linq;
using Duckov.Options.UI;
using ModSetting.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ModSetting.UI {
    public class MainMenuPanelUI : PanelUI {
        private GameObject save;

        public override void Init() {
            if (IsInit) return;
            optionsPanel = FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "MainMenu");
            save = new GameObject("save");
            DontDestroyOnLoad(save);
            ImageLength = 100f;
            InitTab();
            InitPrefab();
            RectTransform scrollRectTransform = optionsPanel.GetComponentsInChildren<ScrollRect>(true)
                .Select(scrollRect => scrollRect.GetComponent<RectTransform>())
                .FirstOrDefault(item => item.name == "ScrollView");
            if (scrollRectTransform == null) {
                Debug.LogError("找不到scrollRect");
                scrollRectLength = 1000f;
            } else {
                scrollRectLength = scrollRectTransform.sizeDelta.x;
            }
            IsInit = true;
            Debug.Log("mod设置初始化完毕");
        }

        public void ResetTab() {
            modTabButton.onClicked = null;
            optionsPanel = FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "MainMenu");
            List<OptionsPanel_TabButton> tabButtons =
                optionsPanel.GetInstanceField<List<OptionsPanel_TabButton>>("tabButtons");
            if (tabButtons == null) {
                Debug.LogError("反射获取tabButtons失败");
                return;
            }

            OptionsPanel_TabButton tabButton = tabButtons.FirstOrDefault(item => item != null);
            if (tabButton == null) {
                Debug.Log("找不到不为null的tab");
                return;
            }

            modTabButton.transform.SetParent(tabButton.transform.parent, false);
            // 添加到tabButtons列表
            tabButtons.Add(modTabButton);
            // 调用Setup更新UI
            optionsPanel.InvokeInstanceMethod("Setup");
            var tab = tabButton.GetInstanceField<GameObject>("tab");
            if (tab == null) {
                Debug.LogError("无法反射获取tabButton的tab成员");
                return;
            }

            modContent.transform.SetParent(tab.transform.parent, false);
        }

        protected override void ChildOnEnable() {
            SceneLoader.onStartedLoadingScene += OnStartedLoadingScene;
        }

        protected override void ChildOnDisable() {
            SceneLoader.onStartedLoadingScene -= OnStartedLoadingScene;
            DestroySafely(save);
        }

        private void OnStartedLoadingScene(SceneLoadingContext sceneLoadingContext) {
            if (SceneManager.GetActiveScene().name == "MainMenu") {
                modContent.transform.SetParent(save.transform, false);
                modTabButton.transform.SetParent(save.transform, false);
                Debug.Log("保存组件");
            }
        }
    }
}