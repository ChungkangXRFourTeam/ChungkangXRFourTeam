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
    private InputAction _grabAction;
    private InputAction _swingAction;

    private Rigidbody2D _rigid;
    private PlayerController _playerController;

    public bool IsFall => _foot.IsGrounded == false && _fallAction.IsPressed();
    public bool IsBouncing => 
        _interaction.GetContractInfoOrNull<ActorContractInfo>()
        .GetBehaviourOrNull<IBActorPhysics>()
        .IsSwingState;

    public bool IsJump => _foot.IsGrounded == false && _jumpAction.IsPressed();
    public bool IsGrab { get; set; }
    private bool _lateGrabState = false;
    private void Start()
    {
        _moveAction = InputManager.GetMainGameAction("Move");
        _fallAction = InputManager.GetMainGameAction("Fall");
        _jumpAction = InputManager.GetMainGameAction("Jump");
        _grabAction = InputManager.GetMainGameAction("Grab");
        _swingAction = InputManager.GetMainGameAction("Swing");

        _rigid = _interaction.GetComponent<Rigidbody2D>();
        _playerController = _interaction.GetComponent<PlayerController>();

        _grabAction.started += OnGrabAction;
        _swingAction.canceled += OnSwingAction;
        
        SetState(Idle);
    }

    private void LateUpdate()
    {
        if (_lateGrabState)
        {
            _lateGrabState = false;
            IsGrab = _playerController.GrabState;
        }
    }

    private void OnGrabAction(InputAction.CallbackContext ctx)
    {
        _lateGrabState = true;
    }
    private void OnSwingAction(InputAction.CallbackContext ctx) =>IsGrab = false;

    private void OnDestroy()
    {
        _grabAction.started -= OnGrabAction;
        _swingAction.canceled -= OnSwingAction;
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

        float timer = 0f;
        do
        {
            _ani.SetState(new PAniState()
            {
                State = EPCAniState.Bouncing,
                Rotation = Quaternion.Euler(0f, _rigid.velocity.x > 0f ? 180f : 0f, 0f)
            });
            timer += Time.deltaTime;

            if (IsGrab)
            {
                SetState(Throw);
                yield break;
            }
            if (IsFall)
            {
                SetState(Fall);
                yield break;
            }

            yield return new WaitForEndOfFrame();
        } while (timer <= 0.25f);
        
        _ani.SetState(new PAniState()
        {
            State = EPCAniState.Falling_Bouncing,
            Rotation = Quaternion.Euler(0f, _rigid.velocity.x > 0f ? 180f : 0f, 0f)
        });

        if (_interaction.GetComponent<PlayerController>().AniKnockback)
        {
            _interaction.GetComponent<PlayerController>().AniKnockback = false;
            goto B;
        }

        yield return new WaitUntil(() => IsBouncing == false);

        do
        {
            _ani.SetState(new PAniState()
            {
                State = EPCAniState.Falling_Basics,
                Rotation = Quaternion.Euler(0f, _rigid.velocity.x > 0f ? 180f : 0f, 0f)
            });
            if (IsGrab)
            {
                SetState(Throw);
                yield break;
            }
            if (IsFall)
            {
                SetState(Fall);
                yield break;
            }

            yield return new WaitForEndOfFrame();
        } while (_foot.IsGrounded == false);
        
        if (IsGrab)
        {
            SetState(Throw);
            yield break;
        }
        if (IsFall)
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
            if (IsGrab)
            {
                SetState(Throw);
                yield break;
            }
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
            
            if (IsGrab)
            {
                SetState(Throw);
            }
            else if (IsBouncing)
            {
                SetState(Bouncing);
            }
            //if (_fallAction.IsPressed())
            //
            //   SetState(Fall);
            //
            else if (IsJump)
            {
                SetState(Jump);
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
        

        while (_foot.IsGrounded == false)
        {
            _ani.SetState(new PAniState()
            {
                State = EPCAniState.Jump,
                Rotation = Quaternion.Euler(0f, _moveAction.ReadValue<Vector2>().x > 0f ? 180f : 0f, 0f)
            });
            
            if (IsGrab)
            {
                SetState(Throw);
                yield break;
            }
            if (IsBouncing)
            {
                SetState(Bouncing);
                yield break;
            }
            if (IsFall)
            {
                SetState(Fall);
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return null;

        if (_moveAction.IsPressed())
        {
            SetState(Run);
        }
        else
        {
            _ani.SetState(new PAniState()
            {
                State = EPCAniState.Landing_Jump,
                Rotation = Quaternion.Euler(0f, _moveAction.ReadValue<Vector2>().x > 0f ? 180f : 0f, 0f)
            });
        
            SetState(Idle); 
        }
    }

    private IEnumerator Throw()
    {
        yield return null;

        float cameraX = 0f;
        do
        {
            cameraX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            
            _ani.SetState(new PAniState()
            {
                State = EPCAniState.Throw_1,
                Rotation = Quaternion.Euler(0f, cameraX <= _rigid.transform.position.x ? 0f : 180f, 0f)
            });

            yield return new WaitForEndOfFrame();
        } while (IsGrab);
        
        
        cameraX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        _ani.SetState(new PAniState()
        {
            State = EPCAniState.Throw_2,
            Rotation = Quaternion.Euler(0f, cameraX <= _rigid.transform.position.x ? 0f : 180f, 0f)
        });
        
        yield return new WaitForSeconds(0.1f);
        SetState(Idle);
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
            if (IsGrab)
            {
                SetState(Throw);
            }
            else if (IsBouncing)
            {
                SetState(Bouncing);
            }
            else if (IsFall)
            {
                SetState(Fall);
            }
            else if (IsJump)
            {
                SetState(Jump);
            }
            else if (_moveAction.IsPressed())
            {
                SetState(Run);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
