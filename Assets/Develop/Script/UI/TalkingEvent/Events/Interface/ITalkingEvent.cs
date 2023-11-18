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
    public UniTask OnEventBefore();
    public UniTask OnEvent();
    public UniTask OnEventEnd();
    public UniTask OnEventStart();

    public bool IsInvalid();

}