using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using XRProject.Utils.Log;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _inst;

    public const string LOG_SIGNATURE = "effectManager";
    private const string B_LOG_SIGNATURE = "effectManager";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (_inst == null)
        {
            var obj = new GameObject("EffectManager/");
            _inst = obj.AddComponent<EffectManager>();
            DontDestroyOnLoad(obj);

            var list = Resources.Load<EffectTableSet>("EffectTableSet")?.TableList ?? new List<EffectTable>();
            _inst._pool = new EffectPool(list);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }
    }

    private void OnDestroy()
    {
        _inst = null;
    }

    private EffectPool _pool;

    public static void EnqueueCommand(EffectCommand command)
    {
        if (!_inst) return;
    }

    public static void ImmediateCommand(EffectCommand command)
    {
        if (!_inst) return;

        Invoke(ref command);
    }

    [CanBeNull]
    public static EffectItem EffectItem(string effectKey)
    {
        if (string.IsNullOrEmpty(effectKey) || string.IsNullOrWhiteSpace(effectKey))
        {
            XLog.LogError($"EffectItem: 유효하지 않는 effectKey('{effectKey}')", EffectManager.LOG_SIGNATURE);
            return null;
        }

        var str = effectKey.Split('/');
        if (str == null || str.Length < 2)
        {
            XLog.LogError($"EffectItem: 유효하지 않는 effectKey('{effectKey}')", EffectManager.LOG_SIGNATURE);
            return null;
        }
        
        var effect = _inst._pool.Pop(effectKey);

        return effect;
    }

    List<float> durationList = new List<float>();

    private static void Invoke(ref EffectCommand command)
    {
        EffectItem item = _inst._pool.Pop(command.EffectKey);

        if (item == null)
        {
            XLog.LogError($"{B_LOG_SIGNATURE}: 유효하지 않은 EffectKey('{command.EffectKey}') 입니다.", LOG_SIGNATURE);
            return;
        }

        item.IsEnabled = true;
        _inst.durationList.Clear();
        int childCount = item.EffectObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var p = item.ParticleSystem;
            item.ApplyCommand(ref command);

            p.Play();
            _inst.durationList.Add(p.main.duration);
        }

        Sequence s = DOTween.Sequence();
        var callback = command.OnComplationCallback;
        s.SetDelay(_inst.durationList.Max()).OnComplete(() =>
        {
            _inst._pool.Push(item);
            callback?.Invoke();
        });
    }
}