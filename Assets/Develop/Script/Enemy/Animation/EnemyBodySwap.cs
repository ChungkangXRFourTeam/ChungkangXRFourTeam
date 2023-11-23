using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


public class ChangeableAnimationBody : MonoBehaviour
{
    public virtual bool IsEnabled { get; }

    public virtual void SetEnable(bool isEnabled)
    {
        throw new NotImplementedException();
    }

    public virtual T GetComponentOrNull<T>() where T : Component
    {
        return GetComponent<T>();
    }
}


[System.Serializable]
public class AnimationBodyChanger
{
    [System.Serializable]
    public class KeyPair
    {
        public string Key;
        public ChangeableAnimationBody Body;

        [NonSerialized] public Component CachedComponent;
    }
    
    [SerializeField] private List<KeyPair> _pairs;

    public AnimationBodyChanger Change(string key)
    {
        foreach (var pair in _pairs)
        {
            if (pair.Body == null) continue;
            pair.Body.SetEnable(pair.Key == key);
        }

        return this;
    }

    [CanBeNull]
    public ChangeableAnimationBody GetBodyOrNull(string key)
    {
        foreach (var pair in _pairs)
        {
            if (pair.Key == key)
            {
                return pair.Body;
            }
        }

        return null;
    }

    [CanBeNull]
    public T GetBodyGetComponentOrNull<T>(string key) where T : Component 
    {
        foreach (var pair in _pairs)
        {
            if (pair.Key == key && pair.Body)
            {
                pair.CachedComponent = pair.Body.GetComponentOrNull<T>();

                return (T)pair.CachedComponent;
            }
        }

        return null;
    }
}