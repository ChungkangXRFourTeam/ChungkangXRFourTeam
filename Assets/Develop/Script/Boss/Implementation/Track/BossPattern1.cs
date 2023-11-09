using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public static partial class BossPatternFactory
    {
        public static Track Pattern1(Transform transform, IPatternFactoryIngredient ingredients)
        {
            var cTrack = new Track();

            cTrack
                .AddAction(new NormalLazerAction(transform, ingredients.NormalLazerData))
                .AddAction(new DelayAction(3f))
                ;

            cTrack.Predicate = (target, parent) =>
            {
                if (target is not Track track) return;
                if (parent is not Track p) return;

                if (target.IsEnd())
                {
                    p.ReplayAction();
                }
            };
            
            return cTrack;
        }
    }

}

