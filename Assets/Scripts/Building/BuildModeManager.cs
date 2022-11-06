using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
[RequireComponent(typeof(GridBuildingManager))]
public class BuildModeManager : MonoBehaviour
{
    private GameManager gameManager;
    private GridBuildingManager gridBuildingManager;

    private GameObject selectedObj;

    [SerializeField]
    private LayerMask furnitureLayerMask;

    private GameObject lastTile;
    public GameObject originalTile;

    private bool canPlace;

    private Transform furnitureHolder;

    public void buildWithObject(GameObject obj)
    {
        selectedObj = Instantiate(obj);
        selectedObj.layer = 2;
    }

    public void buildWithObject(GameObject tile, GameObject obj)
    {
        originalTile = tile;
        selectedObj = GetTopParent(obj);
        setFurnLayer(selectedObj, 2);
    }

    public GameObject GetTopParent(GameObject obj)
    {
        GameObject top = obj;
        if (obj.transform.parent != furnitureHolder)
        {
            top = GetTopParent(obj.transform.parent.gameObject);
        }

        return top;
    }


    public void setFurnLayer(GameObject furn, int layerInt)
    {
        furn.layer = layerInt;

        foreach (Transform t in furn.transform)
        {
            t.gameObject.layer = layerInt;
        }
    }

    public void posFurnAboveTile(GameObject tile, GameObject obj)
    {
        int bounds = GetComponent<GridBuildingManager>().buildingSize;

        obj.transform.position = new Vector3(Mathf.Clamp(Mathf.Round(tile.transform.position.x), 0, bounds),
            obj.transform.localScale.y / 2,
            Mathf.Clamp(Mathf.Round(tile.transform.position.z), 0, bounds));
    }

    public void cancelBuild()
    {
        if (selectedObj)
        {
            if (selectedObj.transform.IsChildOf(furnitureHolder))
            {
                posFurnAboveTile(originalTile, selectedObj);
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
            Tile original = originalTile.GetComponent<Tile>();
            original.occupiedBy = null;
            originalTile = null;

            posFurnAboveTile(lastTile, selectedObj);
            selectedObj.layer = 6;

            Tile last = lastTile.GetComponent<Tile>();
            last.occupiedBy = selectedObj;
            lastTile = null;

            selectedObj = null;
        }
        else
        {
            int bounds = GetComponent<GridBuildingManager>().buildingSize;

            GameObject newFurn = Instantiate(selectedObj,
                new Vector3(Mathf.Clamp(Mathf.Round(lastTile.transform.position.x), 0, bounds),
                selectedObj.transform.localScale.y / 2,
                Mathf.Clamp(Mathf.Round(lastTile.transform.position.z), 0, bounds)),
                selectedObj.transform.rotation, furnitureHolder);

            newFurn.transform.parent = furnitureHolder.transform;

            setFurnLayer(newFurn, 6);

            Desk desk = newFurn.GetComponent<Desk>();
            if (desk)
            {
                GetComponent<GameManager>().desks.Add(desk);
            }

            Tile tile = lastTile.GetComponent<Tile>();

            if (tile != null)
            {
                tile.occupiedBy = newFurn;
            }
        }
    }

    public void Rotate()
    {
        if (selectedObj)
        {
            Quaternion rotation = selectedObj.transform.rotation;
            rotation *= Quaternion.Euler(0, 90, 0);

            selectedObj.transform.rotation = rotation;
        }
    }

    public void OnEnable()
    {
        gameManager = GetComponent<GameManager>();
        gridBuildingManager = GetComponent<GridBuildingManager>();

        furnitureHolder = GameObject.Find("Furniture").transform;
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
                    
                    Tile occupied = hit.transform.GetComponent<Tile>();
                    if (occupied != null && occupied.occupiedBy != null)
                    {
                        
                        Desk desk = occupied.occupiedBy.GetComponent<Desk>();
                        if (desk)
                        {
                            GetComponent<GameManager>().SelectDesk(desk);
                        }
                        else
                        {
                            originalTile = hit.transform.gameObject;

                            buildWithObject(hit.transform.gameObject, occupied.occupiedBy);
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Rotate();
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
                lastTile = hit.transform.gameObject;

                Tile tile = lastTile.GetComponent<Tile>();
                if (tile != null && (tile.occupiedBy == null || tile.occupiedBy == selectedObj) )
                {
                    posFurnAboveTile(lastTile, selectedObj);

                    //selectedObj.SetActive(true);
                    canPlace = true;
                }
                else
                {
                    //selectedObj.SetActive(false);
                    canPlace = false;
                }
            }
            else
            {
                //selectedObj.SetActive(false);
                canPlace = false;
            }
        }
    }
}
