using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XRProject.Helper;

public class PlayerDefaultState : BaseState
{
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
}
