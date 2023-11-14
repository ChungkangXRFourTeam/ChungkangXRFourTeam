using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ActionType
{
    Started,
    Performed,
    Canceled
}
public class InputManager : MonoBehaviour
{
    private static InputManager instance = null;
    private static InputActionListener actionListener = null;
    private static InputActionMap _mainGameActionMap = null;
    private static InputActionMap _eventTalkMap = null;
    
    
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            actionListener = new InputActionListener();
            _mainGameActionMap = actionListener.asset.FindActionMap("MainGame");
            _eventTalkMap = actionListener.asset.FindActionMap("TalkEvent");
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void Start()
    {
        InitMainGameAction();
    }

    public void InitMainGameAction()
    {
        IEnumerator<InputAction> actions = _mainGameActionMap.GetEnumerator();
        while (actions.MoveNext())
        {
                actions.Current.Enable();
            
        }
            
    }

    public void DisableMainGameAction()
    {
        IEnumerator<InputAction> actions = _mainGameActionMap.GetEnumerator();
        while (actions.MoveNext())
        {
            actions.Current.Disable();
            
        }
    }
    
    public void InitTalkEventAction()
    {
        IEnumerator<InputAction> actions = _eventTalkMap.GetEnumerator();
        while (actions.MoveNext())
        {
            actions.Current.Enable();
            
        }
            
    }
    
    public void DisableTalkEventAction()
    {
        IEnumerator<InputAction> actions = _eventTalkMap.GetEnumerator();
        while (actions.MoveNext())
        {
            actions.Current.Disable();
            
        }
    }

    
    public static InputManager Instance
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

    public InputActionListener ActionListener
    {
        get
        {
            if (actionListener != null) 
                return actionListener;
            return null;
        }
    }

    [CanBeNull]
    public static InputAction GetMainGameAction(string action)
    { 
        InputAction foundAction = _mainGameActionMap.FindAction(action);
        if (Application.isPlaying && foundAction != null)
            return foundAction;
        else
        {
            return null;
        }
    }

    [CanBeNull]
    public static InputAction GetTalkEventAction(string action)
    { 
        InputAction foundAction = _eventTalkMap.FindAction(action);
        if (Application.isPlaying && foundAction != null)
            return foundAction;
        else
        {
            return null;
        }
    }
    public static void RegisterActionToMainGame(string actionName,Action<InputAction.CallbackContext> callback, ActionType actionType)
    {
        InputAction foundAction = GetMainGameAction(actionName);
        
        switch (actionType)
        {
            case ActionType.Started:
                foundAction.started += callback;
                break;
            case ActionType.Performed:
                foundAction.performed += callback;
                break;
            case ActionType.Canceled:
                foundAction.canceled += callback;
                break;
        }

    }
    public static void UnRegisterActionToMainGame(string actionName, Action<InputAction.CallbackContext> callback, ActionType actionType)
    {
        InputAction foundAction = GetMainGameAction(actionName);
        
        switch (actionType)
        {
            case ActionType.Started:
                foundAction.started -= callback;
                break;
            case ActionType.Performed:
                foundAction.performed -= callback;
                break;
            case ActionType.Canceled:
                foundAction.canceled -= callback;
                break;
        }

    }
    
    public static void RegisterActionToTalkEvent(string actionName,Action<InputAction.CallbackContext> callback, ActionType actionType)
    {
        InputAction foundAction = GetTalkEventAction(actionName);
        
        switch (actionType)
        {
            case ActionType.Started:
                foundAction.started += callback;
                break;
            case ActionType.Performed:
                foundAction.performed += callback;
                break;
            case ActionType.Canceled:
                foundAction.canceled += callback;
                break;
        }

    }
    public static void UnRegisterActionToTalkEvent(string actionName, Action<InputAction.CallbackContext> callback, ActionType actionType)
    {
        InputAction foundAction = GetTalkEventAction(actionName);
        
        switch (actionType)
        {
            case ActionType.Started:
                foundAction.started -= callback;
                break;
            case ActionType.Performed:
                foundAction.performed -= callback;
                break;
            case ActionType.Canceled:
                foundAction.canceled -= callback;
                break;
        }

    }
}
