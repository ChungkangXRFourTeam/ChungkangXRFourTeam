using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionController), typeof(BoxCollider2D))]
public class BossLazerInteractor : MonoBehaviour
{
    private InteractionController _interaction;
    private BoxCollider2D _col;
    private ParticleSystem _particle;

    private bool isAttacked;
    private void Awake()
    {
        _interaction = GetComponent<InteractionController>();
        _col = GetComponent<BoxCollider2D>();
        _particle = GetComponentInChildren<ParticleSystem>();

        _interaction.SetContractInfo(
            ActorContractInfo.Create(transform, () => false)
        );

        _interaction.OnContractActor += OnContractActor;
    }

    public void SetCollision(bool value)
    {
        _col.enabled = value;
    }

    private void Update()
    {
        if (_particle.isPlaying == false)
        {
            isAttacked = false;
        }
        SetCollision(_particle.isPlaying && isAttacked == false);
    }

    private void OnContractActor(ActorContractInfo info)
    {
        if (gameObject.activeSelf == false) return;
        if (info.Transform.GetComponent<PlayerController>() == false) return;

        if (info.TryGetBehaviour(out IBActorHit hit))
        {
            isAttacked = true;
            hit.DoHit(_interaction.ContractInfo, 1f);
        }
    }
}
