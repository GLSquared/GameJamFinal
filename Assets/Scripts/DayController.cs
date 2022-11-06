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
    private float minute;
    private int startHour = 1;
    private float endHour = 17;
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
            
            float dayCompletion = (hour-8) / (endHour-8) * 100f;
            print(dayCompletion);
            emitter.SetParameter("Workday", dayCompletion);

            if (minute > timeConstant)
            {
                hour++;
                minute = 0;

                if (hour > endHour)
                {
                    isDayOver = true;
                    emitter.Stop();
                    this.enabled = false;
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
        hour = startHour + 8;
        
        StartCoroutine(IncrementMinutes());
    }
    
    private void OnEnable()
    {
        isDayOver = false;
        emitter.Play();
    }

    private void OnDisable()
    {
        hour = 1;
    }
}
