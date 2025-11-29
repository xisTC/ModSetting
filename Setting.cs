using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Modding;
using ModSetting.Api;
using ModSetting.Log;
using UnityEngine;
using Logger = ModSetting.Log.Logger;
using Object = UnityEngine.Object;

namespace ModSetting {
    public static class Setting {
        public static LogLevel LogLevel { get; private set; } = LogLevel.Warning;
        public static float MainMenuTitleFontSize { get; private set; }
        public static float MainMenuImageLength { get; private set; }
        public static float GlobalTitleFontSize { get; private set; }
        public static float GlobalTitleImageLength { get; private set; }
        public static float TitleSpace { get; private set; }
        private static SettingsBuilder builder;
        public static event Action<float> OnMainMenuTitleFontSizeChanged;
        public static event Action<float> OnMainMenuImageLengthChanged;
        public static event Action<float> OnGlobalTitleFontSizeChanged;
        public static event Action<float> OnGlobalTitleImageLengthChanged;
        public static event Action<float> OnTitleSpaceChanged;

        private static void SetLogLevel(LogLevel logLevel) {
            Logger.Info("改变日志等级:" + logLevel);
            LogLevel = logLevel;
        }

        private static void SetMainMenuTitleFontSize(float fontSize) {
            MainMenuTitleFontSize = fontSize;
            OnMainMenuTitleFontSizeChanged?.Invoke(fontSize);
        }

        private static void SetMainMenuImageLength(float imageLength) {
            MainMenuImageLength = imageLength;
            OnMainMenuImageLengthChanged?.Invoke(imageLength);
        }

        private static void SetGlobalTitleFontSize(float fontSize) {
            GlobalTitleFontSize = fontSize;
            OnGlobalTitleFontSizeChanged?.Invoke(fontSize);
        }

        private static void SetGlobalTitleImageLength(float imageLength) {
            GlobalTitleImageLength = imageLength;
            OnGlobalTitleImageLengthChanged?.Invoke(imageLength);
        }

        private static void SetTitleSpace(float space) {
            TitleSpace = space;
            OnTitleSpaceChanged?.Invoke(space);
        }

        public static Transform Parent { get; private set; }

        public static void Init(ModInfo modInfo) {
            ModLocalizationManager.onLanguageChanged += OnLanguageChanged;
            Parent = new GameObject("ModSetting").transform;
            Object.DontDestroyOnLoad(Parent);
            builder = SettingsBuilder.Create(modInfo);
            LogLevel = builder.GetSavedValue(nameof(LogLevel), out string logLevelString)
                ? Enum.TryParse(ModLocalizationManager.GetKey(logLevelString), out LogLevel logLevel)
                    ? logLevel
                    : LogLevel.Warning
                : LogLevel.Warning;
            MainMenuTitleFontSize =
                builder.GetSavedValue(nameof(MainMenuTitleFontSize), out float mainMenuTitleFontSize)
                    ? mainMenuTitleFontSize
                    : 36f;
            MainMenuImageLength = builder.GetSavedValue(nameof(MainMenuImageLength), out float mainMenuImageLength)
                ? mainMenuImageLength
                : 100f;
            GlobalTitleFontSize = builder.GetSavedValue(nameof(GlobalTitleFontSize), out float globalTitleFontSize)
                ? globalTitleFontSize
                : 36f;
            GlobalTitleImageLength =
                builder.GetSavedValue(nameof(GlobalTitleImageLength), out float globalTitleImageLength)
                    ? globalTitleImageLength
                    : 50f;
            TitleSpace = builder.GetSavedValue(nameof(TitleSpace), out float titleSpace) ? titleSpace : 200f;
            AddUI();
        }

        public static void Clear() {
            ModLocalizationManager.onLanguageChanged -= OnLanguageChanged;
        }

        private static void AddUI() {
            List<string> logOptions = new List<string> {
                ModLocalizationManager.GetText(ModLocalizationManager.LOG_INFO),
                ModLocalizationManager.GetText(ModLocalizationManager.LOG_WARNING),
                ModLocalizationManager.GetText(ModLocalizationManager.LOG_ERROR),
            };
            builder
                .AddDropdownList(nameof(LogLevel),
                    ModLocalizationManager.GetText(ModLocalizationManager.LOG_LEVEL),
                    logOptions,
                    ModLocalizationManager.GetText(LogLevel.ToString()),
                    text => {
                        string key = ModLocalizationManager.GetKey(text);
                        SetLogLevel(Enum.Parse<LogLevel>(key));
                    })
                .AddSlider(nameof(MainMenuTitleFontSize),
                    ModLocalizationManager.GetText(ModLocalizationManager.MAIN_MENU_TITLE_FONT_SIZE_DESCRIPTION),
                    MainMenuTitleFontSize, new Vector2(0, 50f), SetMainMenuTitleFontSize)
                .AddSlider(nameof(MainMenuImageLength),
                    ModLocalizationManager.GetText(ModLocalizationManager.MAIN_MENU_IMAGE_LENGTH_DESCRIPTION),
                    MainMenuImageLength, new Vector2(0, 300f), SetMainMenuImageLength)
                .AddSlider(nameof(GlobalTitleFontSize),
                    ModLocalizationManager.GetText(ModLocalizationManager.GLOBAL_TITLE_FONT_SIZE_DESCRIPTION),
                    GlobalTitleFontSize, new Vector2(0, 60f), SetGlobalTitleFontSize)
                .AddSlider(nameof(GlobalTitleImageLength),
                    ModLocalizationManager.GetText(ModLocalizationManager.GLOBAL_TITLE_IMAGE_LENGTH_DESCRIPTION),
                    GlobalTitleImageLength, new Vector2(0, 300f), SetGlobalTitleImageLength)
                .AddSlider(nameof(TitleSpace),
                    ModLocalizationManager.GetText(ModLocalizationManager.TITLE_SPACE_DESCRIPTION),
                    TitleSpace, new Vector2(0, 500f), SetTitleSpace)
                .AddButton("Reset",
                    ModLocalizationManager.GetText(ModLocalizationManager.RESET_DESCRIPTION),
                    ModLocalizationManager.GetText(ModLocalizationManager.RESET_BUTTON_TEXT), Reset);
        }

        private static void OnLanguageChanged(SystemLanguage obj) {
            builder.Clear();
            AddUI();
        }

        private static void Reset() {
            SetLogLevel(LogLevel.Warning);
            SetTitleSpace(200f);
            SetGlobalTitleImageLength(100f);
            SetGlobalTitleFontSize(36f);
            SetMainMenuImageLength(100f);
            SetMainMenuTitleFontSize(36f);
            builder.SetValue(nameof(LogLevel), ModLocalizationManager.GetText(LogLevel.ToString()))
                .SetValue(nameof(MainMenuTitleFontSize), MainMenuTitleFontSize)
                .SetValue(nameof(MainMenuImageLength), MainMenuImageLength)
                .SetValue(nameof(GlobalTitleFontSize), GlobalTitleFontSize)
                .SetValue(nameof(GlobalTitleImageLength), GlobalTitleImageLength)
                .SetValue(nameof(TitleSpace), TitleSpace);
        }
    }
}