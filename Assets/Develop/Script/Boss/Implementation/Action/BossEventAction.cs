using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XRProject.Boss
{
    public enum EBossPhase
    {
        Phase1,
        Phase2,
        Phase3,
    }
    
    public class BossEventAction : IAction
    {
        private EBossPhase _phase;
        
        public BossEventAction(EBossPhase phase)
        {
            _phase = phase;
        }
        public void Begin()
        {
            
        }

        public void End()
        {
        }

        public IEnumerator EValuate()
        {
            UniTask? wait = null;
            
            switch (_phase)
            {
                case EBossPhase.Phase1:
                    wait = TalkingEventManager.Instance.InvokeCurrentEvent(new DeathEvent());
                    break;
                case EBossPhase.Phase2:
                    break;
                case EBossPhase.Phase3:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            yield return new WaitUntil(() =>
            {
                if (wait.HasValue) return false;
                return wait.Value.Status == UniTaskStatus.Pending;
            });
        }

        public ITrackPredicate Predicate { get; set; }
    }   
}
