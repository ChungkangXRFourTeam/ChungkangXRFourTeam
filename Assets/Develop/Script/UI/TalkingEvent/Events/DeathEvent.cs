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

public class DeathEvent : ITalkingEvent
{
    private GameObject _player;
    private PlayerController _playerController;
    private PlayerEventAnimationController _playerEventAnimationController;
    private CanvasGroup _testCaseText;
    private GameObject _fade;
    public async UniTask OnEventBefore()
    {
        _player = GameObject.FindWithTag("Player"); 
        _fade = GameObject.FindWithTag("Fade");
        _testCaseText = _fade.GetComponentInChildren<CanvasGroup>();
        _fade.GetComponentInChildren<TextMeshProUGUI>().text =
            "TestCase : " + ++GameSetting.GameCount;
        _testCaseText.alpha = 0;
        _playerEventAnimationController = _player.GetComponent<PlayerEventAnimationController>();
        _playerEventAnimationController.EnableEventAnimatorController();
        _playerEventAnimationController.PlayEventAnim(EventAnimationName.DEATH);
        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
        AsyncOperation result = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        result.allowSceneActivation = false;
        if (!result.isDone)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }

        EventFadeChanger.Instance.FadeIn(2.0f);
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1f);
        result.allowSceneActivation = true;
        while (_testCaseText.alpha < 1)
        {
            _testCaseText.alpha += Time.unscaledDeltaTime;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        
        while (_testCaseText.alpha > 0)
        {
            _testCaseText.alpha -= Time.unscaledDeltaTime;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        _playerEventAnimationController.DisableEventAnimatorController();
        
        
        
        await UniTask.Yield();
    }

    public async UniTask OnEvent()
    {
        
        
        await UniTask.Yield();
        
    }

    public async UniTask OnEventEnd()
    {
        EventFadeChanger.Instance.FadeOut(1);
        await UniTask.Yield();
    }

    public bool IsInvalid()
    {
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        if(playerController.IsDestroyed) 
            return true;
        return false;
    }


}
