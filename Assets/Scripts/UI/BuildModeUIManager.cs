using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildModeUIManager : MonoBehaviour
{

    public List<GameObject> buildables = new List<GameObject>();
    public List<GameObject> buyableFrames = new List<GameObject>();
    public GameObject itemBuyableFrame;
    private int price;
    private Mesh maskMesh;
    
    // Start is called before the first frame update
    void Start()
    {
        maskMesh = new Mesh();
        UpdateUpgradePrice();
        foreach (GameObject buildable in buildables)
        {
            /*
             * Get Left and right coordinate of mask image
             * Render gameobjects only within that region
             * 
             */
            GameObject buyableFrame = Instantiate(itemBuyableFrame, transform.Find("Mask").GetChild(0));
            GameObject previewObj = Instantiate(buildable, buyableFrame.transform.Find("GameObject").transform);
            previewObj.transform.localPosition = Vector3.zero;
            buyableFrame.transform.Find("ItemTitle").GetComponent<TextMeshProUGUI>().text = buildable.name;
            buyableFrame.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().text = "$" + buildable.GetComponent<Buyable>().price;
            


            buyableFrame.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject.Find("GameManager").GetComponent<BuildModeManager>().buildWithObject(buildable);
            });

            buyableFrames.Add(buyableFrame);
        }
    }

    public void ToggleVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void UpdateUpgradePrice()
    {
        price = Mathf.RoundToInt(Mathf.Pow(GameObject.Find("GameManager").GetComponent<GridBuildingManager>().buildingSize, 3f) * 4f);
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

    private void Update()
    {
        Image mask = transform.Find("Mask").GetComponent<Image>();
        print(mask.transform.position + " " + mask.rectTransform.rect);

        float leftCo = mask.transform.position.x - (mask.transform.position.x / 22);
        float rightCo = mask.transform.position.x + (mask.transform.position.x / 22);

        print(buyableFrames[0].transform.position);

        foreach (GameObject frame in buyableFrames)
        {
            if (frame.transform.position.x < leftCo ||
                frame.transform.position.x > rightCo)
            {
                frame.transform.Find("GameObject").gameObject.SetActive(false);
            }
            else
            {
                frame.transform.Find("GameObject").gameObject.SetActive(true);
            }
                
        }
    }
}
