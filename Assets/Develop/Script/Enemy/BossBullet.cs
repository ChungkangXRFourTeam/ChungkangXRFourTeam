using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    private float _speed;
    private float _destroyTime;

    private InteractionController _interaction;
    private bool _isDestroyed;
    public static BossBullet Create(BossBullet bullet, Vector2 pos, Vector2 dir, float speed, float destroyTime = 5f)
    {
        var obj = Instantiate(bullet);
        obj.transform.up = dir;
        obj.transform.position = pos;
        obj._speed = speed;
        obj._destroyTime = destroyTime;

        return obj;
    }

    private void Awake()
    {
        _interaction = GetComponentInChildren<InteractionController>();

        _interaction.SetContractInfo(ActorContractInfo.Create(transform, () => _isDestroyed));
        
        _interaction.OnContractActor += OnActorHit;
        _interaction.OnContractObject += OnObjectHit;
    }

    private void OnActorHit(ActorContractInfo info)
    {
    }
    private void OnObjectHit(ObjectContractInfo info)
    {
        DoDestroy();
    }

    private float _timer = 0f;
    private void Update()
    {
        transform.Translate(Vector2.up * (_speed * Time.deltaTime), Space.Self);

        if (_timer >= _destroyTime)
        {
            DoDestroy();
        }

        _timer += Time.deltaTime;
    }

    private void DoDestroy()
    {
        _isDestroyed = true;
        Destroy(gameObject);
    }
}
