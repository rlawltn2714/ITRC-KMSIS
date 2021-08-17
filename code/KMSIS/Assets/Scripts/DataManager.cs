using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public TextAsset data;
    string[,] buildingData;
    int lineSize, rowSize;

    void Start()
    {
        string currentText = data.text.Substring(0, data.text.Length - 1);
        string[] line = currentText.Split('\n');
        lineSize = line.Length;
        rowSize = line[0].Split('\t').Length;
        buildingData = new string[lineSize, rowSize];

        for (int i = 0; i < lineSize; i++)
        {
            string[] row = line[i].Split('\t');
            for (int j = 0; j < rowSize; j++)
            {
                buildingData[i, j] = row[j];
            }
        }

        for (int i = 0; i < 11; i++)
        {
            Debug.Log(buildingData[0, i]);
        }
    }
}
