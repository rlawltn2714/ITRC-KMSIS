using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    public RectTransform dragSelectBox;
    public Material selectMaterial;
    public Material normalMaterial;

    private List<GameObject> selectedObjectList;
    private RaycastHit hit;
    private Vector2 clickPosition;
    private GameObject buildings;
    private GameObject mainCamera;
    private RayManager rayManager;
    private SunManager sunManager;
    private DataManager dataManager;

    private float clickTime;
    private float rotateLR;
    private float rotateUD;
    private float zoomSpeed = 20.0f;
    private float rotateSpeed = 5.0f;
    private float moveSpeed = 1.5f;

    void Start()
    {
        buildings = GameObject.Find("Buildings");
        mainCamera = GameObject.Find("Main Camera");
        rayManager = GameObject.Find("RayManager").GetComponent<RayManager>();
        sunManager = GameObject.Find("SunManager").GetComponent<SunManager>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        selectedObjectList = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            for (int i = 0; i < selectedObjectList.Count; i++)
            {
                List<RaycastHit> hitPointList = rayManager.GetPointOnObject(selectedObjectList[i]);
                Debug.Log(rayManager.Ratio(hitPointList, sunManager.CalculateSunVector(GameObject.Find("Directional Light").transform.eulerAngles.y, GameObject.Find("Directional Light").transform.eulerAngles.x)) + "%");
                rayManager.InstantiateObject(hitPointList);
            }
        }
        if (Input.GetKey(KeyCode.Return))
        {
            for (int i = 0; i < selectedObjectList.Count; i++)
            {
                dataManager.FindBuilding(selectedObjectList[i]);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            clickPosition = Input.mousePosition;
            clickTime = Time.time;
        }

        if (Input.GetMouseButton(0))
        {
            UpdateDragSelectBox(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - clickTime > 0.2)
            {
                // Long click
                dataManager.Save();
            }
            else
            {
                Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(clickPosition);
                
                if (Physics.Raycast(ray, out hit))
                {
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    {
                        if (hit.transform.gameObject.name != "Dem" && hit.transform.gameObject.name != "DemRigidbody")
                        {
                            bool isSelected = false;
                            for (int i = 0; i < selectedObjectList.Count; i++)
                            {
                                if (hit.collider.gameObject.name == selectedObjectList[i].name)
                                {
                                    isSelected = true;
                                    selectedObjectList.RemoveAt(i);
                                    hit.collider.gameObject.GetComponent<MeshRenderer>().material = normalMaterial;
                                    break;
                                }
                            }
                            if (!isSelected)
                            {
                                selectedObjectList.Add(hit.collider.gameObject);
                                hit.collider.gameObject.GetComponent<MeshRenderer>().material = selectMaterial;
                            }
                        }
                    }
                    else
                    {
                        ClearSelection();
                        if (hit.transform.gameObject.name != "Dem" && hit.transform.gameObject.name != "DemRigidbody")
                        {
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

            if (Vector3.Distance(clickPosition, Input.mousePosition) < 2f)
            {
                dragSelectBox.gameObject.SetActive(false);
            }
            else
            {
                ReleaseDragSelectBox();
            }
        }

        if (Input.GetMouseButton(1))
        {
            mainCamera.transform.eulerAngles += rotateSpeed * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        }

        if (Input.GetAxis("Mouse ScrollWheel") * zoomSpeed != 0)
        {
            mainCamera.GetComponent<Camera>().fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            mainCamera.transform.position += mainCamera.transform.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            CameraPositionRangeCheck();
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            mainCamera.transform.position += mainCamera.transform.forward * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            CameraPositionRangeCheck();
        }
    }

    void CameraPositionRangeCheck()
    {
        if (mainCamera.transform.position.y < 0)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z);
        }
        if (mainCamera.transform.position.y > 7)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z);
        }
        if (mainCamera.transform.position.z < -8)
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

    void ReleaseDragSelectBox()
    {
        dragSelectBox.gameObject.SetActive(false);

        Vector2 min = dragSelectBox.anchoredPosition - (dragSelectBox.sizeDelta / 2);
        Vector2 max = dragSelectBox.anchoredPosition + (dragSelectBox.sizeDelta / 2);

        List<Vector2> pointList = new List<Vector2>();
        List<int> objectNumList = new List<int>();

        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(buildings.transform.GetChild(i).position);

            if (screenPosition.x > min.x && screenPosition.x < max.x && screenPosition.y > min.y && screenPosition.y < max.y)
            {
                pointList.Add(screenPosition);
                objectNumList.Add(i);
            }
        }

        if (!Input.GetKey(KeyCode.LeftControl) & !Input.GetKey(KeyCode.RightControl))
        {
            ClearSelection();
        }

        for (int i = 0; i < pointList.Count; i++)
        {
            Ray ray = Camera.main.ScreenPointToRay(pointList[i]);

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

    void ClearSelection()
    {
        selectedObjectList.Clear();
        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            buildings.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = normalMaterial;
        }
    }

}
