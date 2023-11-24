using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialCutscene : ITalkingEvent
{
    
    private GameObject _image1; 
    private GameObject _image2; 
    private GameObject _fade; 
    private GameObject _cutscene;
    private Vector2 _image1Dest; 
    private Vector2 _image2Dest; 
    private Vector2 _image1Start; 
    private Vector2 _image2Start;
    
    private GameObject _eyeBlack;
    private Vector2 _eyeBlackStart;
    private Vector2 _eyeBlackEnd;

    private GameObject _player;
    private Rigidbody2D _playerRigid;
    private PlayerAnimationController _playerAnim;

    private float _imageScrollSpeed = 5.0f;

    private Volume _eyeVolume;
    private VolumeProfile _eyeVolumeProfile;

    private bool notTalking;
    private TalkingPanelInfo _nonTargetPanel;
    private TalkingPanelInfo _blurPanel;
    
    protected List<Dictionary<string, object>> _eventTexts;
    protected TalkingPanelInfo _playerPanel;
    protected TalkingPanelInfo _targetPanel;
    protected string _scriptPath = "EventTextScript/";
    protected  List<string> _comments;
    protected int _textCount;
    private string[] contents;
    public async UniTask OnEventBefore()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        
        _playerAnim = _player.GetComponent<PlayerAnimationController>();
        _playerRigid = _player.GetComponent<Rigidbody2D>();
        
        _nonTargetPanel = GameObject.Find("Facing CutScene").GetComponent<TalkingPanelInfo>();
        _blurPanel = GameObject.Find("BlurPanel").GetComponent<TalkingPanelInfo>();
        _scriptPath += "TutorialText";
        _eventTexts = CSVReader.Read(_scriptPath);
        _comments = new List<string>();
        
        for (int i = 0; i < _eventTexts.Count; i++)
        {
            _comments.Add(_eventTexts[i][EventTextType.Content.ToString()].ToString());
        }
        
        _fade = GameObject.FindWithTag("Fade"); 
        EventFadeChanger.Instance.Fade_img = _fade.GetComponent<CanvasGroup>();
        _cutscene = GameObject.FindGameObjectWithTag("CutScene");
        _image1 = _cutscene.transform.Find("Image 1").gameObject;
        _image2 = _cutscene.transform.Find("Image 2").gameObject;
        _eyeBlack = GameObject.Find("BlurPanel").transform.Find("EyeBlack").gameObject;

        _image1Dest = _image1.transform.position;
        _image2Dest = _image2.transform.position;
        _image1Start = GameObject.Find("Image1Start").transform.position;
        _image2Start = GameObject.Find("Image2Start").transform.position;

        _eyeVolume = GameObject.Find("CutsceneVolume").GetComponent<Volume>();
        _eyeVolumeProfile = _eyeVolume.sharedProfile;
        
        if (_eyeVolumeProfile.TryGet(out DepthOfField dof) && _eyeVolumeProfile.TryGet(out Vignette vignette))
        {
                dof.focalLength.value = 300;
                vignette.intensity.value = 0.75f;
        }
        
        _eyeBlackStart = GameObject.Find("BlurPanel").transform.Find("EyeBlackStart").position;
        _eyeBlackEnd = GameObject.Find("BlurPanel").transform.Find("EyeBlackEnd").position;
        
        _nonTargetPanel._panel.SetActive(false);
        
        _cutscene.SetActive(true);
        
        _playerPanel = GameObject.FindGameObjectWithTag("Player").GetComponent<TalkingPanelInfo>();
        _targetPanel = GameObject.FindGameObjectWithTag("Observer").GetComponent<TalkingPanelInfo>();
        
        _playerPanel._panel.SetActive(false);
        _targetPanel._panel.SetActive(false);
        
        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
        _image1.transform.position = _image1Start;
        _image2.transform.position = _image2Start;
        
        _image1.SetActive(false);
        _image2.SetActive(false);

        InputManager.Instance.DisableMainGameAction();
        InputManager.Instance.InitTalkEventAction();
        
        await UniTask.Yield();
        //EventFadeChanger.Instance.FadeIn(0.5f,0.7f);
        //await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 0.7f);
    }

    public async UniTask OnEvent()
    {
        string target;
        await UniTask.Delay(TimeSpan.FromSeconds(3.0f));
        contents = _comments.ToArray();
        InputAction action = InputManager.GetTalkEventAction("NextText");

        notTalking = true; 
        if (action != null && _nonTargetPanel._eventText.TryGetComponent(out TextMeshProUGUI component)) 
            {
                if (notTalking)
            { 
                for (int i = 0; i < 5; i++)
                {
                    _nonTargetPanel._panel.SetActive(true);
                    Talk(contents,"None");
                    await UniTask.WaitUntil(() => TypingSystem.instance.isTypingEnd);
                    _nonTargetPanel._endButton.SetActive(true);
                    await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                }
                _nonTargetPanel._panel.SetActive(false);
                
                for (int i = 0; i < 2; i++) 
                { 
                    while (Mathf.Abs(_eyeBlack.transform.position.y - _eyeBlackEnd.y) >= 10F) 
                    { 
                        _eyeBlack.transform.position = Vector2.Lerp(_eyeBlack.transform.position, _eyeBlackEnd,Time.unscaledDeltaTime); 
                        await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime)); 
                    } 
                    _nonTargetPanel._panel.SetActive(true);
                    Talk(contents,"None");
                    await UniTask.WaitUntil(() => TypingSystem.instance.isTypingEnd);
                    _nonTargetPanel._endButton.SetActive(true);
                    await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                    _nonTargetPanel._panel.SetActive(false);
                    
                    await UniTask.Delay(TimeSpan.FromSeconds(1.0f)); 
                    while (Mathf.Abs(_eyeBlack.transform.position.y - _eyeBlackStart.y) >= 10f) 
                    { 
                        if (i == 1)
                            break;
                        _eyeBlack.transform.position = Vector2.Lerp(_eyeBlack.transform.position, _eyeBlackStart,Time.unscaledDeltaTime);
                    await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
                    }
                    await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                }
            
                _image1.SetActive(true);
                _image2.SetActive(true);
                if (_eyeVolumeProfile.TryGet(out DepthOfField dof) && _eyeVolumeProfile.TryGet(out Vignette vignette))
                { 
                    float dt = 0f;
                    
                    while (dt < 2f) 
                    { 
                        
                        dof.focalLength.value -= (150 * Time.unscaledDeltaTime); 
                        vignette.intensity.value -= (0.3f * Time.unscaledDeltaTime); 
                        await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime)); 
                        dt += Time.unscaledDeltaTime; 
                        
                    } 
                }

                for (int i = 0; i < 3; i++)
                {
                    _nonTargetPanel._panel.SetActive(true);
                    Talk(contents,"None");
                    await UniTask.WaitUntil(() => TypingSystem.instance.isTypingEnd);
                    _nonTargetPanel._endButton.SetActive(true);
                    await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                    _nonTargetPanel._panel.SetActive(false);
                }
                
                EventFadeChanger.Instance.FadeIn(0.3f);

                await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1f);
                
                GameObject.Find("BlurPanel").SetActive(false);
            }

                notTalking = true;


            }
        
        EventFadeChanger.Instance.FadeOut(0.3f);

        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha <= 0f);

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        
        while (_textCount != 29 )
        {
            target = _eventTexts[_textCount][EventTextType.Target.ToString()].ToString();
            
            Talk(contents,target);
            await UniTask.WaitUntil(() => TypingSystem.instance.isTypingEnd);
            _playerPanel._endButton.SetActive(true);
            _targetPanel._endButton.SetActive(true);
            await UniTask.WaitUntil(() => action.WasPressedThisFrame());
            _playerPanel._panel.SetActive(false);
            _targetPanel._panel.SetActive(false);

            if (_textCount == 20)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
                
                while (Mathf.Abs(_image1.transform.position.y - _image1Dest.y) >= 0.5f) 
                {
                    _image1.transform.position = Vector2.Lerp(_image1.transform.position, _image1Dest,
                        Time.unscaledDeltaTime * _imageScrollSpeed);
                    await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
                }

                await UniTask.WaitUntil(() => action.WasPressedThisFrame());
            
                while (Mathf.Abs(_image2.transform.position.y - _image2Dest.y) >= 0.5f)
                {
                    _image2.transform.position = Vector2.Lerp(_image2.transform.position, _image2Dest,
                        Time.unscaledDeltaTime * _imageScrollSpeed);
                    await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
                }
            
                await UniTask.WaitUntil(() => action.WasPressedThisFrame());
            
                while (Mathf.Abs(_image2.transform.position.y - _image2Start.y) >= 0.5f)
                {
                    _image1.transform.position = Vector2.Lerp(_image1.transform.position, _image1Start,
                        Time.unscaledDeltaTime * _imageScrollSpeed);
                    _image2.transform.position = Vector2.Lerp(_image2.transform.position, _image2Start,
                        Time.unscaledDeltaTime * _imageScrollSpeed);
                    await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
                }
            }
        }
        
        GameObject endPosition = GameObject.Find("CutScene EndPosition");
        _playerAnim.SetState(new PAniState()
        {
            Restart = false,
            Rotation = Quaternion.Euler(0,180,0),
            State = EPCAniState.Run
        });
        
        while (Mathf.Abs(_player.transform.position.x - endPosition.transform.position.x) >= 0.4f)
        {
            _playerRigid.position += new Vector2(15 * Time.unscaledDeltaTime, 0);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        EventFadeChanger.Instance.FadeIn(0.3f);
        _playerAnim.SetState(new PAniState()
        {
            Restart = false,
            Rotation = Quaternion.identity,
            State = EPCAniState.Idle
        });
    }

    public async UniTask OnEventEnd()
    {
        
        
        EventFadeChanger.Instance.FadeOut(0.7f);

        TextMeshProUGUI description = GameObject.Find("DescriptionMonitor 1").transform.Find("Description Canvas").transform
            .GetChild(0).GetComponent<TextMeshProUGUI>();
        
        TypingSystem.Instance.Typing(contents,description);
        
        InputManager.Instance.InitMainGameAction();
        InputManager.Instance.DisableTalkEventAction();
        
        await UniTask.Yield();
    }

    public bool IsInvalid()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            return true;
        }
        return false;
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
                    TypingSystem.instance.Typing(contents,nonPanelComponent);
                break;
            case "Player" : 
                _playerPanel._panel.SetActive(true);
                _playerPanel._endButton.SetActive(false);
                if(_playerPanel._eventText.TryGetComponent(out TextMeshProUGUI playerComponent)) 
                    TypingSystem.instance.Typing(contents,playerComponent);
                break;
            case "Observer" : 
                _targetPanel._panel.SetActive(true);
                _targetPanel._endButton.SetActive(false);
                if(_targetPanel._eventText.TryGetComponent(out TextMeshProUGUI observerComponent)) 
                    TypingSystem.instance.Typing(contents,observerComponent);
                break;
                
        }
    }
}
