using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollSpace : MonoBehaviour, IBObjectPatrollSpace
{
    [SerializeField] private bool _debug;
    [SerializeField] private InteractionController _interaction;
    [SerializeField] private BoxCollider2D _collider;

    public Vector2 LeftPoint => (Vector2)transform.position + _collider.offset + Vector2.left * _collider.size.x * 0.5f;
    public Vector2 RightPoint => (Vector2)transform.position + _collider.offset + Vector2.right * _collider.size.x * 0.5f;
    public InteractionController Interaction => _interaction;
    public bool IsSelectiveObject => false;
    
    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        
        _interaction.SetContractInfo(ObjectContractInfo.Create(transform, () => false)
            .AddBehaivour<IBObjectPatrollSpace>(this)
        );
    }

    private void OnDrawGizmos()
    {
        if (!_debug) return;
        if (_collider == null) return;
        Gizmos.color = Color.yellow;
        
        Gizmos.DrawLine(LeftPoint, RightPoint);
    }

}
