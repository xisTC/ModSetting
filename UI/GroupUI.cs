using System.Collections.Generic;
using System.Linq;
using Duckov.Modding;
using ModSetting.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.UI {
    public class GroupUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI label;
        private Dictionary<string, GameObject> settingDic = new();
        private Dictionary<string, GroupUI> groupDic = new();
        private List<GameObject> gameObjects = new();
        private bool lastExpandedState;
        private ModInfo modInfo;
        private float scale;
        private float height;

        private void Start() {
            HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup == null) return;
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
        }

        public void Init() {
            CreateTitle();
        }

        public void Setup(ModInfo modInfo, string description, List<string> keys,float scale,bool open) {
            this.modInfo = modInfo;
            lastExpandedState = open;
            label.text = description;
            this.scale = scale;
            RectTransform rectTransform = label.GetComponent<RectTransform>();
            rectTransform.sizeDelta = label.GetPreferredValues(description);
            Image bg = GetComponent<Image>();
            Button button = gameObject.AddComponent<Button>();
            button.image = bg;
            button.onClick.AddListener(OnClickButton);
        }

        public void Add(string key, GameObject go) {
            GameObject lastGo = GetEndGameObject();
            if (!settingDic.TryAdd(key, go)) {
                Logger.Error($"已经有此key的UI,添加至分组失败,key:{key}");
                return;
            }
            gameObjects.Add(go);
            Transform lastTransform = lastGo == null ? transform : lastGo.transform;
            int titleIndex = lastTransform.GetSiblingIndex();
            go.transform.SetSiblingIndex(titleIndex + 1);
            go.SetActive(gameObject.activeSelf && lastGo != null && lastGo.activeSelf);
        }

        public bool RemoveUI(string key) {
            if (settingDic.Remove(key, out var uiGameObject)) {
                gameObjects.Remove(uiGameObject);
                groupDic.Remove(key);
                Destroy(uiGameObject);
                return true;
            }
            return false;
        }

        public void Clear() {
            foreach (var (key, value) in settingDic) {
                ConfigManager.RemoveUI(modInfo, key);
                Destroy(value);
            }

            settingDic.Clear();
            gameObjects.Clear();
            groupDic.Clear();
        }

        public GameObject GetEndGameObject() {
            List<GameObject> candidates = new();
            GameObject endGroupGo = groupDic.Values
                .OrderByDescending(groupUI => groupUI.transform.GetSiblingIndex())
                .FirstOrDefault(ui => ui != null)?
                .GetEndGameObject();
            if (endGroupGo != null) candidates.Add(endGroupGo);
            GameObject endUI = gameObjects.LastOrDefault(ui => ui != null);
            if (endUI != null) candidates.Add(endUI);
            return candidates
                .OrderByDescending(go => go.transform.GetSiblingIndex())
                .FirstOrDefault(item => item != null);
        }

        public bool Contains(GameObject go) {
            if(settingDic.Values.Contains(go)) return true;
            foreach (GroupUI groupUI in groupDic.Values) {
                if (groupUI.Contains(go)) {
                    return true;
                }
            }
            return false;
        }

        public void SetExpandedState(bool expanded) {
            if (expanded) {
                foreach (GameObject ui in settingDic.Values) {
                    ui.SetActive(lastExpandedState);
                }
            } else {
                lastExpandedState = gameObjects.First().activeSelf;
                foreach (GameObject ui in settingDic.Values) {
                    ui.SetActive(false);
                }
            }

            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.SetExpandedState(expanded);
            }
        }

        private void OnDestroy() {
            Clear();
        }

        private void CreateTitle() {
            GameObject titleTextObject = new GameObject("TitleText");
            titleTextObject.transform.SetParent(transform);
            label = titleTextObject.AddComponent<TextMeshProUGUI>();
            label.alignment = TextAlignmentOptions.Left;
            label.fontStyle = FontStyles.Bold;
        }

        private void OnClickButton() {
            GameObject firstObject = gameObjects.FirstOrDefault(item => item != null);
            if (firstObject == null) return;
            bool activeSelf = !firstObject.activeSelf;
            foreach (GameObject ui in settingDic.Values) {
                ui.SetActive(activeSelf);
            }
            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.SetExpandedState(activeSelf);
            }
        }

        public void MoveEnd() {
            foreach (GameObject go in settingDic.Values) {
                go.transform.SetSiblingIndex(transform.parent.childCount - 1);
            }
            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.MoveEnd();
            }
        }

        public void AddGroup(string key, GroupUI groupUI) {
            if (!groupDic.TryAdd(key, groupUI)) {
                Logger.Error($"不能使用相同key,此key分组已存在,key:{key}");
                return;
            }
            Add(key, groupUI.gameObject);
            if (!lastExpandedState) {
                groupUI.SetExpandedState(false);
            }
        }

        public void UpdateHeight(float height) {
            this.height=height*scale;
            RectTransform rectTransform = label.GetComponent<RectTransform>();
            label.fontSize =Mathf.Max(30f,label.fontSize*scale);
            rectTransform.sizeDelta = new Vector2(label.GetPreferredValues(label.text).x, this.height);
            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.UpdateHeight(this.height);
            }
        }
        public void ResetPosition() {
            for (var i = 0; i < gameObjects.Count; i++) {
                var go = gameObjects[i];
                go.transform.SetSiblingIndex(transform.GetSiblingIndex() + i + 1);
            }

            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.ResetPosition();
            }
        }

        public GroupUI GetGroupUI(string key) {
            return groupDic[key];
        }
    }
}