using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{

    List<GameObject> cars = new();
    
    IEnumerator spawnCar()
    {
        GameObject car = Instantiate(Resources.Load("Car") as GameObject, transform.position, transform.rotation);
        cars.Add(car);
        yield return new WaitForSeconds(5f);
        cars.Remove(car);
        Destroy(car);
    }

    IEnumerator carSpawner()
    {
        StartCoroutine(spawnCar());
        yield return new WaitForSeconds(5f);
        StartCoroutine(carSpawner());
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(carSpawner());
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].transform.Translate(-transform.right*Time.deltaTime*15f);
        }
    }
}
