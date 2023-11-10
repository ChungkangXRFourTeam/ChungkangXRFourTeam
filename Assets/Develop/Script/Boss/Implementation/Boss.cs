using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XRProject.Boss;

public class Boss : MonoBehaviour, IPatternFactoryIngredient
{
    private Track _track;
    private TrackPlayer _player;

    [SerializeField] private BaseLazerActionData baseLazerActionData;
    [SerializeField] private BossMeleeData _meleeData;
    [SerializeField] private VerticalLazerData _verticalLazerData;
    [SerializeField] private TopBottomLazerData _topBottomLazerData;

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
        Gizmos.DrawWireCube((Vector2)transform.position + baseLazerActionData.CenterOffset, baseLazerActionData.BoxSize * 2f);
    }

    public VerticalLazerData VerticalLazerData => _verticalLazerData;
    public BossMeleeData MeleeData => _meleeData;
    public TopBottomLazerData TopBottomLazerData => _topBottomLazerData;
    public BaseLazerActionData BaseLazerData => baseLazerActionData;
}
