using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.UI {
    public class TitleUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image icon;
        private Texture2D preview;
        private GameObject endGameObject;
        private float maxLength;
        private float imageLength;
        private Dictionary<string, GameObject> settingDic = new();

        // 添加 Group 字典
        private Dictionary<string, GroupUI> groupDic = new();

        // 添加全局 UI 映射（key -> GroupUI），用于快速查找ui
        private Dictionary<string, GroupUI> uiToGroupMap = new();
        
        private Dictionary<string, GroupUI> nestGroupMap = new();
        private float height;

        private void Start() {
            HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup == null) return;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
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
            UpdateImageLength(imageLength);
            Image bg = GetComponent<Image>();
            Button button = gameObject.GetComponent<Button>();
            if (button == null) button = gameObject.AddComponent<Button>();
            button.image = bg;
            button.onClick.AddListener(OnClickButton);
        }

        private void CreateTitle() {
            GameObject titleTextObject = new GameObject("TitleText");
            titleTextObject.transform.SetParent(transform);
            label = titleTextObject.AddComponent<TextMeshProUGUI>();
            label.alignment = TextAlignmentOptions.Left;
            label.fontStyle = FontStyles.Bold;
        }

        public void Setup(Texture2D texture2D, string description,float fontSize,float imageLength,float maxLength) {
            preview = texture2D;
            label.text = description;
            this.maxLength = maxLength;
            this.imageLength= imageLength;
            height = imageLength + 10 + 10;
            UpdateFontSize(fontSize);
            UpdateSpace(Setting.TitleSpace);
            SetImageAndButton();
        }

        public void Add(string key, GameObject go) {
            GameObject firstGo = settingDic.Values.FirstOrDefault();
            if (!settingDic.TryAdd(key, go)) {
                Logger.Error($"已经有此key的UI,添加失败.key:{key}");
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
                Logger.Error($"已经有此key的UI,添加失败.key:{key}");
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
                groupUI.SetExpandedState(!activeSelf);
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
                    if (nestGroupMap.Values.Contains(groupUI)) {
                        string nestKey = nestGroupMap.FirstOrDefault(kv => kv.Value == groupUI).Key;
                        nestGroupMap.Remove(nestKey);
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
            if (nestGroupMap.Remove(key, out var value)) {
                GroupUI nestGroup= value.GetGroupUI(key);
                if (nestGroup.Contains(endGameObject)) {
                    nestGroup.Clear();
                    endGameObject = null;
                    UpdateEndGameObject();
                }
                return value.RemoveUI(key);
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
                Logger.Error($"不能使用相同key添加至标题,key:{key}");
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
                    if (groupDic.TryGetValue(otherKey, out var ui)) {
                        ui.MoveEnd();
                        nestGroupMap.Add(otherKey, groupUI);
                    } else {
                        uiToGroupMap.Add(otherKey, groupUI);
                    }
                }
            }
            foreach (string otherKey in keys) {
                if (settingDic.Remove(otherKey, out var go)) {
                    if (groupDic.Remove(otherKey, out var ui)) {
                        groupUI.AddGroup(otherKey,ui);
                    } else {
                        groupUI.Add(otherKey, go);
                    }
                } else {
                    Logger.Error($"title中没有此key,key:{otherKey}");
                }
            }
            groupUI.UpdateHeight(height);
            groupUI.ResetPosition();
            //group的add会改变尾部物体，需要更新
            GameObject groupEndGo = groupUI.GetEndGameObject();
            endGameObject = endGameObject.transform.GetSiblingIndex() >
                            groupEndGo.transform.GetSiblingIndex()
                ? endGameObject
                : groupEndGo;
        }

        private void UpdateEndGameObject() {
            List<GameObject> candidates = new();
            GameObject endGroupGo = groupDic.Values
                .OrderByDescending(groupUI => groupUI.transform.GetSiblingIndex())
                .FirstOrDefault(ui => ui != null)?
                .GetEndGameObject();
            if(endGroupGo!=null)candidates.Add(endGroupGo);
            GameObject endUi = settingDic.Values
                .OrderByDescending(go=>go.transform.GetSiblingIndex())
                .FirstOrDefault(item=>item!=null);
            if(endUi!=null)candidates.Add(endUi);
            if(endGameObject!=null)candidates.Add(endGameObject);
            endGameObject = candidates
                .OrderByDescending(go => go.transform.GetSiblingIndex())
                .FirstOrDefault(item => item != null);
        }
        public void UpdateFontSize(float fontSize) {
            label.fontSize = fontSize;
            Vector2 preferredSize = label.GetPreferredValues(label.text);
            RectTransform rectTransform = label.GetComponent<RectTransform>();
            //间距
            float labelLength = maxLength - imageLength - 10 - Setting.TitleSpace - 10;
            if (preferredSize.x > labelLength) {
                rectTransform.sizeDelta = new Vector2(labelLength, preferredSize.y);
                // 设置文本溢出模式为省略号
                label.overflowMode = TextOverflowModes.Ellipsis;
            } else {
                rectTransform.sizeDelta = new Vector2(preferredSize.x, preferredSize.y);
                label.overflowMode = TextOverflowModes.Overflow;
            }
            UpdateHeight();
        }

        public void UpdateImageLength(float imageLength) {
            this.imageLength = imageLength;
            RectTransform iconTransform = icon.GetComponent<RectTransform>();
            iconTransform.sizeDelta = new Vector2(imageLength, imageLength);
            if (imageLength + 10 + 10 + Setting.TitleSpace+label.rectTransform.rect.width> maxLength) UpdateFontSize(label.fontSize);
            UpdateHeight();
        }

        public void UpdateSpace(float space) {
            HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup == null) return;
            layoutGroup.spacing = space;
            if (imageLength + 10 + 10 +space+label.rectTransform.rect.width> maxLength) UpdateFontSize(label.fontSize);
        }

        private void UpdateHeight() {
            if (imageLength + 10 + 10 > height || 
                label.rectTransform.rect.height > height||
                (imageLength+10+10<height&&label.rectTransform.rect.height<height)) {
                height = Mathf.Max(imageLength + 10 + 10, label.rectTransform.rect.height);
                foreach (GroupUI groupUI in groupDic.Values) {
                    groupUI.UpdateHeight(height);
                }
            }
        }
    }
}