using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public partial interface IPatternFactoryIngredient
    {
        public BossMeleeData MeleeData { get; }
    }

    [System.Serializable]
    public class BossMeleeData
    {
    }
    public class BossMeleeAction : IAction
    {
        public void Begin()
        {
            
        }

        public void End()
        {
        }

        public IEnumerator EValuate()
        {
            yield break;
        }

        public ITrackPredicate Predicate { get; set; }
    }

}