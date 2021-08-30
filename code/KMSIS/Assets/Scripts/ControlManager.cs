using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    // This class manages what the user controls.

    // Manager component
    private RayManager rayManager;
    private SunManager sunManager;
    private DataManager dataManager;

    // SelectBox component and list of selected objects
    public RectTransform dragSelectBox;
    private List<GameObject> selectedObjectList;

    // Material component
    public Material selectMaterial;
    public Material normalMaterial;
    
    // RaycastHit variable
    private RaycastHit hit;

    // GameObject component
    private GameObject buildings;
    private GameObject importedBuildings;
    private GameObject mainCamera;

    // Local variable and setting
    private Vector2 clickPosition;
    private float clickTime;
    private float rotateLR;
    private float rotateUD;
    private float zoomSpeed = 20.0f;
    private float rotateSpeed = 5.0f;
    private float moveSpeed = 1.5f;

    void Start()
    {
        // Get GameObject component
        buildings = GameObject.Find("Buildings");
        importedBuildings = GameObject.Find("ImportedBuildings");
        mainCamera = GameObject.Find("Main Camera");
        rayManager = GameObject.Find("RayManager").GetComponent<RayManager>();
        sunManager = GameObject.Find("SunManager").GetComponent<SunManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        
        // Initialize variable
        selectedObjectList = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) // When get spacebar
        {
            // Instantiate plane object using rayManager, and calculate R(x) using sunManager
            for (int i = 0; i < selectedObjectList.Count; i++)
            {
                List<RaycastHit> hitPointList = rayManager.GetPointOnObject(selectedObjectList[i]);
                Debug.Log(rayManager.Ratio(hitPointList, sunManager.CalculateSunVector(GameObject.Find("Directional Light").transform.eulerAngles.y, GameObject.Find("Directional Light").transform.eulerAngles.x)) + "%");
                rayManager.InstantiateObject(hitPointList);
            }
        }
        if (Input.GetKey(KeyCode.Return)) // When get enter
        {
            // Find data of selected buildings
            for (int i = 0; i < selectedObjectList.Count; i++)
            {
                dataManager.FindBuilding(selectedObjectList[i]);
            }
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
                // Long click
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
                            for (int i = 0; i < selectedObjectList.Count; i++)
                            {
                                if (hit.collider.gameObject.name == selectedObjectList[i].name)// When object is already selected
                                {
                                    isSelected = true;
                                    selectedObjectList.RemoveAt(i);
                                    hit.collider.gameObject.GetComponent<MeshRenderer>().material = normalMaterial;
                                    break;
                                }
                            }
                            if (!isSelected) // When object isn't selected
                            {
                                // select object
                                selectedObjectList.Add(hit.collider.gameObject);
                                hit.collider.gameObject.GetComponent<MeshRenderer>().material = selectMaterial;
                            }
                        }
                    }
                    else // When click without ctrl
                    {
                        // Clear selection list
                        ClearSelection();
                        if (hit.transform.gameObject.name != "Dem" && hit.transform.gameObject.name != "DemRigidbody") // When didn't click outside
                        {
                            // select object
                            selectedObjectList.Add(hit.collider.gameObject);
                            hit.collider.gameObject.GetComponent<MeshRenderer>().material = selectMaterial;
                        }
                    }
                }
                else
                {
                    ClearSelection();
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
    }

    // Limit the range of camera
    void CameraPositionRangeCheck()
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
    void UpdateDragSelectBox(Vector2 currentMousePosition)
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
    void ReleaseDragSelectBox()
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
            ClearSelection();
        }

        for (int i = 0; i < pointList.Count; i++)
        {
            // Check if camera can detect the building
            Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(pointList[i]);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == buildings.transform.GetChild(objectNumList[i]).gameObject.name)
                {
                    selectedObjectList.Add(hit.collider.gameObject);
                    hit.collider.gameObject.GetComponent<MeshRenderer>().material = selectMaterial;
                }
            }
        }
    }

    // Clear selectBox
    void ClearSelection()
    {
        selectedObjectList.Clear();
        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            buildings.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = normalMaterial;
        }
        for (int i=0;i< importedBuildings.transform.childCount; i++)
        {
            importedBuildings.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = normalMaterial;
        }
    }

}
