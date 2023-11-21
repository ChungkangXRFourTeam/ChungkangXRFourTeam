using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoolTimeHud : MonoBehaviour
{
    [SerializeField] private Image _panel;
    [SerializeField] private TMP_Text _text;

    private PlayerController _pc;

    public void SetCoolTime(float currentTime, float maxTime)
    {
        if (_panel == false || _text == false) return;
        
        float normalized = Mathf.Clamp01( currentTime / maxTime);
        string coolTime = GetTimeToText(currentTime);

        _panel.fillAmount = normalized;
        _text.text = coolTime;

        bool value = currentTime > 0f;
        _panel.gameObject.SetActive(value);
        _text.gameObject.SetActive(value);
    }

    private string GetTimeToText(float time)
    {
        string str;
        if (time <= 1f)
        {
            float temp = time * 10f;
            temp = (int)temp;
            temp *= 0.1f;
            str = temp.ToString();
        }
        else
        {
            str = ((int)time).ToString();
        }

        return str;
    }
    private void Awake()
    {
        _pc = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();

        if (_pc == false)
            enabled = false;
    }

    private void Update()
    {
        SetCoolTime(_pc.SwingCoolTime, _pc.MaxSwingCoolTime);
    }
}
