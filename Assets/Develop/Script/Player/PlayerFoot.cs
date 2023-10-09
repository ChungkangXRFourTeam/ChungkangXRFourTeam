using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFoot : MonoBehaviour
{
    public event Action<bool> OnChangeIsGround;

    [SerializeField]
    private bool _isGrounded;
    public bool IsGrounded => _isGrounded;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall") ||
            other.gameObject.CompareTag("KnockbackObject"))
        {
            _isGrounded = true;
            OnChangeIsGround?.Invoke(_isGrounded);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall") ||
            other.gameObject.CompareTag("KnockbackObject"))
        {
            _isGrounded = true;
            OnChangeIsGround?.Invoke(_isGrounded);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall") ||
            other.gameObject.CompareTag("KnockbackObject"))
        {
            _isGrounded = false;
            OnChangeIsGround?.Invoke(_isGrounded);
        }
    }
}
