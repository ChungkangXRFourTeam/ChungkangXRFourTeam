using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using XRProject.Utils;
using XRProject.Utils.Log;

public class GameSetting : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Init()
    {
        _inst = null;
    }
    static private GameSetting _inst;

    static public GameSetting Instance => _inst;
    private void Awake()
    {
        _inst = this;
    }

    private void Update()
    {
        Application.targetFrameRate = 60;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }

    void OnApplicationQuit()

    {

        Application.CancelQuit();
#if !UNITY_EDITOR

        System.Diagnostics.Process.GetCurrentProcess().Kill();

#endif

    }

    public bool _isGravityDown;
    public bool IsGravityDown
    {
        get
        {
            return _isGravityDown;
        }
        set
        {
            if (_isGravityDown)
            {
                Physics2D.gravity = new Vector2(0f, 9.81f);
            }
            else
            {
                Physics2D.gravity = new Vector2(0f, -9.81f);
            }

            _isGravityDown = !_isGravityDown;
        }
    }
}
