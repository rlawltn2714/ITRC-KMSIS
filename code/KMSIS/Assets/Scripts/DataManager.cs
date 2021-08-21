using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public TextAsset dongjak;
    public TextAsset ydata;
    string[,] buildingData;
    int lineSize, rowSize;

    void Start()
    {
        string currentText = dongjak.text.Substring(0, dongjak.text.Length - 1);
        string[] dongjak_line = currentText.Split('\n');
        lineSize = dongjak_line.Length;
        rowSize = dongjak_line[0].Split('\t').Length;
        buildingData = new string[lineSize, rowSize];

        for (int i = 0; i < lineSize; i++)
        {
            string[] row = dongjak_line[i].Split('\t');
            for (int j = 0; j < rowSize; j++)
            {
                buildingData[i, j] = row[j];
            }
        }

        currentText = ydata.text.Substring(0, ydata.text.Length - 1);
        string[] ydata_line = currentText.Split('\n');
        lineSize = ydata_line.Length;

        GameObject buildings = GameObject.Find("Buildings");
        for(int i = 0; i < lineSize; i++)
        {
            GameObject temp = buildings.transform.GetChild(i).gameObject;
            if (temp.tag == "Untagged")
            {
                temp.transform.position = new Vector3(temp.transform.position.x, float.Parse(ydata_line[i]), temp.transform.position.z);
                temp.tag = "Building";
            }
        }
    }
}
