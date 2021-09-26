using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // This class manages UI. This might be fixed in the future according to the design.

    // UI component
    public InputField monthInput;
    public InputField dayInput;
    public InputField hourInput;
    public InputField minuteInput;
    public InputField searchInput;
    public Button importButton;
    public Button simulateButton;
    public Button searchButton;
    public InputField[] scaleInput;
    public Button[] scaleUp;
    public Button[] scaleDown;

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

        //  Initialize text of UI
        monthInput.text = System.DateTime.Now.ToString("MM");
        dayInput.text = System.DateTime.Now.ToString("dd");
        hourInput.text = System.DateTime.Now.ToString("HH");
        minuteInput.text = System.DateTime.Now.ToString("mm");

        // Set the position of sun
        SetSunPosition();

        // Turn on UI
        ChangeInterface(-1, 0);
    }

    // Check if mouse is on UI
    public bool IsMouseOnUI(Vector2 clickPosition, int mode)
    {
        List<RectTransform> rectList = new List<RectTransform>();
        if (mode == 0)
        {
            rectList.Add(monthInput.GetComponent<RectTransform>());
            rectList.Add(dayInput.GetComponent<RectTransform>());
            rectList.Add(hourInput.GetComponent<RectTransform>());
            rectList.Add(minuteInput.GetComponent<RectTransform>());
            rectList.Add(searchInput.GetComponent<RectTransform>());
            rectList.Add(importButton.GetComponent<RectTransform>());
            rectList.Add(simulateButton.GetComponent<RectTransform>());
            rectList.Add(searchButton.GetComponent<RectTransform>());

            for (int i = 0; i < rectList.Count; i++)
            {
                if (clickPosition.x > rectList[i].position.x - rectList[i].sizeDelta.x / 2 && clickPosition.y > rectList[i].position.y - rectList[i].sizeDelta.y / 2 && clickPosition.x < rectList[i].position.x + rectList[i].sizeDelta.x / 2 && clickPosition.y < rectList[i].position.y + rectList[i].sizeDelta.y / 2)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Change UI
    public void ChangeInterface(int current, int next)
    {
        if (current == -1 && next == 0)
        {
            SetInterfaceFalse();
            monthInput.gameObject.SetActive(true);
            dayInput.gameObject.SetActive(true);
            hourInput.gameObject.SetActive(true);
            minuteInput.gameObject.SetActive(true);
            searchInput.gameObject.SetActive(true);
            importButton.gameObject.SetActive(true);
            simulateButton.gameObject.SetActive(true);
            searchButton.gameObject.SetActive(true);
        }
        else if (current == -1 && next == 1)
        {
            SetInterfaceFalse();
            for (int i = 0; i < 3; i++)
            {
                scaleInput[i].gameObject.SetActive(true);
                scaleInput[i].text = "1";
                scaleUp[i].gameObject.SetActive(true);
                scaleDown[i].gameObject.SetActive(true);
            }
        }
        else if (current == 1 && next == 0)
        {
            SetInterfaceFalse();
            monthInput.gameObject.SetActive(true);
            dayInput.gameObject.SetActive(true);
            hourInput.gameObject.SetActive(true);
            minuteInput.gameObject.SetActive(true);
            searchInput.gameObject.SetActive(true);
            importButton.gameObject.SetActive(true);
            simulateButton.gameObject.SetActive(true);
            searchButton.gameObject.SetActive(true);
        }
    }

    // Turn off all UI
    private void SetInterfaceFalse()
    {
        monthInput.gameObject.SetActive(false);
        dayInput.gameObject.SetActive(false);
        hourInput.gameObject.SetActive(false);
        minuteInput.gameObject.SetActive(false);
        searchInput.gameObject.SetActive(false);
        importButton.gameObject.SetActive(false);
        simulateButton.gameObject.SetActive(false);
        searchButton.gameObject.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            scaleInput[i].gameObject.SetActive(false);
            scaleUp[i].gameObject.SetActive(false);
            scaleDown[i].gameObject.SetActive(false);
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
            buildingManager.ClearSelectedObjectList();
            float x = 0f, z = 0f;
            for (int i = 0; i < tempList.Count; i++)
            {
                buildingManager.SelectObject(tempList[i]);
                x += tempList[i].transform.position.x;
                z += tempList[i].transform.position.z;
            }
            x = x / tempList.Count;
            z = z / tempList.Count;
            Camera.main.transform.position = new Vector3(x, 0.005f, z) - Camera.main.transform.forward * 2f;
        }
    }
}
