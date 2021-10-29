using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class UIManager : MonoBehaviour
{
    // This class manages UI. This might be fixed in the future according to the design.

    // UI component
    public GameObject basePanel;
    public GameObject recentPanel;
    public GameObject iconPanel;
    public GameObject infoPanel;
    public GameObject sunlightPanel;
    public GameObject periodPanel;
    public GameObject importPanel;
    public GameObject timePanel;
    public GameObject importPreviewPanel;

    public InputField searchInput;
    public InputField[] scaleInput;
    public InputField[] rotationInput;

    // Manager component
    private SunManager sunManager;
    private DataManager dataManager;
    private BuildingManager buildingManager;
    private ControlManager controlManager;
    private AnalysisManager analysisManager;

    // Time variable
    private int[] dayForMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    string yearString, monthString, dayString, hourString, minuteString;

    // Slider click state
    bool sliderClicked;

    void Start()
    {
        // Get manager component
        sunManager = GameObject.Find("SunManager").GetComponent<SunManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        buildingManager = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();
        controlManager = GameObject.Find("ControlManager").GetComponent<ControlManager>();
        analysisManager = GameObject.Find("AnalysisManager").GetComponent<AnalysisManager>();

        // Set variable
        yearString = System.DateTime.Now.ToString("yyyy");
        monthString = System.DateTime.Now.ToString("MM");
        dayString = System.DateTime.Now.ToString("dd");
        hourString = System.DateTime.Now.ToString("HH");
        minuteString = System.DateTime.Now.ToString("mm");

        sliderClicked = false;

        // Set the position of sun
        SetSunPosition();

        // Update time panel
        if (int.TryParse(yearString, out int year) && int.TryParse(monthString, out int month) && int.TryParse(dayString, out int day) && int.TryParse(hourString, out int hour) && int.TryParse(minuteString, out int minute))
        {
            UpdateTimePanel(year, month, day, hour, minute);
            float clock = (float)(hour * 60 + minute) / 1440f;
            timePanel.transform.GetChild(3).GetComponent<Slider>().value = clock;
        }
    }

    // UI index - infoPanel(0), sunlightPanel(1), periodPanel(2), importPreviewPanel(3), importPanel(4)

    // Turn off the UI
    public void TurnOffUI(int index)
    {
        if (index == -1)
        {
            infoPanel.SetActive(false);
            sunlightPanel.SetActive(false);
            periodPanel.SetActive(false);
            importPanel.SetActive(false);
            importPreviewPanel.SetActive(false);
        }
        else if (index == 0)
        {
            infoPanel.SetActive(false);
        }
        else if (index == 1)
        {
            sunlightPanel.SetActive(false);
            periodPanel.SetActive(false);
        }
        else if (index == 3)
        {
            importPreviewPanel.SetActive(false);
        }
        else if (index == 4)
        {
            importPanel.SetActive(false);
        }
    }

    // Turn on the UI
    public void TurnOnUI(int index)
    {
        if (index == 0)
        {
            TurnOffUI(-1);
            infoPanel.SetActive(true);
        }
        else if (index == 1)
        {
            if (buildingManager.GetSelectedBuildingsList().Count == 1)
            {
                if (analysisManager.IsAnalyzing())
                {
                    analysisManager.Release();
                    buildingManager.ClearSelectedBuildingsList();
                    controlManager.SetMode(0);
                    TurnOffUI(-1);
                }
                else
                {
                    TurnOffUI(-1);
                    sunlightPanel.SetActive(true);
                    periodPanel.SetActive(true);
                    controlManager.SetMode(2);
                    analysisManager.Init(buildingManager.GetSelectedBuildingsList()[0]);
                    List<int> temp = analysisManager.AnalyzeBuilding();
                    if (temp != null)
                    {
                        sunlightPanel.transform.GetChild(2).GetComponent<Text>().text = (temp[0] / 60) + "시간 " + (temp[0] % 60) + "분";
                        sunlightPanel.transform.GetChild(6).GetComponent<Text>().text = (temp[1] / 60) + "시간 " + (temp[1] % 60) + "분";
                    }
                    else
                    {
                        analysisManager.Release();
                        buildingManager.ClearSelectedBuildingsList();
                        controlManager.SetMode(0);
                        TurnOffUI(-1);
                    }
                }
            }
        }
        else if (index == 3)
        {
            TurnOffUI(-1);
            importPreviewPanel.SetActive(true);
        }
        else if (index == 4)
        {
            TurnOffUI(-1);
            importPanel.SetActive(true);
        }
    }

    // Load information from imported building
    public void LoadInfo(GameObject building)
    {
        for (int i = 0; i < 3; i++)
        {
            scaleInput[i].text = "1";
        }
        rotationInput[0].text = building.transform.eulerAngles.x.ToString();
        rotationInput[1].text = building.transform.eulerAngles.y.ToString();
        rotationInput[2].text = building.transform.eulerAngles.z.ToString();
    }
    
    // Set slider state
    public void SetSliderState(bool state)
    {
        sliderClicked = state;
    }

    // Get slider state
    public bool GetSliderState()
    {
        return sliderClicked;
    }

    // Update time
    public void UpdateTime()
    {
        float clock = timePanel.transform.GetChild(3).GetComponent<Slider>().value * 1440f;
        int hour = (int)(clock / 60);
        int minute = (int)(clock % 60);
        hourString = hour.ToString();
        minuteString = minute.ToString();
        if (int.TryParse(yearString, out int year) && int.TryParse(monthString, out int month) && int.TryParse(dayString, out int day))
        {
            UpdateTimePanel(year, month, day, hour, minute);
        }
        SetSunPosition();
    }

    // Update time panel
    public void UpdateTimePanel(int year, int month, int day, int hour, int minute)
    {
        if (minute < 10)
        {
            if (hour > 11)
            {
                timePanel.transform.GetChild(2).GetComponent<Text>().text = hour + ":0" + minute + " PM";
            }
            else
            {
                timePanel.transform.GetChild(2).GetComponent<Text>().text = hour + ":0" + minute + " AM";
            }
        }
        else
        {
            if (hour > 11)
            {
                timePanel.transform.GetChild(2).GetComponent<Text>().text = hour + ":" + minute + " PM";
            }
            else
            {
                timePanel.transform.GetChild(2).GetComponent<Text>().text = hour + ":" + minute + " AM";
            }
        }
        timePanel.transform.GetChild(4).GetComponent<Text>().text = month + "/" + day + "/" + year;
        if (dayForMonth[month-1] > day)
        {
            timePanel.transform.GetChild(5).GetComponent<Text>().text = month + "/" + (day + 1) + "/" + year;
        }
        else
        {
            if (month == 12)
            {
                timePanel.transform.GetChild(5).GetComponent<Text>().text = "1/1/" + (year + 1);
            }
            else
            {
                timePanel.transform.GetChild(5).GetComponent<Text>().text = (month + 1) + "/1/" + year;
            }
        }
    }

    // Write building name & location on UI
    public void WriteBuildingInfo(int index)
    {
        TurnOnUI(0);
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
        rect = timePanel.GetComponent<RectTransform>();
        if (clickPosition.x > rect.position.x - rect.sizeDelta.x / 2 && clickPosition.y > rect.position.y - rect.sizeDelta.y / 2 && clickPosition.x < rect.position.x + rect.sizeDelta.x / 2 && clickPosition.y < rect.position.y + rect.sizeDelta.y / 2)
        {
            return true;
        }
        return false;
    }

    // Check if inputfield is focused
    public bool CheckInputfieldFocused()
    {
        if (searchInput.isFocused || scaleInput[0].isFocused || scaleInput[1].isFocused || scaleInput[2].isFocused || rotationInput[0].isFocused || rotationInput[1].isFocused || rotationInput[2].isFocused)
        {
            return true;
        }
        else return false;
    }

    // Get UI value
    public List<float> GetTimeValue()
    {
        if (int.TryParse(monthString, out int month) && int.TryParse(dayString, out int day) && int.TryParse(hourString, out int hour) && int.TryParse(minuteString, out int minute))
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
        if (int.TryParse(monthString, out int month) && int.TryParse(dayString, out int day) && int.TryParse(hourString, out int hour) && int.TryParse(minuteString, out int minute))
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
        if (int.TryParse(monthString, out int month) && int.TryParse(dayString, out int day))
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
