using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "XR/Sound/SoundTableSet", fileName = "SoundTableSet", order = 3)]
public class SoundTableSet : ScriptableObject
{
    [SerializeField] private List<SoundTable> _tables;

    public IReadOnlyList<SoundTable> Tables => _tables;
}
