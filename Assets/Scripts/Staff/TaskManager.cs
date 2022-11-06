using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(GameManager))]
[RequireComponent(typeof(StaffManager))]
public class TaskManager : MonoBehaviour
{
    private GameManager gameManager;
    private StaffManager staffManager;

    private string[] Titles = new string[] { "Work work work", "More work work", "Work work work work" };

    public List<Task> Tasks = new List<Task>();
    public List<Task> DailyFinished = new List<Task>();
    public int TargetDailyTasks;

    public Task RandomTask()
    {
        string title = Titles[Random.Range(0, Titles.Length)];
        int daysRemaining = Random.Range(2, 5);

        return new Task(title, daysRemaining);
    }

    public void CreateNewTask()
    {
        Tasks.Add(RandomTask());
    }

    public void CompleteTask(Task task)
    {
        DailyFinished.Add(task);
        Tasks.Remove(task);
    }

    void CheckCompletedTasks()
    {
        for (int i = 0; i < Tasks.Count; i++)
        {
            Task task = Tasks[i];
            if (task.Finished)
            {
                CompleteTask(task);
            }
        }
    }

    public void UpdateMinute()
    {
        for (int i = 0; i < Tasks.Count; i++)
        {
            Tasks[i].UpdateMinute();
        }

        CheckCompletedTasks();
    }

    public void UpdateDay()
    {
        TargetDailyTasks = GetComponent<StaffManager>().ActiveStaff.Count * 4;
        DailyFinished = new List<Task>();

        CheckCompletedTasks();

        if (Tasks.Count < staffManager.ActiveStaff.Count)
        {
            int taskRemaining = staffManager.ActiveStaff.Count - Tasks.Count;
            
            for (int i = 0; i < taskRemaining; i++)
            {
                Tasks.Add(RandomTask());
            }
        }
    }

    public void OnEnable()
    {
        gameManager = GetComponent<GameManager>();
        staffManager = GetComponent<StaffManager>();
    }
}
