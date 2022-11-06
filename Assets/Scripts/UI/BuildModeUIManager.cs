using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildModeUIManager : MonoBehaviour
{

    public List<GameObject> buildables = new List<GameObject>();
    public GameObject itemBuyableFrame;
    private int price;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateUpgradePrice();
        foreach (GameObject buildable in buildables)
        {
            GameObject buyableFrame = Instantiate(itemBuyableFrame, transform.Find("Panel"));
            Instantiate(buildable, buyableFrame.transform.Find("GameObject").transform).transform.localPosition = Vector3.zero;
            buyableFrame.transform.Find("ItemTitle").GetComponent<TextMeshProUGUI>().text = buildable.name;
            buyableFrame.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().text = "$" + buildable.GetComponent<Buyable>().price;
            
            buyableFrame.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject.Find("GameManager").GetComponent<BuildModeManager>().buildWithObject(buildable);
            });
        }
    }

    public void ToggleVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void UpdateUpgradePrice()
    {
        price = Mathf.RoundToInt(Mathf.Pow(GameObject.Find("GameManager").GetComponent<GridBuildingManager>().buildingSize, 2f) * 10f);
        transform.Find("UpgradeButton").Find("UpgradeText").GetComponent<TextMeshProUGUI>().text =
            "Upgrade building - $" + String.Format("{0:n0}", price);
    }

    public void UpgradeBuilding()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().cash >= price)
        {
            GameObject.Find("GameManager").GetComponent<GridBuildingManager>().buildingSize += 2;
            UpdateUpgradePrice();
        }
        
    }
}
