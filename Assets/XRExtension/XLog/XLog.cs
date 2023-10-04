using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace XRProject.Utils.Log
{
    [Serializable]
    public enum EXLogFilter : int
    {
        Debug = 1,
        Warn = 2,
        Error = 4,
    }
    public static class XLog
    {
        public static void Log(string text, EXLogFilter filter) => Print(ref text, filter);

        public static void LogDebug(string text) => Print(ref text, EXLogFilter.Debug);
        public static void LogWarn(string text) => Print(ref text, EXLogFilter.Warn);
        public static void LogError(string text) => Print(ref text, EXLogFilter.Error);


        #region MyRegion private
        [CanBeNull] private static XLogSetting _setting;
        [CanBeNull] private static XLogPreset _preset;
        private static StringBuilder _strBuilder;
        private static bool _isSuccessLoad;
        
        private const string PATH = "Assets/XRExtension/XLog/";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Init()
        {
            _strBuilder = null;
            _preset = null;
            _setting = null;
            _isSuccessLoad = false;
            
#if UNITY_EDITOR
#else
            return;
#endif
            _strBuilder = new StringBuilder();
            
#if UNITY_EDITOR
            _setting = AssetDatabase.LoadAssetAtPath<XLogSetting>(PATH + "XLogSetting.asset");
#endif

            if (!_setting)
            {
                Debug.LogError("XLog: XLogSetting 파일이 경로에 존재하지 않습니다.");
                return;
            }

            if (!_setting.Preset)
            {
                Debug.LogWarning("XLog: XLogPreset이 비어있습니다. default preset을 불러옵니다.");
#if UNITY_EDITOR
                _preset = AssetDatabase.LoadAssetAtPath<XLogPreset>(PATH + "DefaultPreset.asset");
#endif

                if (!_preset)
                {
                    Debug.LogWarning("XLog: default XLogPreset을 불러오는데 실패했습니다. 로그 기능이 중지됩니다.");
                }
                return;
            }
                
            _isSuccessLoad = true;

            if (!_preset)
            {
                _preset = _setting.Preset;
            }
        }

        private static void Print(ref string text, EXLogFilter filter)
        {
            if (!_isSuccessLoad) return;
            if (((int)_preset.Filter & (int)filter) == 0) return;
            if (string.IsNullOrEmpty(text)) return;

            _strBuilder.Clear();

            if (_preset.LoggingDate)
            {
                _strBuilder.Append($"[{DateTime.UtcNow.ToString()}]");
            }
            
            if (_preset.LoggingSignature && !string.IsNullOrEmpty(_preset.Signature))
            {
                _strBuilder.Append($"[{_preset.Signature}]");
            }
            
            _strBuilder.Append($" {text}");

            switch (filter)
            {
                case EXLogFilter.Debug:
                    Debug.Log(_strBuilder.ToString());
                    break;
                case EXLogFilter.Warn:
                    Debug.LogWarning(_strBuilder.ToString());
                    break;
                case EXLogFilter.Error:
                    Debug.LogError(_strBuilder.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }
        #endregion
        
    }
}
