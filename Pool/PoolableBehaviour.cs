using ModSetting.Config.Data;
using UnityEngine;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Pool {
    public class PoolableBehaviour : MonoBehaviour, IPoolable {
        public UIType UIType { get; set; }
        public virtual void OnGet() {
            Logger.Info($"获取对象:{UIType.ToString()}");
        }

        public virtual void OnRelease() {
            Logger.Info($"释放对象:{UIType.ToString()}");
        }

        public virtual void OnDestroyObject() {
        }

        public void SetActive(bool active) {
            gameObject.SetActive(active);
        }
    }
}