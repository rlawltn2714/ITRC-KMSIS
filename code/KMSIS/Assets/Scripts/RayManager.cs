using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayManager : MonoBehaviour
{
    private GameObject plane;
    private GameObject buildings;
    private GameObject sunlight;
    private RaycastHit hit;
    private float length = 0.25f;
    private int divisor = 120;

    void Start()
    {
        plane = GameObject.Find("Plane");
        buildings = GameObject.Find("Buildings");
        sunlight = GameObject.Find("Directional Light");
    }

    public List<RaycastHit> GetPointOnObject(GameObject building)
    {
        List<RaycastHit> result = new List<RaycastHit>();

        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            buildings.transform.GetChild(i).gameObject.layer = 2;
        }
        building.layer = 0;
        GameObject.Find("Dem").layer = 2;
        GameObject.Find("DemRigidbody").layer = 2;
        GameObject.Find("Colliders").layer = 2;

        float x = building.transform.position.x;
        float y = building.transform.position.y;
        float z = building.transform.position.z;

        for (int i = 1; i < divisor; i++)
        {
            for (int j = 1; j < divisor; j++)
            {
                Vector3 temp = new Vector3(x + length, y + length * (divisor - i) / divisor - length * i / divisor, z + length * (divisor - j) / divisor - length * j / divisor);
                if (Physics.Raycast(temp, new Vector3(-1, 0, 0), out hit, Mathf.Infinity))
                {
                    result.Add(hit);
                }
            }
        }

        for (int i = 1; i < divisor; i++)
        {
            for (int j = 1; j < divisor; j++)
            {
                Vector3 temp = new Vector3(x - length, y + length * (divisor - i) / divisor - length * i / divisor, z + length * (divisor - j) / divisor - length * j / divisor);
                if (Physics.Raycast(temp, new Vector3(1, 0, 0), out hit, Mathf.Infinity))
                {
                    result.Add(hit);
                }
            }
        }

        for (int i = 1; i < divisor; i++)
        {
            for (int j = 1; j < divisor; j++)
            {
                Vector3 temp = new Vector3(x + length * (divisor - i) / divisor - length * i / divisor, y + length, z + length * (divisor - j) / divisor - length * j / divisor);
                if (Physics.Raycast(temp, new Vector3(0, -1, 0), out hit, Mathf.Infinity))
                {
                    result.Add(hit);
                }
            }
        }

        for (int i = 1; i < divisor; i++)
        {
            for (int j = 1; j < divisor; j++)
            {
                Vector3 temp = new Vector3(x + length * (divisor - i) / divisor - length * i / divisor, y - length, z + length * (divisor - j) / divisor - length * j / divisor);
                if (Physics.Raycast(temp, new Vector3(0, 1, 0), out hit, Mathf.Infinity))
                {
                    result.Add(hit);
                }
            }
        }

        for (int i = 1; i < divisor; i++)
        {
            for (int j = 1; j < divisor; j++)
            {
                Vector3 temp = new Vector3(x + length * (divisor - i) / divisor - length * i / divisor, y + length * (divisor - j) / divisor - length * j / divisor, z + length);
                if (Physics.Raycast(temp, new Vector3(0, 0, -1), out hit, Mathf.Infinity))
                {
                    result.Add(hit);
                }
            }
        }

        for (int i = 1; i < divisor; i++)
        {
            for (int j = 1; j < divisor; j++)
            {
                Vector3 temp = new Vector3(x + length * (divisor - i) / divisor - length * i / divisor, y + length * (divisor - j) / divisor - length * j / divisor, z - length);
                if (Physics.Raycast(temp, new Vector3(0, 0, 1), out hit, Mathf.Infinity))
                {
                    result.Add(hit);
                }
            }
        }

        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            buildings.transform.GetChild(i).gameObject.layer = 0;
        }
        GameObject.Find("Dem").layer = 0;
        GameObject.Find("DemRigidbody").layer = 0;
        GameObject.Find("Colliders").layer = 0;
        return result;
    }

    public float Ratio(List<RaycastHit> hitPointList, Vector3 sunVector)
    {
        int sum = 0;
        for (int i = 0; i < hitPointList.Count; i++)
        {
            Vector3 temp = hitPointList[i].point;
            if (!Physics.Raycast(temp, -sunVector, out hit, Mathf.Infinity))
            {
                sum++;
            }
        }
        return (float)(sum) * 100f / (float)(hitPointList.Count);
    }

    public void InstantiateObject(List<RaycastHit> hitPointList)
    {
        for (int i = 0; i < hitPointList.Count; i++)
        {
            GameObject t = Instantiate(plane, hitPointList[i].point, Quaternion.identity);
            t.transform.up = hitPointList[i].normal;
            Destroy(t, 3.0f);
        }
    }
}
