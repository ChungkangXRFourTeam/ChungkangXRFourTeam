using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace XRProject.Boss
{
    public class OpeningLazerAction : BaseLazerAction
    {
        
        public OpeningLazerAction(Transform transform, BaseLazerActionData baseData) : base(transform, baseData)
        {
            
            
        }
        public void Begin()
        {
            
        }

        public void End()
        {
        }

        public override IEnumerator EValuate()
        {
            var playerTransform = GetPlayerOrNull();
            var lazer = BaseData.LazerController;
            if (playerTransform == false) yield break;
            
            Vector2 targetPos;
            ((Vector2, Vector2), (Vector2, Vector2)) points1;
            ((Vector2, Vector2), (Vector2, Vector2)) points2;
            targetPos = (Vector2)playerTransform.position + Vector2.up * 5f;
            points1 = GetAcrossPoints(targetPos + Vector2.one * BaseData.Distance);
            points2 = GetAcrossPoints(targetPos + -Vector2.one * BaseData.Distance);

            
            lazer.SetLinePosition(0, points1.Item1.Item1, points1.Item1.Item2);
            lazer.SetLinePosition(1, points1.Item2.Item1, points1.Item2.Item2);
            lazer.SetLinePosition(2, points2.Item1.Item1, points2.Item1.Item2);
            lazer.SetLinePosition(3, points2.Item2.Item1, points2.Item2.Item2);

            yield return DoCoAttackColoring(0, 4);
            yield return DoAttackColoring(0, 4);
            yield return DoClearColoring(0, 4);
        }

        public ITrackPredicate Predicate { get; set; }
    }

}