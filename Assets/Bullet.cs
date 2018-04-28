using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int damage = 10;

    public void OnTriggerEnter(Collider other)
    {
        Shootable shootable = other.GetComponent<Shootable>();
        if (shootable != null)
        {
            shootable.Hit(damage);
        }
        Destroy(gameObject); // if not shootable - absorb bullet
    }
}
