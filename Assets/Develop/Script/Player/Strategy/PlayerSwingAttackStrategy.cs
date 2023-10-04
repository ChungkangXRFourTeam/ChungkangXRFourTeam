using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

[System.Serializable]
public class PlayerSwingAttackStrategy : IStrategy
{
    [SerializeField] private LineRenderer _lineRenderer;

    [SerializeField] private WrappedValue<float> _swingForce;
    [SerializeField] private WrappedValue<float> _minmumCloseDistance;
    [SerializeField] private WrappedValue<float> _timeScale;
    [SerializeField] private WrappedValue<float> _grabDistance;

    private StateExecutor _executor;

    public void Init(Blackboard sendedBlackboard)
    {
        var container = new StateContainer();
        var blackboard = new Blackboard();

        container
            .AddState<DefaultPlayerState>()
            .AddState<PlayerGrabState>()
            .AddState<PlayerSwingState>()
            .AddState<PlayerShootState>()
            .SetInitialState<DefaultPlayerState>();

        blackboard
            .AddProperty("out_transform", sendedBlackboard.GetProperty<Transform>("out_transform"))
            .AddProperty("out_grabDistance",_grabDistance)
            .AddProperty("out_timeScale", _timeScale)
            .AddProperty("out_lineRenderer", _lineRenderer)
            .AddProperty("out_minmumCloseDistance",_minmumCloseDistance)
            .AddProperty("out_swingForce", _swingForce)
            .AddProperty("in_currentActor_physics", null)
            .AddProperty("in_currentActor_life", null)
            .AddProperty("in_swingDir", new WrappedNullableValue<Vector2>())
            ;
        
        _executor = StateExecutor.Create(container, blackboard);
    }

    public void Update(Blackboard blackboard)
    {
        _executor.Execute();
    }

    public void Reset()
    {
        
    }

}
