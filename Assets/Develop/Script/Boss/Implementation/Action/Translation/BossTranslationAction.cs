using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace XRProject.Boss
{
    public class BossTranslationAction : IAction
    {
        private Transform _transform;
        public BossTranslationAction(Transform transform, IPatternFactoryIngredient ingredient)
        {
            _transform = transform;
        }

        public void Begin()
        {
        }

        public void End()
        {
        }

        public IEnumerator EValuate()
        {
            float angle = 15f;
            var origin = _transform.rotation;
            var toLeft = Quaternion.Euler(0f, 0f, angle);
            var toRight =  Quaternion.Euler(0f, 0f,-angle);
            var left =  toLeft * _transform.rotation;
            var right =  toRight * _transform.rotation;
            float duration = 0.1f;

            var s = DOTween.Sequence();
            s
                .Append(_transform.DORotateQuaternion(left, duration))
                .Append(_transform.DORotateQuaternion(right, duration))
                .Append(_transform.DORotateQuaternion(left, duration))
                .Append(_transform.DORotateQuaternion(right, duration))
                .Append(_transform.DORotateQuaternion(left, duration))
                .Append(_transform.DORotateQuaternion(right, duration))
                .Append(_transform.DORotateQuaternion(left, duration))
                .Append(_transform.DORotateQuaternion(right, duration))
                .Append(_transform.DORotateQuaternion(origin, duration))
            ;

            yield return s.WaitForCompletion();
        }

        public ITrackPredicate Predicate { get; set; }
    }
}