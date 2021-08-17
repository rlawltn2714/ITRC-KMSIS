using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteLevitation : MonoBehaviour
{
    private bool checkCollision;

    void Start()
    {
        checkCollision = false;
    }

    void Update()
    {
        if (!checkCollision)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.01f, this.transform.position.z);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        checkCollision = true;
    }
}
