using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : MonoBehaviour {

    [SerializeField] private GameObject playerCamera;
    [SerializeField] private MonoBehaviour[] playerControlScripts;

    private PhotonView photonView;

	// Use this for initialization
	void Start () {
		photonView = GetComponent<PhotonView>();
        Initialized();
	}


    private void Initialized()
    {
        if (photonView.isMine)
        {

        }
        else
        {
            playerCamera.SetActive(false);

            foreach(MonoBehaviour m in playerControlScripts)
            {
                m.enabled = false;
            }
        }
    }
}
