using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public float lifeTime;
    public float explosionForce;
    public float explosionRadius;
    public float upModifier;
    public LayerMask bombMask;
    public GameObject explosionEffect;


	// Use this for initialization
	void Start () {
        Invoke("Explode", lifeTime);
	}
	
	void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, bombMask);
        foreach (Collider collider in hitColliders)
        {
            Rigidbody colliderBody = collider.GetComponent<Rigidbody>();
            if (colliderBody != null)
            {
                colliderBody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upModifier);
            }
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<Renderer>().enabled = false;
            Invoke("destroy", 2);
        }
    }
}
