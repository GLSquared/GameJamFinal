using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Applicant
{
    public Staff Staff;
    public float Expires;
    public float MaxTime;

    public Applicant(Staff staff)
    {
        Staff = staff;
        MaxTime = Random.Range(10, 60);
        Expires = Time.time + MaxTime;
    }
}

[RequireComponent(typeof(StaffManager))]
[RequireComponent(typeof(GameManager))]
public class ApplicationManager : MonoBehaviour
{
    GameManager gameManager;
    StaffManager staffManager;

    public List<Applicant> Applicants = new();

    private int availableDesks = 0;
    private int MaxApplicants = 10;


    public void PopulateApplicants()
    {
        Applicants.Add(new Applicant(staffManager.RandomStaff()));
    }

    public void PopulateApplicants(int askingWage)
    {
        Applicants.Add(new Applicant(staffManager.RandomStaff(askingWage)));
    }

    public void AcceptApplicant(Applicant applicant)
    {
        Applicants.Remove(applicant);
    }

    public void RevokeApplicant(Applicant applicant)
    {
        Applicants.Remove(applicant);
    }

    public void OnEnable()
    {
        gameManager = GetComponent<GameManager>();
        staffManager = GetComponent<StaffManager>();
    }

    public void UpdateApplications()
    {
        availableDesks = 0;

        foreach(Desk desk in gameManager.desks)
        {
            if (desk.staff == null)
            {
                availableDesks++;
            }
        }
        for (int i = Applicants.Count - 1; i >= 0; i--)
        {
            if (Applicants[i].Expires <= Time.time)
            {
                RevokeApplicant(Applicants[i]);
            }
        }   
        
        if (availableDesks == 0 && Applicants.Count > 0)
        {
            Applicants.Clear();
        } else if (availableDesks > 0 && Applicants.Count < MaxApplicants)
        {
            if (gameManager.askingWage == 0)
                PopulateApplicants();
            else
                PopulateApplicants(gameManager.askingWage);
        }
    }

    private void Update()
    {
        UpdateApplications();
    }
}
