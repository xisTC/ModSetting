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
        private int decimalPlaces=1;
        public event Action<float> onValueChange; 
        public void Init(TextMeshProUGUI label,Slider slider,TMP_InputField valueField,
            string defaultDescription,float defaultValue,float defaultMinValue,float defaultMaxValue,int decimalPlaces=1) {
            this.label = label;
            this.slider = slider;
            this.valueField = valueField;
            label.text =defaultDescription;
            currentValue = defaultValue;
            slider.minValue = defaultMinValue;
            slider.maxValue = defaultMaxValue;
            this.decimalPlaces = decimalPlaces;
        }

        public void Setup(SliderConfig sliderConfig) {
            description = sliderConfig.Description;
            label.text =description;
            currentValue = sliderConfig.Value;
            slider.minValue = sliderConfig.SliderRange.x;
            slider.maxValue = sliderConfig.SliderRange.y;
            decimalPlaces = sliderConfig.DecimalPlaces;
            valueField.characterLimit = sliderConfig.CharacterLimit;
            sliderConfig.OnValueChange +=SliderConfig_OnValueChange ;
            onValueChange += sliderConfig.SetValue;
            slider.onValueChanged.AddListener(Slider_OnValueChanged);
            valueField.onEndEdit.AddListener(InputField_OnEndEdit);
            UpdateValue();
        }

        private void Slider_OnValueChanged(float value) {
            currentValue = RoundToDecimalPlaces(value);
            onValueChange?.Invoke(currentValue);
            UpdateValue();
        }

        private void SliderConfig_OnValueChange(float obj) {
            currentValue = RoundToDecimalPlaces(obj);
            UpdateValue();
        }

        private void InputField_OnEndEdit(string value) {
            if (float.TryParse(value, out float result)) {
                currentValue= Mathf.Clamp(result, slider.minValue, slider.maxValue);
                currentValue = RoundToDecimalPlaces(currentValue);
                onValueChange?.Invoke(currentValue);
            }
            UpdateValue();
        }
        private void UpdateValue() {
            slider.SetValueWithoutNotify(currentValue);
            valueField.SetTextWithoutNotify(currentValue.ToString());
        }
        private float RoundToDecimalPlaces(float value) {
            float multiplier = Mathf.Pow(10, decimalPlaces);
            return Mathf.Round(value * multiplier) / multiplier;
        }
    }
}