using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(InteractionController))]
[RequireComponent(typeof(SpriteRenderer))]
public class DiaryWorldItem : MonoBehaviour, IBObjectDiaryItem
{
    [SerializeField] private int _pageId;
    [SerializeField] private float _radius;
    [SerializeField] private float _floatingY;
    [SerializeField] private float _floatingDuration;
    [SerializeField] private Ease _ease;

    [SerializeField] private Sprite _openSprite;
    [SerializeField] private Sprite _closeSprite;

    private SpriteRenderer _renderer;    
    public InteractionController Interaction { get; private set; }
    public int PageID => _pageId;
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>(); 
        Interaction = GetComponent<InteractionController>();

        Interaction.SetContractInfo(
            ObjectContractInfo.Create(transform, () => transform)
                .AddBehaivour<IBObjectDiaryItem>(this)
            );

        Interaction.OnContractActor += OnContractActor;

        transform.DOMoveY(_floatingY + transform.position.y, _floatingDuration).SetLoops(-1, LoopType.Yoyo).SetEase(_ease);
    }

    private void Update()
    {
        var hit = Physics2D.OverlapCircle(transform.position, _radius, LayerMask.GetMask("Player"));
        SpriteSwap(hit == false);
    }

    private void SpriteSwap(bool far)
    {
        Sprite sprite = far ? _closeSprite : _openSprite;
        _renderer.sprite = sprite;
    }

    private void OnContractActor(ActorContractInfo info)
    {
        if(info.Transform.GetComponent<PlayerController>())
            Collect();
    }

    public void Collect()
    {
        GameObject.Find("UIManager")?
            .GetComponent<UIManager>()?
            .GetUIController<DiaryUIController>("DiaryUI")?
            .AddPage(PageID);
        
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
