using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModSetting.UI {
    public class ButtonUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;
        public event Action onClickButton;
        private void Start() {
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
    }
}