using System;
using System.Collections.Generic;
using System.Text;
using ModSetting.Config;
using ModSetting.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace ModSetting.UI {
    public class InputUI : PoolableBehaviour {
        [SerializeField]private TextMeshProUGUI label;
        [SerializeField]private TMP_InputField valueField;
        private string currentValue;
        public event Action<string> onValueChange; 
        private readonly HashSet<char> allowedChars = new HashSet<char>();
        private void Awake() {
            if(valueField!=null)valueField.onEndEdit.AddListener(InputField_OnEndEdit);
            if(valueField!=null)valueField.onValidateInput+=InputField_OnValidateInput;
            // 添加数字
            for (char c = '0'; c <= '9'; c++) allowedChars.Add(c);
            // 添加英文字母
            for (char c = 'a'; c <= 'z'; c++) allowedChars.Add(c);
            for (char c = 'A'; c <= 'Z'; c++) allowedChars.Add(c);
            // 添加常见符号
            string symbols = " ,.!?;:\\\"'~@#$%^&*+-=/|\\()[]{}<>（）【】《》";
            foreach (char c in symbols) allowedChars.Add(c);
        }

        public void Init(TextMeshProUGUI label,TMP_InputField valueField,
            string defaultDescription,string defaultValue) {
            this.label = label;
            this.valueField = valueField;
            label.text =defaultDescription;
            currentValue = defaultValue;
            valueField.characterLimit = 40;
            valueField.contentType = TMP_InputField.ContentType.Custom;
            LayoutElement layoutElement = valueField.GetComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1;
        }

        public void Setup(InputConfig inputConfig) {
            label.text = inputConfig.Description;
            currentValue = inputConfig.Value;
            valueField.characterLimit = inputConfig.CharacterLimit;
            inputConfig.OnValueChange += InputConfig_OnValueChange;
            onValueChange += inputConfig.SetValue;
            UpdateValue();
        }

        public override void OnRelease() {
            base.OnRelease();
            onValueChange = null;
        }

        private void InputField_OnEndEdit(string value) {
            currentValue = FilterInvalidChars(value);
            onValueChange?.Invoke(currentValue);
            UpdateValue();
        }

        private char InputField_OnValidateInput(string text, int charindex, char addedchar) {
            // 允许控制字符
            if (char.IsControl(addedchar)) return addedchar;
            // 检查字符是否在允许范围内
            if (IsCharAllowed(addedchar))
            {
                return addedchar;
            }
            return '\0';
        }

        private void InputConfig_OnValueChange(string obj) {
            currentValue = obj;
            UpdateValue();
        }

        private void UpdateValue() {
            valueField.SetTextWithoutNotify(currentValue);
        }
        private bool IsCharAllowed(char c)
        {
            // 检查中文字符范围
            if (c >= 0x4E00 && c <= 0x9FFF) return true;
            // 检查预定义的允许字符
            return allowedChars.Contains(c);
        }

        private string FilterInvalidChars(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (IsCharAllowed(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}