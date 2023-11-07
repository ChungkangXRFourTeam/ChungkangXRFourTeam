using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace XRProject.Boss
{
    
    public partial interface IPatternFactoryIngredient
    {
        public NormalLazerActionData NormalLazerData { get; }
    }
    
    [System.Serializable]
    public class NormalLazerActionData
    {
        public Vector2 CenterOffset;
        public Vector2 BoxSize;
        public float Distance;
        public BossLazerController LazerController;
    }
    public class NormalLazerAction : IAction
    {
        private NormalLazerActionData _data;
        private Transform _transform;

        private bool _isAttacked;

        public NormalLazerAction(Transform transform, NormalLazerActionData data)
        {
            _transform = transform;
            _data = data;
        }

        private Vector2 GetSidePoint(Vector2 normal, float length)
        {
            return (Vector2)_transform.position + _data.CenterOffset + -normal * length;
        }

        private (Vector2, Vector2) GetTwoPoint(Vector2 sidePoint, Vector2 normal, float size)
        {
            Vector3 newNormal = normal;
            Vector3 standard = Vector3.forward;

            var cross = (Vector2)Vector3.Cross(standard, newNormal);
            cross = cross.normalized * size;
            
            Vector2 begin = cross + sidePoint;
            Vector2 end = -cross + sidePoint;

            return (begin, end);
        }

        private Vector2 GetInPointFromDelta(Vector2 begin, Vector2 end, float t)
        {
            return Vector2.Lerp(begin, end, t);
        }
        private float GetInverseDelta(float start, float end, float cur)
        {
            return cur / (-start + end) - start / (-start + end);
        }

        private ((Vector2, Vector2), (Vector2, Vector2)) GetAcrossPoints(Vector2 cur)
        {
            Vector2 topCenter = GetSidePoint(Vector2.down, _data.BoxSize.y);
            Vector2 bottomCenter = GetSidePoint(Vector2.up, _data.BoxSize.y);
            Vector2 leftCenter = GetSidePoint(Vector2.right, _data.BoxSize.x);
            Vector2 rightCenter = GetSidePoint(Vector2.left, _data.BoxSize.x);
            var top = GetTwoPoint(topCenter, Vector2.down, _data.BoxSize.x);
            var bottom = GetTwoPoint(bottomCenter, Vector2.up, _data.BoxSize.x);
            var left = GetTwoPoint(leftCenter, Vector2.right, _data.BoxSize.y);
            var right = GetTwoPoint(rightCenter, Vector2.left, _data.BoxSize.y);

            var topPoint = GetInPointFromDelta(top.Item1, top.Item2, GetInverseDelta(top.Item1.x, top.Item2.x, cur.x));
            var bottomPoint = GetInPointFromDelta(bottom.Item1, bottom.Item2, GetInverseDelta(bottom.Item1.x, bottom.Item2.x, cur.x));
            var rightPoint = GetInPointFromDelta(left.Item1, left.Item2, GetInverseDelta(left.Item1.y, left.Item2.y, cur.y));
            var leftPoint = GetInPointFromDelta(right.Item1, right.Item2, GetInverseDelta(right.Item1.y, right.Item2.y, cur.y));

            return ((topPoint, bottomPoint), (leftPoint, rightPoint));

        }
        

        private Transform GetPlayerOrNull()
        {
            var player = Physics2D.OverlapBox((Vector2)_transform.position + _data.CenterOffset, _data.BoxSize * 2f, 0f, LayerMask.GetMask("Player"));

            if (player) return player.transform;
            return null;
        }
        
        public void Begin()
        {
            _isAttacked = false;
        }

        public void EValuate()
        {
            var playerTransform = GetPlayerOrNull();
            var lazer = _data.LazerController;
            if (playerTransform == false) return;
            
            var points1 = GetAcrossPoints(playerTransform.position + Vector3.one * _data.Distance);
            var points2 = GetAcrossPoints(playerTransform.position + -Vector3.one*_data.Distance);
            
            lazer.SetLinePosition(0, points1.Item1.Item1, points1.Item1.Item2);            
            lazer.SetLinePosition(1, points1.Item2.Item1, points1.Item2.Item2);        
            lazer.SetLinePosition(2, points2.Item1.Item1, points2.Item1.Item2);            
            lazer.SetLinePosition(3, points2.Item2.Item1, points2.Item2.Item2);

            _isAttacked = true;
        }

        public bool IsEnd()
        {
            return _isAttacked;
        }

        public Predicate Predicate { get; set; }
    }

}