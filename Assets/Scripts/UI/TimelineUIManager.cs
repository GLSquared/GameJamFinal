using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimelineUIManager : MonoBehaviour
{
    private GameObject gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("Time").GetComponent<TextMeshProUGUI>().text =
            gameManager.GetComponent<DayController>().getTime();

        transform.Find("TimelinePanel").Find("Inner").GetComponent<RectTransform>().anchorMax =
            new Vector2(gameManager.GetComponent<DayController>().dayCompletion / 100f, 1f);

        transform.Find("TasksStats").Find("TasksCompleted").GetComponent<TextMeshProUGUI>().text =
            gameManager.GetComponent<TaskManager>().DailyFinished.Count + "/" +
            gameManager.GetComponent<TaskManager>().TargetDailyTasks;
    }
}
