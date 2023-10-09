using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;



public class ObjectContractInfo : BaseContractT<IObjectBehaviour, ObjectContractInfo>
{
    
}
public class ActorContractInfo : BaseContractT<IActorBehaviour, ActorContractInfo>
{
    
}
public class ClickContractInfo: BaseContractInfo
{
    public int MouseButtonNumber;
    public EClickContractType clickType;
}

public enum EClickContractType
{
    OneClick,
    DoubleClick,
    Pressed
}



public abstract class BaseContractInfo
{
    public bool IsAlive => Check();
    
    protected Func<bool> _destroyChecker;
    private bool Check()
    {
        if (_destroyChecker == null)
            return true;

        bool flag = _destroyChecker();
        if (flag) _destroyChecker = null;

        return !flag;
    }
    [NotNull] public Transform Transform { get; protected set; }
}

public abstract class BaseContractT<TBASE, TCLASS> : BaseContractInfo
    where TBASE : class 
    where TCLASS : BaseContractT<TBASE, TCLASS>, new()
{

    private Dictionary<Type, TBASE> _table = new();

    protected BaseContractT()
    {
    }

    public static TCLASS Create(Transform transform, Func<bool> destroyChecker) =>
        new() { Transform = transform, _destroyChecker = destroyChecker };

    public TCLASS AddBehaivour<T>(TBASE behaviour) where T : class, TBASE
    {
        if(behaviour is null) throw new ArgumentNullException("argument is null");
        if (behaviour is not T) throw new ArgumentException($"[{behaviour.GetType().Name}] is not [{typeof(T).Name}].");

        if (!_table.TryAdd(typeof(T), behaviour))
        {
            throw new ArgumentException($"[{typeof(T).Name}] is already exist");
        }
        
        return this as TCLASS;
    }
    
    public T GetBehaviourOrNull<T>() where T : class, TBASE
    {
        if (_table.TryGetValue(typeof(T), out var v))
        {
            return v as T;
        }
        
        return null;
    }

    public bool TryGetBehaviour<T>(out T value) where T : class, TBASE
    {
        value = GetBehaviourOrNull<T>();

        return value != null;
    }
}