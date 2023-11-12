using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        public float _handAttackDuration;
        public float _handWarningDuration;
        public float _attackDelay;
        public Transform[] hands;
        public Vector2[] handPos;
        public Ease HandMoveEase;
        public Ease HandAttackEase;
        public Ease HandWarningEase;
        public Vector2 WarningPointOffset;
    }
    
    public class MeleeAction : BaseBossAction
    {
        private MeleeData _data;
        private Transform _transform;
        private int _index;
        public MeleeAction(Transform transform,IPatternFactoryIngredient ingredient, int index) : base(transform, ingredient.BaseLazerData)
        {
            _transform = transform;
            _data = ingredient.MeleeData;
            _index = index;
        }

        public override IEnumerator EValuate()
        {
            yield return GotoHandPosition(_data.hands[_index], _data.handPos[_index]);
            yield return new WaitForSeconds(_data._attackDelay);
                
            var playerTransform = GetPlayerOrNull();
            if (playerTransform == false) yield break;
                
            yield return GotoPlayerHat(_data.hands[_index], playerTransform.position);
            yield return AttackPlayer(_data.hands[_index]);
            yield return GotoHandPosition(_data.hands[_index], _data.handPos[_index]);
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
                .Append(
                    hand
                        .DOMove(targetPoint, _data._handAttackDuration)
                        .SetEase(_data.HandAttackEase)
                )
                .AppendInterval(0.75f)
                .Append(hand
                    .DOMove(prevPoint, _data._handMoveDuration)
                    .SetEase(_data.HandMoveEase)
                );
            return s.WaitForCompletion();
        }

    }

}