using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class maketext : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Directory.CreateDirectory(Application.streamingAssetsPath + "/text/");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            CreateTextFile();
        }
    }

    private void CreateTextFile()
    {
        string content = "";
        GameObject buildings = GameObject.Find("Buildings");
        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            GameObject temp = buildings.transform.GetChild(i).gameObject;
            content += temp.transform.position.y.ToString() + "\n";
        }
        File.WriteAllText(Application.streamingAssetsPath + "/text/" + "YData" + ".txt", content);
    }
}
