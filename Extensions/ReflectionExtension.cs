using System.Reflection;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Extensions {
    public static class ReflectionExtension {
        public static T GetInstanceField<T>(this object instance, string fieldName,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            where T : class {
            FieldInfo fieldInfo = GetInstanceFieldInfo(instance, fieldName, bindingFlags);
            return fieldInfo?.GetValue(instance) as T;
        }

        public static FieldInfo GetInstanceFieldInfo(object instance, string fieldName,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) {
            return instance.GetType().GetField(fieldName, bindingFlags);
        }

        public static bool SetInstanceField(this object instance, string fieldName, object newValue) {
            FieldInfo fieldInfo = GetInstanceFieldInfo(instance, fieldName);
            if (fieldInfo == null) {
                Logger.Error($"找不到字段:{fieldName},设置字段失败");
                return false;
            }
            fieldInfo.SetValue(instance, newValue);
            return true;
        }

        public static void InvokeInstanceMethod(this object instance, string methodName, object[] parameters = null,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) {
            MethodInfo methodInfo = instance.GetType().GetMethod(methodName,bindingFlags);
            if (methodInfo == null) {
                Logger.Error($"找不到方法:{methodName},执行方法失败");
            }
            methodInfo?.Invoke(instance, parameters);
        }
    }
}