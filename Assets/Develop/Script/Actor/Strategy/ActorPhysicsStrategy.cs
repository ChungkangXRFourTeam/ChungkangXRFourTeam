using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

[Serializable]
public class ActorPhysicsStrategy : IStrategy, IBActorPhysics
{
    private Rigidbody2D _rigid;
    private InteractionController _interaction;
    public bool IsSwingState
        => _executor.CurrentState is ActorSwingState;

    public Rigidbody2D Rigid => _rigid;
    public InteractionController Interaction => _interaction;

    private StateExecutor _executor;
    private WrappedValue<bool> _isAllowInteraction;
    
    public void Init(Blackboard sendedBlackboard)
    {
        _rigid = sendedBlackboard.GetProperty<Rigidbody2D>("out_rigidbody");
        _interaction = sendedBlackboard.GetProperty<InteractionController>("out_interaction");
        
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

        _isAllowInteraction = sendedBlackboard.GetWrappedProperty<bool>("out_isAllowedInteraction");
    }

    public virtual void AddKnockback(Vector2 vector)
    {
        if (!_isAllowInteraction) return;
        
        _executor.Blackboard.SetWrappedProperty("out_trigger_isSwingState", true);
        _executor.Blackboard.SetWrappedProperty("out_knockbackVector", vector);
    }

    public void AddForce(Vector2 force, ForceMode2D mode)
    {
        Rigid.AddForce(force, mode);
    }

    public Transform GetTransformOrNull()
    {
        return _rigid.transform;
    }
    
    public void OnDetectBlock()
    {
        _executor.Blackboard.SetWrappedProperty("out_trigger_knockbackCollision", true);
    }

    public void OnDetectOtherActor(IBActorPhysics actor)
    {
    }

    public void Update(Blackboard blackboard)
    {
        _executor.Execute();
    }

    public void Reset()
    {
        
    }
}
