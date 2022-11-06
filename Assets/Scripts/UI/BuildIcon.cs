using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    private bool mouse_over = false;
    void Update()
    {
        if (mouse_over)
        {
            transform.Find("GameObject").localRotation =
                transform.Find("GameObject").localRotation * Quaternion.Euler(0, Time.deltaTime*50f, 0);
        }
    }
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
    }
}
