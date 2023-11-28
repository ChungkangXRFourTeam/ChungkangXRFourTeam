using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
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
        public float TraceMiniumDistance;
        public float TraceDuration;
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

        private Vector2 UpPoint => (Vector2)_data.BodyRender.position + _data.UpOffset;
        private Vector2 DownPoint => (Vector2)_data.BodyRender.position + _data.DownOffset;

        public IEnumerator EValuate()
        {
            // goto up
            while (true)
            {
                //if (IsFarPlayer(out var x))
                //{
                //    var ani = _data.BodyRender.GetComponentInChildren<SkeletonAnimation>();
//
                //    if (ani == false)
                //    {
                //        yield return null;
                //        continue;
                //    }
//
                //    if(x < 0f)
                //        ani.AnimationState.SetAnimation(0, "Boss_Wlaking_Left", false);
                //    else
                //        ani.AnimationState.SetAnimation(0, "Boss_Wlaking_Right", false);
                //    
                //    x = Mathf.MoveTowards(_data.BodyRender.position.x, x, _data.TraceDuration);
                //    var pos = _data.BodyRender.position;
                //    pos.x = x;
                //    _data.BodyRender.position = pos;
                //    yield return new WaitForEndOfFrame();
                //}
                //else
                //{
                    yield return _data.BodyRender.transform.DOMove(UpPoint, _data.FloatingDuration).SetEase(_data.Ease).WaitForCompletion();
                    yield return _data.BodyRender.transform.DOMove(DownPoint, _data.FloatingDuration).SetEase(_data.Ease).WaitForCompletion();
                //}
                
            }
        }

        private bool IsFarPlayer(out float x)
        {
            x = 0f;
            var p = GameObject.FindWithTag("Player");
            if (p == false) return false;

            if (Mathf.Abs(p.transform.position.x - _data.BodyRender.position.x) > _data.TraceMiniumDistance)
            {
                x = p.transform.position.x;
                return true;
            }

            return false;
        }

        public ITrackPredicate Predicate { get; set; }
    }

}