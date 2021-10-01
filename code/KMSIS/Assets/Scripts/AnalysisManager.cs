using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private List<GameObject> objectList;
    private List<GameObject> selectedObjectList;
    private GameObject targetBuilding;

    // Variable
    private float maxDistance = 0.02f;

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
        objectList = new List<GameObject>();
        selectedObjectList = new List<GameObject>();
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
        selectedObjectList.Clear();
    }

    // Analyze the sunlight
    public void Analyze()
    {
        float startTime = Time.realtimeSinceStartup;
        if (selectedObjectList.Count < 1)
        {
            Debug.Log("You should select the points.");
        }
        else
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                objectList[i].layer = 2;
            }

            // Analyze
            List<float> dayInfo = uiManager.GetInterfaceValue();
            if (dayInfo == null) return;
            int month = (int)(dayInfo[0]);
            int day = (int)(dayInfo[1]);

            float temp = Time.realtimeSinceStartup;
            for (float clock = 0f; clock < 24f; clock += 24f / 1440f)
            {
                List<double> tempList = sunManager.Calculate(month, day, clock);
                for (int i = 0; i < selectedObjectList.Count; i++)
                {
                    if (rayManager.CheckSunlight(selectedObjectList[i], sunManager.CalculateSunVector(-tempList[0], tempList[1]))) continue;
                }
            }
            Debug.Log("Average time : " + (Time.realtimeSinceStartup - temp) / (selectedObjectList.Count));
            for (int i = 0; i < objectList.Count; i++)
            {
                objectList[i].layer = 0;
            }
        }
        Debug.Log("Execution time : " + (Time.realtimeSinceStartup - startTime));
    }

    // Intantiate plane object on the points
    public void InstantiateObject(List<RaycastHit> hitPointList)
    {
        for (int i = 0; i < hitPointList.Count; i++)
        {
            GameObject temp = Instantiate(pointPrefab, hitPointList[i].point + 0.000005f * hitPointList[i].normal, Quaternion.identity);
            temp.transform.up = hitPointList[i].normal;
            temp.transform.parent = analysisPoints.transform;
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

    // Select points with circle
    public void SelectPoint(GameObject gameObject)
    {
        if (gameObject.transform.parent.name == "AnalysisPoints")
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                if (objectList[i].transform.up == gameObject.transform.up && Vector3.Distance(objectList[i].transform.position, gameObject.transform.position) < maxDistance)
                {
                    if (!selectedObjectList.Contains(objectList[i]))
                    {
                        selectedObjectList.Add(objectList[i]);
                        objectList[i].GetComponent<MeshRenderer>().material = selectMaterial;
                    }
                }
            }
        }
    }

    // Select object
    public void SelectObject(GameObject gameObject)
    {
        if (!selectedObjectList.Contains(gameObject))
        {
            selectedObjectList.Add(gameObject);
            gameObject.GetComponent<MeshRenderer>().material = selectMaterial;
        }
    }

    // Clear selectedObjectList
    public void ClearSelectedObjectList()
    {
        for (int i = 0; i < selectedObjectList.Count; i++)
        {
            selectedObjectList[i].GetComponent<MeshRenderer>().material = pointMaterial;
        }
        selectedObjectList.Clear();
    }

    // Return selectedObjectList
    public List<GameObject> GetSelectedObjectList()
    {
        return selectedObjectList;
    }
}
