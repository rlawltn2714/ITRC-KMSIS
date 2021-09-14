using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalysisManager : MonoBehaviour
{
    // This class manages analysis for the information of sunlight.

    // Manager component
    private RayManager rayManager;

    // GameObject component
    private GameObject plane;

    // GameObject variable
    private List<GameObject> objectList;

    void Start()
    {
        // Get manager component
        rayManager = GameObject.Find("RayManager").GetComponent<RayManager>();

        // Get gameObject component
        plane = GameObject.Find("Plane");

        // Initialize variable
        objectList = new List<GameObject>();
    }

    public void Init(GameObject building)
    {
        List<RaycastHit> hitPointList = rayManager.GetPointOnObject(building);
        InstantiateObject(hitPointList);
        //Debug.Log(rayManager.Ratio(hitPointList, sunManager.CalculateSunVector(GameObject.Find("Directional Light").transform.eulerAngles.y, GameObject.Find("Directional Light").transform.eulerAngles.x)) + "%");
    }

    public void Release()
    {
        DestroyObject();
    }

    // Intantiate plane object on the points
    private void InstantiateObject(List<RaycastHit> hitPointList)
    {
        for (int i = 0; i < hitPointList.Count; i++)
        {
            GameObject temp = Instantiate(plane, hitPointList[i].point + 0.000005f * hitPointList[i].normal, Quaternion.identity);
            temp.transform.up = hitPointList[i].normal;
            objectList.Add(temp);
        }
    }

    // Destroy plane objects in the objectList
    private void DestroyObject()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            Destroy(objectList[i], 0.5f);
        }
        objectList.Clear();
    }
}
