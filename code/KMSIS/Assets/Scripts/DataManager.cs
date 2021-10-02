using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using UnityEngine;
using TriLibCore.Samples;

public class DataManager : MonoBehaviour
{
    // This class manages data.

    // Text component
    public TextAsset data;
    public TextAsset ydata;

    // Manager component
    private ImportManager importManager;

    // GameObject component
    private GameObject buildings;
    private GameObject importedBuildings;

    // SaveFile setting
    private string directory1 = "/SaveFile"; // window - C:/Users/Username/AppData/LocalLow/DefaultCompony/KMSIS/SaveFile/data.save
    private string directory2 = "/SaveFile/Buildings";
    private string filename = "/data.save";
    private List<Building> importedBuildingsList;

    // Local variable (Standard : Chung-Ang University Hospital)
    private string[,] buildingData; // 0 : management_number, 1 : latitude, 2 : longitude, 3 : name, 4 : sido, 5 : gu, 6 : dong, 7 : road_name, 8 : subname, 9 : number,  10 : height, 11 : name_eng
    private int dongjakSize;

    void Start()
    {
        // Text parsing for building data
        string currentText = data.text.Substring(0, data.text.Length - 1);
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

        // Text parsing for deleting levitation
        currentText = ydata.text.Substring(0, ydata.text.Length - 1);
        string[] ydata_line = currentText.Split('\n');
        rowSize = ydata_line.Length;
        for (int i = 0; i < rowSize; i++)
        {
            GameObject temp = GameObject.Find("Buildings").transform.GetChild(i).gameObject;
            if (temp.tag == "Untagged")
            {
                temp.transform.position = new Vector3(temp.transform.position.x, float.Parse(ydata_line[i]), temp.transform.position.z);
                temp.tag = "Building";
            }
        }

        // Get Manager component
        importManager = GameObject.Find("ImportManager").GetComponent<ImportManager>();

        // Get GameObject component
        buildings = GameObject.Find("Buildings");
        importedBuildings = GameObject.Find("ImportedBuildings");

        // Load savefile
        Load();
    }

    // Search data and return list of buildings
    public List<GameObject> SearchWithText(string text)
    {
        if (text == "") return null;
        List<GameObject> result = new List<GameObject>();
        string[] str = text.Split(' ');
        if (str.Length == 0)
        {
            return null;
        }
        else if (str.Length == 1)
        {
            for (int i = 0; i < buildings.transform.childCount; i++)
            {
                List<string> tempList = FindData(i);
                if (tempList[3] != "null" && tempList[3].Contains(str[0]))
                {
                    result.Add(buildings.transform.GetChild(i).gameObject);
                    continue;
                }
                if (tempList[8] != "null" && tempList[8].Contains(str[0]))
                {
                    result.Add(buildings.transform.GetChild(i).gameObject);
                    continue;
                }
                if (tempList[11] != "null" && tempList[11].Contains(str[0]))
                {
                    result.Add(buildings.transform.GetChild(i).gameObject);
                    continue;
                }
            }
            if (result.Count == 0) return null;
            else return result;
        }
        else
        {
            for (int i = 0; i < buildings.transform.childCount; i++)
            {
                List<string> tempList = FindData(i);
                if (tempList[4].Contains(str[0]) && str.Length < 5)
                {
                    bool check = true;
                    for (int j = 1; j < str.Length; j++)
                    {
                        if (tempList[4 + j].Contains(str[j])) continue;
                        else
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check) result.Add(buildings.transform.GetChild(i).gameObject);
                }
            }
        }
        return result;
    }

    // Find data of building and return it
    public List<string> FindData(int index)
    {
        List<string> result = new List<string>();
        for(int i = 0; i < 12; i++)
        {
            result.Add(buildingData[index, i]);
        }
        return result;
    }

    // Convert degree to minute
    private double ConvertToMinute(double value)
    {
        double t = value / 1;
        return t * 60 + (value - t) * 60;
    }

    // Save data
    public void Save()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(Application.persistentDataPath + directory1 + filename, FileMode.Create);
        UserData userData = SaveData();
        binaryFormatter.Serialize(fileStream, userData);
        fileStream.Close();
    }

    // Load data
    private void Load()
    {
        if (File.Exists(Application.persistentDataPath + directory1 + filename)) // When data is exist
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(Application.persistentDataPath + directory1 + filename, FileMode.Open);
            UserData userData = binaryFormatter.Deserialize(fileStream) as UserData;
            LoadData(userData);
            fileStream.Close();
        }
        else
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath + directory1);
            if (!directoryInfo.Exists) // When directory isn't exist
            {
                directoryInfo.Create();
            }
            directoryInfo = new DirectoryInfo(Application.persistentDataPath + directory2);
            if (!directoryInfo.Exists) // When directory isn't exist
            {
                directoryInfo.Create();
            }
            Save();
            Load();
        }
    }

    [Serializable]
    public class Building
    {
        [SerializeField]
        private string name;
        private float[] position;
        private int index;
        private bool active;

        public Building() {
            position = new float[3];
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void SetPosition(Vector3 position)
        {
            this.position[0] = position.x;
            this.position[1] = position.y;
            this.position[2] = position.z;
        }

        public float[] GetPosition()
        {
            return position;
        }

        public void SetIndex(int index)
        {
            this.index = index;
        }

        public int GetIndex()
        {
            return index;
        }

        public void SetActive(bool active)
        {
            this.active = active;
        }

        public bool GetActive()
        {
            return active;
        }
    }

    [Serializable]
    public class UserData
    {
        [SerializeField]
        private float cameraPositionX, cameraPositionY, cameraPositionZ, cameraRotationX, cameraRotationY, cameraRotationZ;
        private List<Building> importedBuildingsList;

        public UserData() {
            importedBuildingsList = new List<Building>();
        }

        public void SetCameraPosition(Vector3 position)
        {
            cameraPositionX = position.x;
            cameraPositionY = position.y;
            cameraPositionZ = position.z;
        }

        public Vector3 GetCameraPosition()
        {
            return new Vector3(cameraPositionX, cameraPositionY, cameraPositionZ);
        }

        public void SetCameraRotation(Vector3 rotation)
        {
            cameraRotationX = rotation.x;
            cameraRotationY = rotation.y;
            cameraRotationZ = rotation.z;
        }

        public Vector3 GetCameraRotation()
        {
            return new Vector3(cameraRotationX, cameraRotationY, cameraRotationZ);
        }

        public List<Building> GetImportedBuildingsList()
        {
            return importedBuildingsList;
        }
    }

    // Save Data using UserData
    private UserData SaveData()
    {
        UserData userData = new UserData();
        userData.SetCameraPosition(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z));
        userData.SetCameraRotation(new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z));
        for (int i = 0; i < importedBuildings.transform.childCount; i++)
        {
            GameObject tempObject = importedBuildings.transform.GetChild(i).gameObject;
            string result = SearchFile(tempObject.name);
            if (result == "null") continue;
            else
            {
                Building temp = new Building();
                temp.SetIndex(i);
                temp.SetName(result);
                temp.SetPosition(new Vector3(tempObject.transform.position.x, tempObject.transform.position.y, tempObject.transform.position.z));
                temp.SetActive(tempObject.activeSelf);
                userData.GetImportedBuildingsList().Add(temp);
            }

        }
        return userData;
    }

    // Load Data using UserData
    private void LoadData(UserData userData)
    {
        Camera.main.transform.position = userData.GetCameraPosition();
        Camera.main.transform.eulerAngles = userData.GetCameraRotation();
        importedBuildingsList = userData.GetImportedBuildingsList();
        for (int i = 0; i < importedBuildingsList.Count; i++)
        {
            importManager.ImportFromPath(Application.persistentDataPath + directory2 + "/" + importedBuildingsList[i].GetName());
        }
    }

    // Load buildling state
    public void LoadBuildingState(int index)
    {
        for (int i = 0; i < importedBuildingsList.Count; i++)
        {
            if (importedBuildingsList[i].GetIndex() == index)
            {
                GameObject tempObject = GameObject.Find("ImportedBuildings").transform.GetChild(index).gameObject;
                tempObject.SetActive(importedBuildingsList[i].GetActive());
                float[] temp = importedBuildingsList[i].GetPosition();
                if (temp.Length != 3) Debug.Log("Error : Position data are invaild.");
                else tempObject.transform.position = new Vector3(temp[0], temp[1], temp[2]);
            }
        }
    }

    // Copy model file
    public void CopyFile(string filePath)
    {
        string fName = Path.GetFileName(filePath);
        File.Copy(filePath, Application.persistentDataPath + directory2 + "/" + fName, true);
    }

    // Search model file
    private string SearchFile(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + directory2 + "/" + fileName + ".stl"))
        {
            return fileName + ".stl";
        }
        else if (File.Exists(Application.persistentDataPath + directory2 + "/" + fileName + ".fbx"))
        {
            return fileName + ".fbx";
        }
        else if (File.Exists(Application.persistentDataPath + directory2 + "/" + fileName + ".obj"))
        {
            return fileName + ".obj";
        }
        else
        {
            return "null";
        }
    }
}
