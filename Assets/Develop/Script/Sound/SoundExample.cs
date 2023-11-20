using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundExample : MonoBehaviour
{
    private void OnGUI()
    {
        // 올바른 예 1
        if (GUILayout.Button("valid case1 play"))
        {
            if (SoundManager.TryGetSound("ui/02_SFX_title_clickMouse", out var sound))
            {
                var source = GetComponent<AudioSource>();
                source.clip = sound;
                source.Play();
            }
        }

        // 올바른 예 2
        if (GUILayout.Button("valid case2 play"))
        {
            var sound = SoundManager.GetSoundOrNull("ui/02_SFX_title_clickMouse");
            var source = GetComponent<AudioSource>();
            source.clip = sound;
            source.Play();
        }

        // 올바른 예 3
        if (GUILayout.Button("valid case3 play"))
        {
            SoundManager.ScheduleSound(new SoundCommand()
            {
                Key = "ui/02_SFX_title_clickMouse"
            });
            
            // Duration: 몇초 뒤에 재생될지 설정
            SoundManager.ScheduleSound(new SoundCommand()
            {
                Key = "ui/02_SFX_title_clickMouse",
                Duration = 1f
            });
            SoundManager.ScheduleSound(new SoundCommand()
            {
                Key = "ui/02_SFX_title_clickMouse",
                Duration = 2f
            });
        }

        // 올바르지 않는 예 1, 유효하지 않는 tableKey 사용
        if (GUILayout.Button("invalid case1 play"))
        {
            // "Ui" 가 아니라 "ui" 임. 잘못 쓴 예.
            var sound = SoundManager.GetSoundOrNull("Ui/02_SFX_title_clickMouse");
            var source = GetComponent<AudioSource>();
            source.clip = sound;
            source.Play();
        }
        
        // 올바르지 않는 예 2, 유효하지 않는 soundKey 사용
        if (GUILayout.Button("invalid case2 play"))
        {
            var sound = SoundManager.GetSoundOrNull("ui/asd");
            var source = GetComponent<AudioSource>();
            source.clip = sound;
            source.Play();
        }
        
        // 올바르지 않는 예 3, 유효하지 않는 형식 사용
        if (GUILayout.Button("invalid case3 play"))
        {
            var sound = SoundManager.GetSoundOrNull("ui asd");
            var source = GetComponent<AudioSource>();
            source.clip = sound;
            source.Play();
        }
    }
}