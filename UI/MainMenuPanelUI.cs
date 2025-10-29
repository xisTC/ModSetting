using System.Collections.Generic;
using System.Linq;
using Duckov.Options.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModSetting.UI {
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