using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public enum EventTextType
{
    ID,
    Target,
    Content,

}

public enum TalkerTarget
{
    Player,
    Observer,
    Boss,
    Enemy,
    
}
public interface ITalkingEvent
{
    public async UniTask OnEventBefore()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
    }
    public async UniTask OnEvent()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
    }

    public async UniTask OnEventEnd()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
    }

    public async UniTask OnEventStart()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
    }

}
