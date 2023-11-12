using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XRProject.Boss;

public class Boss : MonoBehaviour, IPatternFactoryIngredient
{
    private Track _battleTrack;
    private Track _movementTrack;
    private TrackPlayer _battlePlayer;
    private TrackPlayer _movementPlayer;

    [SerializeField] private BaseLazerActionData baseLazerActionData;
    [SerializeField] private MeleeData _meleeData;
    [SerializeField] private VerticalLazerData _verticalLazerData;
    [SerializeField] private TopBottomLazerData _topBottomLazerData;
    [SerializeField] private MovementActionData _movementData;

    private void Awake()
    {
        _battleTrack = BossPatternFactory.CompletionBattleTrack(transform, this);
        _battlePlayer = new TrackPlayer();
        _battlePlayer.Play(_battleTrack);

        _movementTrack = BossPatternFactory.CompletionMovementTrack(transform, this);
        _movementPlayer = new TrackPlayer();
        _movementPlayer.Play(_movementTrack);
    }

    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + baseLazerActionData.CenterOffset, baseLazerActionData.BoxSize * 2f);

        Gizmos.color = Color.red;
        for (int i = 0; i < MeleeData.handPos.Length; i++)
        {
            Gizmos.DrawWireSphere(MeleeData.handPos[i], 0.75f);
        }
    }

    public VerticalLazerData VerticalLazerData => _verticalLazerData;
    public MeleeData MeleeData => _meleeData;
    public TopBottomLazerData TopBottomLazerData => _topBottomLazerData;
    public MovementActionData MovementActionData => _movementData;
    public BaseLazerActionData BaseLazerData => baseLazerActionData;
}
