using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="XR/Player/PlayerProperties", fileName="PlayerPropertiesData ", order = 3)]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float _hp;

    [SerializeField] private float _removeDelayWithProperties;
    [SerializeField] private int _maxPropertiesCount;


    public float Hp => _hp;
    public float RemoveDelayWithProperties => _removeDelayWithProperties;
    public int MaxPropertiesCount => _maxPropertiesCount;
}
