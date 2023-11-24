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
        }

        _collider.enabled = false;
    }
    
    
}
