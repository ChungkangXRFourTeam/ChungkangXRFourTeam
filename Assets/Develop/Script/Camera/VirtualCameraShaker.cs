using System;
using UnityEngine;
using Cinemachine;

public class VirtualCameraShaker : MonoBehaviour
{
    private static VirtualCameraShaker _instance;
    private static CinemachineVirtualCamera _cinemachineVirtualCamera;
    private static CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    private float shakeTimer;
    private float startingIntensity;
    private float shakeTimerTotal;
    private void Awake()
    {

    }

    public static void Init()
    {
        if (_instance)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }

        _instance = new GameObject("[VirtualCameraShaker]").AddComponent<VirtualCameraShaker>();
        DontDestroyOnLoad(_instance.gameObject);
        _cinemachineVirtualCamera = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin =
            _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
        CameraShake(1f,1f,10f);
    }

    public void CameraShake(float duration, float intensity = 1f, float frequency = 1f)
    {

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = frequency;
        
        startingIntensity = intensity;
        shakeTimer = duration;
        shakeTimerTotal = duration;
        


    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            startingIntensity -= Time.deltaTime;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 
                Mathf.Lerp(startingIntensity,0f,shakeTimer/shakeTimerTotal);
        }
        else if (cinemachineBasicMultiChannelPerlin.m_AmplitudeGain != 0)
        {
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 0;
        }
    }
}
