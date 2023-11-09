using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Boss;

public class Boss : MonoBehaviour, IPatternFactoryIngredient
{
    private Track _track;
    private TrackPlayer _player;

    [SerializeField] private NormalLazerActionData _normalLazerActionData;

    private void Awake()
    {
        _track = BossPatternFactory.CompletionTrack(transform, this);
        _player = new TrackPlayer();
        _player.Play(_track);
    }

    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + _normalLazerActionData.CenterOffset, _normalLazerActionData.BoxSize * 2f);
    }

    public NormalLazerActionData NormalLazerData => _normalLazerActionData;
}
