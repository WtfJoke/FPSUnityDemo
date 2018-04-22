using System;
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
    private GameObject selectedBuildObject;


    // Use this for initialization
    void Start()
    {
        selectedBuildObject = previewWall.gameObject;
        previewGround.gameObject.SetActive(false);
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
            previewWall.gameObject.SetActive(true);
            selectedBuildObject = previewWall.gameObject;
            previewGround.gameObject.SetActive(false);
        }else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            previewGround.gameObject.SetActive(true);
            selectedBuildObject = previewGround.gameObject;
            previewWall.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            if (ReferenceEquals(selectedBuildObject, previewWall.gameObject))
            {
                Vector3 position = GetWallPosition();
                Quaternion rotation = GetWallRotationQuaternion();
                GameObject placedWall = Instantiate(wallPrefab, position, rotation);
                placedWall.GetComponent<WallConnection>().placed = true;
            }
            else
            {
                Vector3 position = GetGroundPosition();
                
                GameObject placedGround = Instantiate(previewGround.wallObj.gameObject, position, Quaternion.identity);
                placedGround.GetComponent<WallConnection>().placed = true;
            }
        }


        selectedBuildObject.transform.SetPositionAndRotation(GetWallPosition(), GetWallRotationQuaternion());


    }

    private Vector3 GetPosition()
    {
        if (ReferenceEquals(selectedBuildObject, previewWall.gameObject))
        {
            return GetWallPosition();
        }
        else
        {
            return GetGroundPosition();
        }
    }

    private Vector3 GetGroundPosition()
    {
        Vector3 gridPosition = GetNearestPointOnGrid(spawnPosition.position, 1f);
        Vector3 position = new Vector3(gridPosition.x + 0.5f, gridPosition.y + wallPrefab.transform.position.y, gridPosition.z + 0.5f);
        return position;
    }

    private Vector3 GetWallRotation()
    {
        Vector3 rotation = GetNearestDegree(transform.rotation.eulerAngles + previewWall.currentRotation);
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
}
