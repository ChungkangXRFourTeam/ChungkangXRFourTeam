using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace XRProject.Utils
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "XR/XLog/Presets", fileName = "XLogPreset", order = 3)]
    public class XLogPreset : ScriptableObject
    {
        [XLogFilter][SerializeField] private EXLogFilter filter = EXLogFilter.Debug;
        [SerializeField] private string _signature = null;
        [SerializeField] private bool _loggingDate = false;
        [SerializeField] private bool _loggingSignature = false;

        public EXLogFilter Filter => filter;

        public string Signature => _signature;

        public bool LoggingDate => _loggingDate;

        public bool LoggingSignature => _loggingSignature;
    }

}