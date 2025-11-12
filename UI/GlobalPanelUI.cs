using System.Linq;
using Duckov.Options.UI;
using UnityEngine;


namespace ModSetting.UI {
    public class GlobalPanelUI : PanelUI {
        public override void Init() {
            optionsPanel = FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "DontDestroyOnLoad");
            InitTab();
            InitPrefab();
            IsInit = true;
            Debug.Log("mod设置初始化完毕");
        }
    }
}