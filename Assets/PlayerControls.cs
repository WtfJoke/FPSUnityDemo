using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : Photon.PunBehaviour
{

    public GameObject cam;
    public LayerMask interactionLayer;
    public int maxRange;
    private Rigidbody objInHand;
    public Transform handPosition;
    public Transform spawnPosition;
    public Transform gunPosition;
    public float throwForce;
    public float slowMoFactor;
    public GameObject bombPrefab;
    public GameObject wallPrefab;
    public WallConnection previewWall;
    public WallConnection previewGround;
    public WallConnection previewRamp;


    public WallConnection previewRampDown;
    private WallConnection selectedBuildObject;
    private List<WallConnection> previews;
    public GameObject bulletPrefab;
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;


    void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.isMine)
        {
           // PlayerControls.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
       // DontDestroyOnLoad(this.gameObject);
    }


    // Use this for initialization
    void Start()
    {
        previews = new List<WallConnection>();
        previews.Add(previewWall);
        previews.Add(previewGround);
        previews.Add(previewRamp);
        previews.Add(previewRampDown);
        SetBuildObject(previewWall);


        //if (cam != null)
        //{

        //    //if (PhotonNetwork.connected == false)
        //    //{
        //    //    cam.SetActive(true);

        //    //}
        //    //else if (photonView.isMine)
        //    //{
        //    //    cam.SetActive(true);
        //    //    vThirdPersonCamera actualCam = cam.GetComponent<vThirdPersonCamera>();
        //    //    actualCam.SetTarget(transform);
        //    //}
        //}
        //else
        //{
        //    Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.isMine)
        {
            return;
        }
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
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetBuildObject(previewRampDown);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            if (selectedBuildObject.isTriggering)
            {
                selectedBuildObject.DestroyAndRemoveTrigger();
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
            else if (ReferenceEquals(selectedBuildObject, previewRampDown))
            {
                Vector3 position = GetPosition();
                GameObject placedRamp = Instantiate(previewRampDown.wallObj.gameObject, position, Quaternion.Euler(GetRampDownRotation()));
                placedRamp.GetComponent<WallConnection>().placed = true;

            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Fire();
        }


        if (ReferenceEquals(selectedBuildObject, previewRamp))
        {
            selectedBuildObject.transform.SetPositionAndRotation(GetPosition(),Quaternion.Euler(GetRampRotation()));
        }
        else if (ReferenceEquals(selectedBuildObject, previewRampDown))
        {
            selectedBuildObject.transform.SetPositionAndRotation(GetPosition(), Quaternion.Euler(GetRampDownRotation()));
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

    private Vector3 GetRampDownRotation()
    {
        return new Vector3(-135f, 0, 0) + previewRampDown.currentRotation;
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
        else if (ReferenceEquals(selectedBuildObject, previewRamp))
        {
            return GetRampPosition();
        }
        else
        {
            return GetRampDownPosition();
        }
    }

    private Vector3 GetRampPosition()
    {
        Vector3 gridPosition = GetNearestPointOnGrid(spawnPosition.position, 1f);
        Vector3 rotation = GetRampRotation();
        Vector3 position;
        if (rotation.y == 0)
        {
            position = new Vector3(gridPosition.x + 0.5f, gridPosition.y + 0.05F, gridPosition.z + 0.26f);
        }
        else if (rotation.y == 90)
        {
            position = new Vector3(gridPosition.x + 0.26f, gridPosition.y + 0.05F, gridPosition.z + 0.5f);
        }
        else if (rotation.y == 180)
        {
            position = new Vector3(gridPosition.x + 0.5f, gridPosition.y + 0.05F, gridPosition.z + 0.74f);
        }
        else
        {
            position = new Vector3(gridPosition.x + 0.74f, gridPosition.y + 0.05F, gridPosition.z + 0.5f);
        }

        return position;
    }

    private Vector3 GetRampDownPosition()
    {
        Vector3 gridPosition = GetNearestPointOnGrid(spawnPosition.position, 1f);
        Vector3 rotation = GetRampDownRotation();
        Vector3 position;
        if (rotation.y <= 2)
        {
            position = new Vector3(gridPosition.x + 0.5f, gridPosition.y - 0.75f, gridPosition.z + 0.96f);
        }
        else if (rotation.y == 90)
        {
            position = new Vector3(gridPosition.x + 0.96f, gridPosition.y - 0.75f, gridPosition.z + 0.5f);
        }
        else if (rotation.y == 180)
        {
            position = new Vector3(gridPosition.x + 0.5f, gridPosition.y - 0.75f, gridPosition.z + 0.04f);
        }
        else
        {
            position = new Vector3(gridPosition.x + 0.04f, gridPosition.y - 0.75f, gridPosition.z + 0.5f);
        }

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

        if (selectedBuildObject != null && selectedBuildObject.isTriggering)
        {
            selectedBuildObject.RemoveTrigger();
        }
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

    void Fire()
    {
        Vector3 bulletSpawn = gunPosition.position;
        var bullet = PhotonNetwork.Instantiate(bulletPrefab.name, bulletSpawn, spawnPosition.rotation, 0);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 12;
        IEnumerator destroy = DestroyBullet(bullet);
      
    }


    IEnumerator DestroyBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(5);
        GameManager.Instance.Destroy(bullet);
    }
}
