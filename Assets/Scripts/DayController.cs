using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class DayController : MonoBehaviour
{
    public float timeConstant = 60f;
    private float hour;
    private float minute;
    private int lastMin = 0;
    private float endHour = 17;
    float counter;
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



    private void Start()
    {
        GetComponent<TaskManager>().UpdateDay();
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        hour = 9;

        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDayOver)
        {

            counter += Time.deltaTime;
            minute = Mathf.Round(counter);
            if (lastMin < minute)
            {
                lastMin = Mathf.RoundToInt(minute);
                GetComponent<TaskManager>().UpdateMinute();
            }
            float dayCompletion = hour / endHour * 100f;
            emitter.SetParameter("Workday", dayCompletion);


            if (counter > timeConstant)
            {
                hour++;
                counter = timeConstant;

                if (hour > endHour)
                {
                    isDayOver = true;
                }
            }
        }
    }

    private void OnEnable()
    {
        isDayOver = false;
    }

    private void OnDisable()
    {
        hour = 1;
        counter = timeConstant;
    }
}
