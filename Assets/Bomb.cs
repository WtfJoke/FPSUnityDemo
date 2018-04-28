using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public float lifeTime;
    public float explosionForce;
    public float explosionRadius;
    public float upModifier;
    public LayerMask bombMask;
    public LayerMask anotherBombMask;
    public GameObject explosionEffect;
    private int damage = 200;


	// Use this for initialization
	void Start () {
        Invoke("Explode", lifeTime);
	}
	
	void Explode()
    {
        List<Collider> hitColliders = new List<Collider>(Physics.OverlapSphere(transform.position, explosionRadius, bombMask));
        hitColliders.AddRange(Physics.OverlapSphere(transform.position, explosionRadius, anotherBombMask));

        foreach (Collider collider in hitColliders)
        {
            Rigidbody colliderBody = collider.GetComponent<Rigidbody>();
            Shootable shootable = collider.GetComponent<Shootable>();
            if (colliderBody != null)
            {
                colliderBody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upModifier);
            }
            if (shootable != null)
            {
                float distance =  (transform.position - collider.transform.position).sqrMagnitude;
                int damageDivider = (int)distance * 5;
                shootable.Hit(damage - damageDivider);
            }
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<Renderer>().enabled = false;
            Invoke("destroy", 2);
        }
    }
}
