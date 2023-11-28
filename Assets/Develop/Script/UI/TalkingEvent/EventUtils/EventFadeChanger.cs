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
    private static GameObject _fadeObject;
    private static GameObject _fadeText;
    public CanvasGroup Fade_img;
    
    public static EventFadeChanger Instance {
        get {
            return _instance;
        }
    }
    private static EventFadeChanger _instance;

    private void Start()
    {
        Fade_img = _fadeObject.transform.GetComponentInChildren<CanvasGroup>();
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
        _fadeObject = Instantiate(fadeObj); 
        _fadeObject .transform.GetComponentInChildren<CanvasGroup>().alpha = 0;
        _fadeText = GameObject.FindWithTag("FadeText");
        _fadeText.GetComponent<CanvasGroup>().alpha = 0;

    }
    

    public void FadeIn(float duration,float value = 1.0f, string sceneName = null)
    { 
        _fadeObject.SetActive(true);
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
        if (!_fadeObject.activeSelf)
            _fadeObject.SetActive(true);
        Fade_img.DOFade(0, duration)
            .OnComplete(()=>{
                Fade_img.blocksRaycasts = false;
            });
    }

    public void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        if (!GameObject.FindWithTag("Fade"))
        {
            GameObject fadeObj = Resources.Load<GameObject>("Prefab/FadeObject");
            _fadeObject = Instantiate(fadeObj);
            Fade_img = _fadeObject.transform.GetComponentInChildren<CanvasGroup>();
            Fade_img.alpha = 0;
        }
    }
    
}
