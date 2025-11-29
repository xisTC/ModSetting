using System;
using ModSetting.Config.Data;
using UnityEngine;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Pool {
    public class PoolableSetting<T> : IPoolableSetting where T : PoolableBehaviour{
        public PoolableSetting(UIType uiType, GameObject prefab) {
            UIType = uiType;
            Prefab = prefab;
        }
        public UIType UIType { get; }
        public GameObject Prefab { get; }

        public PoolableBehaviour Create() {
            GameObject gameObject = UnityEngine.Object.Instantiate(Prefab,UIPool.PoolParent);
            if (gameObject == null) {
                Logger.Error($"实例化 Prefab 失败: {Prefab.name}");
                throw new Exception($"实例化 Prefab 失败: {Prefab.name}");
            }
            Logger.Info($"实例化 Prefab 成功: {Prefab.name}");
            T t = gameObject.GetComponent<T>();
            if(t==null)t=gameObject.AddComponent<T>();
            t.UIType = UIType;
            return t;
        }

        public void OnGet(PoolableBehaviour value) {
            value.transform.localScale = Vector3.one;
            value.gameObject.SetActive(true);
            value.name = Prefab.name;
            value.OnGet();
        }

        public void OnRelease(PoolableBehaviour value) {
            value.transform.SetParent(UIPool.PoolParent,false);
            value.gameObject.SetActive(false);
            value.OnRelease();
        }

        public void OnDestroy(PoolableBehaviour value) {
            value.OnDestroyObject();
        }
    }
}