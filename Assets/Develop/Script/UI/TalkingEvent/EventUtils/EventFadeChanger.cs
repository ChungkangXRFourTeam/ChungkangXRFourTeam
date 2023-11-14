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
            return instance;
        }
    }
    private static EventFadeChanger instance;
 
    void Start () {
        if (instance != null) {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;
 
        DontDestroyOnLoad(gameObject);
    }
    
    public void FadeIn(float duration, string sceneName = null){
        Fade_img.DOFade(1, duration)
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

}
