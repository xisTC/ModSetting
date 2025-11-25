using System;
using System.Collections.Generic;
using ModSetting.Log;
using SodaCraft.Localizations;
using UnityEngine;
using Logger = ModSetting.Log.Logger;

namespace ModSetting {
    public static class ModLocalizationManager {
        public const string TAB_NAME = "ModOptions";
        public const string ENABLE = "ON";
        public const string DISABLE = "OFF";
        public const string LOG_LEVEL = "LogLevel";
        public static readonly string LOG_INFO = LogLevel.Info.ToString();
        public static readonly string LOG_WARNING = LogLevel.Warning.ToString();
        public static readonly string LOG_ERROR = LogLevel.Error.ToString();
        public const string MAIN_MENU_TITLE_FONT_SIZE_DESCRIPTION = "Font size of the main menu";
        public const string MAIN_MENU_IMAGE_LENGTH_DESCRIPTION = "Size of the main menu image";
        public const string GLOBAL_TITLE_FONT_SIZE_DESCRIPTION="Font size of in-game menus";
        public const string GLOBAL_TITLE_IMAGE_LENGTH_DESCRIPTION="Size of the in-game menu image";
        public const string TITLE_SPACE_DESCRIPTION="Title interval";
        public const string RESET_DESCRIPTION="Restore the default value";
        public const string RESET_BUTTON_TEXT="Reset";
        public static SystemLanguage CurrentLanguage => LocalizationManager.CurrentLanguage;
        private static Dictionary<SystemLanguage, Dictionary<string, string>> languagePack = new();
        private static Dictionary<SystemLanguage, Dictionary<string, string>> flipMap = new();
        public static event Action<SystemLanguage> onLanguageChanged;
        public static void Init() {
            AddLanguagePack(SystemLanguage.ChineseSimplified)
                .Add(TAB_NAME,"Mod设置")
                .Add(ENABLE,"启用")
                .Add(DISABLE,"禁用")
                .Add(LOG_LEVEL,"日志等级")
                .Add(LOG_INFO,"信息")
                .Add(LOG_WARNING,"警告")
                .Add(LOG_ERROR,"错误")
                .Add(MAIN_MENU_TITLE_FONT_SIZE_DESCRIPTION,"主菜单字体大小")
                .Add(MAIN_MENU_IMAGE_LENGTH_DESCRIPTION,"主菜单图片大小")
                .Add(GLOBAL_TITLE_FONT_SIZE_DESCRIPTION,"游戏内菜单字体大小")
                .Add(GLOBAL_TITLE_IMAGE_LENGTH_DESCRIPTION,"游戏内菜单图片大小")
                .Add(TITLE_SPACE_DESCRIPTION,"标题间隔")
                .Add(RESET_DESCRIPTION,"恢复默认值")
                .Add(RESET_BUTTON_TEXT,"重置")
                .Build();
            AddLanguagePack(SystemLanguage.English)
                .Add(TAB_NAME,TAB_NAME)
                .Add(ENABLE,ENABLE)
                .Add(DISABLE,DISABLE)
                .Add(LOG_LEVEL,LOG_LEVEL)
                .Add(LOG_INFO,LOG_INFO)
                .Add(LOG_WARNING,LOG_WARNING)
                .Add(LOG_ERROR,LOG_ERROR)
                .Add(MAIN_MENU_TITLE_FONT_SIZE_DESCRIPTION,MAIN_MENU_TITLE_FONT_SIZE_DESCRIPTION)
                .Add(MAIN_MENU_IMAGE_LENGTH_DESCRIPTION,MAIN_MENU_IMAGE_LENGTH_DESCRIPTION)
                .Add(GLOBAL_TITLE_FONT_SIZE_DESCRIPTION,GLOBAL_TITLE_FONT_SIZE_DESCRIPTION)
                .Add(GLOBAL_TITLE_IMAGE_LENGTH_DESCRIPTION,GLOBAL_TITLE_IMAGE_LENGTH_DESCRIPTION)
                .Add(TITLE_SPACE_DESCRIPTION,TITLE_SPACE_DESCRIPTION)
                .Add(RESET_DESCRIPTION,RESET_DESCRIPTION)
                .Add(RESET_BUTTON_TEXT,RESET_BUTTON_TEXT)
                .Build();
            AddLanguagePack(SystemLanguage.Japanese)
                .Add(TAB_NAME,"Mod設定")
                .Add(ENABLE,"ON")
                .Add(DISABLE,"OFF")
                .Add(LOG_LEVEL,"ログレベルです")
                .Add(LOG_INFO,"情報です")
                .Add(LOG_WARNING,"警告します")
                .Add(LOG_ERROR,"間違いです")
                .Add(MAIN_MENU_TITLE_FONT_SIZE_DESCRIPTION,"メインメニューのフォントサイズです")
                .Add(MAIN_MENU_IMAGE_LENGTH_DESCRIPTION,"メインメニューの画像サイズです")
                .Add(GLOBAL_TITLE_FONT_SIZE_DESCRIPTION,"ゲーム内メニューフォントサイズ")
                .Add(GLOBAL_TITLE_IMAGE_LENGTH_DESCRIPTION,"ゲーム内メニューの画像サイズです")
                .Add(TITLE_SPACE_DESCRIPTION,"タイトル間隔です")
                .Add(RESET_DESCRIPTION,"デフォルトに戻します")
                .Add(RESET_BUTTON_TEXT,"リセットします")
                .Build();
            AddLanguagePack(SystemLanguage.Korean)
                .Add(TAB_NAME,"Mod설정")
                .Add(ENABLE,"ON")
                .Add(DISABLE,"OFF")
                .Add(LOG_LEVEL,"로그 레벨")
                .Add(LOG_INFO,"정보")
                .Add(LOG_WARNING,"경고")
                .Add(LOG_ERROR,"오류")
                .Add(MAIN_MENU_TITLE_FONT_SIZE_DESCRIPTION,"주 메뉴 글꼴 크기")
                .Add(MAIN_MENU_IMAGE_LENGTH_DESCRIPTION,"주 메뉴 그림 크기")
                .Add(GLOBAL_TITLE_FONT_SIZE_DESCRIPTION,"게임 내 메뉴 글꼴 크기")
                .Add(GLOBAL_TITLE_IMAGE_LENGTH_DESCRIPTION,"게임 내 메뉴 그림 크기")
                .Add(TITLE_SPACE_DESCRIPTION,"제목 간격")
                .Add(RESET_DESCRIPTION,"기본값 복원")
                .Add(RESET_BUTTON_TEXT,"초기화")
                .Build();
            AddLanguagePack(SystemLanguage.Russian)
                .Add(TAB_NAME,"Мод Настройки")
                .Add(ENABLE,"ON")
                .Add(DISABLE,"OFF")
                .Add(LOG_LEVEL,"Уровень записи.")
                .Add(LOG_INFO,"информац")
                .Add(LOG_WARNING,"предупред")
                .Add(LOG_ERROR,"ошибк")
                .Add(MAIN_MENU_TITLE_FONT_SIZE_DESCRIPTION,"Размер шрифта в главном меню")
                .Add(MAIN_MENU_IMAGE_LENGTH_DESCRIPTION,"Размер изображения в главном меню")
                .Add(GLOBAL_TITLE_FONT_SIZE_DESCRIPTION,"Размер шрифта в меню игры")
                .Add(GLOBAL_TITLE_IMAGE_LENGTH_DESCRIPTION,"Размер картинки в меню игры")
                .Add(TITLE_SPACE_DESCRIPTION,"Интервал между заголовками")
                .Add(RESET_DESCRIPTION,"Восстановление умолчания")
                .Add(RESET_BUTTON_TEXT,"перезагрузк")
                .Build();
        }

        public static void OnLanguageChanged(SystemLanguage newLanguage) {
            onLanguageChanged?.Invoke(newLanguage);
        }

        public static void Clear() {
            languagePack.Clear();
            onLanguageChanged = null;
        }

        public static string GetText(string key) => GetText(CurrentLanguage, key);
        public static string GetKey(string text) {
            if (!flipMap.ContainsKey(CurrentLanguage)) {
                Logger.Error($"找不到当前语言:{CurrentLanguage};文本{text}的key");
                return "?";
            }
            Dictionary<string,string> dictionary = flipMap[CurrentLanguage];
            if (!dictionary.TryGetValue(text, out string key)) {
                Logger.Error($"找不到文本{text}的key");
                return "?";
            }
            return key;
        }

        private static string GetText(SystemLanguage language,string key) {
            if (!languagePack.TryGetValue(language,out var dictionary)) return key;
            return dictionary.GetValueOrDefault(key, key);
        }

        private static LanguagePackBuilder AddLanguagePack(SystemLanguage language) {
            return new LanguagePackBuilder(language);
        }

        private class LanguagePackBuilder {
            private readonly SystemLanguage language;
            private readonly Dictionary<string, string> dictionary;
            private readonly Dictionary<string, string> flipDictionary;
            public LanguagePackBuilder(SystemLanguage language) {
                this.language = language;
                dictionary = new Dictionary<string, string>();
                flipDictionary = new Dictionary<string, string>();
            }
            public LanguagePackBuilder Add(string key, string value) {
                if (!dictionary.TryAdd(key, value)) {
                    Logger.Warning($"语言包 {language} 中已存在键: {key}");
                }
                if (!flipDictionary.TryAdd(value, key)) {
                    Logger.Error($"字典在添加语言包 {language} 时，已存在键: {value}");
                }
                return this;
            }
            public void Build() {
                languagePack[language] = dictionary;
                flipMap[language] = flipDictionary;
                Logger.Info($"语言包 {language} 构建完成，包含 {dictionary.Count} 个条目");
            }
        }
    }
}