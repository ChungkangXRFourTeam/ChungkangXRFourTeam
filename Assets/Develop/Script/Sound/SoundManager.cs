using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XRProject.Utils.Log;

using TD = System.Collections.Generic.Dictionary<string,UnityEngine.AudioClip>;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _inst;
    private static float _volumeInitValue = 0.5f;

    private SoundTableSet _tables;
    private Dictionary<string, TD> _tableDict;
    private SoundScheduler _scheduler;
    private Dictionary<string, float> _volumeDict;

    private const string PATH = "SoundTable/SoundTableSet";
    private const string LOG_SIGNATURE = "sound";
    private readonly string[] VOLUMES = { "MASTER_VOLUME", "BACKGROUND_VOLUME", "SOUND_EFFECT_VOLUME", "MUSIC_VOLUME" };
    
    public static void Init()
    {
        InstanceInit();
        TableInit();
        SoundVolumeInit();
    }

    #region Initializer
    private static void InstanceInit()
    {
        if (_inst)
        {
            Destroy(_inst.gameObject);
            _inst = null;
        }

        _inst = new GameObject("[SoundManager]").AddComponent<SoundManager>();
        DontDestroyOnLoad(_inst.gameObject);
        _inst._scheduler = new GameObject("SoundScheduler").AddComponent<SoundScheduler>();
        _inst._scheduler.Init();
        
        /// 스케쥴러가 씬 이동마다 Destroy되어 다음 씬에서 호출되지 않는 문제를 막기 위해 선언
        DontDestroyOnLoad(_inst._scheduler);
    }

    private static void TableInit()
    {
        _inst._tables = Resources.Load<SoundTableSet>(PATH);

        if (_inst._tables == false)
        {
            XLog.LogError("SoundTable load fail", LOG_SIGNATURE);
            return;
        }

        var tableDict = new Dictionary<string, TD>();
        _inst._tableDict = tableDict;
        foreach (var item in _inst._tables.Tables)
        {
            if (tableDict.ContainsKey(item.Key))
            {
                XLog.LogError($"SoundTable key('{item.Key}') is already exist", LOG_SIGNATURE);
                return;
            }
            
            var dict = new TD();
            tableDict.Add(item.Key, dict);

            foreach (var pair in item.Table)
            {
                if (dict.TryAdd(pair.Key, pair.Sound) == false)
                {
                    XLog.LogError($"SoundTable key('{item.Key}') is already exist", LOG_SIGNATURE);
                    return;
                }
            }
        }

    }

    private static void SoundVolumeInit()
    {
        string[] volumes = _inst.VOLUMES;
        
        if (!PlayerPrefs.HasKey("isNew"))
        {
            foreach (string volume in volumes)
            {
                PlayerPrefs.SetFloat(volume,_volumeInitValue);
            }
            PlayerPrefs.SetInt("isNew",1);
        }
        
        var volumeDict = new Dictionary<string, float>();
        _inst._volumeDict = volumeDict;
        
        foreach (string volume in volumes)
        {
            if(volumeDict.ContainsKey(volume))
                XLog.LogError($"SoundVolume key ('{volume}') is already exist",LOG_SIGNATURE);
            
            volumeDict.Add(volume,PlayerPrefs.GetFloat(volume));
        }
    }
    #endregion

    #region private
    private void OnDestroy()
    {
        _inst._scheduler.Release();
        _tableDict = null;
        _volumeDict = null;
        _tables = null;
        _inst = null;
        
    }
    private static bool CheckInit()
    {
        if (_inst == false)
        {
            XLog.LogError("SoundManager is not initialized", LOG_SIGNATURE);
            return false;
        }

        return true;
    }
    private static (string, string)? SplitKey(ref string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        var str = key.Split('/');

        if (str.Length < 2) return null;

        return (str[0], str[1]);
    }
    #endregion
    
    /// <summary>
    /// SoundTable을 참조해 AudioClip 객체를 반환하는 함수입니다.
    /// SoundTable에 없는 key 이거나, 유효하지 않는 key라면 Error 로그를 발생시킵니다.
    /// XLog를 사용하여 에러를 발생시킵니다.
    /// </summary>
    /// <param name="key">사운드 key, readme 참조</param>
    /// <param name="loggingError">실패시 에러 발생 여부</param>
    /// <returns></returns>
    public static AudioClip GetSoundOrNull(string key, bool loggingError = true)
    {
        if (CheckInit() == false) return null;

        string tableKey, soundKey;
        var pair= SplitKey(ref key);
        if (pair == null)
        {
            if(loggingError)
                XLog.LogError($"it is invalid key('{key}')", LOG_SIGNATURE);
            
            return null;
        }

        tableKey = pair.Value.Item1;
        soundKey = pair.Value.Item2;

        if (_inst._tableDict.TryGetValue(tableKey, out var table))
        {
            if (table.TryGetValue(soundKey, out var sound))
            {
                return sound;
            }
            
            if(loggingError)
                XLog.LogError($"it is invalid soundKey('{soundKey}')", LOG_SIGNATURE);
        }
        else
        {
            if(loggingError)
                XLog.LogError($"it is invalid tableKey('{tableKey}')", LOG_SIGNATURE);
        }

        return null;
    }

    /// <summary>
    /// SoundTable을 참조해 AudioClip 객체를 반환하는 함수입니다.
    /// SoundTable에 없는 key 이거나, 유효하지 않는 key라면 Error 로그를 발생시킵니다.
    /// XLog를 사용하여 에러를 발생시킵니다.
    /// </summary>
    /// <param name="key">사운드 key, readme 참조</param>
    /// <param name="sound">반환될 객체</param>
    /// <param name="loggingError">실패시 에러 발생 여부</param>
    /// <returns></returns>
    public static bool TryGetSound(string key, out AudioClip sound, bool loggingError = true)
    {
        sound = null;
        if (CheckInit() == false) return false;
        
        sound = GetSoundOrNull(key, loggingError);

        if (sound == false)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 사운드 재생을 예약합니다.
    /// 몇초 뒤에 재생될지는 Command의 Duration 값을 통해 정의합니다. 
    /// </summary>
    /// <param name="command">사운드 key와 기타 audioSource 설정 값들을 정의하는 컨테이너 객체입니다.</param>
    public static void ScheduleSound(SoundCommand command)
    {
        if (TryGetSound(command.Key, out var sound))
        {
            command.clip = sound;
            
            _inst._scheduler.Schedule(command);
        }
    }

    /// <summary>
    /// 사운드 종류에 따른 볼륨값을 가져옵니다.
    /// 사운드 크기는 사운드 종류 볼륨 * 마스터 볼륨 값입니다.
    /// 키 값은 VolumeName 클래스 안에 정의되어 있습니다.
    /// </summary>
    /// <param name="key">볼륨 정보를 가져오게 할 key값</param>
    public static float GetSoundVolume(string key)
    {
        if (_inst._volumeDict.TryGetValue(key, out float commandVolume) && 
            _inst._volumeDict.TryGetValue(VolumeName.Master, out float masterVolume))
        {
            return commandVolume * masterVolume;
        }

        return 0f;
    }
    
    public static void SetSoundVolume(string key,float value)
    {
        _inst._volumeDict[key] = value;
    }
    
}