using System.Collections.Generic;
using ModSetting.Config.Data;
using UnityEngine;
using UnityEngine.Pool;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Pool {
    public static class UIPool {
        private static readonly bool collectionCheck = true;
        private static readonly int defaultCapacity = 10;
        private static int maxSize = 100;
        private static readonly Dictionary<UIType, IObjectPool<PoolableBehaviour>> pools = new();
        private static List<IPoolableSetting> settings = new();
        public static PoolableBehaviour Spawn(UIType uiType,Transform parent=null) {
            IPoolableSetting poolableSetting = GetIPoolableSetting(uiType);
            PoolableBehaviour poolableBehaviour = Spawn(poolableSetting);
            poolableBehaviour.transform.SetParent(parent,false);
            return poolableBehaviour;
        }
        public static void ReturnToPool(PoolableBehaviour poolable) {
            if (pools.TryGetValue(poolable.UIType,out var pool)) {
                pool.Release(poolable);
                return;
            }
            Logger.Error($"尝试归还的对象不属于任何已知池:{poolable.UIType}");
        }

        public static void AddSetting(IPoolableSetting setting) {
            settings.Add(setting);
        }

        public static void RemoveSetting(IPoolableSetting setting) {
            settings.Remove(setting);
        }
        private static PoolableBehaviour Spawn(IPoolableSetting setting) => GetPoolFor(setting).Get();

        private static IObjectPool<PoolableBehaviour> GetPoolFor(IPoolableSetting setting) {
            if (pools.TryGetValue(setting.UIType, out var pool)) {
                return pool;
            }
            pool = new ObjectPool<PoolableBehaviour>(
                setting.Create,
                setting.OnGet,
                setting.OnRelease,
                setting.OnDestroy,
                collectionCheck,
                defaultCapacity,
                maxSize);
            pools.Add(setting.UIType, pool);
            return pool;
        }
        private static IPoolableSetting GetIPoolableSetting(UIType uiType)
        {
            foreach (IPoolableSetting poolableSetting in settings) {
                if (poolableSetting.UIType == uiType) {
                    return poolableSetting;
                }
            }
            Logger.Error($"没有此物品Setting, UIType: {uiType}");
            return null;
        }
        public static void Clear() {
            pools.Clear();
            settings.Clear();
        }
    }
}