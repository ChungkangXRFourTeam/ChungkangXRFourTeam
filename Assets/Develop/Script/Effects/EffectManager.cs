using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using XRProject.Utils.Log;
using UnityEngine.SceneManagement;
public class EffectManager : MonoBehaviour
{
    private static EffectManager _inst;

    public const string LOG_SIGNATURE = "effectManager";
    private const string B_LOG_SIGNATURE = "effectManager";
    
    public static void Init()
    {
        if (_inst)
        {
            Destroy(_inst.gameObject);
        }
        
        var obj = new GameObject("EffectManager/");
        _inst = obj.AddComponent<EffectManager>();
        DontDestroyOnLoad(obj);

        _inst.PoolAlloc();
    }

    private void PoolAlloc()
    {
        Debug.Log("alloc");
        if(_resTable == null)
            _resTable = Resources.Load<EffectTableSet>("EffectTableSet")?.TableList ?? new List<EffectTable>();
        
        _scheduler = new EffectScheduler();
        
        _pool = new EffectPool(_resTable);
        transform.position = Vector3.zero;
        transform.rotation = quaternion.identity;
        transform.localScale = Vector3.one;
    }
    
    private EffectPool _pool;
    private List<EffectTable> _resTable;
    private EffectScheduler _scheduler;

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

        _inst.durationList.Clear();
        int childCount = item.EffectObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var p = item.ParticleSystem;
            item.ApplyCommand(ref command);

            p.Play();
            _inst.durationList.Add(p.main.duration);
        }
        
        item.IsEnabled = true;
        command.OnBeginCallback?.Invoke(item);

        var callback = command.OnComplationCallback;
        _inst._scheduler.Schedule(_inst.durationList.Max(), () =>
        {
            callback?.Invoke(item);
            _inst._pool.Push(item);
        });
    }

    private void Update()
    {
        _scheduler.Update();
    }
}