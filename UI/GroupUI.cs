using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModSetting.UI {
    public class GroupUI : MonoBehaviour {
        [SerializeField]private TextMeshProUGUI label;
        private float height=50f;
        private GameObject endGameObject;
        private Dictionary<string, GameObject> settingDic = new Dictionary<string, GameObject>();
        private bool lastActive;
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
        public void Setup(string description,List<string> keys,float height,bool open) {
            this.height = height;
            lastActive= open;
            label.text = description;
            Image bg = GetComponent<Image>();
            Button button = gameObject.AddComponent<Button>();
            button.image = bg;
            button.onClick.AddListener(OnClickButton);
            RectTransform labelTransform = label.GetComponent<RectTransform>();
            labelTransform.sizeDelta = new Vector2(200f,height);
            
        }
        public void Add(string key,GameObject go) {
            if (!settingDic.TryAdd(key, go)) {
                Debug.LogError("已经有此key的UI,key:"+key);
                return;
            }
            Transform lastTransform = endGameObject ==null ? transform: endGameObject.transform;
            bool active =endGameObject != null && endGameObject.activeSelf ;
            int titleIndex = lastTransform.GetSiblingIndex();
            go.transform.SetSiblingIndex(titleIndex+1);
            go.SetActive(active);
            endGameObject = go;
        }
        public bool RemoveUI(string key) {
            if (settingDic.TryGetValue(key,out var uiGameObject)) {
                if (endGameObject == uiGameObject) {
                    Transform parent = endGameObject.transform.parent;
                    endGameObject = parent.GetChild(parent.childCount - 1).gameObject;
                }
                Destroy(uiGameObject);
                return settingDic.Remove(key);
            }
            return false;
        }
        public void Clear() {
            foreach (GameObject uiGameObject in settingDic.Values) {
                Destroy(uiGameObject);
            }
            settingDic.Clear();
        }

        public void SetActive(bool active) {
            if (active) {
                foreach (GameObject ui in settingDic.Values) {
                    ui.SetActive(lastActive);
                }
            } else {
                lastActive = endGameObject.activeSelf;
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
            // label.enableAutoSizing = true;
        }

        private void OnClickButton() {
            if(endGameObject==null)return;
            bool activeSelf = endGameObject.activeSelf;
            foreach (GameObject ui in settingDic.Values) {
                ui.SetActive(!activeSelf);
            }
        }
    }
}