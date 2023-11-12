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
                
                .AddAction(new VerticalBossAction(transform, ingredients.BaseLazerData, ingredients.VerticalLazerData))
                .AddAction(new DelayAction(2f))
                .AddAction(new VerticalBossAction(transform, ingredients.BaseLazerData, ingredients.VerticalLazerData))
                .AddAction(new DelayAction(2f))
                
                .AddAction(new StarBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(2f))
                .AddAction(new StarBossAction(transform, ingredients.BaseLazerData))
                .AddAction(new DelayAction(2f))
                
                .AddAction(new TopBottomBossAction(transform, ingredients.BaseLazerData, ingredients.TopBottomLazerData))
                .AddAction(new DelayAction(2f))
                .AddAction(new TopBottomBossAction(transform, ingredients.BaseLazerData, ingredients.TopBottomLazerData))
                .AddAction(new DelayAction(2f))
                ;

            cTrack = new Track();
            cTrack
                .AddAction(new MeleeAction(transform, ingredients, 0))
                .AddAction(new MeleeAction(transform, ingredients, 1))
                //.AddAction(new OpeningLazerAction(transform, ingredients.BaseLazerData))
                //.AddAction(new RandomLazerAction(transform, ingredients.BaseLazerData))
                //.AddAction(new VerticalLazerAction(transform, ingredients.BaseLazerData, ingredients.VerticalLazerData))
                //.AddAction(new StarLazerAction(transform, ingredients.BaseLazerData))
                //.AddAction(new TopBottomLazerAction(transform, ingredients.BaseLazerData, ingredients.TopBottomLazerData))
                .AddAction(new DelayAction(1f))
                ;
            
            cTrack.Predicate = new RepeatPredicate() { Index = 0 };
            
            return cTrack;
        }
    }

}

