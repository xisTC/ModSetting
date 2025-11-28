using System;
using System.Linq;
using Duckov.Modding;
using Duckov.Options.UI;
using ModSetting.Extensions;
using ModSetting.Pool;
using Logger = ModSetting.Log.Logger;


namespace ModSetting.UI {
    public class GlobalPanelUI : PanelUI {
        public override void Init() {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            optionsPanel = FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "DontDestroyOnLoad");
            InitTab();
            // InitPrefab();
            IsInit = true;
            Logger.Info($"游戏内设置初始化完毕,使用时间: {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()-timestamp}ms");
        }

        protected override TitleUI AddOrGetTitle(ModInfo modInfo) {
            if (titleUiDic.TryGetValue(modInfo.GetModId(), out var title)) return title;
            if (modContent == null) return null;
            // TitleUI titleUI = Instantiate(titlePrefab, modContent.transform);
            TitleUI titleUI = UIPrefabFactory.Spawn<TitleUI>(modContent.transform);
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