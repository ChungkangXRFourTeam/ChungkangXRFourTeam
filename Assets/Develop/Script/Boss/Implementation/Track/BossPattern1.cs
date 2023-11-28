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

            float speed = 0.01f;
            cTrack
                .AddAction(new MeleePositionAction(ingredients))
                
                
               .AddAction(new CrossBossAction(transform, ingredients))
               .AddAction(new DelayAction(speed))
               .AddAction(new CrossBossAction(transform, ingredients))
               .AddAction(new DelayAction(speed))
               .AddAction(new CrossBossAction(transform, ingredients))
               .AddAction(new DelayAction(speed))
               //.AddAction(new CrossBossAction(transform, ingredients))
               //.AddAction(new DelayAction(speed))
               //.AddAction(new CrossBossAction(transform, ingredients))
               //.AddAction(new DelayAction(speed))
               //.AddAction(new CrossBossAction(transform, ingredients))
               //.AddAction(new DelayAction(speed))
               //.AddAction(new CrossBossAction(transform, ingredients))
               //.AddAction(new DelayAction(speed))
               //.AddAction(new CrossBossAction(transform, ingredients))
               //.AddAction(new DelayAction(speed))
                
               .AddAction(new MeleeAction(transform, ingredients, 0))
               .AddAction(new DropBoltBossAction(transform, ingredients))
               .AddAction(new DelayAction(speed))
               
               .AddAction(new MeleeAction(transform, ingredients, 1))
               .AddAction(new DropBoltBossAction(transform, ingredients))
               .AddAction(new DelayAction(speed))
                ;

            cTrack.Predicate = ingredients.BaseLazerData.NextTrigger;
            
            return cTrack;
        }
    }

}

