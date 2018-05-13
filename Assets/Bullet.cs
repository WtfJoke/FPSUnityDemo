using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Photon.PunBehaviour {

    public int damage = 10;

    public void OnTriggerEnter(Collider other)
    {

        Shootable shootable = other.GetComponent<Shootable>();
        var health = other.GetComponent<Health>();
        if (shootable == null)
        {
            shootable = other.GetComponentInChildren<Shootable>();
        }
        if (shootable != null)
        {
            //shootable.Hit(damage);
            other.gameObject.GetComponent<PhotonView>().RPC("Hit", PhotonTargets.All, damage);
        }
        if (health != null)
        {
            other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, damage);
        }

        if (other.tag != "Passthrough")
        {
            GameManager.Instance.Destroy(gameObject);  // if not shootable - absorb bullet
        }
        
    }
}
