using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

/*
 * Base interaction
 * --
 */
public interface IBaseBehaviour
{
    public InteractionController Interaction { get; }
}
public interface IObjectBehaviour : IBaseBehaviour
{
    
}
public interface IActorBehaviour : IBaseBehaviour
{
}


/*
 * Object interaction
 * --
 */
#region Object
public interface IBObjectInteractive : IObjectBehaviour
{
    public bool IsSelectiveObject {get; }
}
public interface IBObjectDiaryItem : IObjectBehaviour
{
    public void Collect();
    public int PageID { get; }
}

public interface IBObjectKnockback : IObjectBehaviour
{
    public EActorPropertiesType Properties { get; }
}
public interface IBObjectPatrollSpace : IObjectBehaviour
{
    public Vector2 LeftPoint { get; }
    public Vector2 RightPoint { get; }
}
#endregion



/*
 * Actor interaction
 * --
 */
#region Actor
public interface IBActorPhysics : IActorBehaviour
{
    public bool IsSwingState { get; }

    public void AddKnockback(Vector2 vector);
    public void AddForce(Vector2 force, ForceMode2D mode);
    public void Stop();
    public Transform GetTransformOrNull();
}

public interface IBActorHit : IActorBehaviour
{
    public void DoHit(BaseContractInfo caller, float damage);
}
public interface IBActorProperties : IActorBehaviour
{
    public EActorPropertiesType Properties { get; }
    public void SetProperties(BaseContractInfo caller, EActorPropertiesType type);
}

public interface IBActorLife : IActorBehaviour
{
    public float MaxHp { get; }
    public float CurrentHP { get; }
    public event System.Action<IBActorLife, float, float> ChangedHp;
    public void Die();
}

public interface IBActorPropagation : IActorBehaviour
{
    public int Count { get; set; }
    public int MaxCount { get; }
    public void Propagate(BaseContractInfo caller, Vector2 direction);
    public void BeginPropagate(Vector2 direction);
    public bool IsPropagation { get; }
}

public interface IBActorAttackable : IActorBehaviour
{
    public bool IsAttackable { get; set; }
}

public interface IBEnemyState : IActorBehaviour
{
    public bool CheckCurrentState<T>() where T : BaseState;
}

public interface IBActorThrowable : IActorBehaviour
{
    public bool IsThrowable { get; }
}
#endregion

