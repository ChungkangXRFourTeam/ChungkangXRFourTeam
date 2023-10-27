using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct EffectSchedulerItem
{
    public float Duration { get; set; }
    public Action Completion { get; set; }
    public float CurrentTime { get; private set; }

    public bool UpdateAndExecute(float deltaTime)
    {
        CurrentTime += deltaTime;
        if (CurrentTime > Duration)
        {
            Completion();
            return true;
        }

        return false;
    }
}

public class EffectScheduler
{
    private List<EffectSchedulerItem> _alives = new List<EffectSchedulerItem>();
    private List<EffectSchedulerItem> _swap = new List<EffectSchedulerItem>();
    
    public void Schedule(float duration, Action completion)
    {
        var item = new EffectSchedulerItem() { Duration = duration, Completion = completion };
        _alives.Add(item);
    }

    public void Update()
    {
        float deltaTime = Time.deltaTime;
        foreach (var item in _alives)
        {
            if (!item.UpdateAndExecute(deltaTime))
            {
                _swap.Add(item);
            }
        }
        
        (_swap, _alives) = (_alives, _swap);
        _swap.Clear();
    }
}
