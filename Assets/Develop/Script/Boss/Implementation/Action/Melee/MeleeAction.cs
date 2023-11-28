using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace XRProject.Boss
{
    public partial interface IPatternFactoryIngredient
    {
        public MeleeData MeleeData { get; }
    }
    
    [System.Serializable]
    public class MeleeData
    {
        public float _handMoveDuration;
        public float _handHatMoveSpeed;
        public float _handAttackDuration;
        public float _handWarningDuration;
        public float _attackDelay;
        public Transform[] hands;
        public Vector2[] handPos;
        public Ease HandMoveEase;
        public Ease HandAttackEase;
        public Ease HandWarningEase;
        public Vector2 WarningPointOffset;
        public Vector2 AttackOffset;
        public Vector2 EffectAttackOffset;
        public float angle = 19.68f;
    }


    public class MeleePositionAction : IAction
    {
        private MeleeData _data;
        public MeleePositionAction(IPatternFactoryIngredient ingredient)
        {
            _data = ingredient.MeleeData;
        }
        public void Begin()
        {
        }

        public void End()
        {
        }

        public IEnumerator EValuate()
        {
            Sequence s = DOTween.Sequence();
            for (int i = 0; i < _data.hands.Length; i++)
            {
                s.Join(_data.hands[i].DOMove(_data.handPos[i], _data._handMoveDuration).SetEase(_data.HandMoveEase));
            }
            yield return s.WaitForCompletion();
        }

        public ITrackPredicate Predicate { get; set; }
    }
    public class MeleeAction : BaseBossAction
    {
        private MeleeData _data;
        private MeleeData _meleeData;
        private Transform _transform;
        private int _index;
        public MeleeAction(Transform transform,IPatternFactoryIngredient ingredient, int index) : base(transform, ingredient.BaseLazerData)
        {
            _transform = transform;
            _data = ingredient.MeleeData;
            _index = index;
            _meleeData = ingredient.MeleeData;
        }

        public override IEnumerator EValuate()
        {
            yield return GotoHandPosition(_data.hands[_index], _data.handPos[_index]);
            yield return new WaitForSeconds(_data._attackDelay);

            
            var playerTransform = GetPlayerOrNull();
            if (playerTransform == false) yield break;
            
            _meleeData.hands[0].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Right_Hand_Smash", false);
            _meleeData.hands[1].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Right_Hand_Smash", false);

            //yield return
                DOTween.Sequence()
                    .Join(_meleeData.hands[0].DORotateQuaternion(Quaternion.Euler(0f, 0f, _meleeData.angle), 0.25f))
                    .Join(_meleeData.hands[1].DORotateQuaternion(Quaternion.Euler(0f, 0f, -_meleeData.angle), 0.25f))
                    .WaitForCompletion()
                ;
            
            //yield return new WaitForSeconds(0.367f);

            float timer = 0f;
            while (timer <= 1f)
            {
                timer += Time.deltaTime;
                var dir = playerTransform.position - _data.hands[_index].position;
                dir = dir.normalized;
                dir.z = dir.y = 0f;
                _data.hands[_index].position += dir * (_data._handHatMoveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            
            //yield return GotoPlayerHat(_data.hands[_index], playerTransform.position);
            yield return AttackPlayer(_data.hands[_index]);
        }
        
        public YieldInstruction GotoHandPosition(Transform hand, Vector2 pos)
        {
            return hand.DOMove(pos, _data._handMoveDuration).SetEase(_data.HandMoveEase).WaitForCompletion();
        }

        public YieldInstruction GotoPlayerHat(Transform hand, Vector2 pos)
        {
            var targetPos = hand.position;
            targetPos.x = pos.x;
            return hand.DOMove(targetPos, _data._handMoveDuration).SetEase(_data.HandAttackEase).WaitForCompletion();
        }

        public YieldInstruction AttackPlayer(Transform hand)
        {
            var points = GetTwoPoint(GetSidePoint(Vector2.down, BaseData.BoxSize.y), Vector2.down, BaseData.BoxSize.x);
            var targetPoint = GetPointFromTwoLine(hand.position, (Vector2)hand.position + Vector2.down * 999f, points.Item1,
                points.Item2);
            var warningPoint = (Vector2)hand.position + _data.WarningPointOffset;

            Vector2 prevPoint = hand.position;

            var s = DOTween.Sequence();
            s
                .Append(
                    hand
                        .DOMove(warningPoint, _data._handWarningDuration)
                        .SetEase(_data.HandWarningEase)
                )
                .AppendCallback(() =>
                {
                    var com = hand.GetComponentInChildren<BossHandInteracter>();
                    if (com)
                    {
                        com.Attack();
                    }
                })
                .Append(
                    hand
                        .DOMove(targetPoint + _data.AttackOffset, _data._handAttackDuration)
                        .SetEase(_data.HandAttackEase)
                )
                .AppendCallback(() =>{
                        VirtualCameraShaker.Instance.CameraShake(1.5f, 10f, 100f);
                        EffectManager.ImmediateCommand(new EffectCommand()
                        {
                            EffectKey = "boss/meleeAttack",
                            Position = targetPoint + _data.EffectAttackOffset
                        });
                    }
                    );
                
            var s2 = DOTween.Sequence();
                s2.AppendInterval(0.75f)
                .Append(hand
                    .DOMove(prevPoint, _data._handMoveDuration)
                    .SetEase(_data.HandMoveEase)
                )
                .AppendCallback(() =>
                {
                    GotoHandPosition(_data.hands[_index], _data.handPos[_index]);
                });
                
            return s.WaitForCompletion();
        }

    }

}