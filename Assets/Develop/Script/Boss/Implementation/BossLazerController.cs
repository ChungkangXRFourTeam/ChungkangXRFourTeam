using System;
using System.Collections;
using System.Collections.Generic;
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

        public void SetLineType(int index, BossLazerType type)
        {
            
        }
    }
}
