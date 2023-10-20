using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using XRProject.Helper;

public abstract class ActorPhysics : MonoBehaviour, IBActorPhysics
{
    public bool IsSwingState
        => _executor.CurrentState is ActorSwingState;

    public Rigidbody2D Rigid { get; private set; }
    public InteractionController Interaction { get; private set; }

    private StateExecutor _executor;
    protected void LateInit()
    {
        Rigid = GetComponent<Rigidbody2D>();
        this.Interaction = GetComponentInChildren<InteractionController>(); 
        
        var container = new StateContainer();
        var blackboard = new Blackboard();

        container
            .AddState<ActorDefaultState>()
            .AddState<ActorSwingState>()
            .SetInitialState<ActorDefaultState>()
            ;

        blackboard
            .AddProperty("rigidbody", Rigid)
            .AddProperty("actorPhysics", this)
            
            .AddProperty("out_trigger_isSwingState", new WrappedTriggerValue())
            .AddProperty("out_trigger_knockbackCollision", new WrappedTriggerValue())
            
            
            .AddProperty("out_otherActor", null)
            
            .AddProperty("out_gravity", new WrappedValue<bool>(true))
            .AddProperty("out_knockbackVector", new WrappedValue<Vector2>(Vector2.zero))
            ;
        
        _executor = StateExecutor.Create(container, blackboard);
        _executor.IsDebug = false;
    }

    public void ActorUpdate()
    {
        _executor.Execute();
    }

    public virtual void AddKnockback(Vector2 vector)
    {
        _executor.Blackboard.SetWrappedProperty("out_trigger_isSwingState", true);
        _executor.Blackboard.SetWrappedProperty("out_knockbackVector", vector);
    }

    public void AddForce(Vector2 force, ForceMode2D mode)
    {
        Rigid.AddForce(force, mode);
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    [CanBeNull]
    public abstract Transform GetTransformOrNull();
    
    protected void OnDetectBlock()
    {
        _executor.Blackboard.SetWrappedProperty("out_trigger_knockbackCollision", true);
    }

    public void OnDetectOtherActor(IBActorPhysics actor)
    {
    }
}
