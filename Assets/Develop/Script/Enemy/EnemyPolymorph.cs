using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyPolymorph : MonoBehaviour
{
    // Start is called before the first frame update\

    [SerializeField] private Sprite fireDuck;
    [SerializeField] private Sprite waterDuck;
    private Enemy enemyScript;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        enemyScript = GetComponent<Enemy>();
        
    }

    private void Update()
    {
        if (enemyScript.Properties == EActorPropertiesType.Flame && _spriteRenderer.sprite != fireDuck)
            _spriteRenderer.sprite = fireDuck;
        else if (enemyScript.Properties == EActorPropertiesType.Water && _spriteRenderer.sprite != waterDuck)
            _spriteRenderer.sprite = waterDuck;

    }
}
