using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InputField monthInput;
    public InputField dayInput;
    public InputField hourInput;
    public InputField minuteInput;

    private SunManager sunManager;

    void Start()
    {
        sunManager = GameObject.Find("SunManager").GetComponent<SunManager>();
        monthInput.text = System.DateTime.Now.ToString("MM");
        dayInput.text = System.DateTime.Now.ToString("dd");
        hourInput.text = System.DateTime.Now.ToString("HH");
        minuteInput.text = System.DateTime.Now.ToString("mm");
        SetSunPosition();
    }

    public void SetSunPosition()
    {
        if (int.TryParse(monthInput.text, out int month) && int.TryParse(dayInput.text, out int day) && int.TryParse(hourInput.text, out int hour) && int.TryParse(minuteInput.text, out int minute))
        {
            float clock;
            clock = (float)(hour) + (float)(minute) / 60f;
            List<double> sunDataList = sunManager.Calculate(month, day, clock);
            if (sunDataList != null)
            {
                GameObject.Find("Directional Light").transform.eulerAngles = new Vector3((float)(sunDataList[1]), (float)(sunDataList[0]), 0);
            }
        }
    }
}
