using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace XRProject.Boss
{
    public class OpeningBossAction : BaseBossAction
    {
        
        public OpeningBossAction(Transform transform, BaseLazerActionData baseData) : base(transform, baseData)
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
        }

    }

}