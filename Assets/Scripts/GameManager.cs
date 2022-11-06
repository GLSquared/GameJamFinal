using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    public GameObject giveTaskBtn;

    public StaffManager staffManager;
    public GameObject taskSelectionPanel;
    public GameObject buildModeUIPanel;

    public GameObject endOfDayStatsPanel;
    
    public void SelectDesk(Desk desk)
    {
        selectedStaffPanel.SetActive(true);
        buildModeUIPanel.SetActive(false);
        selectedDesk = desk.gameObject;
        if (desk.staff != null)
        {
            //if has staff
            selectedStaffPanel.transform.Find("StaffTitle").GetComponent<TextMeshProUGUI>().text = desk.staff.Name;
            panelHireBtn.SetActive(false);
            panelTaskBar.SetActive(true);
            panelTaskTitle.SetActive(true);
        }
        else
        {
            //if no staff
            selectedStaffPanel.transform.Find("StaffTitle").GetComponent<TextMeshProUGUI>().text = "Vacant";
            panelHireBtn.SetActive(true);
            panelTaskBar.SetActive(false);
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
        int profit = Mathf.CeilToInt(((popularity/10f) * (ratings/50f)));
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
        GetComponent<GridBuildingManager>().UpdateTiles();
        GetComponent<GridBuildingManager>().GetTile(ownerX, ownerY).occupiedBy = ownerDesk;
    }

    public void AddCharacterToDesk(GameObject desk)
    {
        Instantiate(Resources.Load("Staff"), desk.transform.Find("WorkerPos"));
    }

    public void RemoveCharacterFromDesk(GameObject desk)
    {
        Destroy(desk.transform.Find("WorkerPos").GetChild(0));
    }

    public void EndDayStats()
    {
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
        desks = new List<Desk>();
        AddOwner();
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
            
            giveTaskBtn.SetActive((currentTask==null) && (desk.staff!=null));
            
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
