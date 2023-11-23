using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TitleMenuManger : MonoBehaviour
{
    private float _volumeResetValue = 0.5f;
    
    [SerializeField]
    private Slider _allVolumeSlider;
    [SerializeField]
    private Slider _backgroundVolumeSlider;
    [SerializeField]
    private Slider _musicVolumeSlider;
    [SerializeField]
    private Slider _soundEffectVolumeSlider;
    [SerializeField]
    private Slider _brightnessSlider;
    [SerializeField]
    private Slider _mouseSensitivitySlider;

    [SerializeField] private GameObject _settingPanel;

    private void Awake()
    {
        ResetSettings();
        LoadSettings();
    }

    private void Start()
    {
        _allVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        _backgroundVolumeSlider.onValueChanged.AddListener(OnBackgroundVolumeChanged);
        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _soundEffectVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    public void ResetSettings()
    {
        _allVolumeSlider.value = _volumeResetValue;
        _backgroundVolumeSlider.value = _volumeResetValue;
        _soundEffectVolumeSlider.value = _volumeResetValue;
        _musicVolumeSlider.value = _volumeResetValue;
        _brightnessSlider.value = _volumeResetValue;
        _mouseSensitivitySlider.value = _volumeResetValue;
    }
    

    public void LoadSettings()
    {
        _allVolumeSlider.value = PlayerPrefs.GetFloat(VolumeName.Master);
        _backgroundVolumeSlider.value = PlayerPrefs.GetFloat(VolumeName.Background);
        _soundEffectVolumeSlider.value = PlayerPrefs.GetFloat(VolumeName.SFX);
        _musicVolumeSlider.value = PlayerPrefs.GetFloat(VolumeName.Music);
        _brightnessSlider.value = PlayerPrefs.GetFloat("BRIGHTNESS");
        _mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MOUSE_SENSITIVITY");
    }

    public void CloseSettingPanel()
    {
        PlayerPrefs.SetFloat(VolumeName.Master,_allVolumeSlider.value);
        PlayerPrefs.SetFloat(VolumeName.Background,_backgroundVolumeSlider.value);
        PlayerPrefs.SetFloat(VolumeName.Music,_musicVolumeSlider.value);
        PlayerPrefs.SetFloat(VolumeName.SFX,_soundEffectVolumeSlider.value);
        PlayerPrefs.SetFloat("BRIGHTNESS",_brightnessSlider.value);
        PlayerPrefs.SetFloat("MOUSE_SENSITIVITY",_mouseSensitivitySlider.value);
        PlayerPrefs.Save();
        if(_settingPanel.activeSelf)
            _settingPanel.SetActive(false);
    }

    public void OpenSettingPanel()
    {
        if(!_settingPanel.activeSelf)
            _settingPanel.SetActive(true);
    }

    public void ExitGame()
    { 
        Application.Quit();
#if !UNITY_EDITOR
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#elif UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void StartGame()
    {
        LoadIntroScene().Forget();
    }
    
    public async UniTaskVoid LoadIntroScene()
    {
        EventFadeChanger.Instance.Fade_img.DOFade(1, 2.0f);

        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1.0f);

        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync("Tutorial");

        await UniTask.WaitUntil(() => sceneLoad.isDone);
    }

    public void OnMasterVolumeChanged(float value)
    {
        SoundManager.SetSoundVolume(VolumeName.Master, value);
    }
    
    public void OnBackgroundVolumeChanged(float value)
    {
        SoundManager.SetSoundVolume(VolumeName.Background, value);
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        SoundManager.SetSoundVolume(VolumeName.SFX, value);
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        SoundManager.SetSoundVolume(VolumeName.Music, value);
    }
}
