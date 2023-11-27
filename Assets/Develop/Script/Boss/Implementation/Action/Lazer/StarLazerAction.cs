using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public class StarBossAction : BaseBossAction
    {
        public StarBossAction(Transform transform, IPatternFactoryIngredient ingredient) : base(transform,ingredient.BaseLazerData)
        {
        }
        public override IEnumerator EValuate()
        {
            var playerTransform = GetPlayerOrNull();
            var lazer = BaseData.LazerController;
            if (playerTransform == false) yield break;

            
            Vector2 targetPos = playerTransform.position;
            Vector2 topCenter = GetSidePoint(Vector2.up, BaseData.BoxSize.y);
            Vector2 bottomCenter = GetSidePoint(Vector2.down, BaseData.BoxSize.y);
            Vector2 leftCenter = GetSidePoint(Vector2.left, BaseData.BoxSize.x);
            Vector2 rightCenter = GetSidePoint(Vector2.right, BaseData.BoxSize.x);
            var top = GetTwoPoint(topCenter, Vector2.up, BaseData.BoxSize.x);
            var bottom = GetTwoPoint(bottomCenter, Vector2.down, BaseData.BoxSize.x);
            var left = GetTwoPoint(leftCenter, Vector2.left, BaseData.BoxSize.y);
            var right = GetTwoPoint(rightCenter, Vector2.right, BaseData.BoxSize.y);
            
            int iter = 5;
            var topPoint = GetInPointFromDelta(top.Item1, top.Item2, 0.5f);
            var leftBottomPoint = GetInPointFromDelta(bottom.Item1, bottom.Item2, 0f);
            var rightBottomPoint = GetInPointFromDelta(bottom.Item1, bottom.Item2, 1f);
            var leftMidPoint = GetInPointFromDelta(left.Item1, left.Item2, 0.5f);
            var rightMidPoint = GetInPointFromDelta(right.Item1, right.Item2, 0.5f);
            
            
            //lazer.SetLinePosition(0, topPoint, leftBottomPoint);
            //lazer.SetLinePosition(1, topPoint, rightBottomPoint);
            //lazer.SetLinePosition(2, leftMidPoint, rightBottomPoint);
            //lazer.SetLinePosition(3, leftMidPoint, rightMidPoint);
            //lazer.SetLinePosition(4, leftBottomPoint, rightMidPoint);
            
            
            //yield return DoCoAttackColoring(0, iter - 1);
            //yield return DoAttackColoring(0, iter - 1);
            //yield return DoClearColoring(0, iter - 1);
            
            
            yield break;
        }

    }

}