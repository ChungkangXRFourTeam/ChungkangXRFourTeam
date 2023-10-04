using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ButtonObject : MonoBehaviour, IBObjectInteractive
{
    [System.Serializable]
    public enum EButtonType
    {
        Auto,
        Toggle,
        Once
    }
    
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private EButtonType _buttonType;
    [SerializeField] private bool _awakeEnable = true;
    [SerializeField] private float _spriteRestoreDuration;
    [SerializeField] private Sprite _disabledSprite;
    [SerializeField] private Color _disabledColor;

    [SerializeField] private UnityEvent<ButtonObject> _enableEvent;
    [SerializeField] private UnityEvent<ButtonObject> _disableEvent;

    private bool _isEnabled;
    private Sprite _enabledSprite;
    private Color _enabledColor;
    private bool _onceTrigger;

    public bool Lock { get; set; }   
    public InteractionController Interaction { get; private set; }

    
    private void Awake()
    {
        Interaction = GetComponentInChildren<InteractionController>();
        if (!_renderer)
            _renderer = GetComponent<SpriteRenderer>();

        _isEnabled = _awakeEnable;
        _enabledColor = _renderer.color;
        _enabledSprite = _renderer.sprite;

        if (_awakeEnable)
        {
            EnableButton();
        }
        else if(_buttonType != EButtonType.Auto)
        {
            DisableButton();
        }
        else
        {
            _isEnabled = true;
        }
    }

    private void EnableButton()
    {
        _isEnabled = true;
        _renderer.sprite = _enabledSprite;
        _renderer.color = _enabledColor;
        _disableEvent.Invoke(this);
    }

    private void DisableButton()
    {
        _isEnabled = false;
        if (_disabledSprite)
            _renderer.sprite = _disabledSprite;
        _renderer.color = _disabledColor;
        _enableEvent.Invoke(this);
    }

    private IEnumerator CoOnCounting()
    {
        DisableButton();
        yield return new WaitForSeconds(_spriteRestoreDuration);
        EnableButton();
    }

    private void Action()
    {
        if (_buttonType == EButtonType.Toggle)
        {
            if (_isEnabled)
            {
                DisableButton();
            }
            else
            {
                EnableButton();
            }
        }
        else if(_buttonType == EButtonType.Auto && _isEnabled)
        {
            StartCoroutine(CoOnCounting());
        }
        else if (_buttonType == EButtonType.Once && !_onceTrigger)
        {
            _onceTrigger = true;
            DisableButton();
        }
    }
    public void Activate(object state)
    {
        if (Lock) return;
        if (state is not ClickContractInfo info) return;
        if (info.clickType != EClickContractType.OneClick) return;
        if (info.MouseButtonNumber != 0) return;
        
        Action();
    }

    public bool IsSelectiveObject { get; }
}