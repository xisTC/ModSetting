using System;
using ModSetting.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.UI {
    public class ButtonUI : PoolableBehaviour {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;
        public event Action onClickButton;
        private void Awake() {
            if (button != null) button.onClick.AddListener(()=>onClickButton?.Invoke());
        }
        public void Init(TextMeshProUGUI label, Button button, TextMeshProUGUI text, string defaultDescription,
            string defaultButtonText) {
            this.label = label;
            this.button = button;
            this.text = text;
            label.text = defaultDescription;
            text.text = defaultButtonText;
        }

        public void Setup(string description, string buttonText) {
            label.text = description;
            text.text = buttonText;
        }

        public override void OnRelease() {
            base.OnRelease();
            onClickButton = null;
        }
    }
}