using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "XR/Effect/EffectTableSet", fileName = "EffectTableSet", order = 3)]
public class EffectTableSet : ScriptableObject
{
    [SerializeField] private List<EffectTable> tableList;


    private List<EffectTable> _cachedTableList;

    public List<EffectTable> TableList
    {
        get
        {
            if (_cachedTableList == null)
            {
                _cachedTableList = new(tableList);
            }

            return _cachedTableList;
        }
    }
}