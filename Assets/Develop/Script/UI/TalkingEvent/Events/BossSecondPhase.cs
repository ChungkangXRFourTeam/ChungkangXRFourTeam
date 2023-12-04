using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading;
using Spine.Unity;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Range = UnityEngine.SocialPlatforms.Range;

public class BossPhaseAndDescriptionEvent : ITalkingEvent
{
    private string _sceneName;
    private GameObject _player;
    private GameObject _observer;


    private Transform startPos;
    private PolygonCollider2D _confiner;

    protected List<Dictionary<string, object>> _eventTexts;
    protected TalkingPanelInfo _playerPanel;
    protected TalkingPanelInfo _targetPanel;
    protected string _scriptPath = "EventTextScript/";
    protected  List<string> _comments;
    private string[] contents;
    protected int _textCount;

    public BossPhaseAndDescriptionEvent(string textSrc)
    {
        _scriptPath += "Ending/" + textSrc;
    }
    
    public async UniTask OnEventBefore()
    {
        TalkingEventManager.Instance._isEventEnd = true;
        _eventTexts = CSVReader.Read(_scriptPath);
        _comments = new List<string>();
        for (int i = 0; i < _eventTexts.Count; i++)
        {
            _comments.Add(_eventTexts[i][EventTextType.Content.ToString()].ToString());
        }
        _textCount = 0;
        contents = _comments.ToArray();

        _player = GameObject.FindGameObjectWithTag("Player");
        _observer = GameObject.Find("Boss");
        
        _playerPanel = _player.GetComponent<TalkingPanelInfo>();
        _targetPanel = _observer.GetComponent<TalkingPanelInfo>();
        
        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
        await UniTask.Yield();
    }

    public async UniTask OnEvent()
    {
        InputAction action = InputManager.GetTalkEventAction("NextText"); 
        string target = String.Empty; 
        if(action != null)
        while (_textCount < contents.Length) 
        { 
            target = _eventTexts[_textCount][EventTextType.Target.ToString()].ToString(); 
            Talk(contents,target); 
            await UniTask.WaitUntil(() => TypingSystem.Instance.isTypingEnd); 
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
            _targetPanel._panel.SetActive(false); 
            _playerPanel._panel.SetActive(false);
        }
    }
    
    public async UniTask OnEventEnd()
    {
        
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
