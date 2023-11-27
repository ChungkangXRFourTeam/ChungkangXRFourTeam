using System;
using System.Collections;
using UnityEngine;
using Cinemachine;

public class VirtualCameraShaker : MonoBehaviour
{
    private static VirtualCameraShaker _instance;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    private float shakeTimer;
    private float startingIntensity;
    private float startingFrequency;
    private float shakeTimerTotal;

    public static void Init()
    {
        if (_instance)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }

        _instance = new GameObject("[VirtualCameraShaker]").AddComponent<VirtualCameraShaker>();
        DontDestroyOnLoad(_instance.gameObject);
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

    private void Start()
    {
        _cinemachineVirtualCamera = GameObject.FindWithTag("VirtualCamera")?.GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin =
            _cinemachineVirtualCamera?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        

    }

    public void CameraShake(float duration, float intensity = 1f, float frequency = 1f)
    {
        StopCoroutine("ShakeCamera");
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = frequency;

        startingIntensity = intensity;
        startingFrequency = frequency;
        
        shakeTimer = duration;
        shakeTimerTotal = duration;

        StartCoroutine("ShakeCamera");

    }
    IEnumerator ShakeCamera()
    {
        if (_cinemachineVirtualCamera)
        {
            while (shakeTimer > 0)
            {
                shakeTimer -= Time.unscaledDeltaTime;
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 
                    Mathf.Lerp( 0f,startingIntensity,shakeTimer/shakeTimerTotal);
                cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 
                    Mathf.Lerp(0f ,startingFrequency,shakeTimer/shakeTimerTotal);
                
                yield return new WaitForSeconds(Time.unscaledDeltaTime);
            }

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 0;
        }
    }
}
