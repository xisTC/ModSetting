using System;
using System.Runtime.CompilerServices;

namespace ModSetting.Log {
    public static class Logger {
        private const string PREFIX_MOD_NAME = "[ModSetting]";

        public static void Debug(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "", 
            [CallerLineNumber] int sourceLineNumber = 0) {
            if(Setting.LogLevel>LogLevel.Debug)return;
            string fileName = System.IO.Path.GetFileName(sourceFilePath);
            UnityEngine.Debug.Log(GetMessage($"调用者:[{memberName}](在{fileName}:{sourceLineNumber}){message}"));
        }
        public static void Info(string message) {
            if(Setting.LogLevel>LogLevel.Info)return;
            UnityEngine.Debug.Log(GetMessage(message));
        }
        public static void Warning(string message) {
            if(Setting.LogLevel>LogLevel.Warning)return;
            UnityEngine.Debug.LogWarning(GetMessage(message));
        }

        public static void Error(string message) {
            if(Setting.LogLevel>LogLevel.Error)return;
            UnityEngine.Debug.LogError(GetMessage(message));
        }

        public static void Exception(string message,Exception exception) {
            if(Setting.LogLevel>LogLevel.Error)return;
            UnityEngine.Debug.LogError(GetMessage($"{message}:{exception.StackTrace}"));
        }
        private static string GetMessage(string message) {
            return PREFIX_MOD_NAME + message;
        }
    }

    public enum LogLevel {
        Debug,
        Info,
        Warning,
        Error
    }
}