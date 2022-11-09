using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Staff;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GameManager))]
[RequireComponent(typeof(TaskManager))]
public class StaffManager : MonoBehaviour
{
    private GameManager gameManager;
    private TaskManager taskManager;

    //public string[] Names = new string[] { "Gabriel Bonello", "Luke Musu", "Luke Zammit" };
    public string[] Names = new string[500];

    public List<Staff> ActiveStaff = new List<Staff>();

    private TextAsset namesTxt;

    public int managersOwned = 0;

    public Staff RandomStaff()
    {
        string name = Names[Random.Range(0, Names.Length)];
        SkillType skillType = SkillType.Developer;
        float skill = Random.Range(1f, 75f);
        int expectedWage = (int)(Mathf.Pow(skill, 1.15f) / 1.5f);
        if (Random.Range(1, 8) > 6 && managersOwned < 2)
        {
            skillType = SkillType.Manager;
            skill = 100f;
            expectedWage = (int)(Mathf.Pow(skill, 1.15f) / 1.5f);
        }
        int maxDayWithBadMood = Random.Range(5, 10);

        return new Staff(name, expectedWage, skillType, skill, maxDayWithBadMood);
    }

    public Staff RandomStaff(int askingWage)
    {
        string name = Names[Random.Range(0, Names.Length)];
        SkillType skillType = SkillType.Developer;
        float skill = Random.Range(1f, askingWage * 2.5f);
        int expectedWage = (int)(skill / 2.5f);
        if (Random.Range(1, 8) > 6 && managersOwned < 2)
        {
            skillType = SkillType.Manager;
            skill = 100f;
            expectedWage = (int)(Mathf.Pow(skill, 1.15f) / 1.5f);
        }
        int maxDayWithBadMood = Random.Range(5, 10);

        return new Staff(name, expectedWage, skillType, skill, maxDayWithBadMood);
    }

    public Staff CreateOwner(string name)
    {
        Staff owner = new Staff(name, 0, SkillType.Owner, 50f, 0);

        ActiveStaff.Add(owner);

        return owner;
    }

    public void RecruitStaff(Staff newStaff)
    {
        ActiveStaff.Add(newStaff);
        GetComponent<TaskManager>().CreateNewTask();

        if (newStaff.Type == SkillType.Manager)
            managersOwned++;

        foreach (Desk desk in gameManager.desks)
        {
            if (desk.staff == null)
            {
                desk.staff = newStaff;
                gameManager.AddCharacterToDesk(desk.gameObject);
                break;
            }
        }
    }
    
    public void RecruitStaffToDesk(Staff newStaff, Desk desk)
    {
        ActiveStaff.Add(newStaff);
        desk.staff = newStaff;

        if (newStaff.Type == SkillType.Manager)
            managersOwned++;

        GetComponent<TaskManager>().CreateNewTask();

        gameManager.AddCharacterToDesk(desk.gameObject);
    }

    public void FireStaff(Staff staff)
    {
        ActiveStaff.Remove(staff);

        foreach (Desk desk in gameManager.desks)
        {
            if (desk.staff == staff)
            {
                desk.staff = null;
                gameManager.RemoveCharacterFromDesk(desk.gameObject);
                break;
            }
        }
    }

    public void StaffQuit(Staff staff)
    {
        ActiveStaff.Remove(staff);
    }

    public Task GetTaskFromStaff(Staff staff)
    {
        foreach(Task task in taskManager.Tasks)
        {
            if (task.AssignedTo == staff)
            {
                return task;
            }
        }

        return null;
    }

    public void UpdateHour()
    {
        foreach (Staff staff in ActiveStaff)
        {
            staff.UpdateHour();

            if (staff.Type != SkillType.Owner)
            {
                gameManager.cash-=staff.CurrentWage;
            }

            if (staff.Type == SkillType.Manager)
            {
                List<Desk> inRadius = gameManager.GetDesksInRadius(staff, 2.5f);
                List<Staff> notWorking = new List<Staff>();

                foreach (Desk desk in inRadius)
                {
                    if (desk.staff != null && GetTaskFromStaff(desk.staff) == null && desk.staff.Type != SkillType.Manager)
                    {
                        List<Task> availableTasks = taskManager.GetUnassignedTasks();

                        if (availableTasks.Count > 0)
                        {
                            availableTasks[0].AssignedTo = desk.staff;
                        }
                    }
                }
            }
        }
    }

    public void UpdateDay()
    {
        foreach (Staff staff in ActiveStaff)
        {
            staff.UpdateDay();

            if (staff.DaysWithBadMood > staff.MaxDaysWithBadMood && Random.Range(1, 5) < 5)
            {
                StaffQuit(staff);
            }
        }
    }

    public void OnEnable()
    {
        gameManager = GetComponent<GameManager>();
        taskManager = GetComponent<TaskManager>();
    }

    public void populateNamesArray()
    {
        String text = File.ReadAllText("Assets/Scripts/Staff/names.txt");


        char[] seperator = { '\n' };
        string[] strValues = text.Split(seperator);


        for (int i = 0; i < 500; i++)
        {
            Names[i] = strValues[i];
        }
    }

    private void Start()
    {
        populateNamesArray();
    }
}
