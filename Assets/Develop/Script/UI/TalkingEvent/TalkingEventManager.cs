using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TalkingEventManager : MonoBehaviour
{
    private bool _isEventEnd;
    [SerializeField]
    private GameObject _player;
    public ITalkingEvent _enabledEvent;
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }


    private void Start()
    {
        _enabledEvent = new TalkingEvent();
        InvokeCurrentEvent().Forget();
    }

    private void Update()
    {
        if(_player.TryGetComponent(out PlayerController controller))
        {
            if (!_isEventEnd)
            {
                controller.enabled = false;
            }
            else
            {
                controller.enabled = true;
            }
        }
    }

    public async UniTask InvokeCurrentEvent()
    {
        if (_enabledEvent != null)
        {
            _isEventEnd = false;
        
            await _enabledEvent.OnEventBefore();
            await _enabledEvent.OnEventStart();
            await _enabledEvent.OnEvent();
            await _enabledEvent.OnEventEnd();
        
            _isEventEnd = true;
            _enabledEvent = null;
        }
    }
}
