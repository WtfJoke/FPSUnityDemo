﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallConnection : Photon.MonoBehaviour
{

    public Vector3 currentRotation;
    public bool placed = false;
    public Transform wallObj;
    public LayerMask interactionLayer;
    public bool isTriggering = false;
    public WallConnectionType type;
    public Material removableMaterial;
    private GameObject triggeredObject;
    private Material material;

    // Use this for initialization
    void Start()
    {
        if (!photonView.isMine)
        {
            return;
        }
        
        currentRotation.x = transform.rotation.eulerAngles.x;
        currentRotation.y = 0;
        currentRotation.z = 0;
        if (type == WallConnectionType.RAMP || type == WallConnectionType.RAMPDOWN)
        {
            currentRotation.x = 0;
        }
        material = GetComponentInChildren<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.isMine)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R) && !placed)
        {
            transform.Rotate(0, 90, 0);
            currentRotation.y = transform.rotation.eulerAngles.y;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!photonView.isMine)
        {
            return;
        }
        WallConnection connection = other.GetComponentInParent<WallConnection>();
        if (other.gameObject.layer == interactionLayer.value)
        {
            Debug.Log("hit interaction layer while building wall");
        }
        if (connection != null && connection.placed && this.type == connection.type)
        {
            AddTrigger(other.gameObject);
           
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!photonView.isMine)
        {
            return;
        }
        WallConnection connection = other.GetComponentInParent<WallConnection>();
        if (connection != null && this.type == connection.type)
        {
            RemoveTrigger();
        }
    }

    private void AddTrigger(GameObject trigger)
    {
        isTriggering = true;
        triggeredObject = trigger;
        Renderer renderer = GetComponentInChildren<Renderer>();
        renderer.material = removableMaterial;
    }


    public void RemoveTrigger()
    {
        isTriggering = false;
        triggeredObject = null;
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
       
    }

    public void DestroyAndRemoveTrigger()
    {
        Destroy(triggeredObject);
        RemoveTrigger();
    }
}   
