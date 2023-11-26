using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class KennelInteraction : MonoBehaviour
{
    private InputAction _interactionAction;

    private void Start()
    {
        _interactionAction = InputManager.GetMainGameAction("MountKennel");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_interactionAction.WasPressedThisFrame() && other.CompareTag("Player")
            && TalkingEventManager._isEventEnd)
        {
            TalkingEventManager.Instance.InvokeCurrentEvent(new MountKennelEvent("ThemeA_1")).Forget();
            
        }
    }
    
}
