using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace XRProject.Boss
{
    public class CrossBossAction : BaseBossAction
    {
        private MeleeData _meleeData;

        public CrossBossAction(Transform transform, IPatternFactoryIngredient ingredient) : base(transform, ingredient.BaseLazerData)
        {
            _meleeData = ingredient.MeleeData;
        }
        public override IEnumerator EValuate()
        {
            var playerTransform = GetPlayerOrNull();
            var lazer = BaseData.LazerController;
            if (playerTransform == false) yield break;
            
            Vector2 targetPos = (Vector2)playerTransform.position + Vector2.one * 0.5f;

            BaseData.Ani.AnimationState.SetAnimation(0, "Boss_Thunder_Start", false);
            
            _meleeData.hands[0].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_Start", false);
            _meleeData.hands[1].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_Start", false);
            
            _meleeData.hands[0].DORotateQuaternion(Quaternion.Euler(0f, 0f, BaseData.angle), 0.5f);
            yield return _meleeData.hands[1].DORotateQuaternion(Quaternion.Euler(0f, 0f, -BaseData.angle), 0.5f);
            _meleeData.hands[0].GetComponentInChildren<BossHandInteracter>().SetEffectPlay(true);
            _meleeData.hands[1].GetComponentInChildren<BossHandInteracter>().SetEffectPlay(true);
            
            
            yield return new WaitForSeconds(1.533f);
            BaseData.Ani.AnimationState.SetAnimation(0, "Boss_Thunder_Ing", true);
            
            _meleeData.hands[0].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_Ing", true);
            _meleeData.hands[1].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_Ing", true);
            
            yield return PlayMerge(
                HorizontalPlay(0, targetPos, LazerType.Danger),
                VerticalPlay(0, targetPos, LazerType.Danger)
            );
            
            yield return PlayMerge(
                HorizontalPlay(0, targetPos, LazerType.Lazer),
                VerticalPlay(0, targetPos,LazerType.Lazer)
            );

            BaseData.Ani.AnimationState.SetAnimation(0, "Boss_Thunder_end", false);
            _meleeData.hands[0].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_End", false);
            _meleeData.hands[1].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_End", false);
            
            _meleeData.hands[0].DORotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.5f);
            yield return _meleeData.hands[1].DORotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.5f);
            _meleeData.hands[0].GetComponentInChildren<BossHandInteracter>().SetEffectPlay(false);
            _meleeData.hands[1].GetComponentInChildren<BossHandInteracter>().SetEffectPlay(false);
            
            
            yield return new WaitForSeconds(2.667f);
        }

    }

}