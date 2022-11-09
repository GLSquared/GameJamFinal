using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApplicantUi
{
    public Applicant Applicant;
    public GameObject Panel;

    public ApplicantUi(Applicant applicant, GameObject panel)
    {
        Applicant = applicant;
        Panel = panel;
    }
}

public class ApplicantListManager : MonoBehaviour
{
    public Desk selectedDesk;

    private ApplicationManager applicationManager;
    private List<ApplicantUi> applications = new();

    private bool ApplicationsContains(Applicant applicant)
    {
        for (int i = 0; i < applications.Count; i++)
        {
            if (applicant == applications[i].Applicant)
                return true;
        }

        return false;
    }

    private void OnEnable()
    {
        applicationManager = GameObject.Find("GameManager").GetComponent<ApplicationManager>();
    }

    private void Update()
    {
        // Remove unecessary
        for (int i = applications.Count - 1; i >= 0; i--)
        {
            if (!applicationManager.Applicants.Contains(applications[i].Applicant))
            {
                Destroy(applications[i].Panel);
                applications.RemoveAt(i);
            }
        }

        for (int i = 0; i < applicationManager.Applicants.Count - 1; i++)
        {
            if (!ApplicationsContains(applicationManager.Applicants[i]) )
            {
                Applicant applicant = applicationManager.Applicants[i];

                GameObject applicantBtn =
                Instantiate((GameObject)Resources.Load("UI/ApplicantBtn"), transform.Find("Panel"));
                applicantBtn.transform.Find("ApplicantName").GetComponent<TextMeshProUGUI>().text =
                    applicant.Staff.Type == Staff.SkillType.Manager ? "Manager" : applicant.Staff.Name;
                applicantBtn.transform.Find("Skill").GetComponent<TextMeshProUGUI>().text =
                    Mathf.RoundToInt(applicant.Staff.Skill) + " skill points";
                applicantBtn.transform.Find("Wage").GetComponent<TextMeshProUGUI>().text =
                    "$" + Mathf.RoundToInt(applicant.Staff.ExpectedWage) + "/hr";

                if (applicant.Staff.Type == Staff.SkillType.Manager)
                {
                    applicantBtn.GetComponent<Image>().color = new Color(249f / 255f, 68f / 255f, 97f / 255f);
                }

                Staff staff = applicant.Staff;
                applicantBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Destroy(applicantBtn);
                    if (selectedDesk)
                    {
                        GameObject.Find("GameManager").GetComponent<ApplicationManager>().AcceptApplicant(applicant, selectedDesk);
                        GameObject.Find("GameManager").GetComponent<GameManager>().SelectDesk(selectedDesk);
                    }
                    else
                    {
                        GameObject.Find("GameManager").GetComponent<ApplicationManager>().AcceptApplicant(applicant);
                    }

                    gameObject.SetActive(false);
                });

                applications.Add(new ApplicantUi(applicant, applicantBtn));
            }
        }
    }
}
