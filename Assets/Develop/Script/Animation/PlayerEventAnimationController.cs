using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventAnimationName
{
    public const string IDLE = "Idle";
    public const string DEATH = "Death";
    public const string FALLING_DASH = "FallingDash";
    public const string FALLING_LAND = "FallingLand";
    public const string RUN = "Run";
}
public class PlayerEventAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController _playerAnimationController;
    [SerializeField] private RuntimeAnimatorController _plableAnimatorController;
    [SerializeField] private RuntimeAnimatorController _eventAnimatorController;
    [SerializeField] private Animator _animator;
    [SerializeField] private string state;

    public void EnableEventAnimatorController()
    {
        _playerAnimationController.enabled = false;
        _animator.runtimeAnimatorController = _eventAnimatorController;

        state = _animator.runtimeAnimatorController.name;
    }
    
    public void DisableEventAnimatorController()
    {
        _playerAnimationController.enabled = true;
        _animator.runtimeAnimatorController = _plableAnimatorController;
    }

    public void PlayEventAnim(string state)
    {
        if (_animator.runtimeAnimatorController == _eventAnimatorController)
        {
            _animator.Play(state);
        }
    }
}
