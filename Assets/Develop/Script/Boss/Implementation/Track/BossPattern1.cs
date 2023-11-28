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
                .AddAction(new MeleePositionAction(ingredients))
                
                .AddAction(new CrossBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(1f))
                .AddAction(new CrossBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(1f))
                .AddAction(new CrossBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(1f))
                .AddAction(new CrossBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(1f))
                
                .AddAction(new MeleeAction(transform, ingredients, 0))
                .AddAction(new DropBoltBossAction(transform, ingredients))
                .AddAction(new DelayAction(0.15f))
                
                .AddAction(new MeleeAction(transform, ingredients, 1))
                .AddAction(new DropBoltBossAction(transform, ingredients))
                .AddAction(new DelayAction(0.15f))
                ;

            cTrack.Predicate = ingredients.BaseLazerData.NextTrigger;
            
            return cTrack;
        }
    }

}

