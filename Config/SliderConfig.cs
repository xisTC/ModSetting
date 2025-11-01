using System;
using ModSetting.Config.Data;
using UnityEngine;

namespace ModSetting.Config {
    public class SliderConfig: IConfig{
        public string Key { get; }
        public string Description { get; }
        public Type ValueType { get; }
        public float Value { get; private set; }
        public Vector2 SliderRange { get; private set; }
        public int DecimalPlaces { get; }
        public int CharacterLimit { get;}
        public event Action<float> OnValueChange;
        public SliderConfig(string key, string description, float value, Vector2 sliderRange,int decimalPlaces,int characterLimit) {
            Key = key;
            Description = description;
            Value = value;
            SliderRange = sliderRange;
            DecimalPlaces = decimalPlaces;
            CharacterLimit = characterLimit;
            ValueType = typeof(float);
        }

        public object GetValue() => Value;

        public void SetValue(object value) {
            if (value.GetType() != ValueType) {
                Debug.LogError($"类型不匹配:{ValueType}和{value.GetType()},无法赋值");
                return;
            }
            if ((float)value < SliderRange.x || (float)value > SliderRange.y) {
                Debug.LogError("Slider不能超出范围:"+(float)value);   
            }
            SetValue((float)value);
        }

        public IConfigData GetConfigData() {
            return new SliderConfigData(Key, Description, Value, SliderRange);
        }

        public void SetValue(float value) {
            Value = RoundToDecimalPlaces(value);
            OnValueChange?.Invoke(Value);
        }
        private float RoundToDecimalPlaces(float value) {
            float multiplier = Mathf.Pow(10, DecimalPlaces);
            return Mathf.Round(value * multiplier) / multiplier;
        }
    }
}