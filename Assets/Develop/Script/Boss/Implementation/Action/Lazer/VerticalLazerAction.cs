using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace XRProject.Boss
{
    public partial interface IPatternFactoryIngredient
    {
        public VerticalLazerData VerticalLazerData { get; }
    }

    [System.Serializable]
    public class VerticalLazerData
    {
        public int Interation;
    }
    
    public class VerticalBossAction : BaseBossAction
    {
        private VerticalLazerData _data;
        private bool _dirty;
        private MeleeData _meleeData;
        
        public VerticalBossAction(Transform transform, IPatternFactoryIngredient ingredient, bool dirty) : base(transform, ingredient.BaseLazerData)
        {
            _data = ingredient.VerticalLazerData;
            _dirty = dirty;
            _meleeData = ingredient.MeleeData;
        }

        public override IEnumerator EValuate()
        {
            var playerTransform = GetPlayerOrNull();
            var lazer = BaseData.LazerController;
            if (playerTransform == false) yield break;

            
            BaseData.Ani.AnimationState.SetAnimation(0, "Boss_Thunder_Start", false);
            _meleeData.hands[0].DORotateQuaternion(Quaternion.Euler(0f, 0f, BaseData.angle), 0.5f);
            yield return _meleeData.hands[1].DORotateQuaternion(Quaternion.Euler(0f, 0f, -BaseData.angle), 0.5f);
            _meleeData.hands[0].GetComponentInChildren<BossHandInteracter>().SetEffectPlay(true);
            _meleeData.hands[1].GetComponentInChildren<BossHandInteracter>().SetEffectPlay(true);
            
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
            
            Vector2 targetPos = playerTransform.position;

            var top = GetTwoPoint(GetSidePoint(Vector2.up, BaseData.BoxSize.y), Vector2.up, BaseData.BoxSize.x);

            int iter = _data.Interation;
            List<float> arr = new();

            Vector2 d = _dirty ? Vector2.right * 4f : Vector2.zero;
            
            for (int i = 0; i < iter; i++)
            {
                float t = (float)(i + 1) / (float)iter;
                Vector2 start = Vector2.Lerp(top.Item1, top.Item2, t) + d;
                arr.Add(lazer.Play(i, start, DirectionType.Vertical, LazerType.Danger));
            }
            yield return PlayMerge(arr.ToArray());
            arr.Clear();
            for (int i = 0; i < iter; i++)
            {
                float t = (float)(i + 1) / (float)iter;
                Vector2 start = Vector2.Lerp(top.Item1, top.Item2, t) + d;
                arr.Add(lazer.Play(i, start, DirectionType.Vertical, LazerType.Lazer));
            }
            yield return PlayMerge(arr.ToArray());
            BaseData.Ani.AnimationState.SetAnimation(0, "Boss_Thunder_end", false);
            _meleeData.hands[0].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_End", false);
            _meleeData.hands[1].GetComponentInChildren<SkeletonAnimation>().AnimationState
                .SetAnimation(0, "Boss_Rights_Hand_Thunder_End", false);
            _meleeData.hands[0].GetComponentInChildren<BossHandInteracter>().SetEffectPlay(false);
            _meleeData.hands[1].GetComponentInChildren<BossHandInteracter>().SetEffectPlay(false);
            
            _meleeData.hands[0].DORotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.5f);
            yield return _meleeData.hands[1].DORotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.5f);
            yield return new WaitForSeconds(2.667f);
        }

    }

}