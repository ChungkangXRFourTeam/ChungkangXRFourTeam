using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationState : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController _ani;
    [SerializeField] private PlayerFoot _foot;
    [SerializeField] private InteractionController _interaction;

    private delegate IEnumerator State();

    private State _currentState;
    private InputAction _moveAction;
    private InputAction _fallAction;
    private InputAction _jumpAction;

    public bool IsBouncing => 
        _interaction.GetContractInfoOrNull<ActorContractInfo>()
        .GetBehaviourOrNull<IBActorPhysics>()
        .IsSwingState;
    
    private void Start()
    {
        _moveAction = InputManager.GetMainGameAction("Move");
        _fallAction = InputManager.GetMainGameAction("Fall");
        _jumpAction = InputManager.GetMainGameAction("Jump");
        
        SetState(Idle);
    }

    private void SetState(State state)
    {
        StopAllCoroutines();
        StartCoroutine(state());
    }

    private IEnumerator Bouncing()
    {
        yield return null;
B:
        _ani.SetState(new PAniState()
        {
            State = EPCAniState.Bouncing,
            Rotation = Quaternion.Euler(0f, _moveAction.ReadValue<Vector2>().x > 0f ? 180f : 0f, 0f)
        });

        float timer = 0f;
        while (timer < 0.25f)
        {
            timer += Time.deltaTime;
            
            if (_fallAction.IsPressed())
            {
                SetState(Fall);
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
        
        _ani.SetState(new PAniState()
        {
            State = EPCAniState.Falling_Bouncing,
            Rotation = Quaternion.Euler(0f, _moveAction.ReadValue<Vector2>().x > 0f ? 180f : 0f, 0f)
        });

        if (_interaction.GetComponent<PlayerController>().AniKnockback)
        {
            _interaction.GetComponent<PlayerController>().AniKnockback = false;
            goto B;
        }

        yield return new WaitUntil(() => IsBouncing == false);
        _ani.SetState(new PAniState()
        {
            State = EPCAniState.Falling_Basics,
            Rotation = Quaternion.Euler(0f, _moveAction.ReadValue<Vector2>().x > 0f ? 180f : 0f, 0f)
        });

        while (_foot.IsGrounded == false)
        {
            if (_fallAction.IsPressed())
            {
                SetState(Fall);
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
        
        if (_fallAction.IsPressed())
        {
            SetState(Fall);
            yield break;
        }
        
        yield return null;
        _ani.SetState(new PAniState()
        {
            State = EPCAniState.Landing_Basics,
            Rotation = Quaternion.Euler(0f, _moveAction.ReadValue<Vector2>().x > 0f ? 180f : 0f, 0f)
        });
        
        SetState(Idle);
    }
    private IEnumerator Fall()
    {
        yield return null;

        _ani.SetState(new PAniState()
        {
            State = EPCAniState.Falling_Dash,
            Rotation = Quaternion.Euler(0f, _moveAction.ReadValue<Vector2>().x > 0f ? 180f : 0f, 0f)
        });

        while (true)
        {
            if (IsBouncing)
            {
                SetState(Bouncing);
                yield break;
            }
            
            if (_foot.IsGrounded == false)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForEndOfFrame();
                break;
            }
        }
        
        _ani.SetState(new PAniState()
        {
            State = EPCAniState.Landing_Dash,
            Rotation = Quaternion.Euler(0f, _moveAction.ReadValue<Vector2>().x > 0f ? 180f : 0f, 0f)
        });

        SetState(Idle);
    }

    private IEnumerator Run()
    {
        yield return null;
        
        while (true)
        {
            _ani.SetState(new PAniState()
            {
                State = EPCAniState.Run,
                Rotation = Quaternion.Euler(0f, _moveAction.ReadValue<Vector2>().x > 0f ? 180f : 0f, 0f),
            });
            
            if (IsBouncing)
            {
                SetState(Bouncing);
            }
            if (_fallAction.IsPressed())
            {
                SetState(Fall);
            }
            else if(_moveAction.IsPressed() == false)
            {
                SetState(Idle);
            }
            
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator Jump()
    {
        yield return null;
        
        while (true)
        {
            if (_jumpAction.triggered)
            {
                _ani.SetState(new PAniState()
                {
                    State = EPCAniState.Jump,
                    Rotation = Quaternion.Euler(0f, _moveAction.ReadValue<Vector2>().x > 0f ? 180f : 0f, 0f)
                });
            }
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator Idle()
    {
        yield return null;
        
        _ani.SetState(new PAniState()
        {
            State = EPCAniState.Idle
        });
        
        while (true)
        {
            if (IsBouncing)
            {
                SetState(Bouncing);
            }
            if (_fallAction.IsPressed())
            {
                SetState(Fall);
            }
            else if (_moveAction.IsPressed())
            {
                SetState(Run);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
