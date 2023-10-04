using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [SerializeField] private float _delay;
    [SerializeField] private float _shootWaitDelay;
    [SerializeField] private float _traceSpeed;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _pivot;
    [SerializeField] private BossBullet _bullet;
    private Transform _target;

    private enum EState
    {
        Trace,
        Wait,
        Shoot
    }
    private void Awake()
    {
        _target = GameObject.Find("Player").transform;
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, _pivot.position);
        _pivot.gameObject.SetActive(false);

        StartCoroutine(CoUpdate());
    }
    IEnumerator CoUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(_delay);

            OnWait();
            
            float timer = 0f;
            while (timer < _shootWaitDelay)
            {
                OnTraceTarget();
                
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
                
            }
            OnShootTarget();
        }
    }

    private Vector2 _targetPoint;
    private void OnTraceTarget()
    {
        if (!_target) return;
        
        _targetPoint = Vector2.Lerp(_targetPoint, _target.position, Time.deltaTime * _traceSpeed);
        _lineRenderer.SetPosition(1, _targetPoint);
    }

    private void OnWait()
    {
        if (!_target) return;
        _lineRenderer.enabled = true;
    }

    private void OnShootTarget()
    {
        _lineRenderer.enabled = false;
        if (!_target) return;
        BossBullet.Create(_bullet, _pivot.position, (_targetPoint - (Vector2)_pivot.position).normalized, _bulletSpeed);
        var obj = _target.GetComponentInChildren<InteractionController>();
        if (obj == null) return;

        if (obj.TryGetContractInfo(out ActorContractInfo info) &&
            info.TryGetBehaviour(out IBActorHit hit) &&
            info.TryGetBehaviour(out IBActorPhysics physics) &&
            !physics.IsSwingState)
        {
            hit.DoHit(ActorContractInfo.Create(transform, () => false), 1f);
        }
    }

}
