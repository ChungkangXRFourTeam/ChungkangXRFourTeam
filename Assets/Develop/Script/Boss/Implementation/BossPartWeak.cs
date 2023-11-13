using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public class BossPartWeak : MonoBehaviour
    {
        public InteractionController Interaction { get; private set; }

        private void Awake()
        {
            Interaction = GetComponentInParent<InteractionController>();
        }
    }

}