using System;
using ModSetting.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModSetting.UI {
    public class SliderUI : MonoBehaviour {
        [SerializeField]private TextMeshProUGUI label;
        [SerializeField]private Slider slider;
        [SerializeField]private TMP_InputField valueField;
        private float currentValue;
        private string description;
        public event Action<float> onValueChange; 
        private void Awake() {
            if(slider!=null) slider.onValueChanged.AddListener(Slider_OnValueChanged);
            if(valueField!=null)valueField.onEndEdit.AddListener(InputField_OnEndEdit);
        }

        public void Init(TextMeshProUGUI label,Slider slider,TMP_InputField valueField,
            string defaultDescription,float defaultValue,float defaultMinValue,float defaultMaxValue ) {
            this.label = label;
            this.slider = slider;
            this.valueField = valueField;
            label.text =defaultDescription;
            currentValue = defaultValue;
            slider.minValue = defaultMinValue;
            slider.maxValue = defaultMaxValue;
        }

        public void Setup(SliderConfig sliderConfig) {
            description = sliderConfig.Description;
            label.text =description;
            currentValue = sliderConfig.Value;
            slider.minValue = sliderConfig.SliderRange.x;
            slider.maxValue = sliderConfig.SliderRange.y;
            sliderConfig.OnValueChange +=SliderConfig_OnValueChange ;
            onValueChange += sliderConfig.SetValue;
            UpdateValue();
        }

        private void Slider_OnValueChanged(float value) {
            currentValue = value;
            onValueChange?.Invoke(value);
            UpdateValue();
        }

        private void SliderConfig_OnValueChange(float obj) {
            currentValue = obj;
            UpdateValue();
        }

        private void InputField_OnEndEdit(string value) {
            if (float.TryParse(value, out float result)) {
                currentValue= Mathf.Clamp(result, this.slider.minValue, this.slider.maxValue);
                onValueChange?.Invoke(currentValue);
            }
            UpdateValue();
        }
        private void UpdateValue() {
            slider.SetValueWithoutNotify(currentValue);
            valueField.SetTextWithoutNotify(currentValue.ToString("0.0"));
        }
    }
}