using System;
using System.Collections.Generic;
using ModSetting.Config;
using ModSetting.Pool;
using TMPro;
using UnityEngine;

namespace ModSetting.UI {
    public class DropDownUI : PoolableBehaviour {
        [SerializeField]private TextMeshProUGUI label;
        [SerializeField]private TMP_Dropdown dropdown;
        private List<string> options;
        private string currentOption;
        private string description;
        public event Action<string> onValueChange; 
        public void Init(TextMeshProUGUI label,TMP_Dropdown dropdown,string defaultDescription, List<string> defaultOptions,string defaultOption) {
            this.label = label;
            this.dropdown = dropdown;
            label.text = description;
            options = defaultOptions;
            currentOption=defaultOption;
        }

        public void Setup(DropDownConfig dropDownConfig) {
            description = dropDownConfig.Description;
            label.text = description;
            options = dropDownConfig.Options;
            currentOption = dropDownConfig.Value;
            dropDownConfig.OnValueChange += DropDownConfig_OnValueChange;
            onValueChange += dropDownConfig.SetValue;
            dropdown.onValueChanged.AddListener(Dropdown_OnValueChanged);
            UpdateDropDown();
        }

        public override void OnRelease() {
            base.OnRelease();
            onValueChange=null;
        }

        private void Dropdown_OnValueChanged(int index) {
            currentOption = options[index];
            onValueChange?.Invoke(currentOption);
            UpdateDropDown();
        }

        private void DropDownConfig_OnValueChange(string obj) {
            currentOption = obj;
            UpdateDropDown();
        }

        private void UpdateDropDown() {
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.SetValueWithoutNotify(options.IndexOf(currentOption));
        }
    }
}