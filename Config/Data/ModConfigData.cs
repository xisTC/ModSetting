using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ModSetting.Config.Data {
    [Serializable]
    public class ModConfigData {
        public string modId;
        public List<IConfigData> allConfigDatas;
        [JsonIgnore]private Dictionary<string, IConfigData> configs = new Dictionary<string, IConfigData>();
        public ModConfigData(string modId,List<IConfigData> allConfigDatas) {
            this.modId = modId;
            this.allConfigDatas = allConfigDatas;
            Init();
        }

        private void Init() {
            configs.Clear();
            foreach (IConfigData configData in allConfigDatas) {
                configs.Add(configData.Key,configData);
            }
        }
        public bool HasValue(string key) {
            return configs.ContainsKey(key);
        }

        public T GetValue<T>(string key) {
            if (!HasValue(key)) return default;
            object value = configs[key].GetValue();
            return (T)value;
        }
    }
}