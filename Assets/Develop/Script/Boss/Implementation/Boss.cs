using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public class Boss : MonoBehaviour, IPatternFactoryIngredient, IBActorLife, IBActorHit
    {
        private Track _battleTrack;
        private Track _movementTrack;
        private TrackPlayer _battlePlayer;
        private TrackPlayer _movementPlayer;

        [SerializeField] private float _hp;
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
            
            Interaction.SetContractInfo(ActorContractInfo.Create(transform, ()=>transform).AddBehaivour<IBActorHit>(this));
        }

        private void Update()
        {
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube((Vector2)transform.position + baseLazerActionData.CenterOffset,
                baseLazerActionData.BoxSize * 2f);

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
        private InteractionController _interaction;

        public InteractionController Interaction
        {
            get
            {
                if (!_interaction)
                {
                    _interaction = GetComponentInChildren<InteractionController>();
                }

                return _interaction;
            }
        }
        public void DoHit(BaseContractInfo caller, float damage)
        {
            CurrentHP -= damage;
        }

        public float MaxHp => _hp;

        private float _currentHp;
        public float CurrentHP
        {
            get => _currentHp;
            set
            {
                float backup = _currentHp;
                _currentHp = value;
                ChangedHp?.Invoke(this, backup, _currentHp);

                if (_currentHp <= 0f)
                {
                    Die();
                }
            }
        }
        public event Action<IBActorLife, float, float> ChangedHp;
        public void Die()
        {
            
        }
    }
}