using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Spine.Unity.Editor;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using XRProject.Helper;

public class TalkingEvent : ITalkingEvent
{
    List<Dictionary<string, object>> _eventTexts;
    private GameObject _playerEventText;
    private GameObject _playerTalkImage;
    private GameObject _playerEndButton;
    private string _scriptPath = "EventTextScript/";
    private Dictionary<string, GameObject> _talkers;
    private List<string> _comments;

    public void SetEventTexts()
    {
        _scriptPath += "TestScript";
        _playerEventText = GameObject.Find("PlayerEventText");
        _playerTalkImage = GameObject.Find("PlayerTalkImage");
        _playerEndButton = GameObject.Find("PlayerEndButton");
        _eventTexts = CSVReader.Read(_scriptPath);
        _comments = new List<string>();
        _talkers = new Dictionary<string, GameObject>();
    }

    public async UniTask OnEventBefore()
    {
        _playerTalkImage.SetActive(false);
        _talkers.TryAdd(TalkerTarget.Player.ToString(),_playerEventText);
        for (int i = 0; i < _eventTexts.Count; i++)
        {
            _comments.Add(_eventTexts[i][EventTextType.Content.ToString()].ToString());
            Debug.Log(_eventTexts[i][EventTextType.Content.ToString()].ToString());
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        
        InputManager.Instance.DisableMainGameAction();
        InputManager.Instance.InitTalkEventAction();
    }

    public async UniTask OnEvent()
    {
        if (_playerEventText.TryGetComponent(out TextMeshProUGUI component))
        {
            InputAction action = InputManager.GetTalkEventAction("NextText");
            TypingSystem.instance.Typing(_comments.ToArray(),component);
            _playerTalkImage.SetActive(true);
            while (true)
            {
                if (action != null)
                {
                    _playerEndButton.SetActive(false);
                    await UniTask.WaitUntil(() => TypingSystem.instance.isTypingEnd);
                    _playerEndButton.SetActive(true);
                    await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                    TypingSystem.Instance.GetInputDown();
                    if (TypingSystem.isDialogEnd)
                    {
                        _playerTalkImage.SetActive(false);
                        await UniTask.Delay(TimeSpan.FromMilliseconds(1500));
                        break;
                    }
                }
                
            }

            await UniTask.Yield();
        }
        
    }
    
    public async UniTask OnEventEnd()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        GameObject player = GameObject.Find("Player");
        await MoveToPosition(GameObject.Find("Player"), new Vector2(7, player.transform.position.y), 0.08f);
        
        InputManager.Instance.DisableTalkEventAction();
        InputManager.Instance.InitMainGameAction();
    }

    public async UniTask MoveToPosition(GameObject target, Vector2 posistion, float speed)
    {
        while (Mathf.Abs(target.transform.position.x - posistion.x) >= 0.04f ||
               Mathf.Abs(target.transform.position.y - posistion.y) >= 0.04f)
        {
            target.transform.Translate(new Vector3(speed,0));
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
    }
}
