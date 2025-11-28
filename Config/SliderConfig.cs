using System;
using System.Collections.Generic;
using ModSetting.Config.Data;
using UnityEngine;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Config {
    public class SliderConfig: IConfig{
        public string Key { get; }
        public string Description { get; }
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
        }

        public T GetValue<T>() {
            if (typeof(T) == typeof(float)) {
                return (T)(object)Value;
            }
            if (typeof(T)==typeof(int)) {
                return (T)Convert.ChangeType(Value, typeof(T));
            }
            return default;
        }

        public void SetValue(object value) {
            if (!IsTypeMatch(value.GetType())) {
                Logger.Error($"类型不匹配:{value.GetType()},无法赋值给:{GetTypesString()}");
                return;
            }
            float floatValue = Convert.ToSingle(value);
            if (floatValue < SliderRange.x || floatValue > SliderRange.y) {
                Logger.Error($"Slider不能超出范围,key:{Key},value:{floatValue}");
            }
            SetValue(floatValue);
        }

        public bool IsTypeMatch(Type type) => GetValidTypes().Contains(type);

        public List<Type> GetValidTypes() => new List<Type>() { typeof(float), typeof(int) };

        public string GetTypesString() => string.Join(",", GetValidTypes());

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