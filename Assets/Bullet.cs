using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Photon.PunBehaviour {

    public int damage = 10;

    public void OnTriggerEnter(Collider other)
    {
        if (!photonView.isMine)
        {
            return;
        }
        Shootable shootable = other.GetComponent<Shootable>();
        PlayerControls controls = other.GetComponent<PlayerControls>();
        if (shootable == null)
        {
            shootable = other.GetComponentInChildren<Shootable>();
        }
        if (shootable != null)
        {
            shootable.Hit(damage);
        }

        if (other.tag != "Passthrough")
        {
            GameManager.Instance.Destroy(gameObject);  // if not shootable - absorb bullet
        }
        
    }
}
