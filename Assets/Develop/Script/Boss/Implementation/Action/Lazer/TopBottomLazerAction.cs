using System.Collections;
using System.Collections.Generic;
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
        
        public TopBottomBossAction(Transform transform, IPatternFactoryIngredient ingredient) : base(transform, ingredient.BaseLazerData)
        {
            _data = ingredient.TopBottomLazerData;
        }

        public override IEnumerator EValuate()
        {
            var playerTransform = GetPlayerOrNull();
            var lazer = BaseData.LazerController;
            if (playerTransform == false) yield break;

            
            Vector2 targetPos = playerTransform.position;

            var top = GetSidePoint(Vector2.up, BaseData.BoxSize.y);
            var bottom = GetSidePoint(Vector2.down, BaseData.BoxSize.y);

            yield return PlayMerge(
                HorizontalPlay(0, top + Vector2.down * _data.OffsetTop, LazerType.Danger),
                HorizontalPlay(1, bottom + Vector2.up * _data.OffsetBottom , LazerType.Danger)
            );
            yield return PlayMerge(
                HorizontalPlay(0, top + Vector2.down * _data.OffsetTop, LazerType.Lazer),
                HorizontalPlay(1, bottom + Vector2.up * _data.OffsetBottom, LazerType.Lazer)
            );
        }

    }

}