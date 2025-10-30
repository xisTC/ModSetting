using System;
using UnityEngine;

namespace ModSetting.Config.Data {
    [Serializable]
    public class SliderConfigData : IConfigData {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public UIType UIType => UIType.滑块; 
        public object GetValue() => Value;

        public float Value { get; private set; }
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public SliderConfigData(string key, string description, float value, Vector2 sliderRange) {
            Key = key;
            Description = description;
            Value = value;
            MinValue = sliderRange.x;
            MaxValue = sliderRange.y;
        }
    }
}