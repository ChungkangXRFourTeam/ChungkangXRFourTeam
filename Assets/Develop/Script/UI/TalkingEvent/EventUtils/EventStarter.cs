using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EventStarter : MonoBehaviour
{
    [SerializeField] private string _eventName;
    [SerializeField] private CanvasGroup _fade;
    [SerializeField] private BoxCollider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _fade = GameObject.FindWithTag("Fade").GetComponent<CanvasGroup>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EventFadeChanger.Instance.ChangeFadeObject(_fade);
        if(TalkingEventManager._isEventEnd)
        switch (_eventName)
        {
            case "TutorialCutScene":
                TalkingEventManager.Instance.InvokeCurrentEvent(new TutorialCutscene()).Forget();
                break;
            case "Description 1" :
                TalkingEventManager.Instance.InvokeCurrentEvent(new DescriptionEvent()).Forget();
                break;
            case "Description 2" :
                TalkingEventManager.Instance.InvokeCurrentEvent(new DescriptionEvent2()).Forget();
                break;
            case "Description 3":
                TalkingEventManager.Instance.InvokeCurrentEvent(new DescriptionEvent3()).Forget();
                break;
            case "Description 4":
                TalkingEventManager.Instance.InvokeCurrentEvent(new DescriptionEvent4()).Forget();
                break;
            case "Description 5":
                TalkingEventManager.Instance.InvokeCurrentEvent(new DescriptionEvent5()).Forget();
                break;
            case "ThemeA_1": 
            case "ThemeA_2":
            case "ThemeA_3":
            case "ThemeB_1":
            case "ThemeB_2":
            case "ThemeB_3":
            case "Boss":
                TalkingEventManager.Instance.InvokeCurrentEvent(new MountKennelEvent(_eventName)).Forget();
                break;
            case "BossLanding":
                TalkingEventManager.Instance.InvokeCurrentEvent(new LandingKennelBossEvent()).Forget();
                break;
            case "Landing":
                TalkingEventManager.Instance.InvokeCurrentEvent(new LandingKennelEvent()).Forget();
                break;
            case "Ending":
                TalkingEventManager.Instance.InvokeCurrentEvent(new EndingEvent()).Forget();
                break;
        }

        _collider.enabled = false;
    }
    
    
    
    
}
