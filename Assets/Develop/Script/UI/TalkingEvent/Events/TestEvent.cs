using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Spine.Unity.Editor;
using UnityEngine;
using TMPro;

public class TalkingEvent : ITalkingEvent
{
    List<Dictionary<string, object>> _eventTexts;
    private GameObject _playerEventText;
    private string _scriptPath = "EventTextScript/";
    private Dictionary<string, GameObject> _talkers;
    private List<string> _comments;

    public void SetEventTexts()
    {
        _scriptPath += "TestScript";
        _playerEventText = GameObject.Find("PlayerEventText");
        _eventTexts = CSVReader.Read(_scriptPath);
        _comments = new List<string>();
        _talkers = new Dictionary<string, GameObject>();
    }

    public async UniTask OnEventBefore()
    {
        _talkers.TryAdd(TalkerTarget.Player.ToString(),_playerEventText);
        for (int i = 0; i < _eventTexts.Count; i++)
        {
            _comments.Add(_eventTexts[i][EventTextType.Content.ToString()].ToString());
            Debug.Log(_eventTexts[i][EventTextType.Content.ToString()].ToString());
        }
        await UniTask.Delay(TimeSpan.FromSeconds(1));
    }
    public async UniTask OnEventStart()
    {
    }
    public async UniTask OnEvent()
    {
        for (int i = 0; i < _eventTexts.Count; i++)
        {
            if(_playerEventText.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI component)) 
                TypingSystem.instance.Typing(_comments.ToArray(),component);
            await UniTask.Delay(TimeSpan.FromMilliseconds(int.Parse(_eventTexts[i][EventTextType.Delay.ToString()].ToString())));
        }

        await UniTask.Yield();
    }
    
    public async UniTask OnEventEnd()
    {
        Debug.Log("OnEventEnd");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
    }
}
