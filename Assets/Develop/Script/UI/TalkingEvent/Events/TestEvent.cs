using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TalkingEvent : ITalkingEvent
{
    protected List<Dictionary<string, object>> _eventTexts;
    protected TalkingPanelInfo _playerPanel;
    protected TalkingPanelInfo _targetPanel;
    protected string _scriptPath = "EventTextScript/";
    protected  List<string> _comments;
    protected int _textCount;
    
    public async UniTask OnEventBefore()
    {
        EventFadeChanger.Instance.Fade_img = GameObject.FindGameObjectWithTag("Fade").GetComponent<CanvasGroup>();
        _textCount = 0;
        _scriptPath += "TestScript";
        _playerPanel = GameObject.FindGameObjectWithTag("Player").GetComponent<TalkingPanelInfo>();
        _targetPanel = GameObject.Find("Enemy").GetComponent<TalkingPanelInfo>();
        _eventTexts = CSVReader.Read(_scriptPath);
        _comments = new List<string>();
        
        _playerPanel._talkingImage.SetActive(false);
        _targetPanel._talkingImage.SetActive(false);
        InputManager.Instance.DisableMainGameAction();
        InputManager.Instance.InitTalkEventAction();
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1.0f);
    }

    public async UniTask OnEventStart()
    {
        EventFadeChanger.Instance.FadeOut(0.5f);
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha <= 0);
        for (int i = 0; i < _eventTexts.Count; i++)
        {
            _comments.Add(_eventTexts[i][EventTextType.Content.ToString()].ToString());
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
    }

    public async UniTask OnEvent()
    {
        if (_playerPanel._eventText.TryGetComponent(out TextMeshProUGUI playerComponent))
        {
            if (_targetPanel._eventText.TryGetComponent(out TextMeshProUGUI targetComponent))
            {
                InputAction action = InputManager.GetTalkEventAction("NextText");
                string[] contents = _comments.ToArray();
                _playerPanel._talkingImage.SetActive(false);
                _targetPanel._talkingImage.SetActive(false);
                string target;
                while (true)
                {
                    if (action != null)
                    {
                        if(_textCount != _comments.Count) 
                            target = _eventTexts[_textCount++][EventTextType.Target.ToString()].ToString();
                        else
                        {
                            target = "";
                        }
                        if (target == "Player")
                        {
                            _playerPanel._talkingImage.SetActive(true);
                            _playerPanel._endButton.SetActive(false);
                            TypingSystem.instance.Typing(contents,playerComponent);
                        }
                        else if (target == "Enemy")
                        {
                            _targetPanel._talkingImage.SetActive(true);
                            _targetPanel._endButton.SetActive(false);
                            TypingSystem.instance.Typing(contents,targetComponent);
                        }
                        await UniTask.WaitUntil(() => TypingSystem.instance.isTypingEnd);
                        if (target == "Player")
                        {
                            _playerPanel._endButton.SetActive(true);
                        }
                        else if (target == "Enemy")
                        {
                            _targetPanel._endButton.SetActive(true);
                        }
                        await UniTask.WaitUntil(() => action.WasPressedThisFrame());
                        _playerPanel._talkingImage.SetActive(false);
                        _targetPanel._talkingImage.SetActive(false);
                        if (_textCount == _comments.Count)
                        {
                            TypingSystem.instance.Typing(contents,playerComponent);
                        }
                        if (TypingSystem.isDialogEnd)
                        {
                            await UniTask.Delay(TimeSpan.FromMilliseconds(1000));
                            break;
                        }
                    }

                }
            }

            await UniTask.Yield();
        }
        
    }
    
    public async UniTask OnEventEnd()
    {

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
