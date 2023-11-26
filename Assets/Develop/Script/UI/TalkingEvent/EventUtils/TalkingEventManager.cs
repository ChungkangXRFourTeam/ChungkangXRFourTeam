using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
public class TalkingEventManager : MonoBehaviour
{
    public static bool _isEventEnd;
    private static TalkingEventManager _instance = null;

    
    public static TalkingEventManager Instance
    {
        get
        {
            if (null == _instance)
            {
                return null;
            }
            return _instance;
        }
    }

   public static void Init()
    {
        if (_instance)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }

        _instance = new GameObject("[FadeEventChanger]").AddComponent<TalkingEventManager>();
        DontDestroyOnLoad(_instance.gameObject);
        _isEventEnd = true;
    }
    
    public async UniTask InvokeCurrentEvent(ITalkingEvent sceneEvent)
    {
            if (sceneEvent.IsInvalid() && _isEventEnd)
            {
                _isEventEnd = false;
        
                await sceneEvent.OnEventBefore();
                await sceneEvent.OnEventStart();
                await sceneEvent.OnEvent();
                await sceneEvent.OnEventEnd();
            }
        
            _isEventEnd = true; 
    }
    
    

    
}
