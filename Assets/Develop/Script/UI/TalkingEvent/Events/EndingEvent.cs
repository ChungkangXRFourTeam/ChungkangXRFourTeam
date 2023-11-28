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
using UnityEngine.UI;
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
    private Image _fadeImage;

    private Rigidbody2D _playerRigid;
    
    private GameObject _observerDead;

    private GameObject _endingCanvas;
    private GameObject _image00;
    private GameObject _image01;
    private GameObject _image02;
    private GameObject _image03;
    private GameObject _image04;
    private GameObject _image05;

    private Transform _imagePos00;
    private Transform _imagePos02;
    private Transform _imagePos04;

    private GameObject _leftDoor;
    private GameObject _rightDoor;
        
    private TalkingPanelInfo _nonTargetPanel;

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
        
        _leftDoor = GameObject.Find("Door_L");
        _rightDoor = GameObject.Find("Door_R");
        
        _observerDead = GameObject.Find("EndingObject").transform.GetChild(1).gameObject;
        _endingCanvas = GameObject.Find("EndingCanvas");
        _image00 = _endingCanvas.transform.GetChild(0).gameObject;
        _image01 = _endingCanvas.transform.GetChild(1).gameObject;
        _image02 = _endingCanvas.transform.GetChild(2).gameObject;
        _image03 = _endingCanvas.transform.GetChild(3).gameObject;
        _image04 = _endingCanvas.transform.GetChild(4).gameObject;
        _image05 = _endingCanvas.transform.Find("05").gameObject;
        _imagePos00 = _endingCanvas.transform.GetChild(5).transform.GetChild(0);
        _imagePos02 = _endingCanvas.transform.GetChild(5).transform.GetChild(1);
        _imagePos04 = _endingCanvas.transform.GetChild(5).transform.GetChild(2);
        InputManager.Instance.DisableMainGameAction();
        InputManager.Instance.InitTalkEventAction();
        _scriptPath += "Ending/FinalEnd1";
        _eventTexts = CSVReader.Read(_scriptPath);
        _comments = new List<string>();
        for (int i = 0; i < _eventTexts.Count; i++)
        {
            _comments.Add(_eventTexts[i][EventTextType.Content.ToString()].ToString());
        }
        _textCount = 0;
        
        _player = GameObject.FindGameObjectWithTag("Player");
        _observer = GameObject.Find("EndingObject").transform.GetChild(2).gameObject;
        _fadeImage = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
        _fadeImage.color = Color.white;
        _nonTargetPanel = GameObject.Find("EndingCanvas").GetComponent<TalkingPanelInfo>();
        _playerPanel = _player.GetComponent<TalkingPanelInfo>();
        _targetPanel = _observer.GetComponent<TalkingPanelInfo>();

        _playerRigid = _player.GetComponent<Rigidbody2D>();

        _eventAnimationController = _player.GetComponent<PlayerEventAnimationController>();
        _eventAnimationController.EnableEventAnimatorController();
        _player.transform.Rotate(0,180,0);
        _eventAnimationController.PlayEventAnim(EventAnimationName.IDLE);
        
        _observer.SetActive(false);
        _observerDead.SetActive(false);
        
        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
        _fadeImage.color = Color.black;
        EventFadeChanger.Instance.FadeIn(0.5f);
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1);
        _observer.SetActive(true);
        EventFadeChanger.Instance.FadeOut(0.5f);
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha <= 0);
        await UniTask.Yield();
    }

    public async UniTask OnEvent()
    {
        //ShakeCamera().Forget();
        
        InputAction action = InputManager.GetTalkEventAction("NextText");
        if (action != null)
        {
            string[] contents = _comments.ToArray();
            while (_textCount != 13)
            {
                string target = _eventTexts[_textCount][EventTextType.Target.ToString()].ToString();
                Talk(contents,target);
                await UniTask.WaitUntil(() => TypingSystem.Instance.isTypingEnd);
                SetEndbutton(target);
                await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                ClosePanel(target);
            }
            while (Mathf.Abs(_image00.transform.position.x - _imagePos00.position.x) >= 0.5f) 
            {
                _image00.transform.position = Vector2.Lerp(_image00.transform.position, _imagePos00.position,
                    Time.unscaledDeltaTime * 5);
                await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
            }
            
            await UniTask.WaitUntil(() => action.WasPressedThisFrame());

            CanvasGroup imageCanvasGroup =_image01.GetComponent<CanvasGroup>();
            while (imageCanvasGroup.alpha < 1)
            {
                imageCanvasGroup.alpha += Time.unscaledDeltaTime;
                await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
            }
                

            await UniTask.WaitUntil(() => action.WasPressedThisFrame());
            
            while (Mathf.Abs(_image02.transform.position.x - _imagePos02.position.x) >= 0.5f) 
            {
                _image02.transform.position = Vector2.Lerp(_image02.transform.position, _imagePos02.position,
                    Time.unscaledDeltaTime * 5);
                await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
            }
            
            await UniTask.WaitUntil(() => action.WasPressedThisFrame());
            
            _image03.SetActive(true);
            
            await UniTask.WaitUntil(() => action.WasPressedThisFrame());
            
            EventFadeChanger.Instance.FadeIn(0.5f);

            _fadeImage.color = Color.black;

            await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1f);
            _observerDead.SetActive(true);
            _observer.SetActive(false);
            _image00.SetActive(false);
            _image01.SetActive(false);
            _image02.SetActive(false);
            _image03.SetActive(false);
            
            EventFadeChanger.Instance.FadeOut(0.5f);

            await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha <= 0f);
            
            await MoveToPosition(_player, new Vector2(_observerDead.transform.position.x - 3f,_observerDead.transform.position.y), 0.1f);
            
            while (_textCount != 15)
            {
                string target = _eventTexts[_textCount][EventTextType.Target.ToString()].ToString();
                Talk(contents,target);
                await UniTask.WaitUntil(() => TypingSystem.Instance.isTypingEnd);
                SetEndbutton(target);
                await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                ClosePanel(target);
            }
            
            Vector2 _imageStart = _image04.transform.position;
            
            while (Mathf.Abs(_image04.transform.position.y - _imagePos04.position.y) >= 0.5f) 
            {
                _image04.transform.position = Vector2.Lerp(_image04.transform.position, _imagePos04.position,
                    Time.unscaledDeltaTime * 5);
                await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
            }
            
            while (_textCount != 17)
            {
                string target = _eventTexts[_textCount][EventTextType.Target.ToString()].ToString();
                Talk(contents,target);
                await UniTask.WaitUntil(() => TypingSystem.Instance.isTypingEnd);
                SetEndbutton(target);
                await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                ClosePanel(target);
            }
            
            while (Mathf.Abs(_image04.transform.position.y - _imageStart.y) >= 0.5f) 
            {
                _image04.transform.position = Vector2.Lerp(_image04.transform.position, _imageStart,
                    Time.unscaledDeltaTime * 5);
                await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
            }

            float openDoorTime = 1.5f;
            while (openDoorTime > 0)
            {
                openDoorTime -= Time.unscaledDeltaTime;
                _rightDoor.transform.position += Vector3.right * 0.33f;
                _leftDoor.transform.position += Vector3.left * 0.33f;
                
                await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
                
            }

            _fadeImage.color = Color.white;
            
            EventFadeChanger.Instance.FadeIn(2.0f);
            
            await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1);

            CanvasGroup lastImageFade = _image05.GetComponent<CanvasGroup>();
            while (lastImageFade.alpha < 1)
            {
                lastImageFade.alpha += Time.unscaledDeltaTime;
                await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
            }
            
            while (_textCount != 20)
            {
                string target = _eventTexts[_textCount][EventTextType.Target.ToString()].ToString();
                Talk(contents,target);
                await UniTask.WaitUntil(() => TypingSystem.Instance.isTypingEnd);
                SetEndbutton(target);
                await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                ClosePanel(target);
                if(_textCount == 19)
                    while (lastImageFade.alpha > 0)
                    {
                        lastImageFade.alpha -= Time.unscaledDeltaTime;
                        await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
                    }
            }

            AsyncOperation result = SceneManager.LoadSceneAsync("MainMenu");

            result.allowSceneActivation = false;
            
            ClosePanel("None");
            
            while (_fadeImage.color.r > 0)
            {
                _fadeImage.color = new Color(_fadeImage.color.r - Time.unscaledDeltaTime, _fadeImage.color.g - Time.unscaledDeltaTime, _fadeImage.color.b - Time.unscaledDeltaTime);
                await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
            }

            await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
            
                    
            InputManager.Instance.DisableTalkEventAction();
            InputManager.Instance.InitMainGameAction();

            result.allowSceneActivation = true;


            //await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1.0f);


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
    
    void Talk(string[] contents, string target)
    {
        _textCount++;
        switch (target)
        {
            case "None" :
                
                _nonTargetPanel._panel.SetActive(true);
                _nonTargetPanel._endButton.SetActive(false);
                if(_nonTargetPanel._eventText.TryGetComponent(out TextMeshProUGUI nonPanelComponent)) 
                    TypingSystem.Instance.Typing(contents,nonPanelComponent);
                break;
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
            case "None" :
                _nonTargetPanel._endButton.SetActive(true);
                break;
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
            case "None" :
                _nonTargetPanel._panel.SetActive(true);
                break;
            case "Player" : 
                _playerPanel._panel.SetActive(false);
                break;
            case "Observer" : 
                _targetPanel._panel.SetActive(false);
                break;
                
        }
    }


}
