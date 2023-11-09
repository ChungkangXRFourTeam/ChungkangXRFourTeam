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

        public IEnumerator EValuate()
        {
            Debug.Log("wait");
            yield return new WaitForSeconds(_duration);
            yield return null;
        }

        public bool IsEnd()
        {
            return _timer >= _duration;
        }

        public ITrackPredicate Predicate { get; set; }
    }

}