using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
            string nextStage = String.Empty;
            switch (SceneManager.GetActiveScene().name)
            {
                case "Tutorial":
                case "Canel":
                    nextStage = "ThemeA_1";
                    break;
                case "ThemeA_1" :
                    nextStage = "ThemeA_2";
                    break;
                case "ThemeA_2" :
                    nextStage = "ThemeA_3";
                    break;
                case "ThemeA_3" :
                    nextStage = "ThemeB_1";
                    break;
                case "ThemeB_1" :
                    nextStage = "ThemeB_2";
                    break;
                case "ThemeB_2" :
                    nextStage = "ThemeB_3";
                    break;
                case "ThemeB_3" :
                    nextStage = "Boss";
                    break;
                    
            }
            TalkingEventManager.Instance.InvokeCurrentEvent(new MountKennelEvent(nextStage)).Forget();
            
        }
    }
    
}
