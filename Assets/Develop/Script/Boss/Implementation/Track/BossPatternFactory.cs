using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public static partial class BossPatternFactory
    {
        public static Track CompletionTrack()
        {
            Track parentTrack = new Track();
            
            parentTrack
                .AddAction(Pattern1())
                .AddAction(Pattern1())
                ;
            
            return parentTrack;
        }
    }

}