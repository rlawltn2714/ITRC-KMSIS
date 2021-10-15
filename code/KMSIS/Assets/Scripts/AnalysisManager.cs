using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnalysisManager : MonoBehaviour
{
    // This class manages analysis for the information of sunlight.

    // Manager component
    private RayManager rayManager;
    private SunManager sunManager;
    private UIManager uiManager;

    // GameObject component
    private GameObject pointPrefab;
    private GameObject analysisPoints;

    // Material component
    public Material selectMaterial;
    public Material normalMaterial;
    public Material pointMaterial;

    // GameObject variable
    private List<GameObject> pointList;
    private List<GameObject> selectedPointList;
    private GameObject targetBuilding;

    // Variable
    private float radius = 0.01f;

    void Start()
    {
        // Get manager component
        rayManager = GameObject.Find("RayManager").GetComponent<RayManager>();
        sunManager = GameObject.Find("SunManager").GetComponent<SunManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        // Get gameObject component
        pointPrefab = GameObject.Find("PointPrefab");
        analysisPoints = GameObject.Find("AnalysisPoints");

        // Initialize variable
        pointList = new List<GameObject>();
        selectedPointList = new List<GameObject>();
    }

    // Initialize AnalysisManager
    public void Init(GameObject building)
    {
        targetBuilding = building;
        building.GetComponent<MeshRenderer>().material = pointMaterial;
        List<RaycastHit> hitPointList = rayManager.GetPointOnObject(building);
        InstantiateObject(hitPointList);
    }

    // Release AnalysisManager
    public void Release()
    {
        targetBuilding.GetComponent<MeshRenderer>().material = normalMaterial;
        DestroyObject();
        selectedPointList.Clear();
    }

    // Check environment and estimate execution time
    public float EstimateTime()
    {
        int month = 8, day = 8;
        float startTime = Time.realtimeSinceStartup;
        for (float clock = 0f; clock < 24f; clock += 24f / 1440f)
        {
            List<double> tempList = sunManager.Calculate(month, day, clock);
            if (rayManager.CheckSunlight(selectedPointList[0], sunManager.CalculateSunVector(-tempList[0], tempList[1]))) continue;
        }
        return (Time.realtimeSinceStartup - startTime);
    }

    // Optimize points
    private List<GameObject> OptimizePoints(List<GameObject> pointList)
    {
        if (pointList.Count < 200) return pointList;

        int[] tempList = new int[pointList.Count];
        for (int i = 0; i < tempList.Length; i++)
        {
            tempList[i] = 0;
        }
        System.Random rand = new System.Random();
        int count = 0;
        while (count < 200)
        {
            int index = rand.Next(0, pointList.Count);
            if (tempList[index] != 0) continue;
            else
            {
                tempList[index] = 1;
                count++;
            }
        }
        List<GameObject> optimizedPointList = new List<GameObject>();
        for (int i = 0; i < tempList.Length; i++)
        {
            if (tempList[i] == 1)
            {
                optimizedPointList.Add(pointList[i]);
            }
        }
        return optimizedPointList;
    }

    // Analyze the sunlight
    public void Analyze()
    {
        if (selectedPointList.Count < 1)
        {
            Debug.Log("You should select the points.");
        }
        else if (selectedPointList.Count > 2000)
        {
            Debug.Log("You've choose too many points.");
        }
        else
        {
            for (int i = 0; i < pointList.Count; i++)
            {
                pointList[i].layer = 2;
            }

            List<GameObject> optimizedPointList = OptimizePoints(selectedPointList);
            List<float> dayInfo = uiManager.GetInterfaceValue();
            if (dayInfo == null) return;
            int month = (int)(dayInfo[0]);
            int day = (int)(dayInfo[1]);

            for (float clock = 0f; clock < 24f; clock += 24f / 1440f)
            {
                List<double> tempList = sunManager.Calculate(month, day, clock);
                for (int i = 0; i < optimizedPointList.Count; i++)
                {
                    if (rayManager.CheckSunlight(optimizedPointList[i], sunManager.CalculateSunVector(-tempList[0], tempList[1]))) continue;
                }
            }

            for (int i = 0; i < pointList.Count; i++)
            {
                pointList[i].layer = 0;
            }
        }
    }

    // Intantiate plane object on the points
    public void InstantiateObject(List<RaycastHit> hitPointList)
    {
        for (int i = 0; i < hitPointList.Count; i++)
        {
            GameObject temp = Instantiate(pointPrefab, hitPointList[i].point + 0.000005f * hitPointList[i].normal, Quaternion.identity);
            temp.transform.up = hitPointList[i].normal;
            temp.transform.parent = analysisPoints.transform;
            pointList.Add(temp);
        }
    }

    // Destroy plane objects in the objectList
    private void DestroyObject()
    {
        for (int i = 0; i < pointList.Count; i++)
        {
            Destroy(pointList[i], 0.5f);
        }
        pointList.Clear();
    }

    // Select points with circle
    public void SelectPoint(GameObject gameObject)
    {
        if (gameObject.transform.parent.name == "AnalysisPoints")
        {
            for (int i = 0; i < pointList.Count; i++)
            {
                if (pointList[i].transform.up == gameObject.transform.up && Vector3.Distance(pointList[i].transform.position, gameObject.transform.position) < radius)
                {
                    if (!selectedPointList.Contains(pointList[i]))
                    {
                        selectedPointList.Add(pointList[i]);
                        pointList[i].GetComponent<MeshRenderer>().material = selectMaterial;
                    }
                }
            }
        }
    }

    // Select object
    public void SelectObject(GameObject gameObject)
    {
        if (!selectedPointList.Contains(gameObject))
        {
            selectedPointList.Add(gameObject);
            gameObject.GetComponent<MeshRenderer>().material = selectMaterial;
        }
    }

    // Clear selectedObjectList
    public void ClearSelectedObjectList()
    {
        for (int i = 0; i < selectedPointList.Count; i++)
        {
            selectedPointList[i].GetComponent<MeshRenderer>().material = pointMaterial;
        }
        selectedPointList.Clear();
    }

    // Return selectedObjectList
    public List<GameObject> GetSelectedObjectList()
    {
        return selectedPointList;
    }
}
