using System;
using System.Collections.Generic;

namespace ModSetting.Config.Data {
    [Serializable]
    public struct ConfigData {
        public List<ModConfigData> configDatas;
        public ConfigData(List<ModConfigData> configDatas) {
            this.configDatas = configDatas;
        }

        public void Init() {
            foreach (ModConfigData configData in configDatas) {
                configData.Init();
            }
        }
    }
}