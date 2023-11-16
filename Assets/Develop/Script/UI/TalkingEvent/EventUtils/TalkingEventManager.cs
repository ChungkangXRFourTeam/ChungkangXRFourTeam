using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
public class TalkingEventManager : MonoBehaviour
{
    private bool _isEventEnd;
    private static TalkingEventManager instance = null;
    private Dictionary<string, ITalkingEvent> _sceneEvents;
    public UnityEvent _sceneEvent;
    
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
                
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        _sceneEvents = new Dictionary<string, ITalkingEvent>();
    }
    

    public static TalkingEventManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    
    public async UniTask InvokeCurrentEvent(ITalkingEvent sceneEvent)
    {
        
            if (sceneEvent.IsInvalid())
            {
                _isEventEnd = false;
        
                await sceneEvent.OnEventBefore();
                await sceneEvent.OnEventStart();
                await sceneEvent.OnEvent();
                await sceneEvent.OnEventEnd();
            }
        
            _isEventEnd = true; 
    }
    
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Tutorial":
                _sceneEvents.TryAdd("StartTutorial", new TutorialCutscene());
                break;
        }
    }

    
}
