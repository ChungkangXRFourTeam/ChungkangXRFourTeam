using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//사운드 볼륨 키값을 가지고 있는 클래스
//절대 수정하지 말것
public static class VolumeName {
    public const string Master = "MASTER_VOLUME";
    public const string Background = "BACKGROUND_VOLUME";
    public const string SFX= "SOUND_EFFECT_VOLUME";
    public const string Music = "MUSIC_VOLUME";
}

[System.Serializable]
public struct SoundCommand
{
    // 이건 건들지 말기
    // 스케쥴링용으로 예약된 변수임
    public string Key;
    public float Duration;
    
    // TODO: AudioSource의 설정값들을 조절하고 싶다면, 이곳에 변수를 추가하기
    internal AudioClip clip;
    internal string volumeKey;
}

internal partial class SoundScheduleItem
{
    // TODO: 위에서 추가한 값들을 적용하는 로직 작성
    private void SetProperties(ref SoundCommand command, ref AudioSource source)
    {
        source.clip = command.clip;
        source.volume = SoundManager.GetSoundVolume(command.volumeKey);
    }
}