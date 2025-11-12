using System;
using System.Collections.Generic;
using SodaCraft.Localizations;
using UnityEngine;

namespace ModSetting {
    public static class ModLocalizationManager {
        public const string TAB_NAME = "Mod设置";
        public const string ENABLE = "启用";
        public const string DISABLE = "禁用";
        public static SystemLanguage CurrentLanguage => LocalizationManager.CurrentLanguage;
        private static Dictionary<SystemLanguage, Dictionary<string, string>> languagePack = new();
        public static event Action<SystemLanguage> onLanguageChanged;
        public static void Init() {
            languagePack[SystemLanguage.ChineseSimplified] = new Dictionary<string, string>() {
                {TAB_NAME,TAB_NAME},
                {ENABLE,ENABLE},
                {DISABLE,DISABLE}
            };
            languagePack[SystemLanguage.English] = new Dictionary<string, string>() {
                {TAB_NAME,"ModOptions" },
                {ENABLE,"ON"},
                {DISABLE,"OFF"}
            };
            languagePack[SystemLanguage.Japanese] = new Dictionary<string, string>() {
                {TAB_NAME,"Mod設定" },
                {ENABLE,"ON"},
                {DISABLE,"OFF"}
            };
            languagePack[SystemLanguage.Korean] = new Dictionary<string, string>() {
                {TAB_NAME,"Mod설정" },
                {ENABLE,"ON"},
                {DISABLE,"OFF"}
            };
            languagePack[SystemLanguage.Russian] = new Dictionary<string, string>() {
                { TAB_NAME, "Мод Настройки" },
                { ENABLE, "ON" },
                { DISABLE, "OFF" }
            };
        }
        public static void OnLanguageChanged(SystemLanguage newLanguage) {
            onLanguageChanged?.Invoke(newLanguage);
        }

        public static void Clear() {
            languagePack.Clear();
            onLanguageChanged = null;
        }

        public static string GetText(string key) => GetText(CurrentLanguage, key);

        private static string GetText(SystemLanguage language,string key) {
            if (!languagePack.TryGetValue(language,out var dictionary)) return key;
            return dictionary.GetValueOrDefault(key, key);
        }
    }
}