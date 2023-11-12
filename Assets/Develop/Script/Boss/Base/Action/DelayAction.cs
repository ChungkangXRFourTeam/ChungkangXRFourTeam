using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public class DelayAction : IAction
    {
        private float _duration;

        public DelayAction(float duration)
        {
            _duration = duration;
        }
        public void Begin()
        {
        }

        public void End()
        {
            
        }

        public IEnumerator EValuate()
        {
            yield return new WaitForSeconds(_duration);
        }

        public ITrackPredicate Predicate { get; set; }
    }

}