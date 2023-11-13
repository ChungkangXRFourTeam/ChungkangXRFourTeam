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
                .AddAction(new OpeningBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(1f))
                
                .AddAction(new RandomBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(2f))
                .AddAction(new RandomBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(2f))
                .AddAction(new RandomBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(2f))
                .AddAction(new RandomBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(2f))
                .AddAction(new RandomBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(2f))
                
                
                .AddAction(new MeleePositionAction(ingredients))
                .AddAction(new MeleeAction(transform, ingredients, 0))
                .AddAction(new MeleeAction(transform, ingredients, 1))
                .AddAction(new DelayAction(0.5f))
                
                .AddAction(new MeleeAction(transform, ingredients, 0))
                .AddAction(new MeleeAction(transform, ingredients, 1))
                .AddAction(new DelayAction(0.5f))
                
                .AddAction(new MeleeAction(transform, ingredients, 0))
                .AddAction(new MeleeAction(transform, ingredients, 1))
                .AddAction(new DelayAction(0.5f))
                ;
            
            return cTrack;
        }
    }

}

