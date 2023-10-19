using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

public class EnemyDefaultState : BaseState
{

    public override void Init(Blackboard blackboard)
    {
        blackboard.GetProperty<StrategyExecutor>("out_strategyExecutor", out var se);

        se.Container
            .SetActive<ActorPhysicsStrategy>(true)
            .SetActive<EnemyPropagationStrategy>(true)
            ;
    }

    public override void Enter(Blackboard blackboard)
    {
        blackboard.GetProperty<StrategyExecutor>("out_strategyExecutor", out var se);

        se.Container
            .SetActive<ActorPhysicsStrategy>(true)
            .SetActive<EnemyPropagationStrategy>(true)
            ;
    }

    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        blackboard.GetProperty<StrategyExecutor>("out_strategyExecutor", out var se);

        se.Execute();

        return false;
    }
}
