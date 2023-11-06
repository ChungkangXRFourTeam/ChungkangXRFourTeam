using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public class NormalLazerAction : IAction
    {
        public void Begin()
        {
        }

        public void EValuate()
        {
            Debug.Log("Lazer!!");
        }

        public bool IsEnd()
        {
            return true;
        }

        public Predicate Predicate { get; set; }
    }

}