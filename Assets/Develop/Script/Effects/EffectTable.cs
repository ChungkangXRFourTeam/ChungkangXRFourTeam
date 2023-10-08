using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "XR/Effect/EffectTable", fileName = "EffectTable", order = 3)]
public class EffectTable : ScriptableObject
{
    [SerializeField] private string _tableKey;
    [SerializeField] private List<EffectKeyPair> _table;

    private List<EffectKeyPair> _cachedTable;

    public string TableKey => _tableKey;
    public List<EffectKeyPair> Table
    {
        get
        {
            // editor에서 실수로 table의 값을 바꾸어도 직렬화된 데이터가 변경되지 않도록 cacahe 생성함.
            if (_cachedTable == null)
            {
                _cachedTable = new List<EffectKeyPair>(_table);
            }

            return _cachedTable;
        }
    }
}

[Serializable]
public struct EffectKeyPair
{
    public string Key;
    public GameObject Effect;
}