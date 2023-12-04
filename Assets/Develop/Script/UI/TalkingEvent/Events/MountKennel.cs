using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using Spine.Unity;

public class MountKennelEvent : ITalkingEvent
{
    private string _sceneName;
    private GameObject _player;
    private GameObject _kennel;
    private GameObject _upWall;

    private SkeletonAnimation _kennelAnimation;
    
    private CinemachineVirtualCamera _virtualCamera;

    private Vector2 _kennelPos;
    private Vector2 _kennelEnd;

    private PolygonCollider2D _confiner;
    private CinemachineConfiner _virtualCameraConfiner;

    private Rigidbody2D _playerRigid;
    private Rigidbody2D _kennelRigid;

    private PlayerEventAnimationController _playerEventAnimationController;

    public MountKennelEvent(string sceneName)
    {
        _sceneName = sceneName;
    }
    
    public async UniTask OnEventBefore()
    {
        _virtualCamera = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        _virtualCameraConfiner = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineConfiner>();
        _confiner = GameObject.Find("Confiner").GetComponent<PolygonCollider2D>();
        _upWall = GameObject.Find("UpWall");
        _upWall.GetComponent<BoxCollider2D>().enabled = false;
        InputManager.Instance.DisableMainGameAction();
        InputManager.Instance.InitTalkEventAction();
        Vector2[] paths = _confiner.points;
        for (int i = 0; i < paths.Length; i++)
        {
            if (paths[i].y > 35f)
            {
                paths[i] = new Vector2(paths[i].x,300);
            }
        }
        _confiner.SetPath(0,paths);
        
        _player = GameObject.FindWithTag("Player");
        _kennel = GameObject.FindWithTag("Kennel");
        
        _player.transform.rotation = Quaternion.identity;

        _kennelPos = _kennel.transform.position;
        _kennelEnd = GameObject.Find("KennelEnd").transform.position;
        _playerRigid = _player.GetComponent<Rigidbody2D>();
        _kennelRigid = _kennel.GetComponent<Rigidbody2D>();

        _kennelAnimation = _kennel.GetComponent<SkeletonAnimation>();
        _playerEventAnimationController = _player.GetComponent<PlayerEventAnimationController>();

        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        _playerEventAnimationController.EnableEventAnimatorController();
        
        await MoveToPosition(_player, _kennelPos, 0.1F);
    }

    public async UniTask OnEvent()
    {
        _kennelRigid.bodyType = RigidbodyType2D.Kinematic;
        _playerRigid.bodyType = RigidbodyType2D.Kinematic;
        Transform kennelPivot = _kennel.transform.GetChild(1);
        
        
        _player.transform.rotation = Quaternion.Euler(new Vector3(0,0,180));
        _playerRigid.excludeLayers = Physics2D.AllLayers - (1 << 6 | 1 << 17);
        _kennelRigid.excludeLayers = Physics2D.AllLayers - (1 << 6 | 1 << 17);
        
        _virtualCameraConfiner.m_ConfineScreenEdges = false;
        _playerEventAnimationController.PlayEventAnim(EventAnimationName.FALLING_DASH);

        _kennelAnimation.AnimationName = "Up";
        float floatingTime = 1.0f;
        while (floatingTime > 0f)
        {
            floatingTime -= Time.unscaledDeltaTime;
            _kennelRigid.position += Vector2.up * Time.unscaledDeltaTime;
            _playerRigid.position = kennelPivot.position;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        //_virtualCamera.Follow = null;
        kennelPivot = _kennel.transform.GetChild(0);
        _player.transform.position = kennelPivot.position;
        while (Mathf.Abs(_kennelEnd.y - _kennel.transform.position.y) >= 5f)
        {
            _kennelRigid.position += Vector2.up * 5f;
            _playerRigid.position = kennelPivot.position;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        float dt = 0f;
        while (_player.transform.rotation.eulerAngles.z >= 2)
        {
            dt += Time.unscaledDeltaTime;
            _player.transform.Rotate(0,0,-2);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        AsyncOperation result = SceneManager.LoadSceneAsync(_sceneName);
        while (!result.isDone)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Time.fixedTime));
            
        }

        TalkingEventManager.Instance._isEventEnd = true;

        await UniTask.Yield();

    }
    
    public async UniTask OnEventEnd()
    {
        
    }

    public async UniTask MoveToPosition(GameObject target, Vector2 posistion, float speed)
    {
        Vector2 dir = target.transform.position.x - posistion.x > 0 ? Vector2.left : Vector2.right;
        float fliped = dir.x > 0 ? 180 : 0;
        _playerEventAnimationController.PlayEventAnim(EventAnimationName.RUN);
        _player.transform.rotation = Quaternion.Euler(new Vector3(0,fliped,0));
        while (Mathf.Abs(target.transform.position.x - posistion.x) >= 0.1f)
        {
            if (target.CompareTag("Player"))
                _playerRigid.position += speed * dir;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        target.transform.Rotate(0,0,0);
        _playerEventAnimationController.PlayEventAnim(EventAnimationName.IDLE);
    }

    public bool IsInvalid()
    {
        return true;
    }
}
