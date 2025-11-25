using System.Linq;
using Duckov.Modding;
using Duckov.Options.UI;
using ModSetting.Extensions;
using Logger = ModSetting.Log.Logger;


namespace ModSetting.UI {
    public class GlobalPanelUI : PanelUI {
        public override void Init() {
            optionsPanel = FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "DontDestroyOnLoad");
            InitTab();
            InitPrefab();
            IsInit = true;
            Logger.Info("游戏内设置初始化完毕");
        }

        protected override TitleUI AddOrGetTitle(ModInfo modInfo) {
            if (titleUiDic.TryGetValue(modInfo.GetModId(), out var title)) return title;
            if (modContent == null || titlePrefab == null) return null;
            TitleUI titleUI = Instantiate(titlePrefab, modContent.transform);
            titleUI.name += modInfo.name;
            titleUI.Setup(modInfo.preview, modInfo.displayName, Setting.GlobalTitleFontSize,Setting.GlobalTitleImageLength,uiLenght);
            titleUiDic.Add(modInfo.GetModId(), titleUI);
            return titleUI;
        }

        protected override void ChildOnEnable() {
            Setting.OnGlobalTitleFontSizeChanged+= Setting_OnGlobalTitleFontSizeChanged;
            Setting.OnGlobalTitleImageLengthChanged+= Setting_OnGlobalTitleImageLengthChanged;
        }

        protected override void ChildOnDisable() {
            Setting.OnGlobalTitleFontSizeChanged-= Setting_OnGlobalTitleFontSizeChanged;
            Setting.OnGlobalTitleImageLengthChanged-= Setting_OnGlobalTitleImageLengthChanged;
        }

        private void Setting_OnGlobalTitleFontSizeChanged(float obj) {
            foreach (TitleUI titleUI in titleUiDic.Values) {
                titleUI.UpdateFontSize(obj);
            }
        }

        private void Setting_OnGlobalTitleImageLengthChanged(float obj) {
            foreach (TitleUI titleUI in titleUiDic.Values) {
                titleUI.UpdateImageLength(obj);
            }
        }
    }
}