using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingPanel : MonoBehaviour, IUIController
{
    private UIManager _manager;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _manager = GameObject.Find("UIManager")?.GetComponent<UIManager>();
    }

    public bool IsEnabled
    {
        get => gameObject.activeSelf;
        set
        {
            if (value)
            {
                Activate();
            }
            else
            {
                DeActivate();
            }
        }
    }
    public void Activate()
    {
        gameObject.SetActive(true);
        InputManager.Instance?.DisableMainGameAction();
    }

    public void DeActivate()
    {
        gameObject.SetActive(false);
        InputManager.Instance?.InitMainGameAction();
    }
    
    // Unity Events Functions
    public void OnClickDiaryButton()
    {
        _manager.PushDiary();
    }
}
