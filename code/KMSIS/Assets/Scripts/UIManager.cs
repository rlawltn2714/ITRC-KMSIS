using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // This class manages UI. This might be fixed in the future according to the design.

    // UI component
    public GameObject basePanel;
    public GameObject sunnyIcon;
    public GameObject buildingIcon;
    public GameObject questionIcon;
    public GameObject cogWheelIcon;
    public GameObject infoPanel;

    public InputField searchInput;
    public InputField[] scaleInput;
    public InputField[] rotationInput;

    public InputField monthInput;
    public InputField dayInput;
    public InputField hourInput;
    public InputField minuteInput;
    public Button importButton;
    public Button simulateButton;
    public Button searchButton;

    // Manager component
    private SunManager sunManager;
    private DataManager dataManager;
    private BuildingManager buildingManager;
    private ControlManager controlManager;

    void Start()
    {
        // Get manager component
        sunManager = GameObject.Find("SunManager").GetComponent<SunManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        buildingManager = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();
        controlManager = GameObject.Find("ControlManager").GetComponent<ControlManager>();

        monthInput.text = "0";
        dayInput.text = "0";
        hourInput.text = "0";
        minuteInput.text = "0";

        // Set the position of sun
        SetSunPosition();

        // Turn on UI
        ChangeInterface(-1, 0);
    }

    // Turn off the info panel
    public void TurnOffInfoPanel()
    {
        infoPanel.SetActive(false);
    }

    // Write building name & location on UI
    public void WriteBuildingInfo(int index)
    {
        infoPanel.SetActive(true);
        List<string> result = dataManager.FindData(index);
        if (result[3] == "null")
        {
            infoPanel.transform.GetChild(0).GetComponent<Text>().text = "등록되지 않은 건물입니다.";
        }
        else
        {
            infoPanel.transform.GetChild(0).GetComponent<Text>().text = result[3];
        }
        infoPanel.transform.GetChild(2).GetComponent<Text>().text = result[4] + " " + result[5] + " " + result[6] + " " + result[7];
    }

    // Check if mouse is on UI
    public bool IsMouseOnUI(Vector2 clickPosition)
    {
        RectTransform rect = basePanel.GetComponent<RectTransform>();
        if (clickPosition.x > rect.position.x - rect.sizeDelta.x / 2 && clickPosition.y > rect.position.y - rect.sizeDelta.y / 2 && clickPosition.x < rect.position.x + rect.sizeDelta.x / 2 && clickPosition.y < rect.position.y + rect.sizeDelta.y / 2)
        {
            return true;
        }
        else return false;
    }

    // Check if inputfield is focused
    public bool CheckInputfieldFocused()
    {
        if (monthInput.isFocused || dayInput.isFocused || hourInput.isFocused || minuteInput.isFocused || searchInput.isFocused || scaleInput[0].isFocused || scaleInput[1].isFocused || scaleInput[2].isFocused)
        {
            return true;
        }
        else return false;
    }

    // Change UI
    public void ChangeInterface(int current, int next)
    {
        if (current == -1 && next == 0)
        {
            searchInput.gameObject.SetActive(true);
            importButton.gameObject.SetActive(true);
            simulateButton.gameObject.SetActive(true);
            searchButton.gameObject.SetActive(true);
        }
        else if (current == -1 && next == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                scaleInput[i].gameObject.SetActive(true);
                scaleInput[i].text = "1";
            }
        }
        else if (current == 1 && next == 0)
        {
            searchInput.gameObject.SetActive(true);
            importButton.gameObject.SetActive(true);
            simulateButton.gameObject.SetActive(true);
            searchButton.gameObject.SetActive(true);
        }
    }

    // Get UI value
    public List<float> GetInterfaceValue()
    {
        if (int.TryParse(monthInput.text, out int month) && int.TryParse(dayInput.text, out int day) && int.TryParse(hourInput.text, out int hour) && int.TryParse(minuteInput.text, out int minute))
        {
            // Calculate clock
            float clock = (float)(hour) + (float)(minute) / 60f;
            List<float> result = new List<float>();
            result.Add((float)(month));
            result.Add((float)(day));
            result.Add(clock);
            return result;
        }
        else return null;
    }

    // Set scale of imported building
    public void SetScale()
    {
        if (float.TryParse(scaleInput[0].text, out float x) && float.TryParse(scaleInput[1].text, out float y) && float.TryParse(scaleInput[2].text, out float z))
        {
            controlManager.SetScale(x, y, z);
        }
    }

    // Update scale
    public void UpdateScaleUp(int index)
    {
        if (float.TryParse(scaleInput[index].text, out float temp))
        {
            temp += 0.1f;
            scaleInput[index].text = temp.ToString();
        }
    }

    // Update scale
    public void UpdateScaleDown(int index)
    {
        if (float.TryParse(scaleInput[index].text, out float temp))
        {
            temp -= 0.1f;
            scaleInput[index].text = temp.ToString();
        }
    }

    // Set the position of sun according to the text in UI
    public void SetSunPosition()
    {
        if (int.TryParse(monthInput.text, out int month) && int.TryParse(dayInput.text, out int day) && int.TryParse(hourInput.text, out int hour) && int.TryParse(minuteInput.text, out int minute))
        {
            // Calculate clock
            float clock = (float)(hour) + (float)(minute) / 60f;

            // Calculate data of sun using SunManager
            List<double> sunDataList = sunManager.Calculate(month, day, clock);
            if (sunDataList != null)
            {
                // Set the position of sun
                if (clock > (sunDataList[2] + sunDataList[3]) / 2)
                {
                    GameObject.Find("Directional Light").transform.eulerAngles = new Vector3((float)(sunDataList[1]), (float)(-sunDataList[0]), 0);
                }
                else
                {
                    GameObject.Find("Directional Light").transform.eulerAngles = new Vector3((float)(sunDataList[1]), (float)(sunDataList[0]), 0);
                }
            }
        }
    }

    // Simulate sunlight for a specific day
    public void Simulation()
    {
        if (int.TryParse(monthInput.text, out int month) && int.TryParse(dayInput.text, out int day))
        {
            sunManager.SimulateSunlight(month, day);
        }
    }

    // Search data with text using DataManager
    public void Search()
    {
        List<GameObject> tempList = dataManager.SearchWithText(searchInput.text);
        if (tempList == null || tempList.Count == 0) Debug.Log("검색 결과가 없습니다.");
        else
        {
            buildingManager.ClearSelectedBuildingsList();
            float x = 0f, z = 0f;
            for (int i = 0; i < tempList.Count; i++)
            {
                buildingManager.SelectBuilding(tempList[i]);
                x += tempList[i].transform.position.x;
                z += tempList[i].transform.position.z;
            }
            x = x / tempList.Count;
            z = z / tempList.Count;
            Camera.main.transform.position = new Vector3(x, 0.005f, z) - Camera.main.transform.forward * 2f;
        }
    }
}
