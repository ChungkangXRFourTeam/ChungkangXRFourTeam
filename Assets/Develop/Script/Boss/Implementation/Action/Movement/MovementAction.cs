using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace XRProject.Boss
{
    public partial interface IPatternFactoryIngredient
    {
        public MovementActionData MovementActionData { get; }
    }
    
    [System.Serializable]
    public class MovementActionData
    {
        public float FloatingDuration;
        public Vector2 UpOffset;
        public Vector2 DownOffset;
        public Ease Ease;
        public Transform BodyRender;
    }
    
    public class MovementAction : IAction
    {
        private MovementActionData _data;
        private Transform _transform;
        
        public MovementAction(Transform transform,IPatternFactoryIngredient ingredient)
        {
            _transform = transform;
            _data = ingredient.MovementActionData;
        }
        public void Begin()
        {
            
        }

        public void End()
        {
        }

        private Vector2 UpPoint => (Vector2)_transform.position + _data.UpOffset;
        private Vector2 DownPoint => (Vector2)_transform.position + _data.DownOffset;

        public IEnumerator EValuate()
        {
            // goto up
            while (true)
            {
                yield return _data.BodyRender.transform.DOMove(UpPoint, _data.FloatingDuration).SetEase(_data.Ease).WaitForCompletion();
                yield return _data.BodyRender.transform.DOMove(DownPoint, _data.FloatingDuration).SetEase(_data.Ease).WaitForCompletion();
            }
        }

        public ITrackPredicate Predicate { get; set; }
    }

}