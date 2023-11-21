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
    private CinemachineCameraControll _cameraControll;

    public float CoolTime => _executor.Blackboard.GetUnWrappedProperty<float>("in_coolTime");

    public void Init(Blackboard sendedBlackboard)
    {
        _cameraControll = GameObject.FindWithTag("VirtualCamera")?.GetComponent<CinemachineCameraControll>();
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
            .AddProperty("out_data", _data)
            .AddProperty("out_minmumCloseDistance",new WrappedValue<float>(_data.MinmumCloseDistance))
            .AddProperty("out_swingForce", new WrappedValue<float>(_data.SwingForce))
            .AddProperty("in_currentActor_physics", null)
            .AddProperty("in_currentActor_propagation", null)
            .AddProperty("in_currentActor_life", null)
            .AddProperty("in_swingDir", new WrappedNullableValue<Vector2>())
            .AddProperty("in_coolTime", new WrappedValue<float>())
            .AddProperty("out_lineRenderer", _lineRenderer)
            ;
        
        
        _executor = StateExecutor.Create(container, blackboard);
    }

    public void Update(Blackboard blackboard)
    {
        _executor.Execute();
        
        if (_cameraControll)
        {
            _cameraControll.SetZoomKeyState(_executor.CurrentState is PlayerSwingState);
        }
        
        
        _executor.Blackboard.GetWrappedProperty<float>("in_coolTime", out var coolTime);
        coolTime.Value -= Time.deltaTime;
        if (coolTime < 0f) coolTime.Value = 0f;
        

        blackboard.GetWrappedProperty<bool>("in_isGrabState").Value = _executor.CurrentState is not DefaultPlayerState;
    }

    public void Reset()
    {
        
    }

}
