using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallConnection : MonoBehaviour
{

    public Vector3 currentRotation;
    public bool placed = false;
    public Transform wallObj;
    public LayerMask interactionLayer;

    // Use this for initialization
    void Start()
    {
        currentRotation.x = transform.rotation.eulerAngles.x;
        currentRotation.y = 0;
        currentRotation.z = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !placed)
        {
            transform.Rotate(0, 90, 0);
            currentRotation.y = transform.rotation.eulerAngles.y;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == interactionLayer.value)
        {
            Debug.Log("hit interaction layer while building wall");
        }
    }
}   
