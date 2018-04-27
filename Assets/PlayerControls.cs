﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    public Camera cam;
    public LayerMask interactionLayer;
    public int maxRange;
    private Rigidbody objInHand;
    public Transform handPosition;
    public Transform spawnPosition;
    public float throwForce;
    public float slowMoFactor;
    public GameObject bombPrefab;
    public GameObject wallPrefab;
    public WallConnection previewWall;
    public WallConnection previewGround;
    public WallConnection previewRamp;
    private WallConnection selectedBuildObject;
    private List<WallConnection> previews;


    // Use this for initialization
    void Start()
    {
        previews = new List<WallConnection>();
        previews.Add(previewWall);
        previews.Add(previewGround);
        previews.Add(previewRamp);
        SetBuildObject(previewWall);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = slowMoFactor;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (objInHand != null)
            {
                return;
            }
            RaycastHit hit;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            Debug.DrawLine(ray.origin, ray.GetPoint(maxRange));
            if (Physics.Raycast(ray, out hit, maxRange, interactionLayer.value))
            {
                objInHand = hit.transform.GetComponent<Rigidbody>();
                objInHand.isKinematic = true;
                objInHand.transform.position = handPosition.position;
                objInHand.transform.parent = handPosition;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (objInHand == null)
            {
                // throw bomb
                GameObject bomb = GameObject.Instantiate(bombPrefab, handPosition.position, Quaternion.identity);
                objInHand = bomb.GetComponent<Rigidbody>();
            }

            objInHand.isKinematic = false;
            objInHand.transform.parent = null;
            objInHand.AddForce(cam.transform.forward * throwForce);
            objInHand = null;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetBuildObject(previewWall);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetBuildObject(previewGround);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetBuildObject(previewRamp);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            if (selectedBuildObject.isTriggering)
            {
                Destroy(selectedBuildObject.triggeredObject);
                selectedBuildObject.isTriggering = false;
                selectedBuildObject.triggeredObject = null;
                return;
            }
            if (ReferenceEquals(selectedBuildObject, previewWall))
            {
                Vector3 position = GetWallPosition();
                Quaternion rotation = GetWallRotationQuaternion();
                GameObject placedWall = Instantiate(wallPrefab, position, rotation);
                placedWall.GetComponent<WallConnection>().placed = true;
            }
            else if (ReferenceEquals(selectedBuildObject, previewGround))
            {
                Vector3 position = GetGroundPosition();

                GameObject placedGround = Instantiate(previewGround.wallObj.gameObject, position, Quaternion.identity);
                placedGround.GetComponent<WallConnection>().placed = true;
            }
            else if (ReferenceEquals(selectedBuildObject, previewRamp))
            {
                Vector3 position = GetRampPosition();

                GameObject placedRamp = Instantiate(previewRamp.wallObj.gameObject, position, Quaternion.Euler(GetRampRotation()));
                placedRamp.GetComponent<WallConnection>().placed = true;
            }
        }


        if (ReferenceEquals(selectedBuildObject, previewRamp))
        {
            selectedBuildObject.transform.SetPositionAndRotation(GetPosition(), Quaternion.Euler(GetRampRotation()));
        }
        else
        {
            selectedBuildObject.transform.SetPositionAndRotation(GetWallPosition(), GetWallRotationQuaternion());
        }
        


    }

    private Vector3 GetRampRotation()
    {
        return GetNearestPointOnGrid(previewRamp.currentRotation, 45f);
    }

    private Vector3 GetPosition()
    {
        if (ReferenceEquals(selectedBuildObject, previewWall))
        {
            return GetWallPosition();
        }
        else if (ReferenceEquals(selectedBuildObject, previewGround))
        {
            return GetGroundPosition();
        }
        else
        {
            return GetRampPosition();
        }
    }

    private Vector3 GetRampPosition()
    {
        Vector3 gridPosition = GetNearestPointOnGrid(spawnPosition.position, 1f);
        Vector3 position = new Vector3(gridPosition.x + 0.5f, gridPosition.y + 0.05F, gridPosition.z + 0.26f);
        return position;
    }

    private Vector3 GetGroundPosition()
    {
        Vector3 gridPosition = GetNearestPointOnGrid(spawnPosition.position, 1f);
        Vector3 position = new Vector3(gridPosition.x + 0.5f, gridPosition.y + wallPrefab.transform.position.y, gridPosition.z + 0.5f);
        return position;
    }

    private Vector3 GetWallRotation()
    {
        Vector3 rotation = GetNearestDegree(previewWall.currentRotation);
        return rotation;
    }

    private Quaternion GetWallRotationQuaternion()
    {
        return Quaternion.Euler(GetWallRotation());
    }

    private Vector3 GetWallPosition()
    {
        Vector3 gridPosition = GetNearestPointOnGrid(spawnPosition.position, 1f);
        Vector3 position = new Vector3(gridPosition.x + 0.5f, gridPosition.y + wallPrefab.transform.position.y, gridPosition.z + 0.5f);
        return position;
    }

    public Vector3 GetNearestPointOnGrid(Vector3 currentPosition, float size)
    {
        //currentPosition -= transform.position;

        int xCount = Mathf.RoundToInt(currentPosition.x / size);
        int yCount = Mathf.RoundToInt(currentPosition.y / size);
        int zCount = Mathf.RoundToInt(currentPosition.z / size);

        Vector3 result = new Vector3((float)xCount * size, (float)yCount * size, (float)zCount * size);

        //result += transform.position;
        return result;
    }

    public Vector3 GetNearestDegree(Vector3 rotation)
    {
        return GetNearestPointOnGrid(rotation, 90f);
    }

    private void SetBuildObject(WallConnection objectToBuild)
    {
        foreach (var preview in previews)
        {
            if (preview == objectToBuild)
            {
                selectedBuildObject = objectToBuild;
                selectedBuildObject.gameObject.SetActive(true);
            }
            else
            {
                preview.gameObject.SetActive(false);
            }
        }
    }
}
