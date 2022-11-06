using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    AudioSource buttonPress;

    private void Start()
    {
        buttonPress = GetComponent<AudioSource>();
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
