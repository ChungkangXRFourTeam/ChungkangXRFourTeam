using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

[System.Serializable]
public class PlayerSwingAttackStrategy : IStrategy
{
    private LineRenderer _lineRenderer;
    private PlayerSwingAttackData _data;
    private StateExecutor _executor;

    public void Init(Blackboard sendedBlackboard)
    {
        _data = sendedBlackboard.GetProperty<PlayerSwingAttackData>("out_swingAttackData");
        _lineRenderer = sendedBlackboard.GetProperty<LineRenderer>("out_traceLineRenderer");
        
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
            .AddProperty("out_grabDistance",new WrappedValue<float>(_data.GrabDistance))
            .AddProperty("out_timeScale", new WrappedValue<float>(_data.TimeScale))
            .AddProperty("out_minmumCloseDistance",new WrappedValue<float>(_data.MinmumCloseDistance))
            .AddProperty("out_swingForce", new WrappedValue<float>(_data.SwingForce))
            .AddProperty("in_currentActor_physics", null)
            .AddProperty("in_currentActor_life", null)
            .AddProperty("in_swingDir", new WrappedNullableValue<Vector2>())
            .AddProperty("out_lineRenderer", _lineRenderer)
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
