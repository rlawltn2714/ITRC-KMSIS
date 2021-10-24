using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    // This class manages buildings.

    // GameObject component
    private GameObject buildings;
    private GameObject importedBuildings;
    private List<GameObject> selectedBuildingsList;
    private List<GameObject> deletedBuildingsList;

    // Manager component
    private UIManager uiManager;

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

        // Get manager component
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        // Initialize variable
        selectedBuildingsList = new List<GameObject>();
        deletedBuildingsList = new List<GameObject>();
        viewMode = 0;
    }

    // Update deleted building's state
    public void UpdateDeletedBuildings()
    {
        for (int i = 0; i < deletedBuildingsList.Count; i++)
        {
            deletedBuildingsList[i].SetActive(false);
        }
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
        for (int i = 0; i < deletedBuildingsList.Count; i++)
        {
            deletedBuildingsList[i].SetActive(false);
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
            for (int i = 0; i < deletedBuildingsList.Count; i++)
            {
                deletedBuildingsList[i].SetActive(true);
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
            for (int i = 0; i < deletedBuildingsList.Count; i++)
            {
                deletedBuildingsList[i].SetActive(false);
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
    public void SelectBuilding(GameObject building)
    {
        if (!selectedBuildingsList.Contains(building))
        {
            selectedBuildingsList.Add(building);
            building.GetComponent<MeshRenderer>().material = selectMaterial;
        }
        if (selectedBuildingsList.Count == 1)
        {
            for (int i = 0; i < buildings.transform.childCount; i++)
            {
                if (building == buildings.transform.GetChild(i).gameObject)
                {
                    uiManager.WriteBuildingInfo(i);
                    break;
                }
            }
        }
        else
        {
            uiManager.TurnOffUI(0);
        }
    }

    // Delete from selectedObjectList
    public void DeleteFromSelectedBuildingsList(int index)
    {
        selectedBuildingsList[index].GetComponent<MeshRenderer>().material = normalMaterial;
        selectedBuildingsList.RemoveAt(index);
    }

    // Clear selectedObjectList
    public void ClearSelectedBuildingsList()
    {
        for (int i = 0; i < selectedBuildingsList.Count; i++)
        {
            selectedBuildingsList[i].GetComponent<MeshRenderer>().material = normalMaterial;
        }
        selectedBuildingsList.Clear();
        uiManager.TurnOffUI(0);
    }

    // Return selectedObjectList
    public List<GameObject> GetSelectedBuildingsList()
    {
        return selectedBuildingsList;
    }

    // Return deletedObjectList
    public List<GameObject> GetDeletedBuildingsList()
    {
        return deletedBuildingsList;
    }
}
