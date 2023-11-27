using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class EventFadeChanger : MonoBehaviour
{
    public CanvasGroup Fade_img;
    
    public static EventFadeChanger Instance {
        get {
            return _instance;
        }
    }
    private static EventFadeChanger _instance;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneChanged;
    }
    

    public static void Init()
    {
        if (_instance)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }

        _instance = new GameObject("[EventFadeChanger]").AddComponent<EventFadeChanger>();
        DontDestroyOnLoad(_instance.gameObject);
        
        GameObject fadeObj = Resources.Load<GameObject>("Prefab/FadeObject");
        GameObject fade = Instantiate(fadeObj);
        fade.transform.GetComponentInChildren<CanvasGroup>().alpha = 0;
        DontDestroyOnLoad(fade);

    }
    

    public void FadeIn(float duration,float value = 1.0f, string sceneName = null){
        Fade_img.DOFade(value, duration)
            .OnStart(()=>{
                Fade_img.blocksRaycasts = true; //아래 레이캐스트 막기
            })
            .OnComplete(()=>
            {
                if (sceneName != null)
                   SceneManager.LoadSceneAsync(sceneName);
            });
    }

    public void FadeOut(float duration)
    {
        Fade_img.DOFade(0, duration)
            .OnComplete(()=>{
                Fade_img.blocksRaycasts = false;
            });
    }

    public void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        Fade_img = GameObject.FindWithTag("Fade").GetComponent<CanvasGroup>();
    }
    
}
