using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private InputAction _attackAction;

    private Rigidbody2D _rigid;
    private PlayerController _playerController;

    public bool IsFall => _foot.IsGrounded == false && _fallAction.IsPressed();
    public bool IsBouncing => 
        _interaction.GetContractInfoOrNull<ActorContractInfo>()
        .GetBehaviourOrNull<IBActorPhysics>()
        .IsSwingState;

    public bool IsJump => _foot.IsGrounded == false && _jumpAction.IsPressed();
    public bool IsGrab { get; set; }
    public bool IsAttack => _attackAction.IsPressed() && _foot.IsGrounded;
    private bool _lateGrabState = false;

    private int _attackIndex;
    public static bool AniIsAttacking { get; private set; }
    private void Start()
    {
        _moveAction = InputManager.GetMainGameAction("Move");
        _fallAction = InputManager.GetMainGameAction("Fall");
        _jumpAction = InputManager.GetMainGameAction("Jump");
        _grabAction = InputManager.GetMainGameAction("Grab");
        _swingAction = InputManager.GetMainGameAction("Swing");
        _attackAction = InputManager.GetMainGameAction("Attack");

        _rigid = _interaction.GetComponent<Rigidbody2D>();
        _playerController = _interaction.GetComponent<PlayerController>();

        _grabAction.started += OnGrabAction;
        _swingAction.canceled += OnSwingAction;
        
        SetState(Idle);
    }


    public void OnHitEnd(int i)
    {
        _isAttacking = false;
        _attackIndex = i;
    }

    public void OnHitTimingBegin(int i)
    {
        var data = _playerController.MeleeAttackData;

        string properties = _playerController.Properties == EActorPropertiesType.Flame ? "flame" : "water";
        bool isShowingEffect = _playerController.Properties != EActorPropertiesType.None;
        
        if (i == 1)
        {
            Hit(data.SlashEffectOffsetHit1, data.SlashHitRadiusHit1);
            //Effect(data.SlashEffectOffsetHit1, data.SlashEffectScale1, "player/");
        }
        else if (i == 2)
        {
            Hit(data.SlashEffectOffsetHit2, data.SlashHitRadiusHit2);
            
            if(isShowingEffect)
                Effect(data.SlashEffectOffsetHit2, data.SlashEffectScale2, $"player/attack2_{properties}");
        }
        else if (i == 3)
        {
            Hit(data.SlashEffectOffsetHit3, data.SlashHitRadiusHit3);
            
            if(isShowingEffect)
                Effect(data.SlashEffectOffsetHit3, data.SlashEffectScale3, $"player/attack3_{properties}");
        }
    }

    private void Hit(Vector2 offset, float radius)
    {
        offset.x *= -1f;
        var hit = Physics2D.OverlapCircle(
            transform.position + (transform.rotation * offset),
            radius,
            LayerMask.GetMask("Enemy")
        );

        if (hit == false) return;
            
        hit.gameObject.GetComponent<Enemy>()?.DoHit(_interaction.ContractInfo,1f);
    }

    private bool _isAttacking;
    public void OnHitStart()
    {
        _isAttacking = true;
    }
    private void LateUpdate()
    {
        if (_lateGrabState)
        {
            _lateGrabState = false;
            IsGrab = _playerController.GrabState;
        }

        if (_moveAction.IsPressed())
        {
            beforeRunAngle = _moveAction.ReadValue<Vector2>().x > 0f ? 0f : 180f;
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

    private void Effect(Vector2 offset, Vector2 scale, string key)
    {
        var data = _playerController.MeleeAttackData;
        var mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pos = transform.position;
        pos.z = mp.z = 0f;
            
        var dir = mp - pos;

        var rotation = transform.rotation;

        offset.x *= -1f;
        pos += (transform.rotation * offset);
        
        EffectManager.ImmediateCommand(new EffectCommand()
        {
            EffectKey = key,
            Position = pos,
            Rotation = rotation,
            Scale = scale,
            OnContractActor = (item, info) =>
            {
                if (info.TryGetBehaviour(out IBActorHit hit) &&
                    info.Transform.GetComponent<Enemy>())
                {
                    hit.DoHit(_interaction.ContractInfo,1f);
                }
            }
        });
    }
    private IEnumerator Attack()
    {
        yield return null;
        float cameraX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        var rotation = Quaternion.Euler(0f, beforeRunAngle = cameraX > _rigid.transform.position.x ? 180f : 0f, 0f);
        beforeRunAngle = cameraX > _rigid.transform.position.x ? 180f : 0f;

        AniIsAttacking = true;
        
        _ani.SetState(new PAniState()
        {
            State = EPCAniState.Hit_1,
            Rotation = rotation
        });

        _attackIndex = 1;

        yield return null;
        while (true)
        {
            if (IsBouncing)
            {
                SetState(Bouncing);
                AniIsAttacking = false;
                _isAttacking = false;
                yield break;
            }
            
            if (_isAttacking == false && IsAttack == false)
            {
                _attackIndex = -1;
            }


            if (IsAttack && _attackIndex == -1)
            {
                cameraX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                rotation = Quaternion.Euler(0f, beforeRunAngle = cameraX > _rigid.transform.position.x ? 180f : 0f, 0f);
                beforeRunAngle = cameraX > _rigid.transform.position.x ? 180f : 0f;
                _ani.SetState(new PAniState()
                {
                    State = EPCAniState.Hit_1,
                    Rotation = rotation
                });
                _attackIndex = 1;
            }
            
            if (_attackIndex == -1)
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
                else
                {
                    SetState(Idle);
                }
                AniIsAttacking = false;
                _isAttacking = false;
                yield break;
            }
        
            if(_attackIndex == 2)
            {
                _ani.SetState(new PAniState()
                {
                    State = EPCAniState.Hit_2,
                    Rotation = rotation
                });
            }
            else if(_attackIndex == 3)
            {
                _ani.SetState(new PAniState()
                {
                    State = EPCAniState.Hit_3,
                    Rotation = rotation
                });
            }
            
            yield return null;
        }
        

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

    private float beforeRunAngle;
    private IEnumerator Run()
    {
        yield return null;
        
        while (true)
        {
            float temp = _moveAction.ReadValue<Vector2>().x > 0f ? 180f : 0f;
            _ani.SetState(new PAniState()
            {
                State = EPCAniState.Run,
                Rotation = Quaternion.Euler(0f, temp, 0f),
            });

            if (IsAttack)
            {
                SetState(Attack);
            }
            else if (IsGrab)
            {
                SetState(Throw);
            }
            else if (IsBouncing)
            {
                SetState(Bouncing);
            }
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
        
        while (true)
        {
            _ani.SetState(new PAniState()
            {
                State = EPCAniState.Idle,
                Rotation = Quaternion.Euler(0f, beforeRunAngle, 0f),
            });

            if (IsAttack)
            {
                SetState(Attack);
            }
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
