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

    // Manager component
    private SunManager sunManager;
    private DataManager dataManager;

    void Start()
    {
        // Get manager component
        sunManager = GameObject.Find("SunManager").GetComponent<SunManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();

        //  Initialize text of UI
        monthInput.text = System.DateTime.Now.ToString("MM");
        dayInput.text = System.DateTime.Now.ToString("dd");
        hourInput.text = System.DateTime.Now.ToString("HH");
        minuteInput.text = System.DateTime.Now.ToString("mm");

        // Set the position of sun
        SetSunPosition();
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
        else if (tempList.Count < 10)
        {
            for(int i = 0; i < tempList.Count; i++)
            {
                Debug.Log(tempList[i].name);
            }
        }
        else Debug.Log("검색 결과가 너무 많습니다.");
    }
}
