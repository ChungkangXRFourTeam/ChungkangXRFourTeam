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
        public static void Log(string text, EXLogFilter filter, string signature) => Print(ref text, filter, ref signature);

        public static void LogDebug(string text, string signature) => Print(ref text, EXLogFilter.Debug, ref signature);
        public static void LogWarn(string text, string signature) => Print(ref text, EXLogFilter.Warn, ref signature);
        public static void LogError(string text, string signature) => Print(ref text, EXLogFilter.Error, ref signature);


        #region MyRegion private
        [CanBeNull] private static XLogSetting _setting;
        [CanBeNull] private static XLogPresetKeyValuePair[] _pairs;
        private static Dictionary<string, XLogPreset> _presetTable;
        private static StringBuilder _strBuilder;
        private static bool _isSuccessLoad;
        
        private const string PATH = "Assets/XRExtension/XLog/";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Init()
        {
            _strBuilder = null;
            _pairs = null;
            _setting = null;
            _isSuccessLoad = false;
            _presetTable = new Dictionary<string, XLogPreset>();
            
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

            if (_setting.Preset == null || _setting.Preset.Length == 0)
            {
                Debug.LogWarning("XLog: XLogPreset이 비어있습니다.");
                return;
            }
                
            _isSuccessLoad = true;
            _pairs = _setting.Preset;
            

            if (_pairs != null)
            {
                CheckSignatureValid();
                foreach (var pair in _pairs)
                {
                    if(!pair.Preset) continue;

                    if (pair.Enabled)
                    {
                        if (!_presetTable.TryAdd(pair.Preset.Signature, pair.Preset))
                        {
                            Debug.LogError($"XLog: preset['{pair.Preset.name}']의 signature['{pair.Preset.Signature}']가 중복됩니다.");
                        }
                    }
                }
            }
            
        }

        private static void CheckSignatureValid()
        {
            if (_pairs == null) return;
            
            foreach (var pair in _pairs)
            {
                if(!pair.Preset)continue;
                
                if (string.IsNullOrEmpty(pair.Preset.Signature) || string.IsNullOrWhiteSpace(pair.Preset.Signature))
                {
                    Debug.LogError($"XLog: preset['{pair.Preset.name}']의 signature가 올바르지 않습니다.");
                }
            }
        }

        private static void Print(ref string text, EXLogFilter filter, ref string signature)
        {
            if (!_isSuccessLoad) return;
            if (!_presetTable.TryGetValue(signature, out var preset)) return;
            if (((int)preset.Filter & (int)filter) == 0) return;
            if (string.IsNullOrEmpty(text)) return;
            

            _strBuilder.Clear();

            if (preset.LoggingDate)
            {
                _strBuilder.Append($"[{DateTime.UtcNow.ToString()}]");
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
