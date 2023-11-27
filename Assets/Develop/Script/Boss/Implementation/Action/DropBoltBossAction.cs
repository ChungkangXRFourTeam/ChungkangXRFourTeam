using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace XRProject.Boss
{
    public partial interface IPatternFactoryIngredient
    {
        public DropBoltBossActionData DropBoltBossActionData { get; }
    }
    
    [Serializable]
    public class DropBoltBossActionData
    {
        public float DropInterval;
        public int MaxDropCount;
        public GameObject[] BoltPrototypes;
        public Vector2 Offset;
    }
        
    
    public class DropBoltBossAction : BaseBossAction
    {
        private DropBoltBossActionData _data;
        public DropBoltBossAction(Transform transform, IPatternFactoryIngredient ingredient) : base(transform, ingredient.BaseLazerData)
        {
            _data = ingredient.DropBoltBossActionData;
        }

        public override IEnumerator EValuate()
        {
            int count = 0;

            while (count < _data.MaxDropCount)
            {
                var points = GetTwoPoint(GetSidePoint(Vector2.up, BaseData.BoxSize.y), Vector2.up, BaseData.BoxSize.x);
                float randomValue = Random.value;
                
                DropBolt(Vector2.Lerp(points.Item1 + _data.Offset, points.Item2 + _data.Offset, randomValue));
                count++;
                
                yield return new WaitForSeconds(_data.DropInterval);
            }
        }

        private void DropBolt(Vector2 pos)
        {
            int randomIndex = (int)(Random.value * _data.BoltPrototypes.Length);
            
            var obj = GameObject.Instantiate(_data.BoltPrototypes[randomIndex]);
            obj.SetActive(true);
            obj.transform.position = pos;
        }
    }

}