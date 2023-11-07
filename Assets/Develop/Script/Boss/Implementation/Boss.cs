using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Boss;

public class Boss : MonoBehaviour, IPatternFactoryIngredient
{
    private Track _track;

    [SerializeField] private NormalLazerActionData _normalLazerActionData;

    private void Awake()
    {
        _track = BossPatternFactory.CompletionTrack(transform, this);
    }

    private void Update()
    {
        _track.EValuate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + _normalLazerActionData.CenterOffset, _normalLazerActionData.BoxSize * 2f);
    }

    public NormalLazerActionData NormalLazerData => _normalLazerActionData;
}
