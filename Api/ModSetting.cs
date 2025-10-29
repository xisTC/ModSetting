using System;
using System.Collections.Generic;
using System.Reflection;
using Duckov.Modding;
using UnityEngine;

namespace ModSetting.Api {
    public static class ModSetting {
        private const string ADD_DROP_DOWN_LIST = "AddDropDownList";
        private const string ADD_SLIDER = "AddSlider";
        private const string ADD_TOGGLE = "AddToggle";
        private const string ADD_KEYBINDING="AddKeybinding";
        private const string GET_VALUE="GetValue";
        private const string SET_VALUE="SetValue";
        private const string REMOVE_UI="RemoveUI";
        private const string REMOVE_MOD="RemoveMod";
        private static float Version = 0.1f;
        public const string MOD_NAME = "ModSetting";
        private const string TYPE_NAME = "ModSetting.ModBehaviour";
        private static Type modBehaviour;
        public static bool IsInit { get; private set; }
        private static readonly string[] methodNames=new [] {
            ADD_DROP_DOWN_LIST,
            ADD_SLIDER,
            ADD_TOGGLE,
            ADD_KEYBINDING,
            GET_VALUE,
            SET_VALUE,
            REMOVE_UI,
            REMOVE_MOD
        };

        public static bool Init() {
            if(IsInit) return true;
            modBehaviour=FindTypeInAssemblies(TYPE_NAME);
            if (modBehaviour == null) return false;
            CheckVersion();
            foreach (string methodName in methodNames) {
                MethodInfo methodInfo = modBehaviour.GetMethod(methodName);
                if (methodInfo == null) {
                    Debug.LogError($"{methodName}方法找不到");
                    return false;
                }
            }
            IsInit = true;
            return true;
        }

        public static bool AddDropdownList(ModInfo modInfo,string key,string description,
            List<string> options, string defaultValue,Action<string> onValueChange=null) {
            if (!Available(modInfo,key)) return false;
            MethodInfo methodInfo = GetStaticPublicMethodInfo(ADD_DROP_DOWN_LIST);
            if (methodInfo == null) return false;
            methodInfo.Invoke(null, new object[]{ modInfo, key, description, options, defaultValue, onValueChange });
            return true;
        }

        public static bool AddSlider(ModInfo modInfo, string key, string description,
            float defaultValue, Vector2 sliderRange, Action<float> onValueChange = null) {
            if (!Available(modInfo,key)) return false;
            MethodInfo methodInfo = GetStaticPublicMethodInfo(ADD_SLIDER);
            if (methodInfo == null) return false;
            methodInfo.Invoke(null, new object[]{ modInfo, key, description,defaultValue,sliderRange,onValueChange });
            return true;
        }

        public static bool AddToggle(ModInfo modInfo, string key, string description,
            bool enable, Action<bool> onValueChange = null) {
            if (!Available(modInfo,key)) return false;
            MethodInfo methodInfo = GetStaticPublicMethodInfo(ADD_TOGGLE);
            if (methodInfo == null) return false;
            methodInfo.Invoke(null, new object[]{ modInfo, key, description,enable,onValueChange });
            return true;
        }

        public static bool AddKeybinding(ModInfo modInfo, string key, string description,
            KeyCode keyCode, Action<KeyCode> onValueChange = null) {
            if (!Available(modInfo,key)) return false;
            MethodInfo methodInfo = GetStaticPublicMethodInfo(ADD_KEYBINDING);
            if (methodInfo == null) return false;
            methodInfo.Invoke(null, new object[]{ modInfo, key, description, keyCode, onValueChange });
            return true;
        }

        public static T GetValue<T>(ModInfo modInfo, string key) {
            if (!Available(modInfo,key)) return default;
            MethodInfo methodInfo = GetStaticPublicMethodInfo(GET_VALUE);
            if (methodInfo == null) return default;
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(typeof(T));
            object result = genericMethod.Invoke(null,new object[]{modInfo,key});
            return (T)result;
        }

        public static bool SetValue<T>(ModInfo modInfo, string key, T value) {
            if (!Available(modInfo,key)) return false;
            MethodInfo methodInfo = GetStaticPublicMethodInfo(SET_VALUE);
            if (methodInfo == null) return false;
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(typeof(T));
            return (bool)genericMethod.Invoke(null,new object[]{modInfo,key,value});
        }

        public static bool RemoveUI<T>(ModInfo modInfo, string key) {
            if (!Available(modInfo,key)) return false;
            MethodInfo methodInfo = GetStaticPublicMethodInfo(REMOVE_UI);
            if (methodInfo == null) return false;
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(typeof(T));
            return (bool)genericMethod.Invoke(null,new object[]{modInfo,key});
        }

        public static bool RemoveMod(ModInfo modInfo) {
            if (!Available(modInfo)) return false;
            MethodInfo methodInfo = GetStaticPublicMethodInfo(REMOVE_MOD);
            if (methodInfo == null) return false;
            return (bool)methodInfo.Invoke(null,new object[]{modInfo});
        }

        private static bool Available(ModInfo modInfo) {
            return IsInit && modInfo.displayName != null && modInfo.name != null;
        }

        private static bool Available(ModInfo modInfo,string key) {
            return IsInit && modInfo.displayName != null && modInfo.name != null && key != null;
        }

        private static void CheckVersion() {
            FieldInfo versionField = modBehaviour.GetField("Version", BindingFlags.Public | BindingFlags.Static);
            if (versionField != null && versionField.FieldType == typeof(float))
            {
                float modSettingVersion = (float)versionField.GetValue(null);
                if (!Mathf.Approximately(modSettingVersion, Version))
                    Debug.LogWarning($"警告:ModSetting的版本:{modSettingVersion} (API的版本:{Version})");
            }
        }

        private static Type FindTypeInAssemblies(string typeName) {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) {
                if (assembly.FullName.Contains(MOD_NAME)) {
                    Debug.Log($"找到{MOD_NAME}相关程序集: {assembly.FullName}");
                }
                Type type = assembly.GetType(typeName);
                if (type != null) return type;
            }
            Debug.LogError("找不到程序集");
            return null;
        }
        private static MethodInfo GetStaticPublicMethodInfo(string methodName,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static) {
            if (!IsInit) return null;
            MethodInfo methodInfo = modBehaviour.GetMethod(methodName, bindingFlags);
            return methodInfo;
        }
    }
}