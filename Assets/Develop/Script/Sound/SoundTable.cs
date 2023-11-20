using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using XRProject.Utils.Log;

[CreateAssetMenu(menuName = "XR/Sound/SoundTable", fileName = "SoundTable", order = 3)]
public class SoundTable : ScriptableObject
{
    [SerializeField] private string _key;
    [SerializeField] private List<SoundKeyPair> _table;

    public string Key => _key;
    public IReadOnlyList<SoundKeyPair> Table => _table;
}


[System.Serializable]
public struct SoundKeyPair
{
    public string Key;
    public AudioClip Sound;
}