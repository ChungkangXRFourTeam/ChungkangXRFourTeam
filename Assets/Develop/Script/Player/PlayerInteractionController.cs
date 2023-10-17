using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private float _clickDistance;
    [SerializeField] private bool _debug;
    private int mouse;
    private void Awake()
    {
        InputManager.ActionListener.MainGame.Grab.started += OnRightClick;
        InputManager.ActionListener.MainGame.Grab.canceled += ExitClick;
        InputManager.ActionListener.MainGame.Attack.started += OnLeftClick;
        InputManager.ActionListener.MainGame.Attack.canceled += ExitClick;
        
    }

    private void CheckMouseClickDetection()
    {
        if (mouse == -1) return;


        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        var hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);

        foreach (var hit in hits)
        {
            if (Vector2.Distance(hit.collider.transform.position, transform.position) > _clickDistance) continue;
            
            if (hit.collider.TryGetComponent<InteractionController>(out var obj))
            {
                obj.Activate(new ClickContractInfo()
                {
                    clickType = EClickContractType.OneClick,
                    MouseButtonNumber = mouse
                });
            }
        }
    }
    
    private void Update()
    {
        CheckMouseClickDetection();
    }
    private void OnDrawGizmos()
    {
        if (!_debug) return;
        
        Gizmos.DrawWireSphere(transform.position, _clickDistance);
    }

    void OnRightClick(InputAction.CallbackContext ctx)
    {
        mouse = 1;
    }

    void OnLeftClick(InputAction.CallbackContext ctx)
    {
        mouse = 0;
    }

    void ExitClick(InputAction.CallbackContext ctx)
    {
        mouse = -1;
    }
    
}