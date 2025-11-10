using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// todo 实现嵌套，UI应该持有config，UI销毁时应该移除对config的监听，防止内存泄露
namespace ModSetting.UI {
    public class TitleUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image icon;
        private Texture2D preview;
        private float height = 50f;
        private readonly float textWidth = 300f;
        private string description;
        private GameObject endGameObject;

        private Dictionary<string, GameObject> settingDic = new Dictionary<string, GameObject>();

        // 添加 Group 字典
        private Dictionary<string, GroupUI> groupDic = new Dictionary<string, GroupUI>();

        // 添加全局 UI 映射（key -> GroupUI），用于快速查找
        private Dictionary<string, GroupUI> uiToGroupMap = new Dictionary<string, GroupUI>();

        private void Start() {
            HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup == null) return;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.spacing = 200f;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        }

        public void Init() {
            CreateTitle();
            CreateImage();
        }

        private void CreateImage() {
            GameObject imageObject = new GameObject("Image");
            imageObject.transform.SetParent(transform);
            icon = imageObject.AddComponent<Image>();
        }

        private void SetImageAndButton() {
            if (preview != null)
                icon.sprite = Sprite.Create(preview, new Rect(0, 0, preview.width, preview.height),
                    new Vector2(0.5f, 0.5f));
            RectTransform iconTransform = icon.GetComponent<RectTransform>();
            iconTransform.sizeDelta = new Vector2(height, height);
            RectTransform labelTransform = label.GetComponent<RectTransform>();
            labelTransform.sizeDelta = new Vector2(textWidth, height);
            Image bg = GetComponent<Image>();
            Button button = gameObject.AddComponent<Button>();
            button.image = bg;
            button.onClick.AddListener(OnClickButton);
        }

        private void CreateTitle() {
            GameObject titleTextObject = new GameObject("TitleText");
            titleTextObject.transform.SetParent(transform);
            label = titleTextObject.AddComponent<TextMeshProUGUI>();
            label.alignment = TextAlignmentOptions.Left;
            label.fontStyle = FontStyles.Bold;
            label.autoSizeTextContainer = true;
        }

        public void Setup(Texture2D texture2D, string description, float height) {
            this.description = description;
            preview = texture2D;
            this.height = height;
            label.text = description;
            SetImageAndButton();
        }

        public void Add(string key, GameObject go) {
            GameObject firstGo = settingDic.Values.FirstOrDefault();
            if (!settingDic.TryAdd(key, go)) {
                Debug.LogError("已经有此key的UI,key:" + key);
                return;
            }
            Transform lastTransform = endGameObject == null ? transform : endGameObject.transform;
            int titleIndex = lastTransform.GetSiblingIndex();
            go.transform.SetSiblingIndex(titleIndex + 1);
            go.SetActive(firstGo!=null&& firstGo.activeSelf);
            endGameObject = go;
        }

        private void AddTop(string key, GameObject go) {
            GameObject firstGo = settingDic.Values.FirstOrDefault();
            if (!settingDic.TryAdd(key, go)) {
                Debug.LogError("已经有此key的UI,key:" + key);
                return;
            }
            int titleIndex = transform.GetSiblingIndex();
            go.transform.SetSiblingIndex(titleIndex + 1);
            go.SetActive(firstGo!=null&& firstGo.activeSelf);
            endGameObject = endGameObject.transform.GetSiblingIndex() > go.transform.GetSiblingIndex()
                ? endGameObject
                : go;
        }

        private void OnClickButton() {
            GameObject firstObject = settingDic.Values.FirstOrDefault(item=>item!=null);
            if (firstObject == null) return;
            bool activeSelf = firstObject.activeSelf;
            foreach (GameObject ui in settingDic.Values) {
                ui.SetActive(!activeSelf);
            }

            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.SetActive(!activeSelf);
            }
        }
        public bool RemoveUI(string key) {
            if (settingDic.Remove(key, out var uiGameObject)) {
                if (groupDic.Remove(key,out var groupUI)) {
                    var keysToRemove = uiToGroupMap.
                        Where(kv => kv.Value == groupUI).
                        Select(kv => kv.Key).ToList();
                    foreach (var k in keysToRemove) {
                        uiToGroupMap.Remove(k);
                    }
                    if (groupUI.Contains(endGameObject)) {
                        groupUI.Clear();
                        endGameObject = null;
                        UpdateEndGameObject();
                    }
                } else {
                    if (endGameObject == uiGameObject) {
                        endGameObject = null;
                        UpdateEndGameObject();
                    }
                }
                Destroy(uiGameObject);
                return true;
            }
            if (uiToGroupMap.Remove(key, out var ui)) {
                if (endGameObject == ui.gameObject) {
                    endGameObject = null;
                    UpdateEndGameObject();
                }
                return ui.RemoveUI(key);
            }
            return false;
        }

        public void Clear() {
            foreach (GameObject uiGameObject in settingDic.Values) {
                Destroy(uiGameObject);
            }

            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.Clear();
            }

            settingDic.Clear();
            groupDic.Clear();
            uiToGroupMap.Clear();
        }

        public void AddGroup(string key, GroupUI groupUI, List<string> keys, bool top) {
            if (!groupDic.TryAdd(key, groupUI)) {
                Debug.LogError($"不能使用相同key,key:{key}");
                return;
            }
            if (top) {
                AddTop(key, groupUI.gameObject);
            } else {
                Add(key, groupUI.gameObject);
            }
            //移除尾部插入时的索引变化问题
            foreach (string otherKey in keys) {
                if (settingDic.TryGetValue(otherKey, out var go)) {
                    go.transform.SetSiblingIndex(transform.parent.childCount-1);
                    uiToGroupMap.Add(otherKey, groupUI);
                }
            }
            foreach (string otherKey in keys) {
                if (settingDic.Remove(otherKey, out var go)) {
                    groupUI.Add(otherKey, go);
                } else {
                    Debug.LogError("title中没有此key,key:" + otherKey);
                }
            }
            //group的add会改变尾部物体，需要更新
            GameObject groupLastGo = groupUI.GetEndGameObject();
            endGameObject = endGameObject.transform.GetSiblingIndex() >
                            groupLastGo.transform.GetSiblingIndex()
                ? endGameObject
                : groupLastGo;
        }

        private void UpdateEndGameObject() {
            List<GameObject> candidates = new();
            GameObject lastGroupGo = groupDic.Values
                .OrderByDescending(groupUI => groupUI.transform.GetSiblingIndex())
                .FirstOrDefault(ui => ui != null)?
                .GetEndGameObject();
            if(lastGroupGo!=null)candidates.Add(lastGroupGo);
            GameObject lastUI = settingDic.Values
                .OrderByDescending(go=>go.transform.GetSiblingIndex())
                .FirstOrDefault(item=>item!=null);
            if(lastUI!=null)candidates.Add(lastUI);
            if(endGameObject!=null)candidates.Add(endGameObject);
            endGameObject = candidates
                .OrderByDescending(go => go.transform.GetSiblingIndex())
                .FirstOrDefault(item => item != null);
        }
        public bool HasNest(List<string> keys) => keys.Intersect(groupDic.Keys).Any();
    }
}