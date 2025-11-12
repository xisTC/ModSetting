using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Modding;
using ModSetting.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModSetting.UI {
    public class GroupUI : MonoBehaviour {
        [SerializeField]private TextMeshProUGUI label;
        private Dictionary<string, GameObject> settingDic = new Dictionary<string, GameObject>();
        private bool lastActive;
        private ModInfo modInfo;
        private void Start() {
            HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if(layoutGroup==null)return;
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
        }

        public void Init() {
            CreateTitle();
        }
        public void Setup(ModInfo modInfo,string description,List<string> keys,float height,bool open) {
            this.modInfo = modInfo;
            lastActive= open;
            label.text = description;
            RectTransform rectTransform = label.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(label.GetPreferredValues(description).x, height);
            Image bg = GetComponent<Image>();
            Button button = gameObject.AddComponent<Button>();
            button.image = bg;
            button.onClick.AddListener(OnClickButton);
            
        }
        public void Add(string key,GameObject go) {
            GameObject lastGo = GetEndGameObject();
            if (!settingDic.TryAdd(key, go)) {
                Debug.LogError("已经有此key的UI,key:"+key);
                return;
            }
            Transform lastTransform = lastGo == null ? transform : lastGo.transform;
            int titleIndex = lastTransform.GetSiblingIndex();
            go.transform.SetSiblingIndex(titleIndex+1);
            go.SetActive( lastGo != null && lastGo.activeSelf);
        }
        public bool RemoveUI(string key) {
            if (settingDic.TryGetValue(key,out var uiGameObject)) {
                Destroy(uiGameObject);
                return settingDic.Remove(key);
            }
            return false;
        }
        public void Clear() {
            foreach (var (key, value) in settingDic) {
                ConfigManager.RemoveUI(modInfo, key);
                Destroy(value);
            }
            settingDic.Clear();
        }

        public GameObject GetEndGameObject() {
            return settingDic.Values
                .OrderByDescending(gobject => gobject.transform.GetSiblingIndex())
                .FirstOrDefault(last => last != null);
        }

        public bool Contains(GameObject go) => settingDic.Values.Contains(go);
        public void SetActive(bool active) {
            if (active) {
                foreach (GameObject ui in settingDic.Values) {
                    ui.SetActive(lastActive);
                }
            } else {
                lastActive = settingDic.Values.First().activeSelf;
                foreach (GameObject ui in settingDic.Values) {
                    ui.SetActive(false);
                }
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
            label.autoSizeTextContainer = true;
        }

        private void OnClickButton() {
            GameObject firstObject = settingDic.Values.FirstOrDefault(item=>item!=null);
            if (firstObject == null) return;
            bool activeSelf = firstObject.activeSelf;
            foreach (GameObject ui in settingDic.Values) {
                ui.SetActive(!activeSelf);
            }
        }
    }
}