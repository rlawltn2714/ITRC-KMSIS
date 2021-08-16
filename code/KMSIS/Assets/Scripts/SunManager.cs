using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunManager : MonoBehaviour
{
    private double degToRad = 0.01745329251;
    private double radToDeg = 57.2957795131;
    private double latitude = 37.507424845050046;
    private double longitude = 126.96040234823526;

    private int[] dayForMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    private int dayOfYear;
    private int lstm = 135;
    private double b, g, eot, tc, lst, hra;
    private double azimuth, altitude, sunrise, sunset;

    public List<double> Calculate(int month, int day, float clock)
    {
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
        List<double> sunDataList = new List<double>();
        sunDataList.Add(azimuth);
        sunDataList.Add(altitude);
        sunDataList.Add(sunrise);
        sunDataList.Add(sunset);
        return sunDataList;
    }

    public Vector3 CalculateSunVector(double azimuth, double altitude)
    {
        return new Vector3(Mathf.Sin((float)(azimuth * degToRad)) * Mathf.Cos((float)(altitude * degToRad)), Mathf.Sin(-(float)(altitude * degToRad)), Mathf.Cos((float)(altitude * degToRad)) * Mathf.Cos((float)(azimuth * degToRad)));
    }
}
