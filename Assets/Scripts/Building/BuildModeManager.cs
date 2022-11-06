using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class BuildModeManager : MonoBehaviour
{
    private GameManager gameManager;

    private GameObject selectedObj;
    private Vector3 lastPos;
    private Vector3 originalPos;
    private bool canPlace;
    private Transform furnitureHolder;

    public void buildWithobject(GameObject obj)
    {
        furnitureHolder = GameObject.Find("Furniture").transform;
        if (obj.transform.IsChildOf(furnitureHolder))
        {
            GameObject foundFurn = GetTopParent(obj);
            originalPos = foundFurn.transform.position;
            selectedObj = foundFurn;
            setFurnLayer(selectedObj, 3);
        }
        else
        {
            selectedObj = Instantiate(obj);
        }
    }
    
    public GameObject GetTopParent(GameObject obj)
    {

        GameObject top = obj;
        if (obj.transform.parent != furnitureHolder)
        {
            top=GetTopParent(obj.transform.parent.gameObject);
        }
        
        return top;
    }

    public void setFurnLayer(GameObject furn, int layerInt)
    {
        foreach (Transform t in furn.transform)
        {
            t.gameObject.layer = layerInt;
        }
    }

    public void cancelBuild()
    {
        if (selectedObj)
        {
            if (selectedObj.transform.IsChildOf(furnitureHolder))
            {
                selectedObj.transform.position = originalPos;
                setFurnLayer(selectedObj, 0);
                selectedObj = null;
            }
            else
            {
                Destroy(selectedObj);
                selectedObj = null;
            }
        }
    }

    public void PlaceDown()
    {
        if (selectedObj.transform.IsChildOf(furnitureHolder))
        {
            selectedObj.transform.position = lastPos;
            setFurnLayer(selectedObj, 0);
            selectedObj = null;
        }
        else
        {
            GameObject newFurn = Instantiate(selectedObj, lastPos, Quaternion.identity, furnitureHolder);
            setFurnLayer(newFurn, 0);

            Desk desk = newFurn.GetComponent<Desk>();
            if (desk)
            {
                GetComponent<GameManager>().desks.Add(desk);
            }
        }
    }

    public void OnEnable()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //place down object
            if (selectedObj != null)
            {
                if (canPlace)
                {
                    PlaceDown();
                }
            }
            else
            {
                //select object to move
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, 3))
                {
                    if (hit.transform.IsChildOf(furnitureHolder))
                    {
                        GameObject furn = GetTopParent(hit.transform.gameObject);
                        if (furn.GetComponent<Desk>())
                        {
                            GetComponent<GameManager>().SelectDesk(furn.GetComponent<Desk>());
                        }
                        else
                        {
                            buildWithobject(hit.transform.gameObject);
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            cancelBuild();
        }
        
        if (selectedObj)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, 3))
            {
                int bounds = GetComponent<GridBuildingManager>().buildingSize;
                lastPos = new Vector3(Mathf.Clamp(Mathf.Round(hit.point.x), 0, bounds), 
                    selectedObj.transform.localScale.y / 2, 
                    Mathf.Clamp(Mathf.Round(hit.point.z), 0, bounds));
                
                selectedObj.SetActive(true);
                selectedObj.transform.position = lastPos;

                canPlace = !hit.transform.IsChildOf(furnitureHolder);

            }
            else
            {
                canPlace = false;
            }
        }
    }
}
