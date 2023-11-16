using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TitleMenuManger : MonoBehaviour
{
    private static TitleMenuManger instance = null;

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
    
    
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void Start()
    {
        ResetSettings();
        LoadSettings();
    }

    public static TitleMenuManger Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public void ResetSettings()
    {
        _allVolumeSlider.value = 0.5f;
        _backgroundVolumeSlider.value = 0.5f;
        _soundEffectVolumeSlider.value = 0.5f;
        _musicVolumeSlider.value = 0.5f;
        _brightnessSlider.value = 0.5f;
        _mouseSensitivitySlider.value = 0.5f;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("ALL_VOLUME",_allVolumeSlider.value);
        PlayerPrefs.SetFloat("BACKGROUND_VOLUME",_backgroundVolumeSlider.value);
        PlayerPrefs.SetFloat("SOUND_EFFECT_VOLUME",_soundEffectVolumeSlider.value);
        PlayerPrefs.SetFloat("MUSIC_VOLUME",_musicVolumeSlider.value);
        PlayerPrefs.SetFloat("BRIGHTNESS",_brightnessSlider.value);
        PlayerPrefs.SetFloat("MOUSE_SENSITIVITY",_mouseSensitivitySlider.value);
        
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        _allVolumeSlider.value = PlayerPrefs.GetFloat("ALL_VOLUME");
        _backgroundVolumeSlider.value = PlayerPrefs.GetFloat("BACKGROUND_VOLUME");
        _soundEffectVolumeSlider.value = PlayerPrefs.GetFloat("SOUND_EFFECT_VOLUME");
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MUSIC_VOLUME");
        _brightnessSlider.value = PlayerPrefs.GetFloat("BRIGHTNESS");
        _mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MOUSE_SENSITIVITY");
    }

    public void CloseSettingPanel()
    {
        SaveSettings();
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
        EventFadeChanger.Instance.Fade_img.DOFade(1, 3.0f);

        await UniTask.WaitUntil(() => EventFadeChanger.Instance.Fade_img.alpha >= 1.0f);

        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync("QuetesTest");

        await UniTask.WaitUntil(() => sceneLoad.isDone);
    }
}
