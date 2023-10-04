using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace XRProject.Utils.Log
{
    [CreateAssetMenu(menuName = "XR/XLog/Setting", fileName = "XLogSetting", order = 3)]
    public class XLogSetting : ScriptableObject
    {
        [SerializeField] private XLogPreset _preset;

        [CanBeNull] public XLogPreset Preset => _preset;


    }

}