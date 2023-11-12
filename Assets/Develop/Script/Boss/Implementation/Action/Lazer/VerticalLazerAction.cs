using System.Collections;
using System.Collections.Generic;
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
        
        public VerticalBossAction(Transform transform, BaseLazerActionData baseData, VerticalLazerData data) : base(transform, baseData)
        {
            _data = data;
        }

        public override IEnumerator EValuate()
        {
            var playerTransform = GetPlayerOrNull();
            var lazer = BaseData.LazerController;
            if (playerTransform == false) yield break;

            
            Vector2 targetPos = playerTransform.position;

            var top = GetTwoPoint(GetSidePoint(Vector2.up, BaseData.BoxSize.y), Vector2.up, BaseData.BoxSize.x);
            var bottom = GetTwoPoint(GetSidePoint(Vector2.down, BaseData.BoxSize.y), Vector2.down, BaseData.BoxSize.x);

            int iter = _data.Interation;

            for (int i = 0; i < iter; i++)
            {
                float t = (float)(i + 1) / (float)iter;
                Vector2 start = Vector2.Lerp(top.Item1, top.Item2, t);
                Vector2 end = Vector2.Lerp(bottom.Item2, bottom.Item1, t);
                
                lazer.SetLinePosition(i, start, end);
            }
            
            yield return DoCoAttackColoring(0, iter - 1);
            yield return DoAttackColoring(0, iter- 1);
            yield return DoClearColoring(0, iter- 1);
            
            
            yield break;
        }

    }

}