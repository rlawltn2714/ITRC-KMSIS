using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public TextAsset dongjak;
    public TextAsset ydata;
    private string[,] buildingData; // 0 : management_number, 1 : latitude, 2 : longitude, 3 : name, 4 : sido, 5 : gu, 6 : dong, 7 : road_name, 8 : subname, 9 : number,  10 : height, 11 : name_eng
    private int dongjakSize;

    private GameObject buildings;
    private double standardX, standardZ;
    private double latitudeStandard = 2250.44549070300276;
    private double longitudeStandard = 7617.6241408941156;
    private double ratio = 0.254985734830929;

    void Start()
    {
        string currentText = dongjak.text.Substring(0, dongjak.text.Length - 1);
        string[] dongjak_line = currentText.Split('\n');
        dongjakSize = dongjak_line.Length;
        int rowSize = dongjakSize;
        int colSize = dongjak_line[0].Split('\t').Length;
        buildingData = new string[rowSize, colSize];

        for (int i = 0; i < rowSize; i++)
        {
            string[] row = dongjak_line[i].Split('\t');
            for (int j = 0; j < colSize; j++)
            {
                buildingData[i, j] = row[j];
            }
        }

        currentText = ydata.text.Substring(0, ydata.text.Length - 1);
        string[] ydata_line = currentText.Split('\n');
        rowSize = ydata_line.Length;

        buildings = GameObject.Find("Buildings");
        standardX = GameObject.Find("Building.4983").transform.position.x;
        standardZ = GameObject.Find("Building.4983").transform.position.z;
        for (int i = 0; i < rowSize; i++)
        {
            GameObject temp = buildings.transform.GetChild(i).gameObject;
            if (temp.tag == "Untagged")
            {
                temp.transform.position = new Vector3(temp.transform.position.x, float.Parse(ydata_line[i]), temp.transform.position.z);
                temp.tag = "Building";
            }
        }
    }

    public void FindBuilding(int index)
    {
        double latitude, longitude, x, z, dx, dz;
        double mx = (double)(buildings.transform.GetChild(index).position.x);
        double mz = (double)(buildings.transform.GetChild(index).position.z);
        float[] d = new float[dongjakSize];
        int min = -1;
        for (int i = 0; i < dongjakSize; i++)
        {
            latitude = double.Parse(buildingData[i, 1]);
            longitude = double.Parse(buildingData[i, 2]);
            latitude = ConvertToMinute(latitude);
            longitude = ConvertToMinute(longitude);
            x = longitudeStandard - longitude;
            z = latitudeStandard - latitude;
            dx = x - (mx - standardX) * ratio;
            dz = z - (mz - standardZ) * ratio;
            d[i] = Mathf.Sqrt((float)(dx * dx + dz * dz));
            if (min == -1)
            {
                min = i;
            }
            else if (d[min] > d[i])
            {
                min = i;
            }
        }
        PrintAll(min);
    }

    private double ConvertToMinute(double value)
    {
        double t = value / 1;
        return t * 60 + (value - t) * 60;
    }

    private void PrintAll(int index)
    {
        Debug.Log(buildingData[index, 0] + " " + buildingData[index, 1] + " " + buildingData[index, 2] + " " + buildingData[index, 3] + " " + buildingData[index, 6] + " " + buildingData[index, 7] + " " + buildingData[index, 9]);
    }
}
