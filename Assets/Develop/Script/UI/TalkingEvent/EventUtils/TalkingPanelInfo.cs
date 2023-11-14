using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingPanelInfo : MonoBehaviour
{
    public GameObject _panel;
    public GameObject _talkingImage;
    public GameObject _eventText;
    public GameObject _endButton;
    private void Awake()
    {
        _panel = transform.Find("TalkingPanel").gameObject;
        _talkingImage = _panel.transform.GetChild(0).gameObject;
        _eventText = _talkingImage.transform.GetChild(0).gameObject;
        _endButton = _talkingImage.transform.GetChild(1).gameObject;
    }
    
}
