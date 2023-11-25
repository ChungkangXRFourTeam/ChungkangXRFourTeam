using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPCAniState : int
{
    Bouncing,
    Death,
    Idle,
    Jump,
    Run,
    Falling_Basics,
    Falling_Bouncing,
    Falling_Dash,
    Landing_Jump,
    Landing_Basics,
    Landing_Dash,
    Throw_1,
    Throw_2,
    Hit_1,
    Hit_2,
    Hit_3,
}
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _ani;

    [SerializeField] private string DState;
    public EPCAniState State { get; private set; }

    private string[] _triggerFieldTable;
    private PAniState _pendingState;

    private const int ENUM_SIZE = 15;

    private void Awake()
    {
        State = EPCAniState.Idle;
        _ani = GetComponentInChildren<Animator>();

        _triggerFieldTable = new string[ENUM_SIZE + 1];
        for (int i = 0; i <= ENUM_SIZE; i++)
        {
            _triggerFieldTable[i] = ((EPCAniState)i).ToString();
        }

    }

    private void Update()
    {
        if (_pendingState == null) return;
        
        if (_pendingState.State == State && _pendingState.Restart == false)
        {
            Apply(_pendingState, true);
            _pendingState = null;
        }
        else
        {
            State = _pendingState.State;
            Apply(_pendingState, false);
            _pendingState = null;
        }

    }

    public void SetState(PAniState param)
    {
        if (param == null)
        {
            param = new PAniState()
            {
                State = EPCAniState.Idle,
                Rotation = Quaternion.identity,
                Restart = false
            };
        }
        else
        {
            _pendingState = param;
        }
    }

    private void TriggerAni(EPCAniState state)
    {
        int index = (int)state;
        string key = _triggerFieldTable[index];
        
        _ani.SetTrigger(key);
    }

    private void BoolAni(EPCAniState state)
    {
        string key;
        for (int i = 0; i <= ENUM_SIZE; i++)
        {
            key = _triggerFieldTable[i];
            _ani.SetBool(key, false);
        }
        
        int index = (int)state; 
        key = _triggerFieldTable[index];
        _ani.SetBool(key, true);
    }

    private void Apply(PAniState state, bool excludeState)
    {
        transform.rotation = state.Rotation;
        
        if(excludeState == false)
        {
            //TriggerAni(state.State);
            BoolAni(state.State);
            DState = state.State.ToString();
        }
    }
}

public class PAniState
{
    public EPCAniState State { get; set; }
    public Quaternion Rotation { get; set; } = Quaternion.identity;
    public bool Restart { get; set; } = false;
}
