using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class CutScneEventTemplate : ITalkingEvent
{
    protected List<Dictionary<string, object>> _eventInfo;
    private GameObject _cutsceneCanvas;
    private GameObject _playerImage;
    private GameObject _targetImage;
    private string _scriptPath = "EventTextScript/";
    private List<string> _comments;
    private int _textCount;
    public async UniTask OnEventBefore()
    {
        EventFadeChanger.Instance.FadeIn(3.0f);

        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1.0f);
    }

    public async UniTask OnEventStart()
    {
        EventFadeChanger.Instance.FadeOut(2.0f);
        
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha <= 0f);
    }

    public async UniTask OnEvent()
    {
        await UniTask.Yield();
    }

    public async UniTask OnEventEnd()
    {
        await UniTask.Yield();
    }

    protected void GetTextScript(string fileName)
    {
        _scriptPath += fileName;
        _eventInfo = CSVReader.Read(_scriptPath);
        _comments = new List<string>();
    }

    public bool IsInvalid()
    {
        return SceneManager.GetActiveScene().name == "Tutorial";
    }
}
