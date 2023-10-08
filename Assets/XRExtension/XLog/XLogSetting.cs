using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace XRProject.Utils.Log
{
    [CreateAssetMenu(menuName = "XR/XLog/Setting", fileName = "XLogSetting", order = 3)]
    public class XLogSetting : ScriptableObject
    {
        [SerializeField] private XLogPresetKeyValuePair[] _presets;

        [CanBeNull] public XLogPresetKeyValuePair[] Preset => _presets;


    }

    [System.Serializable]
    public struct XLogPresetKeyValuePair
    {
        public bool Enabled;
        public XLogPreset Preset;
    }

}