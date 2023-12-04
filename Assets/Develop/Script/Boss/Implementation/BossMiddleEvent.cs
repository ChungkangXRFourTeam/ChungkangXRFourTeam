using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class BossMiddleEvent : ITalkingEvent
{
    private SkeletonAnimation _ani;
    private System.Action<bool> _endEventCallback;
    public BossMiddleEvent(SkeletonAnimation ani, System.Action<bool> endEventCallback)
    {
        _ani = ani;
        _endEventCallback = endEventCallback;
    }
    public async UniTask OnEventBefore()
    {
    }

    public async UniTask OnEvent()
    {
    }

    public async UniTask OnEventEnd()
    {
    }

    public async UniTask OnEventStart()
    {
        _endEventCallback(true);
        
        var _fadeImage = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
        _fadeImage.color = Color.white;
        
        EventFadeChanger.Instance.FadeIn(1.0f);
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1);
        _ani.skeleton.SetSkin("BP2");
        EventFadeChanger.Instance.FadeOut(1.0f);
        
        _endEventCallback(false);
    }

    public bool IsInvalid()
    {
        return true;
    }
}
