using System.Reflection;
using UnityEngine;

namespace ModSetting {
    public static class ReflectionExtension {
        public static T GetInstanceField<T>(object instance, string fieldName,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            where T : class {
            FieldInfo fieldInfo = GetInstanceFieldInfo(instance, fieldName, bindingFlags);
            return fieldInfo?.GetValue(instance) as T;
        }

        public static FieldInfo GetInstanceFieldInfo(object instance, string fieldName,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) {
            return instance.GetType().GetField(fieldName, bindingFlags);
        }

        public static bool SetInstanceField(object instance, string fieldName, object newValue) {
            FieldInfo fieldInfo = GetInstanceFieldInfo(instance, fieldName);
            if (fieldInfo == null) {
                Debug.Log(fieldName+"=>设置字段失败，字段信息为null:");
                return false;
            }
            fieldInfo.SetValue(instance, newValue);
            return true;
        }

        public static void InvokeInstanceMethod(object instance, string methodName, object[] parameters = null) {
            MethodInfo methodInfo = instance.GetType().GetMethod(methodName);
            methodInfo?.Invoke(instance, parameters);
        }
    }
}