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
    private CanvasGroup _testCaseText;
    public async UniTask OnEventBefore()
    {
        GameObject.FindWithTag("Fade").transform.parent.GetChild(1).gameObject.SetActive(true);
        _testCaseText = GameObject.FindWithTag("Fade").transform.parent.GetChild(1).GetComponent<CanvasGroup>();
        GameObject.FindWithTag("Fade").transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            "TestCase : ";
        _testCaseText.alpha = 0;
        await UniTask.Yield();
    }

    public async UniTask OnEventStart()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
        EventFadeChanger.Instance.FadeIn(2.0f);
        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1f);

        while (_testCaseText.alpha < 1)
        {
            _testCaseText.alpha += Time.unscaledDeltaTime;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        
        Debug.Log(11);
        
        while (_testCaseText.alpha > 0)
        {
            _testCaseText.alpha -= Time.unscaledDeltaTime;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
        }
        
        await UniTask.Yield();
    }

    public async UniTask OnEvent()
    {
        
       AsyncOperation result = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

       if (!result.isDone)
       {
           await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
       }
        
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
