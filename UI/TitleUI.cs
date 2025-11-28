using System.Collections.Generic;
using System.Linq;
using ModSetting.Config.Data;
using ModSetting.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.UI {
    public class TitleUI : PoolableBehaviour {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image icon;
        private Texture2D preview;
        private PoolableBehaviour endGameObject;
        private float maxLength;
        private float imageLength;
        private Dictionary<string, PoolableBehaviour> settingDic = new();

        // 添加 Group 字典
        private Dictionary<string, GroupUI> groupDic = new();

        // 添加全局 UI 映射（key -> GroupUI），用于快速查找ui
        private Dictionary<string, GroupUI> uiToGroupMap = new();
        //key 内部组key，value是外部组
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

        public void Add(string key, PoolableBehaviour go) {
            PoolableBehaviour firstGo = settingDic.Values.FirstOrDefault();
            if (!settingDic.TryAdd(key, go)) {
                Logger.Error($"已经有此key的UI,添加失败.key:{key}");
                return;
            }
            Transform lastTransform = endGameObject == null ? transform : endGameObject.transform;
            int titleIndex = lastTransform.GetSiblingIndex();
            go.transform.SetSiblingIndex(titleIndex + 1);
            go.SetActive(firstGo!=null&& firstGo.gameObject.activeSelf);
            endGameObject = go;
        }

        private void AddTop(string key, PoolableBehaviour go) {
            PoolableBehaviour firstGo = settingDic.Values.FirstOrDefault();
            if (!settingDic.TryAdd(key, go)) {
                Logger.Error($"已经有此key的UI,添加失败.key:{key}");
                return;
            }
            int titleIndex = transform.GetSiblingIndex();
            go.transform.SetSiblingIndex(titleIndex + 1);
            go.SetActive(firstGo!=null&& firstGo.gameObject.activeSelf);
            endGameObject = endGameObject.transform.GetSiblingIndex() > go.transform.GetSiblingIndex()
                ? endGameObject
                : go;
        }

        private void OnClickButton() {
            PoolableBehaviour firstObject = settingDic.Values.FirstOrDefault(item=>item!=null);
            if (firstObject == null) return;
            bool activeSelf = firstObject.gameObject.activeSelf;
            foreach (PoolableBehaviour ui in settingDic.Values) {
                ui.SetActive(!activeSelf);
            }

            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.SetExpandedState(!activeSelf);
            }
        }
        public bool RemoveUI(string key) {
            if (settingDic.Remove(key, out var uiGameObject)) {
                if (groupDic.Remove(key,out var groupUI)) {
                    if (groupUI.Contains(endGameObject)) {
                        groupUI.RemoveAll();
                        endGameObject = null;
                        UpdateEndGameObject();
                    }
                    groupUI.RemoveAll();
                    groupUI.OnNestGroupRemoved-=GroupUI_OnNestGroupRemoved;
                    groupUI.OnUIRemoved-=GroupUI_OnUIRemoved;
                } else {
                    if (endGameObject == uiGameObject) {
                        endGameObject = null;
                        UpdateEndGameObject();
                    }
                }
                UIPrefabFactory.ReturnToPool(uiGameObject);
                return true;
            }
            if (uiToGroupMap.Remove(key, out var ui)) {
                if (endGameObject == ui) {
                    endGameObject = null;
                    UpdateEndGameObject();
                }
                return ui.RemoveUI(key);
            }
            return RemoveNestGroup(key);
        }

        private bool RemoveNestGroup(string key) {
            if (nestGroupMap.Remove(key, out var value)) {
                GroupUI nestGroup= value.GetGroupUI(key);
                if (nestGroup.Contains(endGameObject)) {
                    nestGroup.RemoveAll();
                    endGameObject = null;
                    UpdateEndGameObject();
                }
                nestGroup.RemoveAll();
                return value.RemoveUI(key);
            }
            return false;
        }

        public void Clear() {
            foreach (PoolableBehaviour uiGameObject in settingDic.Values) {
                UIPrefabFactory.ReturnToPool(uiGameObject);
            }

            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.RemoveAll();
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
            groupUI.OnNestGroupRemoved += GroupUI_OnNestGroupRemoved;
            groupUI.OnUIRemoved += GroupUI_OnUIRemoved;
            if (top) {
                AddTop(key, groupUI);
            } else {
                Add(key, groupUI);
            }
            //移除尾部插入时的索引变化问题
            foreach (string otherKey in keys) {
                if (settingDic.TryGetValue(otherKey, out var go)) {
                    go.transform.SetSiblingIndex(transform.parent.childCount-1);
                    if (groupDic.TryGetValue(otherKey, out var ui)) {
                        ui.MoveToEnd();
                        if (!nestGroupMap.TryAdd(otherKey, groupUI)) {
                            Logger.Error($"嵌套分组的key未清除干净:key:{otherKey}");
                        }
                    } else {
                        if (!uiToGroupMap.TryAdd(otherKey, groupUI)) {
                            Logger.Error($"UI映射的key未清除干净:key:{otherKey}");
                        }
                    }
                }
            }
            foreach (string otherKey in keys) {
                if (settingDic.Remove(otherKey, out var go)) {
                    if (groupDic.Remove(otherKey, out var ui)) {
                        ui.OnNestGroupRemoved -= GroupUI_OnNestGroupRemoved;
                        ui.OnUIRemoved -= GroupUI_OnUIRemoved;
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
            PoolableBehaviour groupEndGo = groupUI.GetEndGameObject();
            endGameObject = endGameObject.transform.GetSiblingIndex() >
                            groupEndGo.transform.GetSiblingIndex()
                ? endGameObject
                : groupEndGo;
        }

        private void UpdateEndGameObject() {
            List<PoolableBehaviour> candidates = new();
            PoolableBehaviour endGroupGo = groupDic.Values
                .OrderByDescending(groupUI => groupUI.transform.GetSiblingIndex())
                .FirstOrDefault(ui => ui != null)?
                .GetEndGameObject();
            if(endGroupGo!=null)candidates.Add(endGroupGo);
            PoolableBehaviour endUi = settingDic.Values
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

        private void GroupUI_OnNestGroupRemoved(string obj) {
            nestGroupMap.Remove(obj);
            Logger.Info($"移除嵌套分组的key:{obj}");
        }

        private void GroupUI_OnUIRemoved(string obj) {
            uiToGroupMap.Remove(obj);
            Logger.Info($"移除UI映射的key:{obj}");
        }
    }
}