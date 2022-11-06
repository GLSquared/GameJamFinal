using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApplicantListManager : MonoBehaviour
{
    public Desk selectedDesk;

    IEnumerator refreshApplicants()
    {

        foreach (Transform t in transform.Find("Panel"))
        {
            Destroy(t.gameObject);
        }

        List<Applicant> applicants = GameObject.Find("GameManager").GetComponent<ApplicationManager>().Applicants;

        print(applicants.Count);
        for (int i = 0; i < Mathf.Min(7, applicants.Count); i++)
        {
            GameObject applicantBtn = Instantiate((GameObject) Resources.Load("UI/ApplicantBtn"), transform.Find("Panel"));
            applicantBtn.transform.Find("ApplicantName").GetComponent<TextMeshProUGUI>().text =
                applicants[i].Staff.Name;
            applicantBtn.transform.Find("Skill").GetComponent<TextMeshProUGUI>().text =
                Mathf.RoundToInt(applicants[i].Staff.Skill).ToString() + " skill points";
            applicantBtn.transform.Find("Wage").GetComponent<TextMeshProUGUI>().text =
                 "$" + Mathf.RoundToInt(applicants[i].Staff.ExpectedWage).ToString() + "/hr";

            Staff staff = applicants[i].Staff;
            applicantBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(applicantBtn);
                if (selectedDesk)
                {
                    GameObject.Find("GameManager").GetComponent<StaffManager>().RecruitStaffToDesk(staff, selectedDesk);
                    GameObject.Find("GameManager").GetComponent<GameManager>().SelectDesk(selectedDesk);
                }
                else
                {
                    GameObject.Find("GameManager").GetComponent<StaffManager>().RecruitStaff(staff);
                }
                gameObject.SetActive(false);
            });
        }
        
        yield return new WaitForSeconds(1f);
        StartCoroutine(refreshApplicants());
    }

    private void Start()
    {
        StartCoroutine(refreshApplicants());
    }
}
