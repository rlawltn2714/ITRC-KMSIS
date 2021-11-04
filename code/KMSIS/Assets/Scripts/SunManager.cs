using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SunManager : MonoBehaviour
{
    // This class manages sun.

    // Manager component
    private UIManager uiManager;

    // Convert value
    private double degToRad = 0.01745329251;
    private double radToDeg = 57.2957795131;

    // Latitude and longitude (Standard : Chung-Ang University Hospital)
    private double latitude = 37.507424845050046;
    private double longitude = 126.96040234823526;

    // Value needed to calculate data of sun
    private int[] dayForMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    private int dayOfYear;
    private int lstm = 135;
    private double b, g, eot, tc, lst, hra;
    private double azimuth, altitude, sunrise, sunset;

    // Variable for simulation
    private bool simulationMode = false;
    private int simulationMonth, simulationDay;
    private float simulationTime, simulationSunrise, simulationSunset, simulationInterval;

    void Start()
    {
        // Get manager component
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    void FixedUpdate()
    {
        if (simulationMode) // When simulation mode and need to update the position of sun
        {
            // Calculate position of sun
            float clock = simulationTime * 24f / 1440f;
            List<double> tempList = Calculate(simulationMonth, simulationDay, clock);
            if (tempList != null)
            {
                // Set the position of sun
                if (clock > (tempList[2] + tempList[3]) / 2)
                {
                    GameObject.Find("Directional Light").transform.eulerAngles = new Vector3((float)(tempList[1]), (float)(-tempList[0]), 0);
                }
                else
                {
                    GameObject.Find("Directional Light").transform.eulerAngles = new Vector3((float)(tempList[1]), (float)(tempList[0]), 0);
                }
            }

            // Add interval
            simulationTime += 1;
            uiManager.timePanel.transform.GetChild(3).GetComponent<Slider>().value = simulationTime;

            if (simulationTime >= 1439) // When simulation is end
            {
                simulationMode = false;
            }
        }
    }

    // Calculate azimuth, altitude, sunrise, sunset four values
    public List<double> Calculate(int month, int day, float clock)
    {
        // Calculate dayOfYear
        dayOfYear = 0;
        for (int i = 1; i < month; i++)
        {
            dayOfYear += dayForMonth[i - 1];
        }
        if (day > dayForMonth[month - 1] || day < 1)
        {
            Debug.Log("Unexpected value, day = " + day);
            return null;
        }
        else
        {
            dayOfYear += day;
        }

        // Calculate data of sun
        b = (double)((dayOfYear - 81) * 360) / 365;
        g = radToDeg * Mathf.Asin(Mathf.Sin((float)(23.45 * degToRad)) * Mathf.Sin((float)(b * degToRad)));
        eot = 9.87 * Mathf.Sin((float)(2 * b * degToRad)) - 7.53 * Mathf.Cos((float)(b * degToRad)) - 1.5 * Mathf.Sin((float)(b * degToRad));
        tc = 4 * (longitude - lstm) + eot;
        lst = clock + tc / 60;
        hra = 15 * (lst - 12);
        altitude = Mathf.Asin(Mathf.Sin((float)(g * degToRad)) * Mathf.Sin((float)(latitude * degToRad)) + Mathf.Cos((float)(g * degToRad)) * Mathf.Cos((float)(latitude * degToRad)) * Mathf.Cos((float)(hra * degToRad)));
        altitude = radToDeg * altitude;
        azimuth = Mathf.Sin((float)(g * degToRad)) * Mathf.Cos((float)(latitude * degToRad)) - Mathf.Cos((float)(g * degToRad)) * Mathf.Sin((float)(latitude * degToRad)) * Mathf.Cos((float)(hra * degToRad));
        azimuth = Mathf.Acos((float)(azimuth / Mathf.Cos((float)(altitude * degToRad))));
        azimuth = radToDeg * azimuth;
        sunrise = 12f - radToDeg * Mathf.Acos(-Mathf.Tan((float)(latitude * degToRad)) * Mathf.Tan((float)(g * degToRad))) / 15f - tc / 60f;
        sunset = 12f + radToDeg * Mathf.Acos(-Mathf.Tan((float)(latitude * degToRad)) * Mathf.Tan((float)(g * degToRad))) / 15f - tc / 60f;
        
        // Make list and return it
        List<double> sunDataList = new List<double>();
        sunDataList.Add(azimuth);
        sunDataList.Add(altitude);
        sunDataList.Add(sunrise);
        sunDataList.Add(sunset);
        return sunDataList;
    }

    // Calculate DirectionalLight.forward using azimuth, altitude
    public Vector3 CalculateSunVector(double azimuth, double altitude)
    {
        return new Vector3(Mathf.Sin((float)(azimuth * degToRad)) * Mathf.Cos((float)(altitude * degToRad)), Mathf.Sin(-(float)(altitude * degToRad)), Mathf.Cos((float)(altitude * degToRad)) * Mathf.Cos((float)(azimuth * degToRad)));
    }

    // Start simulation
    public void StartSimulate()
    {
        // Calculate sunrise and sunset
        List<int> timeInfo = uiManager.GetTime();

        // Set values
        simulationMonth = timeInfo[0];
        simulationDay = timeInfo[1];
        simulationTime = timeInfo[2] * 60 + timeInfo[3];

        // Turn on the simulation mode
        simulationMode = true;
    }

    // Stop simulation
    public void StopSimulate()
    {
        simulationMode = false;
    }
}