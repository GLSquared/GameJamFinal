using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    AudioSource buttonPress;
    List<RaycastResult> results = new List<RaycastResult>();
    private PointerEventData ped;

    private void Start()
    {
        ped = new PointerEventData(EventSystem.current);
        buttonPress = GetComponent<AudioSource>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.Euler(15.62f -((Input.mousePosition.y-(Screen.height/2f))/Screen.height)*5f, 
                -37.843f + ((Input.mousePosition.x-(Screen.width/2f))/Screen.width)*5f,
                0f),
            Time.deltaTime*5f);
    }

    public void startGame()
    {
        buttonPress.Play(0);
        SceneManager.LoadScene("CarsTest");
    }

    public void quitGame()
    {
        buttonPress.Play(0);

        Application.Quit();
    }
}
