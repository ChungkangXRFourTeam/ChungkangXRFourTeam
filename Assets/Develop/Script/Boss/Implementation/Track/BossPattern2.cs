using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public static partial class BossPatternFactory
    {
        public static Track Pattern2(Transform transform, IPatternFactoryIngredient ingredients)
        {
            var cTrack = new Track();

            List<IAction> actionList = new List<IAction>()
            {
                new TopBottomBossAction(transform, ingredients),
                new VerticalBossAction(transform, ingredients, true),
                new TopBottomBossAction(transform, ingredients),
                new VerticalBossAction(transform, ingredients, false),
                new TopBottomBossAction(transform, ingredients),
                new VerticalBossAction(transform, ingredients, true),
                new TopBottomBossAction(transform, ingredients),
                new VerticalBossAction(transform, ingredients, true),
                new TopBottomBossAction(transform, ingredients),
                new VerticalBossAction(transform, ingredients, false),
            };
            
            cTrack
                //.AddAction(new MeleePositionAction(ingredients))
                
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(0.5f))
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(0.5f))
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(0.5f))
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(0.5f))
                
                .AddAction(new MeleeAction(transform, ingredients, 0))
                .AddAction(new DropBoltBossAction(transform, ingredients))
                .AddAction(new DelayAction(0.1f))
                
                .AddAction(new MeleeAction(transform, ingredients, 1))
                .AddAction(new DropBoltBossAction(transform, ingredients))
                .AddAction(new DelayAction(0.1f))
                ;

            cTrack.Predicate = ingredients.BaseLazerData.NextTrigger;
            
            return cTrack;
        }
    }

}