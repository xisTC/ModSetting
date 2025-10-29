using System.Linq;
using Duckov.Options.UI;

// NOTE 保存为预制件关键，需要将字段设置为public或者使用SerializeField来修饰字段，这样实例化预制件的时候，内部才会有值，否则为null
namespace ModSetting.UI {
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
}