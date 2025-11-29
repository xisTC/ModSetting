using System;
using ModSetting.Config;
using ModSetting.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModSetting.UI {
    public class ToggleUI : PoolableBehaviour{
        [SerializeField]private TextMeshProUGUI label;
        [SerializeField]private Button button;
        [SerializeField]private TextMeshProUGUI text;
        private bool enable = false;
        public event Action<bool> onValueChange;
        private string enableString;
        private string disableString;
        private void Start() {
            if (button != null) button.onClick.AddListener(OnClickButton);
        }

        public void Init(TextMeshProUGUI label,Button button,TextMeshProUGUI text,string defaultDescription,bool defaultEnable=false) {
            this.label = label;
            this.button = button;
            this.text = text;
            label.text = defaultDescription;
            enable = defaultEnable;
            UpdateText();
        }

        public void Setup(ToggleConfig toggleConfig) {
            label.text = toggleConfig.Description;
            enable = toggleConfig.Enable;
            toggleConfig.OnValueChange += ToggleConfig_OnValueChange;
            onValueChange += toggleConfig.SetValue;
            ModLocalizationManager.onLanguageChanged += OnLanguageChanged;
            enableString = ModLocalizationManager.GetText(ModLocalizationManager.ENABLE);
            disableString = ModLocalizationManager.GetText(ModLocalizationManager.DISABLE);
            UpdateText();
        }

        private void UpdateText() {
            text.text = enable ? enableString: disableString;
            button.image.color = enable ? Color.green : Color.red;
        }
        
        private void OnClickButton() {
            enable = !enable;
            UpdateText();
            onValueChange?.Invoke(enable);
        }

        private void OnLanguageChanged(SystemLanguage obj) {
            enableString = ModLocalizationManager.GetText(ModLocalizationManager.ENABLE);
            disableString = ModLocalizationManager.GetText(ModLocalizationManager.DISABLE);
            UpdateText();
        }
        public override void OnRelease() {
            base.OnRelease();
            ModLocalizationManager.onLanguageChanged -= OnLanguageChanged;
            onValueChange=null;
        }
        private void ToggleConfig_OnValueChange(bool obj) {
            enable = obj;
            UpdateText();
        }
    }
}