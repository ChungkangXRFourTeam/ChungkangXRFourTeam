using System;
using System.Collections;
using System.Collections.Generic;
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
    
    
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            actionListener = new InputActionListener();
            _mainGameActionMap = actionListener.asset.FindActionMap("MainGame");
            DontDestroyOnLoad(this.gameObject);
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

    private void InitMainGameAction()
    {
        IEnumerator<InputAction> actions = _mainGameActionMap.GetEnumerator();
        while (actions.MoveNext())
        {
                actions.Current.Enable();
            
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

    public static InputAction GetMainGameAction(string action)
    {
        return _mainGameActionMap.FindAction(action, true);
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
}
