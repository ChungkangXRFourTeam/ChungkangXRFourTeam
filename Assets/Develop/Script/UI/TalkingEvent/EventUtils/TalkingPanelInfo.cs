using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
        _panel.transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        if (CompareTag("Player"))
        {
            _panel.transform.rotation = Quaternion.Euler(0,0,0); 
            if(gameObject.transform.rotation.eulerAngles.y == 180)
                _panel.transform.localPosition = new Vector3(-2.37f, 4.11f);
            else
                _panel.transform.localPosition = new Vector3(2.5f, 4.11f);
        } 
    }
}
