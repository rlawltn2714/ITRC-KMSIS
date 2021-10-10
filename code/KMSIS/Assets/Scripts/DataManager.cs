using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Threading;
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
    private BuildingManager buildingManager;

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
        buildingManager = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();

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
            Camera.main.transform.position = new Vector3(0.5806503f, 1.184541f, 4.311463f);
            Camera.main.transform.eulerAngles = new Vector3(16.365f, 161.884f, 0f);
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
        private float[] scale;
        private float[] rotation;
        private int index;
        private bool active;

        public Building() {
            position = new float[3];
            scale = new float[3];
            rotation = new float[3];
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

        public void SetScale(Vector3 scale)
        {
            this.scale[0] = scale.x;
            this.scale[1] = scale.y;
            this.scale[2] = scale.z;
        }

        public float[] GetScale()
        {
            return scale;
        }

        public void SetRotation(Vector3 rotation)
        {
            this.rotation[0] = rotation.x;
            this.rotation[1] = rotation.y;
            this.rotation[2] = rotation.z;
        }

        public float[] GetRotation()
        {
            return rotation;
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
        private List<int> deletedBuildingsList;

        public UserData() {
            importedBuildingsList = new List<Building>();
            deletedBuildingsList = new List<int>();
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

        public List<int> GetDeletedBuildingsList()
        {
            return deletedBuildingsList;
        }
    }

    // Save Data using UserData
    private UserData SaveData()
    {
        UserData userData = new UserData();
        userData.SetCameraPosition(Camera.main.transform.position);
        userData.SetCameraRotation(Camera.main.transform.eulerAngles);
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
                temp.SetPosition(tempObject.transform.position);
                temp.SetScale(tempObject.transform.localScale);
                temp.SetRotation(tempObject.transform.eulerAngles);
                temp.SetActive(tempObject.activeSelf);
                userData.GetImportedBuildingsList().Add(temp);
            }
        }
        List<GameObject> tempList = buildingManager.GetDeletedBuildingsList();
        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            if (tempList.Contains(buildings.transform.GetChild(i).gameObject))
            {
                userData.GetDeletedBuildingsList().Add(i);
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
            Thread.Sleep(500);
        }
        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            if (userData.GetDeletedBuildingsList().Contains(i))
            {
                buildingManager.GetDeletedBuildingsList().Add(buildings.transform.GetChild(i).gameObject);
                buildings.transform.GetChild(i).gameObject.SetActive(false);
            }
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
                if (!importedBuildingsList[i].GetActive())
                {
                    buildingManager.GetDeletedBuildingsList().Add(tempObject);
                }
                float[] temp = importedBuildingsList[i].GetPosition();
                if (temp.Length != 3) Debug.Log("Error : Position data are invaild.");
                else tempObject.transform.position = new Vector3(temp[0], temp[1], temp[2]);
                temp = importedBuildingsList[i].GetScale();
                if (temp.Length != 3) Debug.Log("Error : Scale data are invaild.");
                else tempObject.transform.localScale = new Vector3(temp[0], temp[1], temp[2]);
                temp = importedBuildingsList[i].GetRotation();
                if (temp.Length != 3) Debug.Log("Error : Rotation data are invaild.");
                else tempObject.transform.eulerAngles = new Vector3(temp[0], temp[1], temp[2]);
            }
        }
    }

    // Copy model file
    public void CopyFile(string filePath)
    {
        string fileName = Path.GetFileName(filePath);
        File.Copy(filePath, Application.persistentDataPath + directory2 + "/" + fileName, true);
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
