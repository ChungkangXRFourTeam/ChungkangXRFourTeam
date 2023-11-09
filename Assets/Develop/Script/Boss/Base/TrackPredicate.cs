using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public interface ITrackPredicate
    {
        public void Process(ActionList actionList, Track track, int index);
    }

    public class RepeatPredicate : ITrackPredicate
    {
        public int Index { get; set; }
        public void Process(ActionList actionList, Track track, int index)
        {
            actionList.GotoCursorOnBasedCurrentTrack(Index);
        }
    }

}