using System;
using ModSetting.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModSetting.UI {
    public class ToggleUI : MonoBehaviour {
        [SerializeField]private TextMeshProUGUI label;
        [SerializeField]private Button button;
        [SerializeField]private TextMeshProUGUI text;
        private bool enable = false;
        public event Action<bool> onValueChange; 
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
            UpdateText();
        }

        private void UpdateText() {
            text.text = enable ? "启用" : "禁用";
            button.image.color = enable ? Color.green : Color.red;
        }

        private void OnClickButton() {
            enable = !enable;
            UpdateText();
            onValueChange?.Invoke(enable);
        }

        private void ToggleConfig_OnValueChange(bool obj) {
            enable = obj;
            UpdateText();
        }
    }
}