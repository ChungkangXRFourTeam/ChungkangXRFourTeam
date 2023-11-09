using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public class DelayAction : IAction
    {
        private float _timer;
        private float _duration;

        public DelayAction(float duration)
        {
            _duration = duration;
        }
        public void Begin()
        {
            _timer = 0f;
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