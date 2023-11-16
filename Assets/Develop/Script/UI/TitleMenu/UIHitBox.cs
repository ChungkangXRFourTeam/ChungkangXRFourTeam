using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHitBox : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInParent<Image>().enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponentInParent<Image>().enabled = false;
    }
}
