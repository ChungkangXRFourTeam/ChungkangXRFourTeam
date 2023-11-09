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
                .AddAction(new DelayAction(1f))
                ;

            cTrack.Predicate = new RepeatPredicate() { Index = 0 };
            
            return cTrack;
        }
    }

}

