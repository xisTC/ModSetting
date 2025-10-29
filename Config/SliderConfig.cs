using System;
using UnityEngine;

namespace ModSetting.Config {
    public class SliderConfig {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public float DefaultValue { get; private set; }
        public Vector2 SliderRange { get; private set; }
        public event Action<float> onValueChange;
        public SliderConfig(string key, string description, float defaultValue, Vector2 sliderRange) {
            Key = key;
            Description = description;
            DefaultValue = defaultValue;
            SliderRange = sliderRange;
        }
        public void SetValue(float value) {
            DefaultValue = value;
            onValueChange?.Invoke(value);
        }
    }
}