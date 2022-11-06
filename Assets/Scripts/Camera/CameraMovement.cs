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
    float moveBorderLeft;
    float moveBorderTop;
    float moveBorderRight;
    float moveBorderBottom;
    float marginHorizontal;
    float marginVertical;

    Vector3 dragOrigin;
    float dragSpeed = 50f;


    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        
        screenWidth = mainCam.pixelWidth;
        screenHeight = mainCam.pixelHeight;

        marginHorizontal = screenWidth * borderSize;
        marginVertical = screenHeight * borderSize;

        moveBorderLeft = 0 + marginHorizontal;
        moveBorderTop = screenHeight - marginVertical;
        moveBorderRight = screenWidth - marginHorizontal;
        moveBorderBottom = 0 + marginVertical;

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

        transform.position += new Vector3(skewedInput.x, 0, skewedInput.z);
    }

    void mouseMove()
    {
        if (Input.mousePosition.x <= moveBorderLeft  ||
            Input.mousePosition.x >= moveBorderRight ||
            Input.mousePosition.y >= moveBorderTop   ||
            Input.mousePosition.y <= moveBorderBottom)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 screenCenter = new Vector3(mainCam.pixelWidth/2, mainCam.pixelHeight/2);
            Vector3 worldCenterToScreen = mainCam.ScreenToWorldPoint(screenCenter);

            Vector3 mouseToWorldPoint = mainCam.ScreenToWorldPoint(mousePos);

            Vector3 dir = (mouseToWorldPoint - worldCenterToScreen).normalized;
            //print(screenCenter);

            transform.position = transform.position + new Vector3(dir.x, 0, dir.y) * camSpeed;
        }
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

}
