using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BuildModeManager))]
[RequireComponent(typeof(GridBuildingManager))]
public class GameManager : MonoBehaviour
{
    public int popularity = 1;
    public int ratings = 50;
    public int cash = 100;
    public int askingWage = 0;
    public int DailyPofit = 0;
    
    public TextMeshProUGUI cashTxt;
    public TextMeshProUGUI popularityTxt;
    public TextMeshProUGUI staffTxt;
    public TextMeshProUGUI inflowTxt;
    public TextMeshProUGUI expensesTxt;
    
    public List<Desk> desks;

    public GameObject selectedStaffPanel;
    public GameObject selectedDesk;
    public GameObject staffHiringPanel;
    
    //staffPanelStuff
    public GameObject panelHireBtn;
    public GameObject panelTaskBar;
    public GameObject panelTaskTitle;
    public GameObject panelMoodBar;
    public GameObject giveTaskBtn;

    public StaffManager staffManager;
    public GameObject taskSelectionPanel;
    public GameObject buildModeUIPanel;

    public GameObject endOfDayStatsPanel;

    public Desk GetDeskFromStaff(Staff staff)
    {
        foreach (Desk desk in desks)
        {
            if (desk.staff == staff)
                return desk;
        }

        return null;
    }

    public List<Desk> GetDesksInRadius(Staff staff, float radius)
    {
        List<Desk> desksInRad = new List<Desk>();
        Desk deskCenter = GetDeskFromStaff(staff);

        foreach (Desk desk in desks)
        {
            if (deskCenter!=desk && (deskCenter.transform.position - desk.transform.position).magnitude <= radius)
                desksInRad.Add(desk);
        }

        return desksInRad;
    }
    
    public void SelectDesk(Desk desk)
    {
        selectedStaffPanel.SetActive(true);
        buildModeUIPanel.SetActive(false);
        selectedDesk = desk.gameObject;
        if (desk.staff != null)
        {
            //if has staff
            selectedStaffPanel.transform.Find("StaffTitle").GetComponent<TextMeshProUGUI>().text = desk.staff.Name;
            if (desk.staff.Type == Staff.SkillType.Manager)
            {
                selectedStaffPanel.transform.Find("StaffTitle").GetComponent<TextMeshProUGUI>().text = desk.staff.Name + "\nManager";
                panelTaskBar.SetActive(false);
                panelMoodBar.SetActive(true);
                panelTaskTitle.SetActive(false);
            } else
            {
                panelTaskBar.SetActive(true);
                panelMoodBar.SetActive(true);
                panelTaskTitle.SetActive(true);
            }
            panelHireBtn.SetActive(false);
        }
        else
        {
            //if no staff
            selectedStaffPanel.transform.Find("StaffTitle").GetComponent<TextMeshProUGUI>().text = "Vacant";
            panelHireBtn.SetActive(true);
            panelTaskBar.SetActive(false);
            panelMoodBar.SetActive(false);
            panelTaskTitle.SetActive(false);
        }
    }

    public void SelectTaskForStaff()
    {
        taskSelectionPanel.GetComponent<TaskSelectionManager>().SelectTask(selectedDesk.GetComponent<Desk>().staff);
    }
    
    public void SelectDeskToHireFor()
    {
        staffHiringPanel.GetComponent<ApplicantListManager>().selectedDesk = selectedDesk.GetComponent<Desk>();
        staffHiringPanel.SetActive(true);
    }

    public void MoveSelectedDesk()
    {
        GameObject origTile = GetComponent<GridBuildingManager>().GetTileOccupiedBy(selectedDesk).tilePiece;
        GetComponent<BuildModeManager>().originalTile = origTile;
        GetComponent<BuildModeManager>().buildWithObject(origTile, selectedDesk);
    }

    IEnumerator ProfitLoop()
    {
        int profit = Mathf.CeilToInt((popularity/20f) * (ratings/35f));
        cash += profit;
        DailyPofit += profit;

        inflowTxt.text = "$" + String.Format("{0:n0}", profit*12) + "/hr";
        yield return new WaitForSeconds(GetComponent<DayController>().timeConstant*(5f/60f));
        StartCoroutine(ProfitLoop());
        
    }

    IEnumerator PopularityLoop()
    {
        float playerIncrease = (ratings/100f) * staffManager.ActiveStaff.Count * Random.Range(3, 4);
        yield return new WaitForSeconds(GetComponent<DayController>().timeConstant*(5f/60f));

        popularity += Mathf.RoundToInt(playerIncrease);
        StartCoroutine(PopularityLoop());
    }

    IEnumerator MoodLoop()
    {
        foreach (Staff staff in staffManager.ActiveStaff)
        {
            staff.UpdateHour();
            print(staff.Mood);
        }
        yield return new WaitForSeconds(GetComponent<DayController>().timeConstant);
        StartCoroutine(MoodLoop());
    }

    void AddOwner()
    {
        //add owner desk

        int ownerX = 1;
        int ownerY = 1;

        GameObject ownerDesk = (GameObject)Instantiate(Resources.Load("Buyables/Desk"), new Vector3(ownerX, .5f, ownerY), Quaternion.identity, 
            GameObject.Find("Furniture").transform);
        ownerDesk.layer = 6;
        GetComponent<BuildModeManager>().setFurnLayer(ownerDesk, 6);
        ownerDesk.GetComponent<Desk>().staff = staffManager.CreateOwner("gab");
        AddCharacterToDesk(ownerDesk);
        desks.Add(ownerDesk.GetComponent<Desk>());
        GetComponent<GridBuildingManager>().UpdateTiles();
        GetComponent<GridBuildingManager>().GetTile(ownerX, ownerY).occupiedBy = ownerDesk;
    }

    public void AddCharacterToDesk(GameObject desk)
    {
        GameObject npc = Instantiate(Resources.Load("Staff"), desk.transform.Find("WorkerPos")) as GameObject;
        npc.transform.GetChild(0).GetChild(Random.Range(0, npc.transform.GetChild(0).childCount - 1)).gameObject.SetActive(true);
    }

    public void RemoveCharacterFromDesk(GameObject desk)
    {
        Destroy(desk.transform.Find("WorkerPos").GetChild(0));
    }

    public void EndDayStats()
    {

        if (cash < 0)
        {
            SceneManager.LoadScene("GameOver");
        }
        
        UpdateRatings();
        endOfDayStatsPanel.SetActive(true);
        endOfDayStatsPanel.transform.Find("Panel").Find("CompletedTasksTxt").GetComponent<TextMeshProUGUI>().text 
            = GetComponent<TaskManager>().DailyFinished.Count + " tasks completed!";

        endOfDayStatsPanel.transform.Find("Panel").Find("RevenueTxt").GetComponent<TextMeshProUGUI>().text
            = "$" + String.Format("{0:n0}", DailyPofit) + " earned!";
        DailyPofit = 0;

        endOfDayStatsPanel.transform.Find("Ratings").GetComponent<Image>().fillAmount = Mathf.Clamp(ratings / 100f, 0f, 1f);
    }
    
    public void UpdateRatings()
    {
        float tasksCompleted = GetComponent<TaskManager>().DailyFinished.Count;
        float targetTasks = GetComponent<TaskManager>().TargetDailyTasks;

        ratings = Mathf.Clamp(Mathf.RoundToInt(((tasksCompleted / targetTasks)*.85f)*100f), 25, 100);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        staffManager = GetComponent<StaffManager>();
        StartCoroutine(ProfitLoop());
        StartCoroutine(PopularityLoop());
        StartCoroutine(MoodLoop());
        desks = new List<Desk>();
        AddOwner();
        GetComponent<DayController>().enabled = true;
    }

    int GetTotalExpenses()
    {
        int totalExpenses = 0;
        foreach (Staff staff in staffManager.ActiveStaff)
        {
            totalExpenses += staff.CurrentWage;
        }

        return totalExpenses;
    }

    // Update is called once per frame
    void Update()
    {
        staffTxt.text = GetComponent<StaffManager>().ActiveStaff.Count.ToString();
        cashTxt.text = "$" + String.Format("{0:n0}", cash);
        popularityTxt.text = String.Format("{0:n0}", popularity);

        expensesTxt.text = "$" + String.Format("{0:n0}", GetTotalExpenses()) + "/hr";
        
        if (selectedStaffPanel.activeSelf)
        {
            Desk desk = selectedDesk.GetComponent<Desk>();
            Task currentTask = staffManager.GetTaskFromStaff(desk.staff);
            
            giveTaskBtn.SetActive((currentTask==null) && (desk.staff!=null) && (desk.staff.Type != Staff.SkillType.Manager));

            if (desk.staff != null)
            {
                float moodPerc = desk.staff.Mood / 100f;
                panelMoodBar.transform.Find("Inner").GetComponent<RectTransform>().anchorMax = new Vector2(moodPerc, 1f);
                panelMoodBar.transform.Find("Inner").GetComponent<Image>().color =
                    Color.Lerp( new Color(255f / 255f, 88f/255f, 117f / 255f),
                        new Color(144f / 255f, 1f, 88f / 255f), moodPerc);
                
                panelMoodBar.transform.Find("TaskName").GetComponent<TextMeshProUGUI>().color =
                    Color.Lerp( new Color(106f / 255f, 32f/255f, 31f / 255f),
                        new Color(54f / 255f, 106f/255f, 31 / 255f), moodPerc);

                panelMoodBar.transform.Find("TaskName").GetComponent<TextMeshProUGUI>().text = "Mood: " + Mathf.RoundToInt(moodPerc*100f) + "%";
            }
            
            if (currentTask != null)
            {
                float comp = currentTask.Completion;
                panelTaskBar.transform.Find("Inner").GetComponent<RectTransform>().anchorMax = new Vector2(Mathf.Max(.1f, comp/100f), 1f);
                panelTaskBar.transform.Find("TaskName").GetComponent<TextMeshProUGUI>().text = currentTask.Name;
            }
            else
            {
                panelTaskBar.transform.Find("Inner").GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                panelTaskBar.transform.Find("TaskName").GetComponent<TextMeshProUGUI>().text = "No tasks given";
            }
        }
    }
}
