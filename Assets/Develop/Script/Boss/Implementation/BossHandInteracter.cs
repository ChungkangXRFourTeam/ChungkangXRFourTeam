using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(InteractionController), typeof(BoxCollider2D))]
public class BossHandInteracter : MonoBehaviour
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

    public void Attack()
    {
        isAttacked = true;
        SetCollision(true);
        DOTween.Kill(this);
        DOTween.Sequence().SetDelay(0.35f).OnComplete(()=>
        {
            isAttacked = false;
            SetCollision(false);
        }).SetId(this);
    }

    private void OnContractActor(ActorContractInfo info)
    {
        if (isAttacked == false) return;
        if (gameObject.activeSelf == false) return;
        if (info.Transform.GetComponent<PlayerController>() == false) return;

        if (info.TryGetBehaviour(out IBActorHit hit))
        {
            isAttacked = false;
            SetCollision(false);
            hit.DoHit(_interaction.ContractInfo, 1f);
        }
    }
}
