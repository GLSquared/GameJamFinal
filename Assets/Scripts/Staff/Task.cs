using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Staff;

public class Task
{
    public string Name;

    public float Completion;
    public int DaysRemaining;
    public bool Finished;

    public Staff AssignedTo;

    public Task(string name, int daysRemaining)
    {
        Name = name;
        DaysRemaining = daysRemaining;
    }

    public Task(string name, int daysRemaining, Staff assignedTo)
    {
        Name = name;
        DaysRemaining = daysRemaining;
        AssignedTo = assignedTo;
    }

    public void ChangeAssignedTo(Staff assignedTo)
    {
        AssignedTo = assignedTo;
    }

    public void UpdateMinute()
    {
        if (!Finished && AssignedTo != null)
        {
            Completion = Math.Clamp(Completion + (AssignedTo.Skill / 5f), 0f, 100f);
        }

        if (Completion >= 100f)
        {
            Finished = true;
            AssignedTo = null;
            GameObject.Find("GameManager").GetComponent<TaskManager>().CreateNewTask();
        }
    }
}
