using System;
using UnityEngine;
using Cinemachine;

public class VirtualCameraShaker : MonoBehaviour
{
    private static VirtualCameraShaker _instance;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private float shakeTimer;
    private float startingIntensity;
    private float shakeTimerTotal;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            DontDestroyOnLoad(_instance);
        }
    }

    public static VirtualCameraShaker Instance
    {
        get
        {
            if(_instance != null) 
                return _instance;
            
            return null;
        }
    }

    public void CameraShake(float intensity, float duration)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        startingIntensity = intensity;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        shakeTimer = duration;
        

    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime; 
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
                _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 
                Mathf.Lerp(startingIntensity,0f,shakeTimer/shakeTimerTotal);
        }
    }
}
