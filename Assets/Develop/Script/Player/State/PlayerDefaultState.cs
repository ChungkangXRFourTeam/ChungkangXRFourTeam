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

        if(_cameraControll) 
            _cameraControll.SetZoomKeyState(InputManager.GetMainGameAction("Grab").IsPressed());

        return false;
    }
}