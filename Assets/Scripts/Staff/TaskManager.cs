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

    public Task RandomTask()
    {
        string title = Titles[Random.Range(0, Titles.Length)];
        int daysRemaining = Random.Range(2, 5);

        return new Task(title, daysRemaining);
    }

    public void AssignTask(Task task, Staff staff)
    {
        task.AssignedTo = staff;
    }

    public void UnAssignTask(Task task)
    {
        task.AssignedTo = null;
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

    public void UpdateHour()
    {
        foreach(Task task in Tasks)
        {
            task.UpdateHour();
        }
    }

    public void UpdateDay()
    {
        DailyFinished = new List<Task>();

        foreach (Task task in Tasks)
        {
            task.UpdateDay();

            if (task.Finished)
            {
                CompleteTask(task);
            }
        }

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
