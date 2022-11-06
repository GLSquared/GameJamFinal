using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSelectionManager : MonoBehaviour
{
    private Staff selectedStaff;

    public void SelectTask()
    {
        List<Task> tasks = GameObject.Find("GameManager").GetComponent<TaskManager>().Tasks;
        
        foreach (Transform t in transform.Find("Panel"))
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < tasks.Count; i++)
        {
            
        }
    }

}
