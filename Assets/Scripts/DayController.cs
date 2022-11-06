using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class DayController : MonoBehaviour
{
    public float timeConstant = 60f;
    private int hour;
    private int minute;
    private int startHour = 9;
    private int endHour = 17;
    private StudioEventEmitter emitter;

    private bool isDayOver = false;

    public bool IsDayOver
    {
        get{ return isDayOver; }
    }

    public String getTime()
    {
        var formatedHour = hour.ToString("00");
        var formatedMinute = minute.ToString("00");

        return formatedHour + ":" + formatedMinute;
    }


    IEnumerator IncrementMinutes()
    {
        if (!isDayOver)
        {

            minute++;
            GetComponent<TaskManager>().UpdateMinute();
            
            float dayCompletion = hour / endHour * 100f;
            emitter.SetParameter("Workday", dayCompletion);

            if (minute > 60)
            {
                hour++;
                minute = 0;

                if (hour > endHour)
                {
                    isDayOver = true;
                }
            }
            
        }

        yield return new WaitForSeconds(1);

        if (!isDayOver)
        {
            StartCoroutine(IncrementMinutes());
        }
    }

    private void Start()
    {
        GetComponent<TaskManager>().UpdateDay();
        
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        hour = startHour;
        
        StartCoroutine(IncrementMinutes());
    }
    
    private void OnEnable()
    {
        isDayOver = false;
    }

    private void OnDisable()
    {
        hour = 1;
    }
}
