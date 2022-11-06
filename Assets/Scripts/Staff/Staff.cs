using System;
using System.Collections;
using System.Collections.Generic;

public class Staff
{
    public enum SkillType
    {
        None,
        Developer,
        Manager,
        Owner
    }

    public string Name;

    public int CurrentWage;
    public int ExpectedWage;

    public SkillType Type;
    public float Skill;

    public float Mood;
    public int DaysWithBadMood;
    public int MaxDaysWithBadMood;

    public Staff(string name, int expectedWage, SkillType type, float skill, int maxDaysWithBadMood)
    {
        Name = name;
        CurrentWage = expectedWage;
        ExpectedWage = expectedWage;
        Type = type;
        Skill = skill;
        Mood = 100f;
        MaxDaysWithBadMood = maxDaysWithBadMood;
        DaysWithBadMood = 0;
    }

    public void UpdateHour()
    {
        if (Type != SkillType.Owner)
        {
            if (CurrentWage >= ExpectedWage && Mood < 100f)
            {
                Mood = Math.Clamp(Mood + 0.01f, 0f, 100f);
            }
            else if (CurrentWage < ExpectedWage && Mood > 0f)
            {
                Mood = Math.Clamp(Mood - 0.01f, 0f, 100f);
            }
        }
    }

    public void UpdateDay()
    {
        if (Type == SkillType.Developer)
            if (Mood <= 0f)
            {
                DaysWithBadMood++;
            }
            else if (Mood > 0f && DaysWithBadMood > 0)
            {
                DaysWithBadMood--;

                if (Mood > 50f && Skill < 100f)
                {
                    Skill = Math.Clamp(Skill + (0.001f * Mood) * (float)Math.Log(Skill), Skill, 100f);
                }
            }
            else if (Type == SkillType.Owner)
            {
                Skill = Math.Clamp(Skill + (0.001f * Mood) * (float)Math.Log(Skill), Skill, 100f);
            }
    }

    public override string ToString()
    {
        return Name + " " + Type + " " + ExpectedWage +  " " + Skill;
    }
}
