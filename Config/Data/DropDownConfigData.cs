using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModSetting.Config.Data {
    [Serializable]
    public class DropDownConfigData : IConfigData{
        public string Key { get; private set; }
        public string Description { get; private set; }
        public UIType UIType => UIType.下拉列表;

        public string Value { get; private set; }

        public List<string> Options { get; private set; }

        public object GetValue() => Value;

        public DropDownConfigData(string key, string description, string value, List<string> options) {
            Key = key;
            Description = description;
            Value = value;
            Options = options;
        }
    }
}