using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace XRProject.Boss
{
    public partial interface IPatternFactoryIngredient
    {
        public BaseLazerActionData BaseLazerData { get; }
    }

    [System.Serializable]
    public class BaseLazerActionData
    {
        public Vector2 CenterOffset;
        public Vector2 BoxSize;
        public float Distance;
        public float CoAttackDuration;
        public float AttackDuration;
        public float AttackClearDuration;
        public BossLazerController LazerController;
        public SkeletonAnimation Ani;
        public float angle = 38.06f;
    }

    public abstract class BaseBossAction : IAction
    {
        protected BaseLazerActionData BaseData { get; private set; }
        protected Transform transform { get; private set; }

        public BaseBossAction(Transform transform, BaseLazerActionData baseData)
        {
            this.transform = transform;
            BaseData = baseData;
        }
        protected Vector2 GetSidePoint(Vector2 normal, float length)
        {
            return (Vector2)transform.position + BaseData.CenterOffset + normal * length;
        }

        protected (Vector2, Vector2) GetTwoPoint(Vector2 sidePoint, Vector2 normal, float size)
        {
            Vector3 newNormal = normal;
            Vector3 standard = Vector3.forward;

            var cross = (Vector2)Vector3.Cross(standard, newNormal);
            cross = cross.normalized * size;

            Vector2 begin = -cross + sidePoint;
            Vector2 end = cross + sidePoint;

            return (begin, end);
        }

        protected Vector2 GetInPointFromDelta(Vector2 begin, Vector2 end, float t)
        {
            return Vector2.Lerp(begin, end, t);
        }

        protected float GetInverseDelta(float start, float end, float cur)
        {
            return cur / (-start + end) - start / (-start + end);
        }

        protected ((Vector2, Vector2), (Vector2, Vector2)) GetAcrossPoints(Vector2 cur)
        {
            Vector2 topCenter = GetSidePoint(Vector2.down, BaseData.BoxSize.y);
            Vector2 bottomCenter = GetSidePoint(Vector2.up, BaseData.BoxSize.y);
            Vector2 leftCenter = GetSidePoint(Vector2.right, BaseData.BoxSize.x);
            Vector2 rightCenter = GetSidePoint(Vector2.left, BaseData.BoxSize.x);
            var top = GetTwoPoint(topCenter, Vector2.down, BaseData.BoxSize.x);
            var bottom = GetTwoPoint(bottomCenter, Vector2.up, BaseData.BoxSize.x);
            var left = GetTwoPoint(leftCenter, Vector2.right, BaseData.BoxSize.y);
            var right = GetTwoPoint(rightCenter, Vector2.left, BaseData.BoxSize.y);

            var topPoint = GetInPointFromDelta(top.Item1, top.Item2, GetInverseDelta(top.Item1.x, top.Item2.x, cur.x));
            var bottomPoint = GetInPointFromDelta(bottom.Item1, bottom.Item2,
                GetInverseDelta(bottom.Item1.x, bottom.Item2.x, cur.x));
            var rightPoint =
                GetInPointFromDelta(left.Item1, left.Item2, GetInverseDelta(left.Item1.y, left.Item2.y, cur.y));
            var leftPoint = GetInPointFromDelta(right.Item1, right.Item2,
                GetInverseDelta(right.Item1.y, right.Item2.y, cur.y));

            return ((topPoint, bottomPoint), (leftPoint, rightPoint));
        }

        protected Vector2 GetPointFromTwoLine(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            return new Vector2(
                ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) /
                ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x)),
                ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) /
                ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x))
            );
        }

        protected ((Vector2, Vector2), (Vector2, Vector2)) GetAcrossRandomPoints(Vector2 cur)
        {
            Vector2 topCenter = GetSidePoint(Vector2.up, BaseData.BoxSize.y);
            Vector2 bottomCenter = GetSidePoint(Vector2.down, BaseData.BoxSize.y);
            Vector2 leftCenter = GetSidePoint(Vector2.left, BaseData.BoxSize.x);
            Vector2 rightCenter = GetSidePoint(Vector2.right, BaseData.BoxSize.x);
            var top = GetTwoPoint(topCenter, Vector2.up, BaseData.BoxSize.x);
            var bottom = GetTwoPoint(bottomCenter, Vector2.down, BaseData.BoxSize.x);
            var left = GetTwoPoint(leftCenter, Vector2.left, BaseData.BoxSize.y);
            var right = GetTwoPoint(rightCenter, Vector2.right, BaseData.BoxSize.y);

            var topPoint = GetInPointFromDelta(top.Item1, top.Item2, Random.value);
            var bottomPoint = GetPointFromTwoLine(topPoint, (cur - topPoint) * 100000f, bottom.Item1, bottom.Item2);
            var leftPoint = GetInPointFromDelta(left.Item1, left.Item2, Random.value);
            var rightPoint = GetPointFromTwoLine(leftPoint, (cur - leftPoint) * 100000f, right.Item1, right.Item2);

            return ((topPoint, bottomPoint), (leftPoint, rightPoint));
        }

        protected Transform GetPlayerOrNull()
        {
            var player = Physics2D.OverlapBox((Vector2)transform.position + BaseData.CenterOffset, BaseData.BoxSize * 2f, 0f,
                LayerMask.GetMask("Player"));

            if (player) return player.transform;
            return null;
        }

        protected float HorizontalPlay(int index, Vector2 targetPos, LazerType type)
        {
            var lazer = BaseData.LazerController;
            Vector2 rightCenter = GetSidePoint(Vector2.right, BaseData.BoxSize.x);
            var points = GetTwoPoint(rightCenter, Vector2.right, BaseData.BoxSize.y);


            var delta = GetInverseDelta(points.Item1.y, points.Item2.y, targetPos.y);

            Vector2 startPos = GetInPointFromDelta(points.Item1, points.Item2, delta);
            float duration = lazer.Play(index, startPos, DirectionType.Horizontal, type);

            return duration;
        }

        protected float VerticalPlay(int index, Vector2 targetPos, LazerType type)
        {
            var lazer = BaseData.LazerController;
            Vector2 upCenter = GetSidePoint(Vector2.up, BaseData.BoxSize.y);
            var points = GetTwoPoint(upCenter, Vector2.up, BaseData.BoxSize.x);
            var delta = GetInverseDelta(points.Item1.x, points.Item2.x, targetPos.x);
            
            Vector2 startPos = GetInPointFromDelta(points.Item1, points.Item2, delta);
            
            float duration = lazer.Play(index, startPos, DirectionType.Vertical, type);

            return duration;
        }

        protected YieldInstruction PlayMerge(params float[] durations)
        {
            float duration = durations.Max();
            return new WaitForSeconds(duration);
        }
        public virtual void Begin()
        {
        }

        public virtual void End()
        {
        }

        public abstract IEnumerator EValuate();

        public ITrackPredicate Predicate { get; set; }
    }
}