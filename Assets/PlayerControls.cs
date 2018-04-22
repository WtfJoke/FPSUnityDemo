using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

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


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
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
            if(Physics.Raycast(ray, out hit, maxRange, interactionLayer.value))
            {
                objInHand = hit.transform.GetComponent<Rigidbody>();
                objInHand.isKinematic = true;
                objInHand.transform.position = handPosition.position;
                objInHand.transform.parent = handPosition;
            }
        }else if (Input.GetKeyDown(KeyCode.Mouse0))
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
            

        }else if (Input.GetKeyDown(KeyCode.B))
        {
            Vector3 position = GetWallPosition();
            Vector3 rotation = GetWallRotation();
            Quaternion rotationQuaternion = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            GameObject placedWall = Instantiate(wallPrefab, position, rotationQuaternion);
            placedWall.GetComponent<WallConnection>().placed = true;

        }
        WallConnection con = GetComponentInChildren<WallConnection>();
        con.transform.SetPositionAndRotation(GetWallPosition(), GetWallRotationQuaternion());//Quaternion.Euler(GetNearestDegree(transform.rotation.eulerAngles)));
    }

    private Vector3 GetWallRotation()
    {
        WallConnection wallConnection = GetComponentInChildren<WallConnection>();
        Vector3 rotation = GetNearestDegree(transform.rotation.eulerAngles + wallConnection.currentRotation);
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
