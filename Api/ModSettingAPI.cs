using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Duckov.Modding;
using UnityEngine;

public static class ModSettingAPI {
    private const string ADD_DROP_DOWN_LIST = "AddDropDownList";
    private const string ADD_SLIDER = "AddSlider";
    private const string ADD_TOGGLE = "AddToggle";
    private const string ADD_KEYBINDING = "AddKeybinding";
    private const string GET_VALUE = "GetValue";
    private const string SET_VALUE = "SetValue";
    private const string REMOVE_UI = "RemoveUI";
    private const string REMOVE_MOD = "RemoveMod";
    private const string ADD_INPUT = "AddInput";
    private const string HAS_CONFIG = "HasConfig";
    private const string GET_SAVED_VALUE = "GetSavedValue";
    private const string ADD_KEYBINDING_WITH_DEFAULT = "AddKeybindingWithDefault";
    private const string ADD_BUTTON = "AddButton";
    private static float Version = 0.3f;
    public const string MOD_NAME = "ModSetting";
    private const string TYPE_NAME = "ModSetting.ModBehaviour";
    private static Type modBehaviour;
    private static ModInfo modInfo;

    public static bool IsInit { get; private set; }

    // 缓存委托避免重复反射
    private static Dictionary<string, Delegate> methodCache = new Dictionary<string, Delegate>();

    private static readonly string[] methodNames = new[] {
        ADD_DROP_DOWN_LIST,
        ADD_SLIDER,
        ADD_TOGGLE,
        ADD_KEYBINDING,
        GET_VALUE,
        SET_VALUE,
        REMOVE_UI,
        REMOVE_MOD,
        ADD_INPUT,
        HAS_CONFIG,
        GET_SAVED_VALUE,
        ADD_KEYBINDING_WITH_DEFAULT
    };

    public static bool Init(ModInfo modInfo) {
        if (IsInit) return true;
        ModSettingAPI.modInfo = modInfo;
        modBehaviour = FindTypeInAssemblies(TYPE_NAME);
        if (modBehaviour == null) return false;
        VersionAvailable();
        foreach (string methodName in methodNames) {
            MethodInfo[] methodInfos = modBehaviour.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == methodName)
                .ToArray();
            if (methodInfos.Length == 0) {
                Debug.LogError($"{methodName}方法找不到");
                return false;
            }
        }

        IsInit = true;
        return true;
    }

    public static bool AddDropdownList(string key, string description,
        List<string> options, string defaultValue, Action<string> onValueChange = null) {
        if (!Available(key)) return false;
        Type delegateType = typeof(Action<ModInfo,string,string,List<string>,string,Action<string>>);
        return InvokeMethod(ADD_DROP_DOWN_LIST,
            ADD_DROP_DOWN_LIST,
            new object[]{modInfo,key,description,options,defaultValue,onValueChange},
            delegateType);
    }

    public static bool AddSlider(string key, string description,
        float defaultValue, Vector2 sliderRange, Action<float> onValueChange = null,int decimalPlaces = 1,int characterLimit=5) {
        if (!Available(key)) return false;
        Type[] paramTypes = {
            typeof(ModInfo), typeof(string), typeof(string),
            typeof(float), typeof(Vector2),typeof(Action<float>),typeof(int),typeof(int)
        };
        Type delegateType = typeof(Action<ModInfo, string, string, float, Vector2, Action<float>, int, int>);
        return InvokeMethod(ADD_SLIDER + "Float",
            ADD_SLIDER,
            new object[]
                { modInfo, key, description, defaultValue, sliderRange, onValueChange, decimalPlaces, characterLimit },
            delegateType,
            paramTypes);
    }

    public static bool AddSlider(string key, string description,
        int defaultValue, int minValue, int maxValue, Action<int> onValueChange = null,int characterLimit=5) {
        if (!Available(key)) return false;
        Type[] paramTypes = {
            typeof(ModInfo), typeof(string), typeof(string),
            typeof(int), typeof(int), typeof(int), typeof(Action<int>),typeof(int)
        };
        Type delegateType = typeof(Action<ModInfo,string,string,int,int,int,Action<int>,int>);
        return InvokeMethod(ADD_SLIDER + "Int", ADD_SLIDER,
            new object[]
                { modInfo, key, description, defaultValue, minValue, maxValue, onValueChange, characterLimit },
            delegateType,
            paramTypes);
    }

    public static bool AddToggle(string key, string description,
        bool enable, Action<bool> onValueChange = null) {
        if (!Available(key)) return false;
        Type delegateType = typeof(Action<ModInfo, string, string, bool, Action<bool>>);
        return InvokeMethod(ADD_TOGGLE,
            ADD_TOGGLE,
            new object[] { modInfo, key, description, enable, onValueChange },
            delegateType);
    }

    public static bool AddKeybinding(string key, string description,
        KeyCode keyCode, Action<KeyCode> onValueChange = null) {
        if (!Available(key)) return false;
        return InvokeMethod(ADD_KEYBINDING,
            ADD_KEYBINDING,
            new object[] { modInfo, key, description, keyCode, onValueChange },
            typeof(Action<ModInfo,string,string,KeyCode,Action<KeyCode>>));
    }

    public static bool AddKeybindingWithDefault(string key, string description,
        KeyCode keyCode, KeyCode defaultKeyCode, Action<KeyCode> onValueChange = null) {
        if (!Available(key)) return false;
        return InvokeMethod(ADD_KEYBINDING_WITH_DEFAULT,
            ADD_KEYBINDING_WITH_DEFAULT,
            new object[] { modInfo, key, description, keyCode,defaultKeyCode,onValueChange },
            typeof(Action<ModInfo,string,string,KeyCode,KeyCode,Action<KeyCode>>));
    }

    public static bool AddInput(string key, string description,
        string defaultValue, int characterLimit = 40, Action<string> onValueChange = null) {
        if (!Available(key)) return false;
        return InvokeMethod(ADD_INPUT,
            ADD_INPUT,
            new object[] { modInfo, key, description, defaultValue, characterLimit, onValueChange },
            typeof(Action<ModInfo, string, string, string, int, Action<string>>));
    }

    public static bool AddButton(string key, string description,
        string buttonText = "按钮", Action onClickButton = null) {
        if (!Available(key)) return false;
        return InvokeMethod(ADD_BUTTON,
            ADD_BUTTON,
            new object[] { modInfo, key, description, buttonText, onClickButton},
            typeof(Action<ModInfo, string, string, string,Action>));
    }
    public static bool GetValue<T>(string key, Action<T> callback = null) {
        if (!Available(key)) return false;
        MethodInfo methodInfo = GetStaticPublicMethodInfo(GET_VALUE);
        if (methodInfo == null) return false;
        MethodInfo genericMethod = methodInfo.MakeGenericMethod(typeof(T));
        genericMethod.Invoke(null, new object[] { modInfo, key, callback });
        return true;
    }

    public static bool SetValue<T>(string key, T value, Action<bool> callback = null) {
        if (!Available(key)) return false;
        MethodInfo methodInfo = GetStaticPublicMethodInfo(SET_VALUE);
        if (methodInfo == null) return false;
        MethodInfo genericMethod = methodInfo.MakeGenericMethod(typeof(T));
        genericMethod.Invoke(null, new object[] { modInfo, key, value, callback });
        return true;
    }

    public static bool HasConfig() {
        if (!Available()) return false;
        MethodInfo methodInfo = GetStaticPublicMethodInfo(HAS_CONFIG);
        if (methodInfo == null) return false;
        return (bool)methodInfo.Invoke(null,new object[]{modInfo});
    }

    public static bool GetSavedValue<T>(string key, out T value) {
        value = default;
        if (!Available(key)) return false;
        MethodInfo methodInfo = GetStaticPublicMethodInfo(GET_SAVED_VALUE);
        if (methodInfo == null) return false;
        MethodInfo genericMethod=methodInfo.MakeGenericMethod(typeof(T));
        // 准备参数数组（注意：out 参数需要特殊处理）
        object[] parameters = new object[] { modInfo, key, null };
        bool result=(bool)genericMethod.Invoke(null,parameters);
        // 获取 out 参数的值
        value = (T)parameters[2];
        return result;
    }

    public static bool RemoveUI(string key, Action<bool> callback = null) {
        if (!Available(key)) return false;
        return InvokeMethod(REMOVE_UI,
            REMOVE_UI,
            new object[] { modInfo, key, callback },
            typeof(Action<ModInfo, string, Action<bool>>));
    }

    public static bool RemoveMod(Action<bool> callback = null) {
        if (!Available()) return false;
        Type delegateType = typeof(Action<ModInfo,Action<bool>>);
        return InvokeMethod(REMOVE_MOD, REMOVE_MOD, new object[] { modInfo, callback }, delegateType);
    }

    private static bool Available() {
        return IsInit && modInfo.displayName != null && modInfo.name != null;
    }

    private static bool Available(string key) {
        return IsInit && modInfo.displayName != null && modInfo.name != null && key != null;
    }

    private static bool VersionAvailable() {
        FieldInfo versionField = modBehaviour.GetField("Version", BindingFlags.Public | BindingFlags.Static);
        if (versionField != null && versionField.FieldType == typeof(float)) {
            float modSettingVersion = (float)versionField.GetValue(null);
            if (!Mathf.Approximately(modSettingVersion, Version)) {
                Debug.LogWarning($"警告:ModSetting的版本:{modSettingVersion} (API的版本:{Version}),新功能将无法使用");
                return false;
            }
            return true;
        }
        return false;
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

        Debug.Log("找不到程序集");
        return null;
    }

    private static MethodInfo GetStaticPublicMethodInfo(string methodName, Type[] parameterTypes = null) {
        if (!IsInit) return null;
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;
        if (parameterTypes != null) {
            MethodInfo[] methodInfos = modBehaviour.GetMethods(bindingFlags).Where(m => m.Name == methodName).ToArray();
            return methodInfos.Where(methodInfo => {
                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length != parameterTypes.Length) return false;
                for (int i = 0; i < parameters.Length; i++) {
                    // 处理参数类型匹配（包括继承和接口实现）
                    if (!IsParameterTypeMatch(parameters[i].ParameterType, parameterTypes[i]))
                        return false;
                }

                return true;
            }).FirstOrDefault();
        } else {
            MethodInfo methodInfo = modBehaviour.GetMethod(methodName, bindingFlags);
            return methodInfo;
        }
    }

    private static bool IsParameterTypeMatch(Type parameterType, Type providedType) {
        // 精确匹配
        if (parameterType == providedType)
            return true;
        // 处理值类型和可空类型
        if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == providedType)
            return true;
        // 处理继承关系
        if (parameterType.IsAssignableFrom(providedType))
            return true;
        return false;
    }

    private static bool InvokeMethod(string cacheKey,string methodName,object[] parameters,Type delegateType,Type[] paramTypes=null) {
        if (!methodCache.ContainsKey(cacheKey))
        {
            MethodInfo method = GetStaticPublicMethodInfo(methodName,paramTypes);
            if (method == null) return false;
            // 创建委托
            methodCache[cacheKey] = Delegate.CreateDelegate(delegateType, method);
        }
        try
        {
            methodCache[cacheKey].DynamicInvoke(parameters);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"委托调用{methodName}失败: {ex.Message}");
            return false;
        }
    }
}