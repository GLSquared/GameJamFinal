using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class DayController : MonoBehaviour
{
    public float timeConstant = 60f;
    private int hour;
    private float minute;
    public int dayNumber = 0;
    private int startHour = 8;
    private float endHour = 17;
    private StudioEventEmitter emitter;
    public float dayCompletion;

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
            
            dayCompletion = (hour-8) / (endHour-8) * 100f;
            emitter.SetParameter("Workday", dayCompletion);

            if (minute > timeConstant)
            {
                hour++;
                GetComponent<StaffManager>().UpdateHour();
                minute = 0;

                if (hour > endHour)
                {
                    dayNumber++;
                    GetComponent<GameManager>().EndDayStats();
                    isDayOver = true;
                    emitter.Stop();
                    enabled = false;
                }
            }
            
        }

        yield return new WaitForSeconds(timeConstant/60f);

        if (!isDayOver)
        {
            StartCoroutine(IncrementMinutes());
        }
    }

    private void Start()
    {
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        hour = startHour;
    }
    
    private void OnEnable()
    {
        GetComponent<TaskManager>().UpdateDay();
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        isDayOver = false;
        emitter.Play();
        StartCoroutine(IncrementMinutes());
    }

    private void OnDisable()
    {
        hour = startHour;
    }
}
