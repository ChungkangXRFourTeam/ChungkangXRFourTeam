using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace XRProject.Boss
{
    public class Boss : MonoBehaviour, IPatternFactoryIngredient, IBActorLife, IBActorHit
    {
        private Track _battleTrack;
        private Track _movementTrack;
        private TrackPlayer _battlePlayer;
        private TrackPlayer _movementPlayer;
        private TrackPlayer _patternTranslationPlayer;

        [SerializeField] private SkeletonAnimation _mapDoor;
        [SerializeField] private float _hp;
        [SerializeField] private BaseLazerActionData baseLazerActionData;
        [SerializeField] private MeleeData _meleeData;
        [SerializeField] private VerticalLazerData _verticalLazerData;
        [SerializeField] private TopBottomLazerData _topBottomLazerData;
        [SerializeField] private MovementActionData _movementData;
        [FormerlySerializedAs("_dropVaultBossActionData")] [SerializeField] private DropBoltBossActionData dropBoltBossActionData;

        private void Awake()
        {
            CurrentHP = MaxHp;
            
            _battleTrack = BossPatternFactory.CompletionPhase(transform, this);
            //_battleTrack = BossPatternFactory.MeleeTestTrack(transform, this);
            //_battleTrack = BossPatternFactory.LazerTestTrack(transform, this);
            _battlePlayer = new TrackPlayer();
            _battlePlayer.Play(_battleTrack);

            _movementTrack = BossPatternFactory.CompletionMovementTrack(transform, this);
            _movementPlayer = new TrackPlayer();
            _movementPlayer.Play(_movementTrack);

            var translationTrack = BossPatternFactory.CompleitionTranslationTrack(transform, this);
            _patternTranslationPlayer = new TrackPlayer();
            _patternTranslationPlayer.ActionList.Paused = true;
            _patternTranslationPlayer.Play(translationTrack);
            
            Interaction.SetContractInfo(ActorContractInfo.Create(transform, ()=>transform).AddBehaivour<IBActorHit>(this));

            StartCoroutine(CoUpdate());


            ChangedHp += (life, prev, current) =>
            {
                float value = current / (MaxHp <= 0f ? 1f : MaxHp);
                if (value < 0.5f)
                {
                    BaseLazerData.NextTrigger.TriggerNextPhase();
                }
            };
        }

        private IEnumerator CoUpdate()
        {
            while (true)
            {
                float percentage = CurrentHP / MaxHp;

                if (percentage <= 0.5f) break;
                yield return new WaitForEndOfFrame();
            }
            
            _battlePlayer.ActionList.Paused = true;
            _battlePlayer.ActionList.NextTrack();
            _patternTranslationPlayer.ActionList.Paused = false;

            while (!_patternTranslationPlayer.ActionList.IsActionEndedCurrentTrack)
            {
                yield return new WaitForEndOfFrame();
            }
            
            _battlePlayer.ActionList.Paused = false;
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
        public DropBoltBossActionData DropBoltBossActionData => dropBoltBossActionData;
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
            gameObject.SetActive(false);
        }
    }
}