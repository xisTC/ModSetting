using ModSetting.Config.Data;
using UnityEngine;

namespace ModSetting.Pool {
    public interface IPoolableSetting{
        public UIType UIType { get; }
        public GameObject Prefab { get; }
        PoolableBehaviour Create();
        void OnGet(PoolableBehaviour value);
        void OnRelease(PoolableBehaviour value);
        void OnDestroy(PoolableBehaviour value);
    }
}