using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace XRProject.Boss
{
    public enum DirectionType
    {
        Horizontal,
        Vertical
    }
    public enum LazerType
    {
        Lazer,
        Danger
    }
    public class BossLazerController : MonoBehaviour
    {
        [SerializeField] private Transform[] _horizontalEffect;
        [SerializeField] private Transform[] _verticalEffect;
        [SerializeField] private Transform[] _horizontalDanger;
        [SerializeField] private Transform[] _verticalDanger;

        private bool IsEffectInValidIndex(int index, DirectionType type)
        {
            if (type == DirectionType.Horizontal)
                return index >= _horizontalEffect.Length || index < 0;
            else
                return index >= _verticalEffect.Length || index < 0;
        }
        private bool IsDangerInValidIndex(int index, DirectionType type)
        {
            if (type == DirectionType.Horizontal)
                return index >= _horizontalDanger.Length || index < 0;
            else
                return index >= _verticalDanger.Length || index < 0;
        }

        public float Play(int index, Vector2 pos, DirectionType dType, LazerType lType)
        {
            if (lType == LazerType.Danger) return PlayDanger(index, pos, dType);
            else return PlayLazer(index, pos, dType);
        }
        public float PlayLazer(int index, Vector2 pos, DirectionType type)
        {
            if (IsEffectInValidIndex(index, type)) return -1f;

            var effectTransform = type == DirectionType.Horizontal ? _horizontalEffect[index] : _verticalEffect[index];

            effectTransform.position = pos;
            effectTransform.gameObject.SetActive(true);
            var system = effectTransform.GetComponentInChildren<ParticleSystem>();
            system.Play();
            if (TalkingEventManager.Instance._isElectricFirstCasting)
            {
                TalkingEventManager.Instance.InvokeCurrentEvent(new BossPhaseAndDescriptionEvent("FirstElectricAttack")).Forget();
                TalkingEventManager.Instance._isElectricFirstCasting = false;
            }
            return system.main.duration;
        }
        public float PlayDanger(int index, Vector2 pos, DirectionType type)
        {
            if (IsDangerInValidIndex(index, type)) return -1f;

            var effectTransform = type == DirectionType.Horizontal ? _horizontalDanger[index] : _verticalDanger[index];

            effectTransform.position = pos;
            effectTransform.gameObject.SetActive(true);
            var system = effectTransform.GetComponentInChildren<ParticleSystem>();
            system.Play();
            return  system.main.startLifetimeMultiplier;
        }
    }
}
