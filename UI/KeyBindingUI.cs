using System;
using System.Collections.Generic;
using Duckov.Modding;
using ModSetting.Config;
using ModSetting.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.UI {
    public class KeyBindingUI : PoolableBehaviour {
        [SerializeField]private TextMeshProUGUI label;
        [SerializeField]private Button rebindButton;
        [SerializeField]private Button clearButton;
        [SerializeField]private TextMeshProUGUI text;
        private bool isWaitingForInput = false;
        private KeyCode defaultKeyCode;
        private KeyCode newKeyCode;
        private KeyBindingManager keyBindingManager;
        private HashSet<KeyCode> validKeyCodes;
        public event Action<KeyCode> onValueChange;
        public KeyCode CurrentKey => newKeyCode;
        private ModInfo modInfo;
        private string key;
        private void Awake() {
            if (rebindButton != null) rebindButton.onClick.AddListener(OnClickBindingButton);
            if (clearButton != null) clearButton.onClick.AddListener(OnClickClearButton);
        }

        private void Update() {
            if(!isWaitingForInput) return;
            HandleKeyInput();
        }


        public void Init(TextMeshProUGUI label, Button rebindButton, Button clearButton,TextMeshProUGUI text,string defaultDescription,KeyCode defaultKeyCode) {
            this.label = label;
            this.rebindButton = rebindButton;
            this.text = text;
            label.text = defaultDescription;
            this.defaultKeyCode = defaultKeyCode;
            this.clearButton = clearButton;
        }

        public void Setup(KeyBindingConfig keyBindingConfig,KeyBindingManager keyBindingManager,List<KeyCode> validKeyCodes) {
            label.text = keyBindingConfig.Description;
            defaultKeyCode = keyBindingConfig.DefaultKeyCode;
            modInfo = keyBindingConfig.ModInfo;
            key=keyBindingConfig.Key;
            this.keyBindingManager = keyBindingManager;
            onValueChange += keyBindingConfig.SetValue;
            keyBindingConfig.OnValueChange +=SetNewKey;
            keyBindingManager.AddKeyCode(newKeyCode,this);
            SetNewKey(keyBindingConfig.KeyCode);
            this.validKeyCodes = new HashSet<KeyCode>(validKeyCodes);
            this.validKeyCodes.ExceptWith(new [] { KeyCode.None ,KeyCode.Escape});
            
        }

        private void HandleKeyInput() {
            if (!Input.anyKeyDown) return;
            foreach (KeyCode keyCode in validKeyCodes) {
                if (Input.GetKeyDown(keyCode)) {
                    CompleteRebinding(keyCode);
                    return;
                }
            }
        }

        private void StartRebinding() {
            if (isWaitingForInput || keyBindingManager.IsRebinding) return;
            rebindButton.interactable = false;
            isWaitingForInput = true;
            keyBindingManager.StartBinding();
            rebindButton.image.color=Color.green;
            text.text = "按任意键";
        }

        private void CompleteRebinding(KeyCode newKey) {
            rebindButton.interactable = true;
            isWaitingForInput = false;
            keyBindingManager.StopBinding();
            SetNewKey(newKey);
            onValueChange?.Invoke(newKey);
            UpdateDisplay();
        }

        private void OnDisable() {
            if(isWaitingForInput)CancelRebinding();
        }

        public override void OnRelease() {
            base.OnRelease();
            keyBindingManager.RemoveModKeyBinding(modInfo,key);
            onValueChange=null;
        }
        public void CancelRebinding() {
            rebindButton.interactable = true;
            isWaitingForInput = false;
            keyBindingManager.StopBinding();
            UpdateDisplay();
        }

        private void SetNewKey(KeyCode keyCode) {
            if(newKeyCode==keyCode)return;
            keyBindingManager.RemoveKeyCode(newKeyCode, this);
            newKeyCode = keyCode;
            keyBindingManager.AddKeyCode(newKeyCode,this);
        }

        public void UpdateDisplay() {
            text.text = GetKeyCodeDisplayName(newKeyCode);
            rebindButton.image.color = keyBindingManager.GetButtonColor(newKeyCode);
        }

        private void OnClickClearButton() {
            SetNewKey(defaultKeyCode);
            onValueChange?.Invoke(defaultKeyCode);
            UpdateDisplay();
        }

        private void OnClickBindingButton() {
            StartRebinding();
        }
        
        string GetKeyCodeDisplayName(KeyCode key) {
            // 美化键位显示名称
            switch (key) {
                case KeyCode.LeftControl: return "左Ctrl";
                case KeyCode.RightControl: return "右Ctrl";
                case KeyCode.LeftShift: return "左Shift";
                case KeyCode.RightShift: return "右Shift";
                case KeyCode.LeftAlt: return "左Alt";
                case KeyCode.RightAlt: return "右Alt";
                case KeyCode.Return: return "回车";
                case KeyCode.Backspace: return "退格";
                case KeyCode.Space: return "空格";
                case KeyCode.UpArrow: return "↑";
                case KeyCode.DownArrow: return "↓";
                case KeyCode.LeftArrow: return "←";
                case KeyCode.RightArrow: return "→";
                case KeyCode.Mouse0: return "鼠标左键";
                case KeyCode.Mouse1: return "鼠标右键";
                case KeyCode.Mouse2: return "鼠标中键";
                default: return key.ToString();
            }
        }
    }
}