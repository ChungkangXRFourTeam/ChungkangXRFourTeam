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
        public static Track CompletionTrack(Transform transform, IPatternFactoryIngredient ingredients)
        {
            Track parentTrack = new Track();
            
            parentTrack
                .AddAction(Pattern1(transform, ingredients))
                .AddAction(Pattern2())
                ;
            
            return parentTrack;
        }
    }

}