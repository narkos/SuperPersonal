using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sketch : MonoBehaviour
{

    public GameObject myPrefab;

    void Start()
    {


        // Linear distribution
        // for (int i = 0; i < totalCubes; i++)
        // {
        //     float percentage = (float)i / (float)totalCubes;
        //     float x = percentage * totalDistance;
        //     float y = 5;
        //     float z = 0;

        //     var newCube = (GameObject)Instantiate(myPrefab, new Vector3(x, y, z), Quaternion.identity);
        //     newCube.GetComponent<CubeScript>().SetSize(1.0f - percentage);
        //     newCube.GetComponent<CubeScript>().rotateSpeed = 0;
        // }

        int totalCubes = 30;
        float totalDistance = 2.9f;
        // SIN
        for (int i = 0; i < totalCubes; i++)
        {
            float percentage = (float)i / (float)totalCubes;
            float sin = Mathf.Sin(percentage * Mathf.PI / 2);

            float x = 1.8f + sin * totalDistance;
            float y = 5;
            float z = 0;

            var newCube = (GameObject)Instantiate(myPrefab, new Vector3(x, y, z), Quaternion.identity);
            newCube.GetComponent<CubeScript>().SetSize(0.35f * (1.0f - percentage));
            newCube.GetComponent<CubeScript>().rotateSpeed = 0.2f  + percentage * 4.0f;
        }

    }

    void Update()
    {

    }
}
