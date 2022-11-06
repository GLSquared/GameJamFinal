using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Camera mainCam;
    Vector3 input;
    [SerializeField] float camSpeed = 0.01f;
    [SerializeField] float borderSize = 0.05f;
    int screenWidth;
    int screenHeight;

    Vector3 dragOrigin;
    float dragSpeed = 50f;

    float minX, maxX, minZ, maxZ, distance;


    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        
        screenWidth = mainCam.pixelWidth;
        screenHeight = mainCam.pixelHeight;

        StartCoroutine(GetClampDistance());

    }

    void Update()
    {
        input = new Vector3(Input.GetAxis("Horizontal"), 10 , Input.GetAxis("Vertical"));
        move();
        DragMouse();
        zoom();
    }

    void move()
    {
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

        var skewedInput = matrix.MultiplyPoint3x4(input * camSpeed);

        print(skewedInput);
        transform.position += new Vector3(skewedInput.x, 0, skewedInput.z);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), transform.position.y, Mathf.Clamp(transform.position.z, minZ, maxZ));
    }

    void zoom()
    {
        
        if (Input.mouseScrollDelta.y > 0)
        {
            mainCam.orthographicSize -= .1f;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            mainCam.orthographicSize += .1f;
        }
    }

    void DragMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 newPosition = new Vector3();
        newPosition.x = Input.GetAxis("Mouse X");
        newPosition.z = Input.GetAxis("Mouse Y");

        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        var skewedInput = matrix.MultiplyPoint3x4(-newPosition);
        // translates to the opposite direction of mouse position.
        transform.position += skewedInput * dragSpeed * Time.deltaTime;
    }

    IEnumerator GetClampDistance()
    {
        yield return new WaitForSeconds(1);
        GameObject tilearea = GameObject.Find("TileArea");

        GameObject firstTile = tilearea.transform.GetChild(0).gameObject;
        GameObject lastTile = tilearea.transform.GetChild(tilearea.transform.childCount-1).gameObject;


        distance = (lastTile.transform.position - firstTile.transform.position).magnitude;
        print(distance);

        minX = -distance*2;
        maxX = distance/2;
        minZ = -distance*2;
        maxZ = distance/2;
    }

}
