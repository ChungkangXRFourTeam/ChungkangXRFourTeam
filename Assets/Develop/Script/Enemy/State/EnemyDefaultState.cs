using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;
using XRProject.Helper;

public class EnemyDefaultState : BaseState
{
    private StateExecutor _actionExecutor;
    private StateExecutor _movingObserverExecutor;
    public override void Init(Blackboard sendedBlackboard)
    {
        sendedBlackboard.GetProperty<StrategyExecutor>("out_strategyExecutor", out var se);

        se.Container
            .SetActive<ActorPhysicsStrategy>(true)
            ;

        StateContainer actionContainer = new StateContainer();
        actionContainer
            .AddState<EnemyPatrollState>()
            .AddState<EnemyPropagatingState>()
            .AddState<EnemyNothingState>()
            .AddState<EnemySwingState>()
            .SetInitialState<EnemyNothingState>()
            ;
        StateContainer movingObserverContainer = new StateContainer();
        movingObserverContainer
            .AddState<EnemyMovingState>()
            .AddState<EnemyStopState>()
            .AddState<EnemySleepState>()
            .SetInitialState<EnemySleepState>()
            ;
        
        _actionExecutor = StateExecutor.Create(actionContainer, sendedBlackboard);
        _movingObserverExecutor = StateExecutor.Create(movingObserverContainer, sendedBlackboard);
    }

    public override void Enter(Blackboard blackboard)
    {
        blackboard.GetProperty<StrategyExecutor>("out_strategyExecutor", out var se);

        se.Container
            .SetActive<ActorPhysicsStrategy>(true)
            ;
    }

    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        blackboard.GetProperty<StrategyExecutor>("out_strategyExecutor", out var se);

        se.Execute();
        _actionExecutor.Execute();
        _movingObserverExecutor.Execute();

        blackboard.SetProperty("test", _actionExecutor.CurrentState.ToString());

        return false;
    }
}

public class EnemyPatrollState : BaseState
{
    private Vector2 _leftPoint;
    private Vector2 _rightPoint;
    private bool _goLeft;
    private Vector2 TargetPoint => _goLeft ? _leftPoint : _rightPoint;
    private EnemyData _data;
    private Blackboard _cachedBlackboard;    
    public override void Init(Blackboard blackboard)
    {
        _cachedBlackboard = blackboard;
        
        _data = blackboard.GetProperty<EnemyData>("out_enemyData");
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        interaction.OnContractObject += OnContractActor;
    }
    public override void Release(Blackboard blackboard)
    {
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        interaction.OnContractObject -= OnContractActor;
    }
    public override void Enter(Blackboard blackboard)
    {
        blackboard.GetUnWrappedProperty<(Vector2, Vector2)>("out_patrollPoints", out var points);
        _leftPoint = points.Item1;
        _rightPoint = points.Item2;
        _goLeft = Random.value > 0.5f;
        blackboard.GetProperty<PropagationInfo>("out_propagationInfo", out var pi);
        pi.Count = 0;

    }
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        bool gotoPatrollSpace = EnemyPatrollState.ReadyGotoPatroll(blackboard);
        bool gotoSwingState = EnemySwingState.ReadyGotoSwing(blackboard);
        bool gotoProgagating = EnemyPropagatingState.ReadyGotoPropagating(blackboard);

        if (gotoSwingState)
        {
            executor.SetNextState<EnemySwingState>();
        }
        else if (gotoProgagating)
        {
            executor.SetNextState<EnemyPropagatingState>();
        }
        else if (!gotoPatrollSpace)
        {
            executor.SetNextState<EnemyNothingState>();
        }
        else
        {
            Patroll(blackboard);
        }
        
        return false;
    }

    private void Patroll(Blackboard blackboard)
    {
        var transform = blackboard.GetProperty<Transform>("out_transform");
        
        if (Mathf.Abs(TargetPoint.x - transform.position.x) <= 0.1f + 1.3f * 0.5f)
        {
            _goLeft = !_goLeft;
        }

        var dir = TargetPoint - (Vector2)transform.position;
        dir = dir.normalized;
        dir = Vector3.Project(dir, Vector3.right).normalized;

        transform.position += (Vector3)dir * (_data.MovementSpeed * Time.deltaTime);
    }

    private void OnContractActor(ObjectContractInfo info)
    {
        var blackboard = _cachedBlackboard;
        
        if (info.TryGetBehaviour(out IBObjectPatrollSpace patrollSpace))
        {
            blackboard.GetWrappedProperty<bool>("out_isEnteredPatrollSpace").Value = true;
            blackboard.GetWrappedProperty<(Vector2, Vector2)>("out_patrollPoints").Value =
                (patrollSpace.LeftPoint, patrollSpace.RightPoint);
        }
        
    }
    public static bool ReadyGotoPatroll(Blackboard blackboard)
    {
        blackboard.GetUnWrappedProperty<bool>("out_isEnteredPatrollSpace", out var entered);

        return entered;
    }
}

public class EnemySwingState : BaseState
{
    private Blackboard _cachedBlackboard;    
    public override void Enter(Blackboard blackboard)
    {
        _cachedBlackboard = blackboard;
        
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        interaction.OnContractActor += OnContractActor;
        interaction.OnContractObject += OnContractObject;
    }
    public override void Exit(Blackboard blackboard)
    {
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        interaction.OnContractActor -= OnContractActor;
        interaction.OnContractObject -= OnContractObject;
    }

    public override void Release(Blackboard blackboard)
    {
        DOTween.Kill(this);
    }
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        bool isSleep = blackboard.GetUnWrappedProperty<bool>("in_isSleep");
        bool gotoPatroll = EnemyPatrollState.ReadyGotoPatroll(blackboard);

        if (ReadyGotoSwing(blackboard))
        {
            blackboard.GetProperty<PropagationInfo>("out_propagationInfo", out var info);
            blackboard.GetProperty<EnemyData>("out_enemyData", out var data);
            info.Count = data.PropagationCount;
        }
        
        if (isSleep && gotoPatroll)
        {
            executor.SetNextState<EnemyPatrollState>();
        }
        else if (isSleep)
        {
            executor.SetNextState<EnemyNothingState>();
        }
        else
        {
            // swing..   
        }
        
        return false;
    }

    private void OnContractActor(ActorContractInfo info)
    {
    }
    
    private void OnContractObject(ObjectContractInfo info)
    {
        var transform = _cachedBlackboard.GetProperty<Transform>("out_transform");
        
        if (info.TryGetBehaviour(out IBObjectInteractive interactive) &&
            info.Transform.gameObject.CompareTag("KnockbackObject"))
        {
            EffectManager.ImmediateCommand(new EffectCommand()
            {
                EffectKey = "actor/knockbackHit",
                Position = transform.position
            });
        }
    }
    public static bool ReadyGotoSwing(Blackboard blackboard)
    {
        blackboard.GetProperty<StrategyExecutor>("out_strategyExecutor", out var se);
        bool gotoSwingState = se.Container.Get<ActorPhysicsStrategy>().IsSwingState;
        return gotoSwingState;
    }
}

public class PropagationInfo : IBActorPropagation
{
    public int Count { get; set; }
    public Vector2 ForceVector { get; set; }
    public InteractionController Interaction { get; private set; }
    public int MaxCount { get; set; }
    public PropagationInfo(InteractionController interaction)
    {
        Interaction = interaction;
    }


    public void Propagate(BaseContractInfo caller, int count, Vector2 forceVector)
    {
        Count = count < 0 ? 0 : count;
        ForceVector = forceVector;
    }

    public bool IsPropagation => Count > 0;
}
public class EnemyPropagatingState : BaseState
{
    private Blackboard _cachedBlackboard;
    private PropagationInfo _propagationInfo;
    private EnemyData _data;
    public override void Init(Blackboard blackboard)
    {
        _cachedBlackboard = blackboard;
        
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        interaction.OnContractActor += OnContractActor;

        _propagationInfo = blackboard.GetProperty<PropagationInfo>("out_propagationInfo");
        _data = blackboard.GetProperty<EnemyData>("out_enemyData");
        _propagationInfo.MaxCount = _data.PropagationCount;

    }

    public override void Enter(Blackboard blackboard)
    {
        Propagate(blackboard);
    }
    public override void Exit(Blackboard blackboard)
    {
    }
    public override void Release(Blackboard blackboard)
    {
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        interaction.OnContractActor -= OnContractActor;
    }
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        bool gotoPatrollSpace = EnemyPatrollState.ReadyGotoPatroll(blackboard);
        bool gotoSwingState = EnemySwingState.ReadyGotoSwing(blackboard);
        bool gotoProgagating = EnemyPropagatingState.ReadyGotoPropagating(blackboard);

        blackboard.GetUnWrappedProperty<bool>("in_isSleep", out var isSleep);
        
        
        if (gotoProgagating && !isSleep)
        {
            executor.SetNextState<EnemyPropagatingState>();
        }
        else if (gotoSwingState)
        {
            executor.SetNextState<EnemySwingState>();
        }
        else if (gotoPatrollSpace)
        {
            executor.SetNextState<EnemyPatrollState>();
        }
        else
        {
            executor.SetNextState<EnemyNothingState>();
        }
        
        return false;
    }


    private void Propagate(Blackboard blackboard)
    {
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        if (interaction.ContractInfo is ActorContractInfo info && 
            info.TryGetBehaviour(out IBActorPhysics physics))
        {
            physics.AddForce(_propagationInfo.ForceVector, ForceMode2D.Impulse);
            _propagationInfo.ForceVector = Vector2.zero;
        }
    }

    private void OnContractActor(ActorContractInfo info)
    {
        var blackboard = _cachedBlackboard;
        
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        if (info.Transform.GetComponent<Enemy>() == null) return;

        if (
            info.TryGetBehaviour(out IBActorPropagation propagation) &&
            info.TryGetBehaviour(out IBActorPhysics physics) &&
            interaction.TryGetContractInfo(out ActorContractInfo myInfo) &&
            myInfo.TryGetBehaviour(out IBActorPhysics myPhysics) &&
            !myPhysics.IsSwingState &&
            propagation.Count > 0
        )
        {
            blackboard.GetProperty("out_transform", out Transform transform);
            Vector3 v = (info.Transform.position - transform.position).normalized;
            v = Vector3.Project(v, Vector3.right).normalized * _data.PropagationForce;

            
            int count = propagation.Count - 1;
            
            _propagationInfo.Propagate(interaction.ContractInfo, count, -v);
            if (count <= 0) return;
            EffectManager.ImmediateCommand(new EffectCommand()
            {
                EffectKey = "actor/enemyHit",
                Position = Vector3.Lerp(transform.position, info.Transform.position, 0.5f)
            });

            physics.Stop();
            physics.AddForce(v * 0.75f, ForceMode2D.Impulse);
            propagation.Count = 0;
        }
    }
    
    public static bool ReadyGotoPropagating(Blackboard blackboard)
    {
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        blackboard.GetProperty<PropagationInfo>("out_propagationInfo", out var propagationInfo);

        if (
            interaction.ContractInfo is ActorContractInfo myInfo &&
            myInfo.TryGetBehaviour(out IBActorPhysics myPhysics) &&
            !myPhysics.IsSwingState &&
            propagationInfo.Count > 0
        )
        {
            return true;
        }

        return false;
    }
}

public class EnemyNothingState : BaseState
{
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        bool gotoPatrollSpace = EnemyPatrollState.ReadyGotoPatroll(blackboard);
        bool gotoSwingState = EnemySwingState.ReadyGotoSwing(blackboard);
        bool gotoProgagating = EnemyPropagatingState.ReadyGotoPropagating(blackboard);

        if (gotoSwingState)
        {
            executor.SetNextState<EnemySwingState>();
        }
        else if (gotoProgagating)
        {
            executor.SetNextState<EnemyPropagatingState>();
        }
        else if (gotoPatrollSpace)
        {
            executor.SetNextState<EnemyPatrollState>();
        }
        else
        {
            // none..
        }
        
        return false;
    }
}

public class EnemyMovingState : BaseState
{
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        var gotoMoveStop = !EnemyMovingState.ReadyMoving(blackboard);

        if (gotoMoveStop)
        {
            executor.SetNextState<EnemyStopState>();
        }
        else
        {
            // move..
        }
        
        return false;
    }

    public override void Enter(Blackboard blackboard)
    {
        blackboard.GetWrappedProperty<bool>("in_isMoving", out var isMoving);
        isMoving.Value = true;
    }

    public override void Exit(Blackboard blackboard)
    {
        blackboard.GetWrappedProperty<bool>("in_isMoving", out var isMoving);
        isMoving.Value = false;
    }

    public static bool ReadyMoving(Blackboard blackboard)
    {
        blackboard.GetProperty<Rigidbody2D>("out_rigidbody", out var rigid);

        if (rigid.velocity.sqrMagnitude >= 0.001f)
        {
            return true;
        }

        return false;
    }
}    

public class EnemyStopState : BaseState
{
    private float _timer = 0f;
    private EnemyData _data;

    public override void Init(Blackboard blackboard)
    {
        _data = blackboard.GetProperty<EnemyData>("out_enemyData");
    }
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        var gotoMoving = EnemyMovingState.ReadyMoving(blackboard);
        var gotoSleep = PollingStop(blackboard);
        
        if (gotoMoving)
        {
            executor.SetNextState<EnemyMovingState>();
        }
        else if(gotoSleep)
        {
            executor.SetNextState<EnemySleepState>();
        }
        else
        {
            // stop..
        }
        
        return false;
    }

    private bool PollingStop(Blackboard blackboard)
    {
        _timer += Time.deltaTime;

        if (_timer >= _data.SleepDecisionTime)
        {
            return true;
        }

        return false;
    }

    public override void Enter(Blackboard blackboard)
    {
        _timer = 0f;
        blackboard.GetWrappedProperty<bool>("in_isStop", out var isStop);
        isStop.Value = true;
    }

    public override void Exit(Blackboard blackboard)
    {
        _timer = 0f;
        blackboard.GetWrappedProperty<bool>("in_isStop", out var isStop);
        isStop.Value = false;
    }
}    

public class EnemySleepState : BaseState
{
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        bool gotoMove = EnemyMovingState.ReadyMoving(blackboard);

        if (gotoMove)
        {
            executor.SetNextState<EnemyMovingState>();
        }

        return false;
    }
    public override void Enter(Blackboard blackboard)
    {
        blackboard.GetWrappedProperty<bool>("in_isSleep", out var isSleep);
        isSleep.Value = true;
    }

    public override void Exit(Blackboard blackboard)
    {
        blackboard.GetWrappedProperty<bool>("in_isSleep", out var isSleep);
        isSleep.Value = false;
    }
}    
