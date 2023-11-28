using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using Range = UnityEngine.SocialPlatforms.Range;

public class EndingEvent : ITalkingEvent
{
    private string _sceneName;
    private GameObject _player;
    private GameObject _kennel;
    private GameObject _upWall;
    private GameObject _observer;
    private GameObject _boss;
    private GameObject _sceneChangeImage;

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

    private PlayerEventAnimationController _eventAnimationController;
    
    protected List<Dictionary<string, object>> _eventTexts;
    protected TalkingPanelInfo _playerPanel;
    protected TalkingPanelInfo _targetPanel;
    protected string _scriptPath = "EventTextScript/";
    protected  List<string> _comments;
    protected int _textCount;
    
    public async UniTask OnEventBefore()
    {
        _scriptPath += "Ending/FinalEnd1";
        _eventTexts = CSVReader.Read(_scriptPath);
        _comments = new List<string>();
        for (int i = 0; i < _eventTexts.Count; i++)
        {
            _comments.Add(_eventTexts[i][EventTextType.Content.ToString()].ToString());
        }
        _textCount = 0;
        
        _player = GameObject.FindGameObjectWithTag("Player");
        _observer = GameObject.FindGameObjectWithTag("Boss");

        _playerPanel = _player.GetComponent<TalkingPanelInfo>();
        _targetPanel = _observer.GetComponent<TalkingPanelInfo>();

        _eventAnimationController = _player.GetComponent<PlayerEventAnimationController>();
        _eventAnimationController.EnableEventAnimatorController();
        _player.transform.Rotate(0,180,0);
        _eventAnimationController.PlayEventAnim(EventAnimationName.IDLE);
        
        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
 
        await UniTask.Yield();
    }

    public async UniTask OnEvent()
    {
        ShakeCamera().Forget();
        
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
        
        InputManager.Instance.DisableTalkEventAction();
        InputManager.Instance.InitMainGameAction();
        
    }

    public async UniTask MoveToPosition(GameObject target, Vector2 posistion, float speed)
    {
        Vector2 dir = target.transform.position.x - posistion.x > 0 ? Vector2.left : Vector2.right;
        
        float fliped = dir.x > 0 ? 180 : 0;
        while (Mathf.Abs(target.transform.position.x - posistion.x) >= 0.1f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        target.transform.Rotate(0,0,0);
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

    async UniTaskVoid ShakeCamera()
    {
        while (!TalkingEventManager._isEventEnd)
        {
            float NextShakeTime = Random.Range(3f, 5f);

            await UniTask.Delay(TimeSpan.FromSeconds(NextShakeTime));

            float shakeDuration = Random.Range(1f, 3f);
            float shakeIntensity = 5f;
            float shakeFrequency = 1f;
            VirtualCameraShaker.Instance.CameraShake(shakeDuration,shakeIntensity,shakeFrequency);
            
            await UniTask.Delay(TimeSpan.FromSeconds(shakeDuration));
        }
    }
}
