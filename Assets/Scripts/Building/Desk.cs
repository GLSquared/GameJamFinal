using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desk : MonoBehaviour
{
    public Staff staff;
    public Canvas canvas;
    public Canvas managerCanvas;
    public StaffManager staffManager;

    private void Start()
    {
        staffManager = GameObject.Find("GameManager").GetComponent<StaffManager>();
    }

    public void Update()
    {
        Camera camera = Camera.main;
        canvas.transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
        managerCanvas.transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);

        if ((staff!=null) && (staffManager.GetTaskFromStaff(staff)!=null))
        {
            canvas.transform.gameObject.SetActive(true);
            canvas.transform.Find("Progress").GetComponent<RectTransform>().anchorMax =
                Vector2.Lerp(canvas.transform.Find("Progress").GetComponent<RectTransform>().anchorMax,
                    new Vector2(staffManager.GetTaskFromStaff(staff).Completion / 100f, 1f), Time.deltaTime*10f);
        }
        else
        {
            canvas.transform.gameObject.SetActive(false);
        }

        if ((staff != null) && (staff.Type == Staff.SkillType.Manager))
        {
            managerCanvas.transform.gameObject.SetActive(true);
        }

    }
}
