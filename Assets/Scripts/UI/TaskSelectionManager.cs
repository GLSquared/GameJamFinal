using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskSelectionManager : MonoBehaviour
{
    private Staff selectedStaff;
    
    public void SelectTask(Staff staff)
    {
        selectedStaff = staff;
        gameObject.SetActive(true);
        List<Task> tasks = GameObject.Find("GameManager").GetComponent<TaskManager>().Tasks;
        foreach (Transform t in transform.Find("Panel"))
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < Mathf.Min(7, tasks.Count); i++)
        {
            if (tasks[i].AssignedTo == null)
            {
                GameObject taskPanel = Instantiate((GameObject)Resources.Load("UI/TaskBtn"), transform.Find("Panel"));
                taskPanel.transform.Find("ApplicantName").GetComponent<TextMeshProUGUI>().text = tasks[i].Name;

                Task task = tasks[i];
                taskPanel.GetComponent<Button>().onClick.AddListener(() =>
                {
                    task.AssignedTo = selectedStaff;
                    gameObject.SetActive(false);
                });
            }
        }
    }

}
