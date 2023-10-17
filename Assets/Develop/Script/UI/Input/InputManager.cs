using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance = null;
    private static InputActionListener actionListener = null;
    
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            actionListener = new InputActionListener();
            
            actionListener.MainGame.Move.Enable();
            actionListener.MainGame.Attack.Enable();
            actionListener.MainGame.Grab.Enable();
            actionListener.MainGame.Jump.Enable();
            actionListener.MainGame.BoundMode.Enable();
            actionListener.MainGame.Fall.Enable();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
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

    public static InputActionListener ActionListener
    {
        get
        {
            if (actionListener != null) 
                return actionListener;
            return null;
        }
    }
}
