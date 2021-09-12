using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    // This class manages buildings.

    // GameObject component
    private GameObject buildings;
    private GameObject importedBuildings;
    private List<GameObject> selectedObjectList;
    private List<GameObject> deletedObjectList;

    // Material component
    public Material selectMaterial;
    public Material normalMaterial;

    // View mode
    private int viewMode; // 0 : normal view, 1 : deleted object view, 2 : selected object view

    void Start()
    {
        // Get GameObject component
        buildings = GameObject.Find("Buildings");
        importedBuildings = GameObject.Find("ImportedBuildings");

        // Initialize variable
        selectedObjectList = new List<GameObject>();
        deletedObjectList = new List<GameObject>();
        viewMode = 0;
    }

    // Show all object
    public void ShowAll()
    {
        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            buildings.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < importedBuildings.transform.childCount; i++)
        {
            importedBuildings.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < deletedObjectList.Count; i++)
        {
            deletedObjectList[i].SetActive(false);
        }
        viewMode = 0;
    }

    // Change view mode
    public void ChangeView()
    {
        if (viewMode == 0)
        {
            for (int i = 0; i < buildings.transform.childCount; i++)
            {
                buildings.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < importedBuildings.transform.childCount; i++)
            {
                importedBuildings.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < deletedObjectList.Count; i++)
            {
                deletedObjectList[i].SetActive(true);
            }
            viewMode = 1;
        }
        else if (viewMode == 1)
        {
            for (int i = 0; i < buildings.transform.childCount; i++)
            {
                buildings.transform.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 0; i < importedBuildings.transform.childCount; i++)
            {
                importedBuildings.transform.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 0; i < deletedObjectList.Count; i++)
            {
                deletedObjectList[i].SetActive(false);
            }
            viewMode = 0;
        }
        else
        {
            Debug.Log("Error - unexpected value, viewMode = " + viewMode);
            viewMode = 0;
        }
    }

    // Select object
    public void SelectObject(GameObject building)
    {
        if (!selectedObjectList.Contains(building))
        {
            selectedObjectList.Add(building);
            building.GetComponent<MeshRenderer>().material = selectMaterial;
        }
    }

    // Delete from selectedObjectList
    public void DeleteFromSelectedObjectList(int index)
    {
        selectedObjectList[index].GetComponent<MeshRenderer>().material = normalMaterial;
        selectedObjectList.RemoveAt(index);
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

    // Return deletedObjectList
    public List<GameObject> GetDeletedObjectList()
    {
        return deletedObjectList;
    }
}
