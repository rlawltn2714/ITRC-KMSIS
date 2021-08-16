using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunlightSensing : MonoBehaviour
{
    private GameObject sunlight;
    RaycastHit hit;
    float MaxDistance = 15f;

    void Start()
    {
        sunlight = GameObject.Find("Directional Light");
        Debug.Log("light direction : " + -sunlight.transform.forward);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = new Vector3(transform.position.x - 0.05f, transform.position.y, transform.position.z);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = new Vector3(transform.position.x + 0.05f, transform.position.y, transform.position.z);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.05f);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.05f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.DrawRay(transform.position, -sunlight.transform.forward * MaxDistance, Color.blue, 1f);
            if (Physics.Raycast(transform.position, -sunlight.transform.forward, out hit, MaxDistance))
            {
                Debug.Log("light X, position : " + hit.point);
            }
            else
            {
                Debug.Log("light O");
            }
        }
    }
}
