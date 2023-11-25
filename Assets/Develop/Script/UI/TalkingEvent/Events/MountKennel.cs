using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class MountKennelEvent : ITalkingEvent
{
    private string _sceneName;
    private GameObject _player;
    private GameObject _kennel;
    private GameObject _upWall;

    private CinemachineVirtualCamera _virtualCamera;

    private Vector2 _kennelPos;
    private Vector2 _kennelEnd;

    private PolygonCollider2D _confiner;
    private CinemachineConfiner _virtualCameraConfiner;

    private Rigidbody2D _playerRigid;
    private Rigidbody2D _kennelRigid;

    private PlayerAnimationController _playerAnimationController;

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
                paths[i] = new Vector2(paths[i].x,120);
            }
        }
        _confiner.SetPath(0,paths);
        
        _player = GameObject.FindWithTag("Player");
        _kennel = GameObject.FindWithTag("Kennel");

        _kennelPos = _kennel.transform.position;
        _kennelEnd = GameObject.Find("KennelEnd").transform.position;
        _playerRigid = _player.GetComponent<Rigidbody2D>();
        _kennelRigid = _kennel.GetComponent<Rigidbody2D>();

        _playerAnimationController = _player.GetComponent<PlayerAnimationController>();

        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        
        await MoveToPosition(_player, _kennelPos, 0.1F);
    }

    public async UniTask OnEvent()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
        
        //_virtualCamera.Follow = null;
        _kennelRigid.bodyType = RigidbodyType2D.Kinematic;
        _playerRigid.bodyType = RigidbodyType2D.Kinematic;
        
        _virtualCameraConfiner.m_ConfineScreenEdges = false;
        _playerAnimationController.SetState(new PAniState()
        {
            State = EPCAniState.Falling_Dash,
            Rotation = Quaternion.Euler(0,0,-180),
            Restart = false
        });
        
        while (Mathf.Abs(_kennelEnd.y - _kennel.transform.position.y) >= 5f)
        {
            _kennelRigid.position += Vector2.up * 5f;
            _playerRigid.position += Vector2.up * 5f;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        float dt = 0f;
        while (_player.transform.rotation.eulerAngles.z >= 2)
        {
            dt += Time.unscaledDeltaTime;
            _player.transform.Rotate(0,0,-2);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        AsyncOperation result =  SceneManager.LoadSceneAsync(_sceneName);
        
        TalkingEventManager.Instance._isEventEnd = true;
        while (!result.isDone)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Time.fixedTime));
            
        }

        await UniTask.Yield();

    }
    
    public async UniTask OnEventEnd()
    {
        
    }

    public async UniTask MoveToPosition(GameObject target, Vector2 posistion, float speed)
    {
        Vector2 dir = target.transform.position.x - posistion.x > 0 ? Vector2.left : Vector2.right;
        float fliped = dir.x > 0 ? 180 : 0;
        _playerAnimationController.SetState(new PAniState()
        {
            State = EPCAniState.Run,
            Rotation = Quaternion.Euler(0,fliped,0),
            Restart = false
        });
        while (Mathf.Abs(target.transform.position.x - posistion.x) >= 0.1f)
        {
            if (target.CompareTag("Player"))
                _playerRigid.position += speed * dir;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        target.transform.Rotate(0,0,0);
        _playerAnimationController.SetState(new PAniState()
        {
            State = EPCAniState.Idle,
            Rotation = Quaternion.identity,
            Restart = false
        });
    }

    public bool IsInvalid()
    {
        return true;
    }
}
