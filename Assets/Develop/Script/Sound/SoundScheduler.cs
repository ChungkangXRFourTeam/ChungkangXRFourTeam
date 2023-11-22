
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Utils.Log;

internal class SoundScheduler : MonoBehaviour
{
    private Stack<SoundScheduleItem> _unusedPool = new();
    private LinkedList<SoundScheduleItem> _scheduledList = new();
    private LinkedList<SoundScheduleItem> _pendingList = new();

    internal void Init(int defaultPoolSize = 5)
    {
        for (int i = 0; i < defaultPoolSize; i++)
        {
            _unusedPool.Push(new SoundScheduleItem(gameObject));
        }
    }

    internal void Release()
    {
        foreach (var item in _unusedPool)
        {
            item.Release();
        }
        foreach (var item in _scheduledList)
        {
            item.Release();
        }
        foreach (var item in _pendingList)
        {
            item.Release();
        }

        _unusedPool = null;
        _scheduledList = null;
        _pendingList = null;
    }
    private void TryReAllocPool()
    {
        if (_unusedPool.Count > 0) return;
        
        for (int i = 0; i < (_scheduledList.Count + _pendingList.Count) * 0.5; i++)
        {
            _unusedPool.Push(new SoundScheduleItem(gameObject));
        }
    }
    
    private void PushPool(SoundScheduleItem item)
    {
        _unusedPool.Push(item);
    }

    private SoundScheduleItem PopPool()
    {
        TryReAllocPool();
        
        var item = _unusedPool.Pop();
        item.Reset();
        
        return item;
    }

    private void PushSchedule(SoundScheduleItem item)
    {
        _scheduledList.AddLast(item);
    }
    private void PushPending(SoundScheduleItem item)
    {
        _pendingList.AddLast(item);
    }

    internal void Schedule(SoundCommand command)
    {
        var item = PopPool();

        item.Set(command);

        PushPending(item);
    }

    private void LateUpdate()
    {
        LinkedListNode<SoundScheduleItem> currentNode = null;
        

        currentNode = _pendingList.First;

        while (currentNode != null)
        {
            var item = currentNode.Value;
            
            if (item.pendingTimer >= item.Duration)
            {
                var next = currentNode.Next;
                _pendingList.Remove(currentNode);
                currentNode = next;
                
                item.Play();
                
                PushSchedule(item);
            }
            else
            {
                item.pendingTimer += Time.deltaTime;
                currentNode = currentNode.Next;
            }
        }
        currentNode = _scheduledList.First;
        
        while (currentNode != null)
        {
            var item = currentNode.Value;
            if (item.scheduleTimer >= item.Length)
            {
                var next = currentNode.Next;
                _scheduledList.Remove(currentNode);
                currentNode = next;
                
                item.Reset();
                
                PushPool(item);
            }
            else
            {
                currentNode = currentNode.Next;
                item.scheduleTimer += Time.deltaTime;
                item.SetVolumeIfChanged();
            }
        }
    }
}
internal partial class SoundScheduleItem
{
    internal SoundCommand Command;
    internal float Length => Command.clip.length;
    internal float Duration => Command.Duration;

    private AudioSource _source;
    internal float scheduleTimer;
    internal float pendingTimer;

    public SoundScheduleItem(GameObject parent)
    {
        Command = new SoundCommand();
        
        _source = new GameObject("__ScheduledItem__").AddComponent<AudioSource>();
        _source.transform.SetParent(parent.transform);
    }

    public void Set(SoundCommand command)
    {
        Reset();
        this.Command = command;
    }

    public void Reset()
    {
        scheduleTimer = 0f;
        pendingTimer = 0f;
        _source.clip = null;
        Command = new SoundCommand();
    }

    public void Release()
    {
        Reset();
        
        GameObject.Destroy(_source.gameObject);
        _source = null;
    }

    public void Play()
    {
        SetProperties(ref Command, ref _source);
        _source.Play();
    }

    public void SetVolumeIfChanged()
    {
        float currentValue = SoundManager.GetSoundVolume(Command.volumeKey);
        if (_source.volume - currentValue != 0f)
            _source.volume = currentValue;
    }
}
