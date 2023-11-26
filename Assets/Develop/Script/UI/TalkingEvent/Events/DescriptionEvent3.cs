using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DescriptionEvent3 : ITalkingEvent
{
    
    private List<Dictionary<string, object>> _eventTexts;
    private PlayerController _playerController;
    private TalkingPanelInfo _playerPanel;
    private TalkingPanelInfo _targetPanel;
    private GameObject _observer;
    private Animator _playerAnim;
    private string _scriptPath = "EventTextScript/";
    private  List<string> _comments;
    private int _textCount;
    private string[] contents;
    private string target;
    private bool _eventPaused;
    
    public async UniTask OnEventBefore()
    {
        _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _playerAnim = GameObject.FindWithTag("Player").GetComponent<Animator>();
        _observer = GameObject.FindGameObjectWithTag("Observer");
        _scriptPath += "Opning/DescriptionText3";
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
        await UniTask.WaitUntil(() => playerRigid.velocity.y == 0);
        
        EventFadeChanger.Instance.FadeIn(0.3f);
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1.0f);
        _observer.SetActive(true);
        await UniTask.Yield();
        
        
    }

    public async UniTask OnEvent()
    {
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        _observer.transform.position = new Vector2(playerPos.x + 9f, playerPos.y + 7.5f);
        EventFadeChanger.Instance.FadeOut(0.7f);
        InputAction action = InputManager.GetTalkEventAction("NextText");
        _textCount = 0;
        
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha <= 0f);
        if(action != null)
            while (_textCount < contents.Length-1)
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
        
        TextMeshProUGUI description = GameObject.Find("DescriptionMonitor 4").transform.Find("Description Canvas").transform
            .GetChild(0).GetComponent<TextMeshProUGUI>();
        
        TypingSystem.Instance.Typing(contents,description);
        
        await UniTask.WaitUntil(() => TypingSystem.Instance.isTypingEnd);
        
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
