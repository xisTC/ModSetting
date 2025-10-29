using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Test {
    public class TitleUI : MonoBehaviour {
        [SerializeField]private TextMeshProUGUI label;
        [SerializeField]private Image icon;
        private Texture2D preview;
        private float height=50f;
        private readonly float textWidth=300f;
        private string description;
        private GameObject endGameObject;
        private Dictionary<string, GameObject> settingDic = new Dictionary<string, GameObject>();
        private void Start() {
            HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if(layoutGroup==null)return;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.spacing = 200f;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
                layoutGroup.childAlignment --;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
                layoutGroup.childAlignment ++;
            }
        }

        public void Init() {
            // 创建标题文本
            CreateTitle();
            //创建Image
            CreateImage();
        }
        private void CreateImage() {
            GameObject imageObject = new GameObject("Image");
            imageObject.transform.SetParent(transform);
            icon = imageObject.AddComponent<Image>();
        }

        private void Refresh() {
            if(preview!=null) icon.sprite=Sprite.Create(preview,new Rect(0, 0,preview.width,preview.height),new Vector2(0.5f,0.5f));
            RectTransform iconTransform = icon.GetComponent<RectTransform>();
            iconTransform.sizeDelta = new Vector2(height, height);
            RectTransform labelTransform = label.GetComponent<RectTransform>();
            labelTransform.sizeDelta = new Vector2(textWidth,height);
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

        public void Setup(Texture2D texture2D,string description,float height) {
            this.description = description;
            preview = texture2D;
            this.height = height;
            label.text = description;
            Refresh();
        }

        public void Add(string key,GameObject go) {
            if (!settingDic.TryAdd(key, go)) {
                Debug.LogError("已经有此key的UI,key:"+key);
                return;
            }
            Transform lastTransform = endGameObject ==null ? transform: endGameObject.transform;
            int titleIndex = lastTransform.GetSiblingIndex();
            go.transform.SetSiblingIndex(titleIndex+1);
            go.SetActive(false);
            endGameObject = go;
        }

        private void OnClickButton() {
            if(endGameObject==null)return;
            bool activeSelf = endGameObject.activeSelf;
            foreach (GameObject ui in settingDic.Values) {
                ui.SetActive(!activeSelf);
            }
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
    }
}