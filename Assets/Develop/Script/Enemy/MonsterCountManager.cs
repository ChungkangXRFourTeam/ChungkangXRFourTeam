using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MonsterCountManager : MonoBehaviour
{
    private int _monsterCount;
    [SerializeField]
    private GameObject _kennelPrefab;

    [SerializeField] private Transform _spawnPoint; 

    [SerializeField] private GameObject _player;

    private void Awake()
    {
        ChangeMonsterCount();
    }

    private void ChangeMonsterCount()
    {
        _monsterCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Debug.Log(_monsterCount);
        if (_monsterCount == 0)
        {
            GameObject kennel = Instantiate(_kennelPrefab, _spawnPoint.position ,quaternion.identity);
            Debug.Log(kennel.name);
        }
        
    }
}
