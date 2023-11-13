using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Boss
{
    public class NextPattern : ITrackPredicate
    {
        private Boss _boss;
        public NextPattern(Boss boss)
        {
            _boss = boss;
        }
        public void Process(ActionList actionList)
        {
            float percentage = _boss.CurrentHP / _boss.MaxHp;
            if (percentage < 0.5f)
            {
                actionList.NextTrack();
            }
        }
    }

}