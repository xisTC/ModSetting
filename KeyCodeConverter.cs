using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = ModSetting.Log.Logger;

namespace ModSetting {
    public static class KeyCodeConverter {
        private static readonly Dictionary<Key, KeyCode> keyToKeyCodeMap = new();
        private static readonly Dictionary<KeyCode, Key> keyCodeToKeyMap = new();
        private static bool isInit;
        private static void AddMapping(Key key, KeyCode keyCode) {
            keyToKeyCodeMap[key] = keyCode;
            keyCodeToKeyMap[keyCode] = key;
        }

        public static KeyCode ConverterToKeyCode(Key key) {
            if (!isInit) Logger.Error($"KeyCodeConverter未初始化,转化key失败");
            return keyToKeyCodeMap.GetValueOrDefault(key, KeyCode.None);
        }

        public static Key ConverterToKey(KeyCode keyCode) {
            if (!isInit) Logger.Error($"KeyCodeConverter未初始化,转化keyCode失败");
            return keyCodeToKeyMap.GetValueOrDefault(keyCode, Key.None);
        }

        public static void Clear() {
            keyToKeyCodeMap.Clear();
            keyCodeToKeyMap.Clear();
            isInit = false;
        }
        public static void Init() {
            if(isInit) return;
            isInit = true;
            AddMapping(Key.None, KeyCode.None);
            AddMapping(Key.Space, KeyCode.Space);
            AddMapping(Key.Enter, KeyCode.Return);
            AddMapping(Key.Tab, KeyCode.Tab);
            AddMapping(Key.Backquote, KeyCode.BackQuote);
            AddMapping(Key.Quote, KeyCode.Quote);
            AddMapping(Key.Semicolon, KeyCode.Semicolon);
            AddMapping(Key.Comma, KeyCode.Comma);
            AddMapping(Key.Period, KeyCode.Period);
            AddMapping(Key.Slash, KeyCode.Slash);
            AddMapping(Key.Backslash, KeyCode.Backslash);
            AddMapping(Key.LeftBracket, KeyCode.LeftBracket);
            AddMapping(Key.RightBracket, KeyCode.RightBracket);
            AddMapping(Key.Minus, KeyCode.Minus);
            AddMapping(Key.Equals, KeyCode.Equals);
            AddMapping(Key.A, KeyCode.A);
            AddMapping(Key.B, KeyCode.B);
            AddMapping(Key.C, KeyCode.C);
            AddMapping(Key.D, KeyCode.D);
            AddMapping(Key.E, KeyCode.E);
            AddMapping(Key.F, KeyCode.F);
            AddMapping(Key.G, KeyCode.G);
            AddMapping(Key.H, KeyCode.H);
            AddMapping(Key.I, KeyCode.I);
            AddMapping(Key.J, KeyCode.J);
            AddMapping(Key.K, KeyCode.K);
            AddMapping(Key.L, KeyCode.L);
            AddMapping(Key.M, KeyCode.M);
            AddMapping(Key.N, KeyCode.N);
            AddMapping(Key.O, KeyCode.O);
            AddMapping(Key.P, KeyCode.P);
            AddMapping(Key.Q, KeyCode.Q);
            AddMapping(Key.R, KeyCode.R);
            AddMapping(Key.S, KeyCode.S);
            AddMapping(Key.T, KeyCode.T);
            AddMapping(Key.U, KeyCode.U);
            AddMapping(Key.V, KeyCode.V);
            AddMapping(Key.W, KeyCode.W);
            AddMapping(Key.X, KeyCode.X);
            AddMapping(Key.Y, KeyCode.Y);
            AddMapping(Key.Z, KeyCode.Z);
            AddMapping(Key.Digit1,KeyCode.Alpha1);
            AddMapping(Key.Digit2,KeyCode.Alpha2);
            AddMapping(Key.Digit3,KeyCode.Alpha3);
            AddMapping(Key.Digit4,KeyCode.Alpha4);
            AddMapping(Key.Digit5,KeyCode.Alpha5);
            AddMapping(Key.Digit6,KeyCode.Alpha6);
            AddMapping(Key.Digit7,KeyCode.Alpha7);
            AddMapping(Key.Digit8,KeyCode.Alpha8);
            AddMapping(Key.Digit9,KeyCode.Alpha9);
            AddMapping(Key.Digit0,KeyCode.Alpha0);
            AddMapping(Key.LeftShift, KeyCode.LeftShift);
            AddMapping(Key.RightShift, KeyCode.RightShift);
            AddMapping(Key.LeftAlt, KeyCode.LeftAlt);
            AddMapping(Key.AltGr, KeyCode.AltGr);
            AddMapping(Key.RightAlt, KeyCode.RightAlt);
            AddMapping(Key.LeftCtrl,KeyCode.LeftControl);
            AddMapping(Key.RightCtrl, KeyCode.RightControl);
            AddMapping(Key.LeftApple, KeyCode.LeftApple);
            AddMapping(Key.LeftCommand, KeyCode.LeftCommand);
            AddMapping(Key.LeftMeta, KeyCode.LeftMeta);
            AddMapping(Key.LeftWindows, KeyCode.LeftWindows);
            AddMapping(Key.RightApple, KeyCode.RightApple);
            AddMapping(Key.RightCommand, KeyCode.RightCommand);
            AddMapping(Key.RightMeta, KeyCode.RightMeta);
            AddMapping(Key.RightWindows, KeyCode.RightWindows);
            AddMapping(Key.ContextMenu,KeyCode.Menu);
            AddMapping(Key.Escape, KeyCode.Escape);
            AddMapping(Key.LeftArrow, KeyCode.LeftArrow);
            AddMapping(Key.RightArrow, KeyCode.RightArrow);
            AddMapping(Key.UpArrow, KeyCode.UpArrow);
            AddMapping(Key.DownArrow, KeyCode.DownArrow);
            AddMapping(Key.Backspace, KeyCode.Backspace);
            AddMapping(Key.PageDown, KeyCode.PageDown);
            AddMapping(Key.PageUp, KeyCode.PageUp);
            AddMapping(Key.Home, KeyCode.Home);
            AddMapping(Key.End, KeyCode.End);
            AddMapping(Key.Insert, KeyCode.Insert);
            AddMapping(Key.Delete, KeyCode.Delete);
            AddMapping(Key.CapsLock, KeyCode.CapsLock);
            AddMapping(Key.NumLock,KeyCode.Numlock);
            AddMapping(Key.PrintScreen,KeyCode.Print);
            AddMapping(Key.ScrollLock, KeyCode.ScrollLock);
            AddMapping(Key.Pause, KeyCode.Pause);
            AddMapping(Key.NumpadEnter, KeyCode.KeypadEnter);
            AddMapping(Key.NumpadDivide, KeyCode.KeypadDivide);
            AddMapping(Key.NumpadMultiply, KeyCode.KeypadMultiply);
            AddMapping(Key.NumpadPlus, KeyCode.KeypadPlus);
            AddMapping(Key.NumpadMinus, KeyCode.KeypadMinus);
            AddMapping(Key.NumpadPeriod, KeyCode.KeypadPeriod);
            AddMapping(Key.NumpadEquals, KeyCode.KeypadEquals);
            AddMapping(Key.Numpad0, KeyCode.Keypad0);
            AddMapping(Key.Numpad1, KeyCode.Keypad1);
            AddMapping(Key.Numpad2, KeyCode.Keypad2);
            AddMapping(Key.Numpad3, KeyCode.Keypad3);
            AddMapping(Key.Numpad4, KeyCode.Keypad4);
            AddMapping(Key.Numpad5, KeyCode.Keypad5);
            AddMapping(Key.Numpad6, KeyCode.Keypad6);
            AddMapping(Key.Numpad7, KeyCode.Keypad7);
            AddMapping(Key.Numpad8, KeyCode.Keypad8);
            AddMapping(Key.Numpad9, KeyCode.Keypad9);
            AddMapping(Key.F1, KeyCode.F1);
            AddMapping(Key.F2, KeyCode.F2);
            AddMapping(Key.F3, KeyCode.F3);
            AddMapping(Key.F4, KeyCode.F4);
            AddMapping(Key.F5, KeyCode.F5);
            AddMapping(Key.F6, KeyCode.F6);
            AddMapping(Key.F7, KeyCode.F7);
            AddMapping(Key.F8, KeyCode.F8);
            AddMapping(Key.F9, KeyCode.F9);
            AddMapping(Key.F10, KeyCode.F10);
            AddMapping(Key.F11, KeyCode.F11);
            AddMapping(Key.F12, KeyCode.F12);
            // AddMapping(Key.OEM1,);
            // AddMapping(Key.OEM2,);
            // AddMapping(Key.OEM3,);
            // AddMapping(Key.OEM4,);
            // AddMapping(Key.OEM5,);
            AddMapping(Key.F13, KeyCode.F13);
            AddMapping(Key.F14, KeyCode.F14);
            AddMapping(Key.F15, KeyCode.F15);
            AddMapping(Key.F16, KeyCode.F16);
            AddMapping(Key.F17, KeyCode.F17);
            AddMapping(Key.F18, KeyCode.F18);
            AddMapping(Key.F19, KeyCode.F19);
            AddMapping(Key.F20, KeyCode.F20);
            AddMapping(Key.F21, KeyCode.F21);
            AddMapping(Key.F22, KeyCode.F22);
            AddMapping(Key.F23, KeyCode.F23);
            AddMapping(Key.F24, KeyCode.F24);
        }
    }
}