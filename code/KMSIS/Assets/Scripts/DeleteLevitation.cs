using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteLevitation : MonoBehaviour
{
    private GameObject buildings;
    private GameObject colliders;

    void Start()
    {
        buildings = GameObject.Find("Buildings");
        colliders = GameObject.Find("DemRigidbody").transform.GetChild(0).gameObject;
        foreach(var collider in colliders.GetComponents<BoxCollider>())
        {
            collider.isTrigger = true;
        }
    }

    void Update()
    {
        for (int i = 0; i < buildings.transform.childCount; i++)
        {
            GameObject temp = buildings.transform.GetChild(i).gameObject;
            if (temp.tag == "Untagged")
            {
                temp.transform.position = new Vector3(temp.transform.position.x, temp.transform.position.y - 0.01f, temp.transform.position.z);
            }
        }
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        collider.gameObject.tag = "Building";
    }
}
