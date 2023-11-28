using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public partial interface IPatternFactoryIngredient
    {
    } 

    public static partial class BossPatternFactory
    {
        public static Track MeleeTestTrack(Transform transform, IPatternFactoryIngredient ingredients)
        {
            Track parentTrack = new Track();
            
            var cTrack = new Track();

            cTrack
                
                .AddAction(new MeleePositionAction(ingredients))
                .AddAction(new MeleeAction(transform, ingredients, 0))
                .AddAction(new MeleeAction(transform, ingredients, 1))
                .AddAction(new DelayAction(0.5f))
                ;
            
            parentTrack.AddAction(cTrack);
            cTrack.Predicate = new RepeatPredicate() {Index = 0};
            
            return parentTrack;
        }
        public static Track LazerTestTrack(Transform transform, IPatternFactoryIngredient ingredients)
        {
            Track parentTrack = new Track();
            
            var cTrack = new Track();

            cTrack
                //.AddAction(new RandomBossAction(transform, ingredients.BaseLazerData))
                //.AddAction(new TopBottomBossAction(transform, ingredients))
                .AddAction(new DropBoltBossAction(transform, ingredients))
                //.AddAction(new VerticalBossAction(transform, ingredients))
                //.AddAction(new TopBottomBossAction(transform, ingredients))
                .AddAction(new DelayAction(0.5f))
                ;

            cTrack.Predicate = new RepeatPredicate() { Index = 0 };
            
            parentTrack.AddAction(cTrack);
            
            return parentTrack;
        }
        public static Track CompletionBattleTrack(Transform transform, IPatternFactoryIngredient ingredients)
        {
            Track parentTrack = new Track();
            
            parentTrack
                .AddAction(Pattern1(transform, ingredients))
                .AddAction(Pattern2(transform, ingredients))
                ;
            
            return parentTrack;
        }
    
        public static Track CompletionMovementTrack(Transform transform, IPatternFactoryIngredient ingredients)
        {
            Track parentTrack = new Track();

            parentTrack
                .AddAction(new MovementAction(transform, ingredients))
                ;
        
            return parentTrack;
        }

        public static Track CompleitionTranslationTrack(Transform transform, IPatternFactoryIngredient ingredient)
        {
            
            Track parentTrack = new Track();

            parentTrack
                .AddAction(new BossTranslationAction(transform, ingredient))
                ;
        
            return parentTrack;
        }


        public static Track CompletionPatternPhase1(Transform transform, IPatternFactoryIngredient ingredient)
        {
            Track parentTrack = new Track();

            parentTrack
                .AddAction(Pattern1(transform, ingredient))
                .AddAction(Pattern2(transform, ingredient))
                ;
            
            return parentTrack;
        }
    }

}