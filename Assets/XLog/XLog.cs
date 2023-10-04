using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace XRProject.Utils
{
    [Serializable]
    public enum EXLogFilter : int
    {
        Info = 1,
        Debug = 2,
        Warn = 4,
        Error = 8,
    }
    public static class XLog
    {
        private static XLogSetting _setting;
        [CanBeNull] private static bool _isTryLoadSetting = false;
        [CanBeNull] private static bool _isSuccessLoad = false;
        private static XLogPreset _preset;
        private static void LoadSetting()
        {
            if (!_isTryLoadSetting)
            {
#if UNITY_EDITOR
                _setting = AssetDatabase.LoadAssetAtPath<XLogSetting>("Assets/XLog/XLogSetting.asset");
#endif

                if (_setting)
                {
                    _preset = _setting.Preset;

                    if (_preset)
                    {
                        _isSuccessLoad = true;
                    }
                    else
                    {
                        Debug.LogWarning("XLog: XLogPreset이 비어있습니다. 로그는 출력되지 않습니다.");
                    }
                }
                else
                {
                    Debug.LogError("XLog: XLogSetting 파일이 경로에 존재하지 않습니다.");
                }
                _isTryLoadSetting = true;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Init()
        {
            _preset = null;
            _setting = null;
            _isTryLoadSetting = false;
            _isSuccessLoad = false;

            LoadSetting();
        }

        private static bool GuardSetting() => !_isSuccessLoad;
        
        private static void Print(ref string text, EXLogFilter filter)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (GuardSetting()) return;
            if (((int)_preset.Filter & (int)filter) == 0) return;

            StringBuilder str = new StringBuilder();

            if (_preset.LoggingDate)
            {
                str.Append($"[{DateTime.UtcNow.ToString()}]");
            }
            
            if (_preset.LoggingSignature && !string.IsNullOrEmpty(_preset.Signature))
            {
                str.Append($"[{_preset.Signature}]");
            }
            
            str.Append($" {text}");
            Debug.Log(str.ToString());
        }

        public static void Log(string text, EXLogFilter filter) => Print(ref text, filter);

        public static void LogInfo(string text) => Print(ref text, EXLogFilter.Info);
        public static void LogDebug(string text) => Print(ref text, EXLogFilter.Debug);
        public static void LogWarn(string text) => Print(ref text, EXLogFilter.Warn);
        public static void LogError(string text) => Print(ref text, EXLogFilter.Error);
    }
}
