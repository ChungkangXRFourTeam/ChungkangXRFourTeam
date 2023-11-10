using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace XRProject.Boss
{
    public enum BossLazerType
    {
        
    }
    public class BossLazerController : MonoBehaviour
    {
        [SerializeField] private LineRenderer[] _lineRenderers;

        private bool IsInValidIndex(int index)
        {
            return index >= _lineRenderers.Length || index < 0;
        }
        
        public void SetLinePosition(int index, Vector2 a, Vector2 b)
        {
            if (IsInValidIndex(index)) return;

            _lineRenderers[index].positionCount = 2;
            _lineRenderers[index].SetPosition(0, a);
            _lineRenderers[index].SetPosition(1, b);
        }
        public Sequence SetLineColorTween(int index, Color color, float duration)
        {
            if (IsInValidIndex(index)) return DOTween.Sequence();

            _lineRenderers[index].positionCount = 2;
            
            var s = DOTween.Sequence();
            var c = _lineRenderers[index].sharedMaterial.color;
            c.a = 0;
            s.Join(_lineRenderers[index].DOColor(
                    new Color2(c, c),
                    new Color2(color, color),
                    duration)
                );

            return s;
        }

        public LineRenderer GetRendererOrNull(int index)
        {
            if (IsInValidIndex(index)) return null;

            return _lineRenderers[index];
        }

        public void SetLineType(int index, BossLazerType type)
        {
            
        }
    }
}
