using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using DG.Tweening;
using UnityEngine;

namespace XRProject.Boss
{
    public class CrossBossAction : BaseBossAction
    {
        
        public CrossBossAction(Transform transform, BaseLazerActionData baseData) : base(transform, baseData)
        {
        }
        public override IEnumerator EValuate()
        {
            var playerTransform = GetPlayerOrNull();
            var lazer = BaseData.LazerController;
            if (playerTransform == false) yield break;
            
            Vector2 targetPos = (Vector2)playerTransform.position + Vector2.one * 0.5f;
            
            
            yield return PlayMerge(
                HorizontalPlay(0, targetPos, LazerType.Danger),
                VerticalPlay(0, targetPos, LazerType.Danger)
            );
            
            yield return PlayMerge(
                HorizontalPlay(0, targetPos, LazerType.Lazer),
                VerticalPlay(0, targetPos,LazerType.Lazer)
            );
        }

    }

}