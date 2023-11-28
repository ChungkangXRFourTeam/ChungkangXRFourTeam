using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DescriptionEvent5 : ITalkingEvent
{
    private GameObject _kennel;
    private Rigidbody2D _kennelRigid;
    private PlayerController _playerController;
    private GameObject _player;
    private List<Dictionary<string, object>> _eventTexts;
    private TalkingPanelInfo _playerPanel;
    private TalkingPanelInfo _targetPanel;
    private GameObject _observer;
    private GameObject _upWall;
    private PlayerEventAnimationController _playerEventAnimation;
    private string _scriptPath = "EventTextScript/";
    private  List<string> _comments;
    private int _textCount;
    private string[] contents;
    private string target;
    private bool _eventPaused;
    
    public async UniTask OnEventBefore()
    {
        
        _player = GameObject.FindWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _playerEventAnimation = _player.GetComponent<PlayerEventAnimationController>();
        _playerEventAnimation.EnableEventAnimatorController();
        _kennel = GameObject.FindWithTag("Kennel");
        _kennelRigid = _kennel.GetComponent<Rigidbody2D>();

        _kennelRigid.bodyType = RigidbodyType2D.Static;
        _playerController.CurrentHP /= 2;
        _observer = GameObject.FindGameObjectWithTag("Observer");
        _scriptPath += "Opning/DescriptionText5";
        
        _eventTexts = CSVReader.Read(_scriptPath);
        _comments = new List<string>();
        for (int i = 0; i < _eventTexts.Count; i++)
        {
            _comments.Add(_eventTexts[i][EventTextType.Content.ToString()].ToString());
        }
        contents = _comments.ToArray();
        target = "";
        
        _playerPanel = GameObject.FindGameObjectWithTag("Player").GetComponent<TalkingPanelInfo>();
        _targetPanel = GameObject.FindGameObjectWithTag("Observer").GetComponent<TalkingPanelInfo>();
        _observer.SetActive(false);
        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
        
        InputManager.Instance.DisableMainGameAction();
        InputManager.Instance.InitTalkEventAction();
        
        Rigidbody2D playerRigid = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        playerRigid.velocity = new Vector2(0,-10);
        
        EventFadeChanger.Instance.FadeIn(0.3f);
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1.0f);
        _observer.SetActive(true);
        await UniTask.Yield();
        
        
    }

    public async UniTask OnEvent()
    {
        _player.transform.rotation = quaternion.identity;
        _player.transform.rotation = Quaternion.Euler(0,180,0);
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        _observer.transform.position = new Vector2(playerPos.x + 9f, playerPos.y + 7.5f);
        EventFadeChanger.Instance.FadeOut(0.7f);
        InputAction action = InputManager.GetTalkEventAction("NextText");
        _textCount = 0;
        
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha <= 0f);
        if(action != null)
            while (_textCount < contents.Length)
            { 
                target = _eventTexts[_textCount++][EventTextType.Target.ToString()].ToString();
                Talk(contents,target);
                await UniTask.WaitUntil(() => TypingSystem.Instance.isTypingEnd);
                _targetPanel._endButton.SetActive(true);
                _playerPanel._endButton.SetActive(true);
                await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                _targetPanel._panel.SetActive(false);
                _playerPanel._panel.SetActive(false);
            }
        
        await UniTask.Yield();
        
    }

    public async UniTask OnEventEnd()
    {
        
        _playerEventAnimation.DisableEventAnimatorController();
        InputManager.Instance.InitMainGameAction();
        InputManager.Instance.DisableTalkEventAction();
        
        await UniTask.Yield();
    }

    public bool IsInvalid()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
            return true;
        return false;
    }

    void Talk(string[] contents, string target)
    {
        switch (target)
        {
            case "Player" : 
                if(_textCount != _comments.Count) 
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
}
