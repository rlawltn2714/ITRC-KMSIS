using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayManager : MonoBehaviour
{
    // This class manages raycast.

    // GameObject component
    private GameObject buildings;
    private GameObject importedBuildings;
    private GameObject sunlight;

    // RaycastHit variable
    private RaycastHit hit;

    // GetPointOnObject setting
    private float length = 0.35f;
    private int divisor = 150;

    void Start()
    {
        // Get GameObject component
        buildings = GameObject.Find("Buildings");
        importedBuildings = GameObject.Find("ImportedBuildings");
        sunlight = GameObject.Find("Directional Light");
    }

    // Get points on the surface of object
    public List<RaycastHit> GetPointOnObject(GameObject building)
    {
        List<RaycastHit> result = new List<RaycastHit>();

        // Layer setting
        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            buildings.transform.GetChild(i).gameObject.layer = 2;
        }
        for (int i = 0; i < importedBuildings.transform.childCount; i++)
        {
            importedBuildings.transform.GetChild(i).gameObject.layer = 2;
        }
        building.layer = 0;
        GameObject.Find("Dem").layer = 2;
        GameObject.Find("DemRigidbody").layer = 2;
        GameObject.Find("Colliders").layer = 2;

        // Local variable
        float x = building.GetComponent<MeshRenderer>().bounds.center.x;
        float y = building.GetComponent<MeshRenderer>().bounds.center.y;
        float z = building.GetComponent<MeshRenderer>().bounds.center.z;

        // Get points using raycast
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

        // Layer restore
        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            buildings.transform.GetChild(i).gameObject.layer = 0;
        }
        for (int i = 0; i < importedBuildings.transform.childCount; i++)
        {
            importedBuildings.transform.GetChild(i).gameObject.layer = 0;
        }
        GameObject.Find("Dem").layer = 0;
        GameObject.Find("DemRigidbody").layer = 0;
        GameObject.Find("Colliders").layer = 0;

        return result;
    }

    // Calculate the percentage of points in sunlight
    public float Ratio(List<GameObject> pointList, Vector3 sunVector)
    {
        int sum = 0;
        for (int i = 0; i < pointList.Count; i++)
        {
            Vector3 temp = pointList[i].transform.position;
            if (!Physics.Raycast(temp, -sunVector, out hit, Mathf.Infinity))
            {
                sum++;
            }
        }
        return (float)(sum) * 100f / (float)(pointList.Count);
    }

    public bool CheckSunlight(GameObject point, Vector3 sunVector)
    {
        Vector3 temp = point.transform.position;
        if (!Physics.Raycast(temp, -sunVector, out hit, Mathf.Infinity)) return true;
        else return false;
    }
}
