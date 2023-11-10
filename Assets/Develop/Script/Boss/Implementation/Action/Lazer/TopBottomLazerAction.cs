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
        public float LineWidth;
        public float Offset;
    }
    
    public class TopBottomLazerAction : BaseLazerAction
    {
        private TopBottomLazerData _data;
        
        public TopBottomLazerAction(Transform transform, BaseLazerActionData baseData, TopBottomLazerData data) : base(transform, baseData)
        {
            _data = data;
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

            
            Vector2 targetPos = playerTransform.position;

            var top = GetTwoPoint(GetSidePoint(Vector2.up, BaseData.BoxSize.y), Vector2.up, BaseData.BoxSize.x);
            var bottom = GetTwoPoint(GetSidePoint(Vector2.down, BaseData.BoxSize.y), Vector2.down, BaseData.BoxSize.x);

            Vector2 offset = new Vector2(0f, _data.Offset);
            top.Item1 = top.Item1 + -offset;
            top.Item2 = top.Item2 + -offset;
            bottom.Item1 = bottom.Item1 + offset;
            bottom.Item2 = bottom.Item2 + offset;
            
            int iter = 2;
            lazer.SetLinePosition(0, top.Item1, top.Item2);
            lazer.SetLinePosition(1, bottom.Item1, bottom.Item2);
            lazer.GetRendererOrNull(0).startWidth = _data.LineWidth;
            lazer.GetRendererOrNull(0).endWidth = _data.LineWidth;
            lazer.GetRendererOrNull(1).startWidth = _data.LineWidth;
            lazer.GetRendererOrNull(1).endWidth = _data.LineWidth;
            
            yield return DoCoAttackColoring(0, iter - 1);
            yield return DoAttackColoring(0, iter- 1);
            yield return DoClearColoring(0, iter- 1);
            
            
            yield break;
        }

        public ITrackPredicate Predicate { get; set; }
    }

}