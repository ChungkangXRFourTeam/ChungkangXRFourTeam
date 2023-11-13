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
                new StarBossAction(transform, ingredients),
                new TopBottomBossAction(transform, ingredients),
                new VerticalBossAction(transform, ingredients),
            };

            cTrack
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(1f))
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(1f))
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(1f))
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(1f))
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(1f))
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(1f))
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(1f))
                .AddAction(actionList[Random.Range(0, actionList.Count)])
                .AddAction(new DelayAction(1f))
                
                
                .AddAction(new MeleePositionAction(ingredients))
                .AddAction(new MeleeAction(transform, ingredients, 0))
                .AddAction(new MeleeAction(transform, ingredients, 1))
                
                .AddAction(new MeleeAction(transform, ingredients, 0))
                .AddAction(new MeleeAction(transform, ingredients, 1))
                
                .AddAction(new MeleeAction(transform, ingredients, 0))
                .AddAction(new MeleeAction(transform, ingredients, 1))
                
                .AddAction(new MeleeAction(transform, ingredients, 0))
                .AddAction(new MeleeAction(transform, ingredients, 1))
                ;
            
            return cTrack;
        }
    }

}