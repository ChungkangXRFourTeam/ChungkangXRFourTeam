using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Utils.Log
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "XR/XLog/Presets", fileName = "XLogPreset", order = 3)]
    public class XLogPreset : ScriptableObject
    {
        [XLogFilter] [SerializeField] private EXLogFilter filter;
        [SerializeField] private string _signature = null;
        [SerializeField] private bool _loggingDate = false;

        public EXLogFilter Filter => filter;
        public string Signature => _signature;
        public bool LoggingDate => _loggingDate;
    }

}