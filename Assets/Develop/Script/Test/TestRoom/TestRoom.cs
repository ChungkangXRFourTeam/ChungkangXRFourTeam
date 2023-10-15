using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoom : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;
    [SerializeField] private Transform _spawnPivot;

    private GameObject _currentEnemy;

    private void Awake()
    {
        if (!_enemy) return;
        
        _enemy = Instantiate(_enemy);
        _enemy.SetActive(false);
    }

    public void SpawnEnemy()
    {
        if (!_enemy) return;
        if (!_spawnPivot) return;
        
        var obj = Create(_enemy, _spawnPivot.position);
    }

    private GameObject Create(GameObject obj, Vector2 pos)
    {
        var newObj = Instantiate(obj);
        newObj.SetActive(true);
        
        newObj.transform.position = pos;
        newObj.transform.rotation = Quaternion.identity;
        
        if (_currentEnemy)
        {
            Destroy(_currentEnemy);
        }
        _currentEnemy = newObj;
        
        return newObj;
    }
}
