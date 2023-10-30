using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private bool isMoved;
    // Start is called before the first frame update
    private void Awake()
    { 
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        InputAction moveAction = InputManager.GetMainGameAction("Move");
        if (moveAction != null && moveAction.IsPressed())
        {
            _animator.SetBool("isMoved",true);
        }
        else
        {
            _animator.SetBool("isMoved",false);
        }

        var x = moveAction?.ReadValue<Vector2>().x ?? 0f;
        bool flipX = x > 0f;
        if (TryGetComponent<SpriteRenderer>(out var renderer))
            renderer.flipX = flipX;
    }
}
