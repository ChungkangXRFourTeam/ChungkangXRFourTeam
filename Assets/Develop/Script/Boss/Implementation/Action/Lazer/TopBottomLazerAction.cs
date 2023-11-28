using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace XRProject.Boss
{
    public partial interface IPatternFactoryIngredient
    {
        public TopBottomLazerData TopBottomLazerData { get; }
    }

    [System.Serializable]
    public class TopBottomLazerData
    {
        public float OffsetTop;
        public float OffsetBottom;
    }
    
    public class TopBottomBossAction : BaseBossAction
    {
        private TopBottomLazerData _data;
        private MeleeData _meleeData;
        
        public TopBottomBossAction(Transform transform, IPatternFactoryIngredient ingredient) : base(transform, ingredient.BaseLazerData)
        {
            _data = ingredient.TopBottomLazerData;
            _meleeData = ingredient.MeleeData;
        }

        public override IEnumerator EValuate()
        {
            var playerTransform = GetPlayerOrNull();
            var lazer = BaseData.LazerController;
            if (playerTransform == false) yield break;

            
            Vector2 targetPos = playerTransform.position;

            var top = GetSidePoint(Vector2.up, BaseData.BoxSize.y);
            var bottom = GetSidePoint(Vector2.down, BaseData.BoxSize.y);
            
            yield return _meleeData.hands[0].DORotateQuaternion(Quaternion.Euler(0f, 0f, BaseData.angle), 0.5f);
            yield return _meleeData.hands[1].DORotateQuaternion(Quaternion.Euler(0f, 0f, -BaseData.angle), 0.5f);

            BaseData.Ani.AnimationState.SetAnimation(0, "Boss_Thunder_Start", false);
            
            _meleeData.hands[0].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_Start", false);
            _meleeData.hands[1].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_Start", false);
            
            yield return new WaitForSeconds(1.533f);
            BaseData.Ani.AnimationState.SetAnimation(0, "Boss_Thunder_Ing", true);
            
            _meleeData.hands[0].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_Ing", true);
            _meleeData.hands[1].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_Ing", true);
            
            yield return PlayMerge(
                HorizontalPlay(0, top + Vector2.down * _data.OffsetTop, LazerType.Danger),
                HorizontalPlay(1, bottom + Vector2.up * _data.OffsetBottom , LazerType.Danger)
            );
            yield return PlayMerge(
                HorizontalPlay(0, top + Vector2.down * _data.OffsetTop, LazerType.Lazer),
                HorizontalPlay(1, bottom + Vector2.up * _data.OffsetBottom, LazerType.Lazer)
            );
            BaseData.Ani.AnimationState.SetAnimation(0, "Boss_Thunder_end", false);
            _meleeData.hands[0].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_End", false);
            _meleeData.hands[1].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_End", false);
            yield return new WaitForSeconds(2.667f);
        }

    }

}