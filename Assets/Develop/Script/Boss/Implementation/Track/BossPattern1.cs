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
                .AddAction(new NormalLazerAction(transform, ingredients.NormalLazerData, true))
                .AddAction(new DelayAction(1f))
                .AddAction(new NormalLazerAction(transform, ingredients.NormalLazerData, false))
                .AddAction(new DelayAction(2f))
                .AddAction(new NormalLazerAction(transform, ingredients.NormalLazerData, false))
                .AddAction(new DelayAction(2f))
                .AddAction(new NormalLazerAction(transform, ingredients.NormalLazerData, false))
                .AddAction(new DelayAction(2f))
                .AddAction(new NormalLazerAction(transform, ingredients.NormalLazerData, false))
                .AddAction(new DelayAction(2f))
                ;

            cTrack.Predicate = new RepeatPredicate() { Index = 0 };
            
            return cTrack;
        }
    }

}

