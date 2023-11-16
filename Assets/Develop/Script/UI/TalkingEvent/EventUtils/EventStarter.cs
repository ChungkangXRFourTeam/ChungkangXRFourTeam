using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EventStarter : MonoBehaviour
{
    [SerializeField] private string _eventName;
    void Start()
    {
        Debug.Log(22);
        TalkingEventManager.Instance.InvokeCurrentEvent(new TutorialCutscene()).Forget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
