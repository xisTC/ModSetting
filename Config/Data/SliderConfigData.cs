using System;
using UnityEngine;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Config.Data {
    [Serializable]
    public class SliderConfigData : IConfigData {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public UIType UIType => UIType.滑块;
        public int Liveness { get; set; } = 1;

        public T GetValue<T>() {
            if (typeof(T) == typeof(float)) {
                return (T)(object)Value;
            }
            if (typeof(T)==typeof(int)) {
                return (T)Convert.ChangeType(Value, typeof(T));
            }
            Logger.Error($"滑块不支持获取此类型的值:{typeof(T)},key:{Key}");
            return default;
        }

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