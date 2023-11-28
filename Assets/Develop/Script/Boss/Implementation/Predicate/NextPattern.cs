using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public class NextPattern : ITrackPredicate
    {
        private bool _goNext;
        
        public NextPattern()
        {
            _goNext = false;
        }

        public void TriggerNextPhase()
        {
            _goNext = true;
        }
        
        public void Process(ActionList actionList)
        {
            if (_goNext == false)
            {
                if (actionList.IsActionEndedCurrentTrack)
                {
                    actionList.GotoCursorOnBasedCurrentTrack(0);
                }
            }
            else
            {
                actionList.NextTrack();
                _goNext = false;
            }
        }
    }

}