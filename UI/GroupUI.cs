using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Modding;
using ModSetting.Config;
using ModSetting.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.UI {
    public class GroupUI : PoolableBehaviour {
        [SerializeField] private TextMeshProUGUI label;
        private Dictionary<string, PoolableBehaviour> settingDic = new();
        private Dictionary<string, GroupUI> groupDic = new();
        private List<PoolableBehaviour> gameObjects = new();
        private bool lastExpandedState;
        private ModInfo modInfo;
        private float scale;
        private float height;
        public event Action<string> OnNestGroupRemoved; 
        public event Action<string> OnUIRemoved;
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
            Button button = gameObject.GetComponent<Button>();
            if(button==null)button=gameObject.AddComponent<Button>();
            button.image = bg;
            button.onClick.AddListener(OnClickButton);
        }

        public void Add(string key, PoolableBehaviour go) {
            PoolableBehaviour lastGo = GetEndGameObject();
            if (!settingDic.TryAdd(key, go)) {
                Logger.Error($"已经有此key的UI,添加至分组失败,key:{key}");
                return;
            }
            gameObjects.Add(go);
            Transform lastTransform = lastGo == null ? transform : lastGo.transform;
            int titleIndex = lastTransform.GetSiblingIndex();
            go.transform.SetSiblingIndex(titleIndex + 1);
            go.SetActive(gameObject.activeSelf && lastGo != null && lastGo.gameObject.activeSelf);
        }

        public bool RemoveUI(string key) {
            if (settingDic.Remove(key,out var uiGameObject)) {
                gameObjects.Remove(uiGameObject);
                ConfigManager.RemoveUI(modInfo, key);
                if (groupDic.Remove(key, out var groupUI)) {
                    groupUI.RemoveAll();
                    OnNestGroupRemoved?.Invoke(key);
                    groupUI.OnNestGroupRemoved-=OnNestGroupRemoved;
                    groupUI.OnUIRemoved-=OnUIRemoved;
                } else {
                    OnUIRemoved?.Invoke(key);
                }
                UIPrefabFactory.ReturnToPool(uiGameObject);
                return true;
            }
            return false;
        }

        public void RemoveAll() {
            foreach (var (key, groupUI) in groupDic) {
                groupUI.RemoveAll();
                settingDic.Remove(key);
                ConfigManager.RemoveUI(modInfo, key);
                OnNestGroupRemoved?.Invoke(key);
                groupUI.OnNestGroupRemoved-=OnNestGroupRemoved;
                groupUI.OnUIRemoved-=OnUIRemoved;
                UIPrefabFactory.ReturnToPool(groupUI);
            }
            groupDic.Clear();
            foreach (var (key, uiGo) in settingDic) {
                ConfigManager.RemoveUI(modInfo, key);
                OnUIRemoved?.Invoke(key);
                UIPrefabFactory.ReturnToPool(uiGo);
            }
            settingDic.Clear();
            gameObjects.Clear();
        }

        private void Clear() {
            foreach (var (key, value) in settingDic) {
                ConfigManager.RemoveUI(modInfo, key);
                UIPrefabFactory.ReturnToPool(value);
            }
            settingDic.Clear();
            gameObjects.Clear();
            groupDic.Clear();
        }

        public PoolableBehaviour GetEndGameObject() {
            List<PoolableBehaviour> candidates = new();
            PoolableBehaviour endGroupGo = groupDic.Values
                .OrderByDescending(groupUI => groupUI.transform.GetSiblingIndex())
                .FirstOrDefault(ui => ui != null)?
                .GetEndGameObject();
            if (endGroupGo != null) candidates.Add(endGroupGo);
            PoolableBehaviour endUI = gameObjects.LastOrDefault(ui => ui != null);
            if (endUI != null) candidates.Add(endUI);
            return candidates
                .OrderByDescending(go => go.transform.GetSiblingIndex())
                .FirstOrDefault(item => item != null);
        }

        public bool Contains(PoolableBehaviour go) {
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
                foreach (PoolableBehaviour ui in settingDic.Values) {
                    ui.SetActive(lastExpandedState);
                }
            } else {
                lastExpandedState = gameObjects.First().gameObject.activeSelf;
                foreach (PoolableBehaviour ui in settingDic.Values) {
                    ui.SetActive(false);
                }
            }

            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.SetExpandedState(expanded);
            }
        }

        public override void OnRelease() {
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
            PoolableBehaviour firstObject = gameObjects.FirstOrDefault(item => item != null);
            if (firstObject == null) return;
            bool activeSelf = !firstObject.gameObject.activeSelf;
            foreach (PoolableBehaviour ui in settingDic.Values) {
                ui.SetActive(activeSelf);
            }
            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.SetExpandedState(activeSelf);
            }
        }

        public void MoveToEnd() {
            foreach (PoolableBehaviour go in settingDic.Values) {
                go.transform.SetSiblingIndex(transform.parent.childCount - 1);
            }
            foreach (GroupUI groupUI in groupDic.Values) {
                groupUI.MoveToEnd();
            }
        }

        public void AddGroup(string key, GroupUI groupUI) {
            if (!groupDic.TryAdd(key, groupUI)) {
                Logger.Error($"不能使用相同key,此key分组已存在,key:{key}");
                return;
            }
            groupUI.OnNestGroupRemoved += OnNestGroupRemoved;
            groupUI.OnUIRemoved += OnUIRemoved;
            Add(key, groupUI);
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