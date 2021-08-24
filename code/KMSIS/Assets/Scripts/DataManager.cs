using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public TextAsset dongjak;
    public TextAsset ydata;
    private string[,] buildingData; // 0 : management_number, 1 : latitude, 2 : longitude, 3 : name, 4 : sido, 5 : gu, 6 : dong, 7 : road_name, 8 : subname, 9 : number,  10 : height, 11 : name_eng
    private int dongjakSize;

    private string directory = "/SaveFile";
    private string filename = "/data.save";

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

        standardX = GameObject.Find("Building.4983").transform.position.x;
        standardZ = GameObject.Find("Building.4983").transform.position.z;
        for (int i = 0; i < rowSize; i++)
        {
            GameObject temp = GameObject.Find("Buildings").transform.GetChild(i).gameObject;
            if (temp.tag == "Untagged")
            {
                temp.transform.position = new Vector3(temp.transform.position.x, float.Parse(ydata_line[i]), temp.transform.position.z);
                temp.tag = "Building";
            }
        }

        Load();
    }

    public void FindBuilding(GameObject building)
    {
        double latitude, longitude, x, z, dx, dz;
        double mx = (double)(building.transform.position.x);
        double mz = (double)(building.transform.position.z);
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

    public void Save()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(Application.persistentDataPath + directory + filename, FileMode.Create);
        UserData userData = SaveData();
        binaryFormatter.Serialize(fileStream, userData);
        fileStream.Close();
    }

    private void Load()
    {
        if (File.Exists(Application.persistentDataPath + directory + filename))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(Application.persistentDataPath + directory + filename, FileMode.Open);
            UserData userData = binaryFormatter.Deserialize(fileStream) as UserData;
            LoadData(userData);
            fileStream.Close();
        }
        else
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath + directory);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }
    }

    [Serializable]
    public class UserData
    {
        [SerializeField]
        private float cameraPositionX, cameraPositionY, cameraPositionZ, cameraRotationX, cameraRotationY, cameraRotationZ;

        public UserData() {}

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
    }

    public UserData SaveData()
    {
        UserData userData = new UserData();
        userData.SetCameraPosition(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z));
        userData.SetCameraRotation(new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z));
        return userData;
    }

    private void LoadData(UserData userData)
    {
        Camera.main.transform.position = userData.GetCameraPosition();
        Camera.main.transform.eulerAngles = userData.GetCameraRotation();
    }
}
