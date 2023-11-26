using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using XRProject.Helper;

public class EnemyDefaultState : BaseState, IBEnemyState
{
    private StateExecutor _actionExecutor;
    private StateExecutor _movingObserverExecutor;
    private InteractionController _interaction;

    public override void Init(Blackboard sendedBlackboard)
    {
        sendedBlackboard.GetProperty<StrategyExecutor>("out_strategyExecutor", out var se);
        _interaction = sendedBlackboard.GetProperty<InteractionController>("out_interaction");

        se.Container
            .SetActive<ActorPhysicsStrategy>(true)
            ;

        StateContainer actionContainer = new StateContainer();
        actionContainer
            .AddState<EnemyPatrollState>()
            .AddState<EnemyDetectionState>()
            .AddState<EnemyAttackState>()
            
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
        blackboard.GetProperty<Transform>("out_transform", out var transform);
        blackboard.GetUnWrappedProperty<bool>("out_isCaught", out var isCaught);
        blackboard.GetUnWrappedProperty<EEnemyType>("out_enemyType", out var enemyType);
        
        transform.GetComponentInChildren<TMP_Text>()?
            .SetText($"{_actionExecutor.CurrentState.ToString()}\n{_movingObserverExecutor.CurrentState.ToString()}\n{(_interaction.ContractInfo as ActorContractInfo).GetBehaviourOrNull<IBActorPropagation>().Count}");

        se.Execute();
        _actionExecutor.Execute();
        _movingObserverExecutor.Execute();

        if (_interaction.TryGetContractInfo(out ActorContractInfo info) &&
            info.TryGetBehaviour(out IBActorPhysics physics) &&
            physics.IsSwingState)
        {
            if (enemyType == EEnemyType.Hedgehog)
            {
                blackboard.GetProperty("out_bodyChanger", out AnimationBodyChanger bodyChanger);
                bodyChanger.Change("move").GetBodyGetComponentOrNull<Animator>("move")?.SetTrigger("grab");
            } 
        }
        
        if (isCaught && enemyType == EEnemyType.Hedgehog)
        {
            blackboard.GetProperty("out_bodyChanger", out AnimationBodyChanger bodyChanger);

            bodyChanger.Change("move").GetBodyGetComponentOrNull<Animator>("move")?.SetTrigger("grab");
        }

        return false;
    }

    public InteractionController Interaction => _interaction;

    public bool CheckCurrentState<T>() where T : BaseState
    {
        if (_actionExecutor.CurrentState is T) return true;
        if (_movingObserverExecutor.CurrentState is T) return true;

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
        
        blackboard.GetUnWrappedProperty("out_enemyType", out EEnemyType enemyType);

        if (enemyType == EEnemyType.Sheep)
        {
            blackboard.GetProperty("out_bodyChanger", out AnimationBodyChanger bodyChanger);
            var ani = bodyChanger
                .Change("default")
                .GetBodyGetComponentOrNull<SkeletonAnimation>("default");
            if (ani.AnimationName != "move")
            {
                ani.AnimationState.SetAnimation(0,"move", true);
            }
        }
        else if (enemyType == EEnemyType.Hedgehog)
        {
            blackboard.GetProperty("out_bodyChanger", out AnimationBodyChanger bodyChanger);
            bodyChanger
                .Change("move")
                .GetBodyGetComponentOrNull<Animator>("move")?
                .SetTrigger("run");
        }
    }

    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        bool gotoPatrollSpace = EnemyPatrollState.ReadyGotoPatroll(blackboard);
        bool gotoSwingState = EnemySwingState.ReadyGotoSwing(blackboard);
        bool gotoProgagating = EnemyPropagatingState.ReadyGotoPropagating(blackboard);
        bool gotoDetection = EnemyDetectionState.GotoDetection(blackboard);

        
        blackboard.GetProperty<Transform>("out_transform", out var transform);
        blackboard.GetProperty<EnemyData>("out_enemyData", out var data);
        blackboard.GetUnWrappedProperty<EEnemyType>("out_enemyType", out var enemyType);
        float angle = TargetPoint.x - transform.position.x;
        if (angle >= 0f) angle = enemyType == EEnemyType.Hedgehog ? 0f : 180f;
        else angle = enemyType == EEnemyType.Hedgehog ? 180f : 0f;
        var rotation = Quaternion.Euler(0f, angle, 
            0f);

        transform.rotation = rotation;
        
        if (gotoProgagating)
        {
            executor.SetNextState<EnemyPropagatingState>();
        }
        else if (gotoSwingState)
        {
            executor.SetNextState<EnemySwingState>();
        }
        else if (!gotoPatrollSpace)
        {
            executor.SetNextState<EnemyNothingState>();
        }
        else if (gotoDetection)
        {
            executor.SetNextState<EnemyDetectionState>();
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
        bool gotoPropagation = EnemyPropagatingState.ReadyGotoPropagating(blackboard);

        if (gotoPropagation)
        {
            executor.SetNextState<EnemyPropagatingState>();
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
    public Vector2 Direction { get; set; }
    public InteractionController Interaction { get; private set; }
    public int MaxCount { get; set; }
    public float Force { get; set; }
    public PropagationInfo(InteractionController interaction)
    {
        Interaction = interaction;
    }

    public void Propagate(BaseContractInfo caller, Vector2 direction)
    {
        Count--;
        if (Count < 0) Count = 0;
        Direction = direction;
    }

    public void BeginPropagate(Vector2 direction)
    {
        Count = MaxCount;
        //Direction = direction;
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
        _propagationInfo.Force = _data.PropagationForce;
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
            info.TryGetBehaviour(out IBActorPhysics physics) &&
            _propagationInfo.Count > 0)
        {
            //physics.Stop();
            physics.AddForce(_propagationInfo.Direction * _propagationInfo.Force, ForceMode2D.Impulse);
            _propagationInfo.Direction = Vector2.zero;
        }
    }

    private void OnContractActor(ActorContractInfo info)
    {
        var blackboard = _cachedBlackboard;

        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        if (info.Transform.GetComponent<Enemy>() == null) return;

        if (
            info.TryGetBehaviour(out IBActorPhysics physics) &&
            info.TryGetBehaviour(out IBEnemyState state) &&
            info.TryGetBehaviour(out IBActorPropagation propagation) &&
            state.CheckCurrentState<EnemyPropagatingState>() &&
            interaction.TryGetContractInfo(out ActorContractInfo myInfo) &&
            myInfo.TryGetBehaviour(out IBEnemyState myState)
        )
        {
            blackboard.GetProperty("out_transform", out Transform transform);
            Vector3 v = (info.Transform.position - transform.position).normalized;
            v = Vector3.Project(v, Vector3.right).normalized;

            if (myState.CheckCurrentState<EnemySwingState>() || myState.CheckCurrentState<EnemyPatrollState>() || myState.CheckCurrentState<EnemyAttackState>())
            {
                _propagationInfo.Count = propagation.Count;
                _propagationInfo.Propagate(interaction.ContractInfo, -v);
            }
            else if (myState.CheckCurrentState<EnemyPropagatingState>())
            {
                _propagationInfo.Propagate(interaction.ContractInfo, -v);
            }
            else
            {
                return;
            }

            EffectManager.ImmediateCommand(new EffectCommand()
            {
                EffectKey = "actor/enemyHit",
                Position = Vector3.Lerp(transform.position, info.Transform.position, 0.5f)
            });

            if (_propagationInfo.Count > 0)
            {
                physics.Stop();
                physics.AddForce(v * (0.75f*_data.PropagationForce), ForceMode2D.Impulse);
            }
            propagation.Count -= 1;
        }
    }

    public static bool ReadyGotoPropagating(Blackboard blackboard)
    {
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        blackboard.GetProperty<PropagationInfo>("out_propagationInfo", out var propagationInfo);

        if (
            interaction.ContractInfo is ActorContractInfo myInfo &&
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
    public override void Enter(Blackboard blackboard)
    {
        blackboard.GetUnWrappedProperty("out_enemyType", out EEnemyType enemyType);

        if (enemyType == EEnemyType.Sheep)
        {
            blackboard.GetProperty("out_skeletonAnimation", out SkeletonAnimation ani);
            ani.AnimationState.SetAnimation(0, "ide", true);
        }
        else if (enemyType == EEnemyType.Hedgehog)
        {
        }
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

public class EnemyDetectionState : BaseState
{
    public override void Enter(Blackboard blackboard)
    {
        blackboard.GetUnWrappedProperty<bool>("out_isCaught", out var isCaught);
        if (isCaught) return;
        
        blackboard.GetUnWrappedProperty("out_enemyType", out EEnemyType enemyType);
        if (enemyType == EEnemyType.Hedgehog)
        {
            blackboard.GetProperty("out_bodyChanger", out AnimationBodyChanger bodyChanger);
            bodyChanger
                .Change("move")
                .GetBodyGetComponentOrNull<Animator>("move")?
                .SetTrigger("run");
        }
        else
        {
            blackboard.GetProperty("out_bodyChanger", out AnimationBodyChanger bodyChanger);
            var ani = bodyChanger
                .Change("default")
                .GetBodyGetComponentOrNull<SkeletonAnimation>("default");
            if (ani.AnimationName != "move")
            {
                ani.AnimationState.SetAnimation(0,"move", true);
            }
        }
    }
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        
        bool gotoAttack = EnemyAttackState.GotoAttack(blackboard);
        bool gotoSwingState = EnemySwingState.ReadyGotoSwing(blackboard);
        bool gotoProgagating = EnemyPropagatingState.ReadyGotoPropagating(blackboard);
        bool gotoDetection = EnemyDetectionState.GotoDetection(blackboard);

        blackboard.GetProperty<Transform>("out_transform", out var transform);
        blackboard.GetProperty<EnemyData>("out_enemyData", out var data);
        var playerCollider = EnemyDetectionState.GetPlayerOrNull(transform.position, data.DetectionDistance);
        if (playerCollider)
        {
            blackboard.GetUnWrappedProperty<EEnemyType>("out_enemyType", out var enemyType);
            float angle = playerCollider.transform.position.x - transform.position.x;
            if (angle >= 0f) angle = enemyType == EEnemyType.Hedgehog ? 0f : 180f;
            else angle = enemyType == EEnemyType.Hedgehog ? 180f : 0f;
            var rotation = Quaternion.Euler(0f, angle, 
                0f);
            transform.rotation = rotation;
        }
        
        if (gotoProgagating)
        {
            executor.SetNextState<EnemyPropagatingState>();
        }
        else if (gotoSwingState)
        {
            executor.SetNextState<EnemySwingState>();
        }
        else if(gotoAttack)
        {
            executor.SetNextState<EnemyAttackState>();
        }
        else if (!gotoDetection)
        {
            executor.SetNextState<EnemyPatrollState>();
        }
        else
        {
            if (!TracePlayer(blackboard))
            {
                executor.SetNextState<EnemyPatrollState>();
            }
        }

        return false;
    }

    private bool TracePlayer(Blackboard blackboard)
    {
        blackboard.GetProperty<EnemyData>("out_enemyData", out var data);
        blackboard.GetProperty<Transform>("out_transform", out var transform);
        var playerCollider = GetPlayerOrNull(transform.position, data.DetectionDistance);

        if (!playerCollider) return false;

        Vector2 dir = (playerCollider.transform.position - transform.position);
        dir.y = 0f;
        dir = dir.normalized;

        transform.position += (Vector3)(data.MovementSpeed * Time.deltaTime * dir);

        return true;
    }

    public static Collider2D GetPlayerOrNull(Vector2 myPos, float radius)
    {
        var playerCollider = Physics2D.OverlapCircle(
            myPos,
            radius,
            LayerMask.GetMask("Player"));

        return playerCollider;
    }

    public static bool GotoDetection(Blackboard blackboard)
    {
        blackboard.GetProperty<EnemyData>("out_enemyData", out var data);
        blackboard.GetProperty<Transform>("out_transform", out var transform);

        return GetPlayerOrNull(transform.position, data.DetectionDistance) != null;
    }
}
public class EnemyAttackState : BaseState
{
    private bool _isAttackEnded;
    private SkeletonAnimation _ani;
    private const string ANI_ATTACK_SHEEP_KEY = "attack";
    private const string ANI_ATTACK_HEDGEHOG_BEFORE_KEY = "before_Attack";
    private const string ANI_ATTACK_HEDGEHOG_REMAIN_KEY = "Attack";
    private const string ANI_ATTACK_HEDGEHOG_AFTER_KEY = "after_Attack";
    private float _hedgehogAttackDurationTimer = 0f;
    private float _hedgehogAttackTickTimer = 0f;
    private HedgehogEffect _flameEffectLeft;
    private HedgehogEffect _flameEffectRight;
    private HedgehogEffect _waterEffectLeft;
    private HedgehogEffect _waterEffectRight;
    private bool _attackLeft = false;
    public override void Init(Blackboard blackboard)
    {
        blackboard.GetProperty("out_skeletonAnimation", out SkeletonAnimation ani);
        _ani = ani;
        _isAttackEnded = true;
        ani.AnimationState.Complete += trackEntry =>
        {
            if(trackEntry.Animation.Name == ANI_ATTACK_SHEEP_KEY)
            {
                _isAttackEnded = true;
            }
            if(trackEntry.Animation.Name == ANI_ATTACK_HEDGEHOG_AFTER_KEY)
            {
                _isAttackEnded = true;
                blackboard.GetProperty("out_bodyChanger", out AnimationBodyChanger bodyChanger);
                bodyChanger.Change("move");
            }
        };

        _flameEffectLeft = EffectManager.EffectItem("actor/hedgehog_flame_left").EffectObject.GetComponent<HedgehogEffect>();
        _flameEffectRight = EffectManager.EffectItem("actor/hedgehog_flame_right").EffectObject.GetComponent<HedgehogEffect>();
        _waterEffectLeft = EffectManager.EffectItem("actor/hedgehog_water_left").EffectObject.GetComponent<HedgehogEffect>();
        _waterEffectRight = EffectManager.EffectItem("actor/hedgehog_water_right").EffectObject.GetComponent<HedgehogEffect>();
    }

    private HedgehogEffect GetCurrentHedgehogEffect(Blackboard blackboard)
    {
        blackboard.GetProperty<EnemyData>("out_enemyData", out var data);
        blackboard.GetProperty<Transform>("out_transform", out var transform);
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        var playerCollider = EnemyDetectionState.GetPlayerOrNull(transform.position, data.AttackDistance);
        if (playerCollider == false) return null;

        float angle = playerCollider.transform.position.x - transform.position.x;
        bool playerOnLeft = angle < 0f;
        if (angle >= 0f) angle = 0f;
        else angle = 180f;
        var rotation = Quaternion.Euler(0f, angle, 
            0f);
        HedgehogEffect currentEffect = null;
            
        if (interaction.TryGetContractInfo(out ActorContractInfo info) &&
            info.TryGetBehaviour(out IBActorProperties properties))
        {
            if (properties.Properties == EActorPropertiesType.Flame)
            {
                currentEffect = playerOnLeft ? _flameEffectLeft : _flameEffectRight;
            }
            else
            {
                currentEffect = playerOnLeft ? _waterEffectLeft : _waterEffectRight;
            }
            
            var effectPos = transform.position + (rotation * data.HedgehogAttackOffset);
            currentEffect.transform.position = effectPos;
        }

        return currentEffect;
    }
    public override void Enter(Blackboard blackboard)
    {
        blackboard.GetProperty<Transform>("out_transform", out var transform);
        blackboard.GetProperty<EnemyData>("out_enemyData", out var data);
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        blackboard.GetProperty("out_skeletonAnimation", out SkeletonAnimation ani);
        blackboard.GetUnWrappedProperty("out_enemyType", out EEnemyType enemyType);
        
        var playerCollider = EnemyDetectionState.GetPlayerOrNull(transform.position, data.AttackDistance);
        if (playerCollider == false) return;

        float angle = playerCollider.transform.position.x - transform.position.x;
        _attackLeft = angle < 0f;
        if (angle >= 0f) angle = enemyType == EEnemyType.Hedgehog ? 0f : 180f;
        else angle = enemyType == EEnemyType.Hedgehog ? 180f : 0f;
        var rotation = Quaternion.Euler(0f, angle, 
            0f);
        
        if (enemyType == EEnemyType.Sheep)
        {
            ani.transform.rotation = rotation;
            ani.AnimationState.SetAnimation(0, ANI_ATTACK_SHEEP_KEY, false);
            _isAttackEnded = false;
        }
        else if(enemyType == EEnemyType.Hedgehog)
        {
            blackboard.GetProperty("out_bodyChanger", out AnimationBodyChanger bodyChanger);
            bodyChanger.Change("default");
            ani.transform.rotation = rotation;
            ani.AnimationState.SetAnimation(0, ANI_ATTACK_HEDGEHOG_BEFORE_KEY, false);
            ani.AnimationState.AddAnimation(0, ANI_ATTACK_HEDGEHOG_REMAIN_KEY, true, 0f);
            _hedgehogAttackDurationTimer = 0f;
            _isAttackEnded = false;
            dirty = false;

            GetCurrentHedgehogEffect(blackboard)?.Play();
        }
        
    }

    private bool dirty = false;
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        bool gotoPropagation = EnemyPropagatingState.ReadyGotoPropagating(blackboard);
        blackboard.GetUnWrappedProperty("out_isSpineHitEvent", out bool isSpineHitEvent);
        blackboard.GetUnWrappedProperty<bool>("out_isCaught", out var isCaught);
        blackboard.GetProperty<InteractionController>("out_interaction", out var interaction);
        
        bool gotoSwing = EnemySwingState.ReadyGotoSwing(blackboard);
        
        if (gotoSwing)
        {
            _isAttackEnded = true;
            executor.SetNextState<EnemySwingState>();
            Debug.Log("pro");
            return false;
        }
        if (gotoPropagation)
        {
            _isAttackEnded = true;
            executor.SetNextState<EnemyPropagatingState>();
            Debug.Log("pro");
            return false;
        }
        
        if (interaction.TryGetContractInfo(out ActorContractInfo info) &&
            info.TryGetBehaviour(out IBActorPhysics physics) &&
            physics.IsSwingState)
        {
            var cf = GetCurrentHedgehogEffect(blackboard);
            if (cf)
            {
                cf.transform.position = Vector2.one * 99999f;
            }
            _isAttackEnded = true;
            return false;
        }

        if (isSpineHitEvent)
        {
            var cf = GetCurrentHedgehogEffect(blackboard);
            if (cf)
            {
                cf.transform.position = Vector2.one * 99999f;
            }
            _isAttackEnded = true;
            return false;
        }
        if (isCaught)
        {
            var cf = GetCurrentHedgehogEffect(blackboard);
            if (cf)
            {
                cf.transform.position = Vector2.one * 99999f;
            }
            _isAttackEnded = true;
        }
            
        if (_isAttackEnded == false)
        {
            blackboard.GetUnWrappedProperty("out_enemyType", out EEnemyType enemyType);
            if (enemyType == EEnemyType.Hedgehog)
            {
                _hedgehogAttackDurationTimer += Time.deltaTime;

                GetCurrentHedgehogEffect(blackboard);
            
                blackboard.GetProperty<EnemyData>("out_enemyData", out var data);
                if (_hedgehogAttackDurationTimer > data.HedgehogAttackDuration && dirty == false)
                {
                    dirty = true;
                    blackboard.GetProperty("out_skeletonAnimation", out SkeletonAnimation ani);
                    ani.AnimationState.SetAnimation(0, ANI_ATTACK_HEDGEHOG_AFTER_KEY, false);
                }
                else
                {
                    if (_hedgehogAttackTickTimer > data.HedgehogAttackTick)
                    {
                        DoHitHedgehog(blackboard);
                        _hedgehogAttackTickTimer = 0f;
                    }
                    else
                    {
                        _hedgehogAttackTickTimer += Time.deltaTime;
                    }
                }
            }
            else if (enemyType == EEnemyType.Sheep)
            {
                blackboard.GetWrappedProperty<bool>("out_isSpineAttackEvent", out var attackEvent);
                if (attackEvent)
                {
                    DoHitSheep(blackboard);
                    attackEvent.Value = false;
                }
            }
            
            return false;
        }
        bool gotoDetection = EnemyDetectionState.GotoDetection(blackboard);
        bool gotoPatroll = EnemyPatrollState.ReadyGotoPatroll(blackboard);

        if (gotoDetection)
        {
            executor.SetNextState<EnemyDetectionState>();
        }
        else if (gotoPatroll)
        {
            executor.SetNextState<EnemyPatrollState>();
        }
        
        return false;
    }

    public override void Exit(Blackboard blackboard)
    {
        blackboard.GetUnWrappedProperty("out_enemyType", out EEnemyType enemyType);
        
        // TODO:  공격 모션 종료 코드 삽입

        _isAttackEnded = true;
    }

    private void DoHitSheep(Blackboard blackboard)
    {
        blackboard.GetProperty<EnemyData>("out_enemyData", out var data);
        blackboard.GetProperty<Transform>("out_transform", out var transform);
        
        var playerCollider = EnemyDetectionState.GetPlayerOrNull(transform.position, data.AttackDistance);

        if (!playerCollider) return;
        if (playerCollider.TryGetComponent(out InteractionController interaction) &&
            interaction.TryGetContractInfo(out ActorContractInfo info) &&
            info.TryGetBehaviour(out IBActorHit hit))
        {
            blackboard.GetProperty<InteractionController>("out_interaction", out var myInteraction);
            hit.DoHit(myInteraction.ContractInfo, data.Damage);
        }
    }
    private void DoHitHedgehog(Blackboard blackboard)
    {
        blackboard.GetProperty<EnemyData>("out_enemyData", out var data);
        blackboard.GetProperty<Transform>("out_transform", out var transform);
        var playerCollider = EnemyDetectionState.GetPlayerOrNull(transform.position, data.AttackDistance);
        
        if (!playerCollider) return;


        if (playerCollider.transform.position.x - transform.position.x < 0f != _attackLeft ||
            playerCollider.transform.position.y - (transform.position.y + data.HedgehogAttackOffset.y) > 0.5f)
        {
            return;
        }
        
        if (playerCollider.TryGetComponent(out InteractionController interaction) &&
            interaction.TryGetContractInfo(out ActorContractInfo info) &&
            info.TryGetBehaviour(out IBActorHit hit))
        {
            blackboard.GetProperty<InteractionController>("out_interaction", out var myInteraction);
            hit.DoHit(myInteraction.ContractInfo, data.Damage);
        }
    }
    public static bool GotoAttack(Blackboard blackboard)
    {
        blackboard.GetUnWrappedProperty<bool>("out_isCaught", out var isCaught);
        if (isCaught) return false;

        blackboard.GetProperty<EnemyData>("out_enemyData", out var data);
        blackboard.GetProperty<Transform>("out_transform", out var transform);

        var playerCollider = Physics2D.OverlapCircle(
            transform.position,
            data.AttackDistance,
            LayerMask.GetMask("Player"));

        if (!playerCollider) return false;
        return true;
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
        else if (gotoSleep)
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