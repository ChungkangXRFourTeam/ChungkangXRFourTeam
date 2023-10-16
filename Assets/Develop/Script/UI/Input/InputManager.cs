using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class InputManager : MonoBehaviour
{
    private InputActionListener actionListener;
    
    [SerializeField] private List<NamedInputAction> actions;

    // Start is called before the first frame update

    private void Awake()
    {
        actionListener = new InputActionListener();
        actions.Clear();
        for (int i = 0; i < actionListener.GetMainGameActions().Count; i++)
        {
            InputAction action = actionListener.GetMainGameActions()[i];
            NamedInputAction namedAction = new NamedInputAction();
            namedAction.elementName = action.name;
            namedAction.action = action;
            actions.Add(namedAction);
        }
    }

    private void OnEnable()
    {
        
    }
}
