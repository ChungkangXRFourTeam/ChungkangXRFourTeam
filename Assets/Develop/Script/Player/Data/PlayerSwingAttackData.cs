
using UnityEngine;

[CreateAssetMenu(menuName="XR/Player/PlayerSwingAttack", fileName="PlayerSwingAttackData ", order = 3)]
public class PlayerSwingAttackData : ScriptableObject
{
    [SerializeField] private float _swingForce;
    [SerializeField] private float _minmumCloseDistance;
    [SerializeField] private float _timeScale;
    [SerializeField] private float _grabDistance;
    [SerializeField] private float _coolTime;
    [SerializeField] private float _coolTimeBoss;
    [SerializeField] private Vector2 _grabOffset;

    public float SwingForce => _swingForce;
    public float MinmumCloseDistance => _minmumCloseDistance;
    public float TimeScale => _timeScale;
    public float GrabDistance => _grabDistance;
    public float CoolTime => _coolTime;
    public float CoolTimeBoss => _coolTimeBoss;
    public Vector2 GrabOffset => _grabOffset;
}