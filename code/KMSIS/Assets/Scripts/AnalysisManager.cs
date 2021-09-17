using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalysisManager : MonoBehaviour
{
    // This class manages analysis for the information of sunlight.

    // Manager component
    private RayManager rayManager;

    // GameObject component
    private GameObject pointPrefab;
    private GameObject analysisPoints;

    // Material component
    public Material selectMaterial;
    public Material normalMaterial;

    // GameObject variable
    private List<GameObject> objectList;
    private List<GameObject> selectedObjectList;

    // Variable
    private float maxDistance = 0.02f;

    void Start()
    {
        // Get manager component
        rayManager = GameObject.Find("RayManager").GetComponent<RayManager>();

        // Get gameObject component
        pointPrefab = GameObject.Find("PointPrefab");
        analysisPoints = GameObject.Find("AnalysisPoints");

        // Initialize variable
        objectList = new List<GameObject>();
        selectedObjectList = new List<GameObject>();
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
        selectedObjectList.Clear();
    }

    // Intantiate plane object on the points
    private void InstantiateObject(List<RaycastHit> hitPointList)
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
            selectedObjectList[i].GetComponent<MeshRenderer>().material = normalMaterial;
        }
        selectedObjectList.Clear();
    }

    // Return selectedObjectList
    public List<GameObject> GetSelectedObjectList()
    {
        return selectedObjectList;
    }
}
