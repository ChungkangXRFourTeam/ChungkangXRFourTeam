using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IUIController
{
    public bool IsEnabled { get; set; }
    public void Activate();
    public void DeActivate();
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private string _settingKey;
    [SerializeField] private string _diaryKey;

    private Dictionary<string, IUIController> _viewTable;
    private Stack<IUIController> _stack;
    
    private void Start()
    {
        _viewTable = new();
        _stack = new();
        AddTable(_settingKey);
        AddTable(_diaryKey);

        InputManager.RegisterActionToUI("Settings", OnKeyInput, ActionType.Started);
    }

    public T GetUIController<T>(string key) where T : class, IUIController
    {
        if (_viewTable.TryGetValue(key, out var com)) return com as T;
        return null;
    }
    
    private void AddTable(string key)
    {
        var obj = GameObject.Find(key);
        if (obj == false) return;
        if (obj.TryGetComponent<IUIController>(out var com) == false) return;
            
        _viewTable.Add(key, com);
        com.DeActivate();
    }
    
    private void OnDestroy()
    {
        InputManager.UnRegisterActionToUI("Settings", OnKeyInput, ActionType.Started);
    }

    private void OnKeyInput(InputAction.CallbackContext context)
    {
        if (_stack.Count == 0)
        {
            if (_viewTable.TryGetValue(_settingKey, out var controller))
            {
                Push(controller);
            }
        }
        else
        {
            Clear();
        }
    }

    public void PushDiary()
    {
        if (_viewTable.TryGetValue(_diaryKey, out var diary))
        {
            Push(diary);
        }
    }

    public void Clear()
    {
        while (Empty() == false)
        {
            Pop();
        }
    }
    
    private void Push(IUIController controller)
    {
        if(Empty() == false)
            _stack.Peek().DeActivate();
        
        InputManager.Instance?.DisableMainGameAction();
        
        _stack.Push(controller);
        controller.Activate();
    }

    private IUIController Pop()
    {
        var com = _stack.Pop();

        if (Empty() == false)
        {
            _stack.Peek().Activate();
            InputManager.Instance?.InitMainGameAction();
        }
        
        com.DeActivate();
        return com;
    }

    public bool Empty() => _stack.Count == 0;


}