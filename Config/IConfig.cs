using System;
using System.Collections.Generic;
using ModSetting.Config.Data;
using UnityEngine;

namespace ModSetting.Config {
    public interface IConfig {
        string Key { get; }
        string Description { get; }
        T GetValue<T>();
        void SetValue(object value);
        bool IsTypeMatch(Type type);
        List<Type> GetValidTypes();
        string GetTypesString();
        IConfigData GetConfigData();
    }
}