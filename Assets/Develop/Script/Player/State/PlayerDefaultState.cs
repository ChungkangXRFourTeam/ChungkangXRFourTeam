using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using XRProject.Helper;

public class PlayerDefaultState : BaseState
{
    private CinemachineCameraControll _cameraControll;

    public override void Init(Blackboard blackboard)
    {
        blackboard.GetProperty<StrategyExecutor>("out_strategyExecutor", out var se);

        se.Container
            .SetActive<PlayerMoveStrategy>(true)
            .SetActive<PlayerPhysicsStrategy>(true)
            .SetActive<PlayerMeleeAttackStrategy>(true)
            .SetActive<PlayerSwingAttackStrategy>(true)
            .SetActive<PlayerBuffStrategy>(true)
            ;

        _cameraControll = GameObject.FindWithTag("VirtualCamera")?.GetComponent<CinemachineCameraControll>();
        InputManager.ActionListener.MainGame.Grab.started += OnZoom;
        InputManager.ActionListener.MainGame.Grab.canceled += ExitZoom;

    }

    public override void Enter(Blackboard blackboard)
    {
        blackboard.GetProperty<StrategyExecutor>("out_strategyExecutor", out var se);

        se.Container
            .SetActive<PlayerMoveStrategy>(true)
            .SetActive<PlayerPhysicsStrategy>(true)
            .SetActive<PlayerMeleeAttackStrategy>(true)
            .SetActive<PlayerSwingAttackStrategy>(true)
            .SetActive<PlayerBuffStrategy>(true)
            ;
    }

    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        blackboard.GetProperty<StrategyExecutor>("out_strategyExecutor", out var se);

        se.Execute();


        return false;
    }

    public override void Exit(Blackboard blackboard)
    {
        if (_cameraControll)
        {
            _cameraControll.SetZoomKeyState(true);
        }
    }

    void OnZoom(InputAction.CallbackContext ctx)
    {
        if(_cameraControll) 
            _cameraControll.SetZoomKeyState(true);
    }

    void ExitZoom(InputAction.CallbackContext ctx)
    {
        if(_cameraControll) 
            _cameraControll.SetZoomKeyState(false);
    }
}