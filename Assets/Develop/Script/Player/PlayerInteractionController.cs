using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private float _clickDistance;
    [SerializeField] private bool _debug;

    private void CheckMouseClickDetection()
    {
        int mouse = -1;
        if (Input.GetMouseButtonDown(0))
            mouse = 0;
        if (Input.GetMouseButtonDown(1))
            mouse = 1;
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
}