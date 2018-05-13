using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    public float lifeTime;
    public float explosionForce;
    public float explosionRadius;
    public float upModifier;
    public LayerMask bombMask;
    public LayerMask anotherBombMask;
    public GameObject explosionEffect;
    private int damage = 200;


    // Use this for initialization
    void Start()
    {
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
            var health = collider.GetComponent<Health>();

            if (colliderBody != null)
            {
                colliderBody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upModifier);
            }
            if (shootable != null || health != null)
            {

                float distance = (transform.position - collider.transform.position).sqrMagnitude;
                int damageReducer = (int)distance * 5;
                int newDamage = damageReducer > damage ? damage : damage - damageReducer;

                if (collider.gameObject.tag == "Player")
                {
                    collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, newDamage);
                }
                else
                {
                    collider.gameObject.GetComponent<PhotonView>().RPC("Hit", PhotonTargets.All, newDamage);
                }

            }
            Destroy(Instantiate(explosionEffect, transform.position, Quaternion.identity), 4f);
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<Renderer>().enabled = false;
            IEnumerator destroy = DestroyBomb(gameObject);
            StartCoroutine(destroy);
            
        }
    }


    IEnumerator DestroyBomb(GameObject bomb)
    {
        yield return new WaitForSeconds(2);
        GameManager.Instance.Destroy(bomb);
    }
}
