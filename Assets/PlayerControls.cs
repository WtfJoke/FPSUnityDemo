using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    public Camera cam;
    public LayerMask interactionLayer;
    public int maxRange;
    private Rigidbody objInHand;
    public Transform handPosition;
    public float throwForce;
    public float slowMoFactor;
    public GameObject bombPrefab;

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
            

        }
	}
}
