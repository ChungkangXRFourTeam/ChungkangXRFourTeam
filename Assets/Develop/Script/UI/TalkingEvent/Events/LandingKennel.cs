using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Spine.Unity;
using TMPro;
using UnityEngine.InputSystem;

public class LandingKennelEvent : ITalkingEvent
{
    private string _sceneName;
    private GameObject _player;
    private GameObject _kennel;
    private GameObject _upWall;

    private SkeletonAnimation _kennelAnimation;
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFramingTransposer _cinemachineFramingTransposer;
    private CinemachineConfiner _cinemachineConfiner;
    
    private Vector2 _kennelPos;
    private Vector2 _kennelEnd;

    private Transform startPos;
    private PolygonCollider2D _confiner;

    private Rigidbody2D _playerRigid;
    private Rigidbody2D _kennelRigid;

    private PlayerEventAnimationController _eventAnimationController;
    
    protected List<Dictionary<string, object>> _eventTexts;
    protected TalkingPanelInfo _playerPanel;
    protected string _scriptPath = "EventTextScript/";
    protected  List<string> _comments;
    protected int _textCount;
    
    public async UniTask OnEventBefore()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "ThemeA_1" || sceneName == "ThemeB_1")
            _scriptPath += "Other/FirstMapEntry";
        _eventTexts = CSVReader.Read(_scriptPath);
        _comments = new List<string>();
        
        for (int i = 0; i < _eventTexts.Count; i++)
        {
            _comments.Add(_eventTexts[i][EventTextType.Content.ToString()].ToString());
        }
        
        await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        _virtualCamera = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        _cinemachineFramingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _cinemachineConfiner = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineConfiner>();
        _confiner = GameObject.Find("Confiner").GetComponent<PolygonCollider2D>();
        _cinemachineConfiner.m_BoundingShape2D = null;
        _cinemachineFramingTransposer.m_YDamping = 0;
        
        _upWall = GameObject.Find("UpWall");
        InputManager.Instance.DisableMainGameAction();
        InputManager.Instance.InitTalkEventAction();

        startPos = GameObject.FindWithTag("ThemeStart").transform;
        
        _player = GameObject.FindWithTag("Player");
        _playerPanel = GameObject.FindGameObjectWithTag("Player").GetComponent<TalkingPanelInfo>();
        _kennel = GameObject.FindWithTag("Kennel");
        _kennelAnimation = _kennel.GetComponent<SkeletonAnimation>();
        _kennelAnimation.AnimationName = "Down";
        _kennelPos = _kennel.transform.position;
        _playerRigid = _player.GetComponent<Rigidbody2D>();
        _kennelRigid = _kennel.GetComponent<Rigidbody2D>();

        _eventAnimationController = _player.GetComponent<PlayerEventAnimationController>();
        _eventAnimationController.EnableEventAnimatorController();

        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        
        _eventAnimationController.PlayEventAnim(EventAnimationName.FALLING_DASH);
        
        _playerRigid.excludeLayers = Physics2D.AllLayers - (1 << 6 | 1 << 17);
        _kennelRigid.excludeLayers = Physics2D.AllLayers - (1 << 6 | 1 << 17);

        _playerRigid.bodyType = RigidbodyType2D.Static;
        _kennelRigid.bodyType = RigidbodyType2D.Static;
        

    }

    public async UniTask OnEvent()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));


        _player.transform.rotation = quaternion.identity;
        
        _playerRigid.bodyType = RigidbodyType2D.Dynamic;
        _kennelRigid.bodyType = RigidbodyType2D.Dynamic;
        await UniTask.WaitUntil(() => _player.transform.position.y < startPos.position.y + 5f);
        _cinemachineFramingTransposer.m_YDamping = 1;
        _cinemachineConfiner.m_BoundingShape2D = _confiner;
        _playerRigid.excludeLayers = 0;
        _kennelRigid.excludeLayers = 0;
        
        _eventAnimationController.PlayEventAnim(EventAnimationName.FALLING_LAND);
        _kennelAnimation.AnimationName = "None";
        await UniTask.Delay(TimeSpan.FromSeconds(0.7f));

        
        await MoveToPosition(_player, new Vector2(_player.transform.position.x + 5, 0),0.1f);
        
        await UniTask.Yield();
        
    }
    
    public async UniTask OnEventEnd()
    {
        _kennelRigid.excludeLayers = Physics2D.AllLayers - (1 << 6 | 1 << 17);
        _kennelRigid.bodyType = RigidbodyType2D.Static;
        _kennelAnimation.AnimationName = "Up";
        float floatingTime = 1.0f;
        while (floatingTime > 0f)
        {
            floatingTime -= Time.unscaledDeltaTime;
            _kennelRigid.position += Vector2.up * Time.unscaledDeltaTime;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        while (Mathf.Abs(_kennelEnd.y - _kennel.transform.position.y) >= 5f)
        {
            _kennelRigid.position += Vector2.up * 5f;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }


        InputAction action = InputManager.GetTalkEventAction("NextText");

        if (action != null)
        {
            string[] contents = _comments.ToArray();
            _playerPanel._panel.SetActive(true);
            _playerPanel._endButton.SetActive(false);
            if(_playerPanel._eventText.TryGetComponent(out TextMeshProUGUI playerComponent)) 
                TypingSystem.Instance.Typing(contents,playerComponent);
        
            await UniTask.WaitUntil(() => TypingSystem.Instance.isTypingEnd);
            _playerPanel._endButton.SetActive(true);
        }
        
        await UniTask.WaitUntil(() => action.WasPressedThisFrame());
        _playerPanel._panel.SetActive(false);
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        
        _cinemachineFramingTransposer.m_YDamping = 1;
        
        startPos.gameObject.SetActive(false);
        GameObject.Destroy(_kennel);
        
        _eventAnimationController.DisableEventAnimatorController();
        InputManager.Instance.DisableTalkEventAction();
        InputManager.Instance.InitMainGameAction();
        
    }

    public async UniTask MoveToPosition(GameObject target, Vector2 posistion, float speed)
    {
        Vector2 dir = target.transform.position.x - posistion.x > 0 ? Vector2.left : Vector2.right;
        float fliped = dir.x > 0 ? 180 : 0;
        _player.transform.rotation = Quaternion.Euler(0,fliped,0);
        _eventAnimationController.PlayEventAnim(EventAnimationName.RUN);
        while (Mathf.Abs(target.transform.position.x - posistion.x) >= 0.1f)
        {
            if (target.CompareTag("Player"))
                _playerRigid.position += speed * dir;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        target.transform.Rotate(0,0,0);
        _eventAnimationController.PlayEventAnim(EventAnimationName.IDLE);
    }

    public bool IsInvalid()
    {
        return true;
    }
}
