using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LandingKennelBossEvent : ITalkingEvent
{
    private string _sceneName;
    private GameObject _player;
    private GameObject _kennel;
    private GameObject _upWall;
    private GameObject _observer;
    private GameObject _boss;
    private GameObject _sceneChangeImage;
    private GameObject _door;
    private SpriteRenderer _kennelRenderer;
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFramingTransposer _cinemachineFramingTransposer;
    private CinemachineConfiner _cinemachineConfiner;

    
    private BezierTransform _observerBezier;
    private BezierTransform _playerBezier;
    
    private Vector2 _kennelPos;
    private Vector2 _kennelEnd;

    private Transform startPos;
    private PolygonCollider2D _confiner;

    private Rigidbody2D _playerRigid;
    private Rigidbody2D _kennelRigid;

    private PlayerAnimationController _playerAnimationController;
    private PlayerEventAnimationController _playerEventAnimationController;
    
    protected List<Dictionary<string, object>> _eventTexts;
    protected TalkingPanelInfo _playerPanel;
    protected TalkingPanelInfo _targetPanel;
    protected string _scriptPath = "EventTextScript/";
    protected  List<string> _comments;
    protected int _textCount;
    
    public async UniTask OnEventBefore()
    { 
        _scriptPath += "Ending/OpenEnd1";
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
        
        _sceneChangeImage = GameObject.Find("SceneChange");
        _upWall = GameObject.Find("UpWall");
        _boss = GameObject.FindWithTag("Boss");
        _boss.SetActive(false);
        InputManager.Instance.DisableMainGameAction();
        InputManager.Instance.InitTalkEventAction();

        startPos = GameObject.FindWithTag("ThemeStart").transform;
        
        _player = GameObject.FindWithTag("Player");
        _observer = GameObject.FindWithTag("Observer");

        _playerEventAnimationController = _player.GetComponent<PlayerEventAnimationController>();
        
        _observerBezier = _observer.GetComponent<BezierTransform>();
        _playerBezier = _player.GetComponent<BezierTransform>();
        
        _playerPanel = GameObject.FindGameObjectWithTag("Player").GetComponent<TalkingPanelInfo>();
        _targetPanel = GameObject.FindWithTag("Observer").GetComponent<TalkingPanelInfo>();
        _kennel = GameObject.FindWithTag("Kennel");
        _kennelPos = _kennel.transform.position;
        _playerRigid = _player.GetComponent<Rigidbody2D>();
        _kennelRigid = _kennel.GetComponent<Rigidbody2D>();

        _playerAnimationController = _player.GetComponent<PlayerAnimationController>();
        _door = GameObject.Find("Door");
        _door.SetActive(false);

        _playerBezier.simulated = true;
        _playerBezier.enabled = false;
        _observerBezier.simulated = true;
        _observerBezier.enabled = false;
        
        GameObject fade = GameObject.FindWithTag("Fade");
        fade.GetComponent<Image>().color = Color.white;
        

        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
        _playerEventAnimationController.EnableEventAnimatorController();
        _playerEventAnimationController.PlayEventAnim(EventAnimationName.FALLING_DASH);
        
        await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        
        _playerRigid.excludeLayers = Physics2D.AllLayers - (1 << 6 | 1 << 17);
        _kennelRigid.excludeLayers = Physics2D.AllLayers - (1 << 6 | 1 << 17);

        _playerRigid.bodyType = RigidbodyType2D.Static;
        _kennelRigid.bodyType = RigidbodyType2D.Static;
        

    }

    public async UniTask OnEvent()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        

        _player.transform.rotation = quaternion.Euler(0, 0, 0);
        
        _playerRigid.bodyType = RigidbodyType2D.Dynamic;
        _kennelRigid.bodyType = RigidbodyType2D.Dynamic;
        await UniTask.WaitUntil(() => _player.transform.position.y < startPos.position.y + 5f);
        _cinemachineFramingTransposer.m_YDamping = 1;
        _cinemachineConfiner.m_BoundingShape2D = _confiner;
        _playerRigid.excludeLayers = 0;
        _kennelRigid.excludeLayers = 0;
        
        _sceneChangeImage.SetActive(false);
        _door.SetActive(true);
        
        _playerEventAnimationController.PlayEventAnim(EventAnimationName.FALLING_LAND);

        await UniTask.Delay(TimeSpan.FromSeconds(0.7f));

        await MoveToPosition(_player, new Vector2(_player.transform.position.x + 5, 0),0.1f);
        
        /*
        Color kennelColor = _kennelRenderer.color;
        
        
        while (_kennelRenderer.color.a >= 0)
        {
            _kennelRenderer.color = new Color(kennelColor.r, kennelColor.g, kennelColor.b, _kennelRenderer.color.a - 0.05f);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        */
        _kennel.SetActive(false);

        InputAction action = InputManager.GetTalkEventAction("NextText");
        if (action != null)
        {
            string[] contents = _comments.ToArray();
            while (_textCount != _comments.Count)
            {
                string target = _eventTexts[_textCount][EventTextType.Target.ToString()].ToString();
                Talk(contents,target);
                await UniTask.WaitUntil(() => TypingSystem.Instance.isTypingEnd);
                SetEndbutton(target);
                await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                ClosePanel(target);

                if (_textCount == 2)
                {
                    _observerBezier.enabled = true;
                    _playerBezier.enabled = true;
                    _playerRigid.simulated = false;
                    await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                    _observerBezier.startAnimation();
                    await UniTask.Delay(TimeSpan.FromSeconds(0.23f));
                    _playerBezier.startAnimation();
                    await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
                    _playerRigid.simulated = true;
                }
            }
            EventFadeChanger.Instance.FadeIn(2.0f);
            
            await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1.0f);
            
            _observer.SetActive(false);
            _boss.SetActive(true);
            
            EventFadeChanger.Instance.FadeOut(1.0f);
            
            await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha <= 0f);
            
        }
        
        await UniTask.Yield();
        
    }
    
    public async UniTask OnEventEnd()
    {
        
        _playerPanel._panel.SetActive(false);
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        
        _cinemachineFramingTransposer.m_YDamping = 1;
        
        startPos.gameObject.SetActive(false);
        
        _boss.SendMessage("EngaugeBoss",SendMessageOptions.DontRequireReceiver);
        
        InputManager.Instance.DisableTalkEventAction();
        InputManager.Instance.InitMainGameAction();
        
    }

    public async UniTask MoveToPosition(GameObject target, Vector2 posistion, float speed)
    {
        Vector2 dir = target.transform.position.x - posistion.x > 0 ? Vector2.left : Vector2.right;
        float fliped = dir.x > 0 ? 180 : 0;
        _player.transform.rotation = Quaternion.Euler(0,fliped,0);
        _playerEventAnimationController.PlayEventAnim(EventAnimationName.RUN);
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
    
    void Talk(string[] contents, string target)
    {
        _textCount++;
        switch (target)
        {
            case "Player" : 
                _playerPanel._panel.SetActive(true);
                _playerPanel._endButton.SetActive(false);
                if(_playerPanel._eventText.TryGetComponent(out TextMeshProUGUI playerComponent)) 
                    TypingSystem.Instance.Typing(contents,playerComponent);
                break;
            case "Observer" : 
                _targetPanel._panel.SetActive(true);
                _targetPanel._endButton.SetActive(false);
                if(_targetPanel._eventText.TryGetComponent(out TextMeshProUGUI observerComponent)) 
                    TypingSystem.Instance.Typing(contents,observerComponent);
                break;
                
        }
    }

    void SetEndbutton(string target)
    {
        switch (target)
        {
            case "Player" : 
                _playerPanel._endButton.SetActive(true);
                break;
            case "Observer" : 
                _targetPanel._endButton.SetActive(true);
                break;
                
        }
    }

    void ClosePanel(string target)
    {
        switch (target)
        {
            case "Player" : 
                _playerPanel._panel.SetActive(false);
                break;
            case "Observer" : 
                _targetPanel._panel.SetActive(false);
                break;
                
        }
    }
}
