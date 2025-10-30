using System;
using ModSetting.Config.Data;
using UnityEngine;

namespace ModSetting.Config {
    public interface IConfig {
        string Key { get; }
        string Description { get; }
        Type ValueType { get; }
        object GetValue();
        void SetValue(object value);
        IConfigData GetConfigData();
    }
}