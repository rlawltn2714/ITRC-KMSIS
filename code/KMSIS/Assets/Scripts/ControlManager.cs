using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriLibCore.Samples;

public class ControlManager : MonoBehaviour
{
    // This class manages what the user controls.

    // Manager component
    private RayManager rayManager;
    private SunManager sunManager;
    private DataManager dataManager;
    private BuildingManager buildingManager;
    private ImportManager importManager;

    // SelectBox component and list of selected objects
    public RectTransform dragSelectBox;

    // RaycastHit variable
    private RaycastHit hit;

    // Material component
    public Material selectMaterial;
    public Material normalMaterial;

    // GameObject component
    private GameObject buildings;
    private GameObject importedBuildings;
    private GameObject mainCamera;

    // Local variable and setting
    private int mode; // -1 : unable control mode, 0 : normal mode, 1 : import mode, 2 : analysis mode
    private Vector2 clickPosition;
    private float clickTime;
    private float rotateLR;
    private float rotateUD;
    private float zoomSpeed = 20.0f;
    private float rotateSpeed = 5.0f;
    private float moveSpeed = 1.5f;
    private Vector3 offset;

    void Start()
    {
        // Get GameObject component
        buildings = GameObject.Find("Buildings");
        importedBuildings = GameObject.Find("ImportedBuildings");
        mainCamera = GameObject.Find("Main Camera");

        // Get manager component
        rayManager = GameObject.Find("RayManager").GetComponent<RayManager>();
        sunManager = GameObject.Find("SunManager").GetComponent<SunManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        buildingManager = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();
        importManager = GameObject.Find("ImportManager").GetComponent<ImportManager>();

        mode = 0;
    }

    void Update()
    {
        if (mode == 0)
        {
            if (Input.GetAxis("Mouse ScrollWheel") * zoomSpeed != 0) // When scroll
            {
                // Update zoom of camera
                mainCamera.GetComponent<Camera>().fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            }

            if (Input.GetAxis("Horizontal") != 0) // When get A D กๆ ก็
            {
                // Update position of camera
                mainCamera.transform.position += mainCamera.transform.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
                CameraPositionRangeCheck();
            }

            if (Input.GetAxis("Vertical") != 0) // When get W S ก่ ก้
            {
                // Update position of camera
                mainCamera.transform.position += mainCamera.transform.forward * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
                CameraPositionRangeCheck();
            }

            if (Input.GetKey(KeyCode.Space)) // When get spacebar
            {
                // Instantiate plane object using rayManager, and calculate R(x) using sunManager
                for (int i = 0; i < buildingManager.GetSelectedObjectList().Count; i++)
                {
                    List<RaycastHit> hitPointList = rayManager.GetPointOnObject(buildingManager.GetSelectedObjectList()[i]);
                    Debug.Log(rayManager.Ratio(hitPointList, sunManager.CalculateSunVector(GameObject.Find("Directional Light").transform.eulerAngles.y, GameObject.Find("Directional Light").transform.eulerAngles.x)) + "%");
                    rayManager.InstantiateObject(hitPointList);
                }
            }

            if (Input.GetKeyUp(KeyCode.Z))
            {
                for (int i = 0; i < buildingManager.GetSelectedObjectList().Count; i++)
                {
                    buildingManager.GetDeletedObjectList().Add(buildingManager.GetSelectedObjectList()[i]);
                    buildingManager.GetSelectedObjectList()[i].SetActive(false);
                }
                buildingManager.ClearSelectedObjectList();
            }

            if (Input.GetKeyUp(KeyCode.X))
            {
                buildingManager.GetDeletedObjectList().Clear();
                buildingManager.ShowAll();
            }

            if (Input.GetKeyUp(KeyCode.C))
            {
                buildingManager.ChangeView();
            }

            if (Input.GetMouseButtonDown(0)) // When left click is start
            {
                // Record position and time
                clickPosition = Input.mousePosition;
                clickTime = Time.time;
            }

            if (Input.GetMouseButton(0)) // When left click is continue
            {
                // Update SelectBox
                UpdateDragSelectBox(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0)) // When left click is end
            {
                if (Time.time - clickTime > 0.2)
                {
                    dataManager.Save();
                }
                else
                {
                    // Check if building is exist on click position
                    Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(clickPosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) // When get ctrl
                        {
                            if (hit.transform.gameObject.name != "Dem" && hit.transform.gameObject.name != "DemRigidbody") // When didn't click outside
                            {
                                // Check if object is already selected
                                bool isSelected = false;
                                for (int i = 0; i < buildingManager.GetSelectedObjectList().Count; i++)
                                {
                                    if (hit.collider.gameObject.name == buildingManager.GetSelectedObjectList()[i].name)// When object is already selected
                                    {
                                        isSelected = true;
                                        buildingManager.DeleteFromSelectedObjectList(i);
                                        break;
                                    }
                                }
                                if (!isSelected) // When object isn't selected
                                {
                                    // select object
                                    buildingManager.SelectObject(hit.collider.gameObject);
                                }
                            }
                        }
                        else // When click without ctrl
                        {
                            // Clear selection list
                            buildingManager.ClearSelectedObjectList();
                            if (hit.transform.gameObject.name != "Dem" && hit.transform.gameObject.name != "DemRigidbody") // When didn't click outside
                            {
                                // select object
                                buildingManager.SelectObject(hit.collider.gameObject);
                            }
                        }
                    }
                    else
                    {
                        buildingManager.ClearSelectedObjectList();
                    }
                }

                if (Vector3.Distance(clickPosition, Input.mousePosition) < 2f) // When click, not drag
                {
                    dragSelectBox.gameObject.SetActive(false);
                }
                else
                {
                    ReleaseDragSelectBox();
                }
            }

            if (Input.GetMouseButton(1)) // When right click is continue
            {
                // Update direction of camera
                mainCamera.transform.eulerAngles += rotateSpeed * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            }
        }
        else if (mode == 1)
        {
            if (Input.GetAxis("Mouse ScrollWheel") * zoomSpeed != 0) // When scroll
            {
                // Update y position of camera
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y - Input.GetAxis("Mouse ScrollWheel"), mainCamera.transform.position.z);
                CameraPositionRangeCheck();
            }

            if (Input.GetAxis("Horizontal") != 0) // When get A D กๆ ก็
            {
                // Update position of camera
                mainCamera.transform.position += mainCamera.transform.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
                CameraPositionRangeCheck();
            }

            if (Input.GetAxis("Vertical") != 0) // When get W S ก่ ก้
            {
                // Update position of camera
                mainCamera.transform.position += mainCamera.transform.up * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
                CameraPositionRangeCheck();
            }

            if (Input.GetMouseButtonDown(0)) // When left click is start
            {
                // Record position
                clickPosition = Input.mousePosition;

                // Check if user click imported object
                Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(clickPosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject == importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).gameObject)
                    {
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = selectMaterial;
                        hit.transform.gameObject.tag = "Selected";
                        offset = mainCamera.GetComponent<Camera>().WorldToScreenPoint(hit.transform.position) - Input.mousePosition;
                    }
                }
            }

            if (Input.GetMouseButton(0)) // When left click is continue
            {
                if (importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).gameObject.tag == "Selected")
                {
                    Vector3 temp = mainCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition + offset);
                    importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).transform.position = new Vector3(temp.x, 0.5f, temp.z);
                }
            }

            if (Input.GetMouseButtonUp(0)) // When left click is end
            {
                if (importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).gameObject.tag == "Selected")
                {
                    importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).gameObject.tag = "Untagged";
                    importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).gameObject.GetComponent<MeshRenderer>().material = normalMaterial;
                    importManager.DeleteLevitation(importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).gameObject);
                }
            }

            if (Input.GetKeyUp(KeyCode.Return)) // When get enter
            {
                SetMode(0);
            }
        }
    }

    // Set mode
    public void SetMode(int value)
    {
        mode = value;
    }

    // Limit the range of camera
    private void CameraPositionRangeCheck()
    {
        if (mainCamera.transform.position.y < 0)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z);
        }
        if (mainCamera.transform.position.y > 7)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, 7, mainCamera.transform.position.z);
        }
        if (mainCamera.transform.position.z < -10)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -10);
        }
        if (mainCamera.transform.position.z > 12)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 12);
        }
        if (mainCamera.transform.position.x < -13)
        {
            mainCamera.transform.position = new Vector3(-13, mainCamera.transform.position.y, mainCamera.transform.position.z);
        }
        if (mainCamera.transform.position.x > 20)
        {
            mainCamera.transform.position = new Vector3(20, mainCamera.transform.position.y, mainCamera.transform.position.z);
        }
    }

    // Update selectBox with mouse position
    private void UpdateDragSelectBox(Vector2 currentMousePosition)
    {
        if (!dragSelectBox.gameObject.activeInHierarchy)
        {
            dragSelectBox.gameObject.SetActive(true);
        }

        float width = currentMousePosition.x - clickPosition.x;
        float height = currentMousePosition.y - clickPosition.y;

        dragSelectBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        dragSelectBox.anchoredPosition = clickPosition + new Vector2(width / 2, height / 2);
    }

    // Release selectBox
    private void ReleaseDragSelectBox()
    {
        dragSelectBox.gameObject.SetActive(false);

        // calculate the range of position
        Vector2 min = dragSelectBox.anchoredPosition - (dragSelectBox.sizeDelta / 2);
        Vector2 max = dragSelectBox.anchoredPosition + (dragSelectBox.sizeDelta / 2);

        List<Vector2> pointList = new List<Vector2>();
        List<int> objectNumList = new List<int>();

        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            // Check if position of building is in the selectBox
            Vector2 screenPosition = mainCamera.GetComponent<Camera>().WorldToScreenPoint(buildings.transform.GetChild(i).position);
            if (screenPosition.x > min.x && screenPosition.x < max.x && screenPosition.y > min.y && screenPosition.y < max.y)
            {
                pointList.Add(screenPosition);
                objectNumList.Add(i);
            }
        }

        if (!Input.GetKey(KeyCode.LeftControl) & !Input.GetKey(KeyCode.RightControl)) // When get ctrl
        {
            buildingManager.ClearSelectedObjectList();
        }

        for (int i = 0; i < pointList.Count; i++)
        {
            // Check if camera can detect the building
            Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(pointList[i]);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == buildings.transform.GetChild(objectNumList[i]).gameObject.name)
                {
                    buildingManager.SelectObject(hit.collider.gameObject);
                }
            }
        }
    }
}
