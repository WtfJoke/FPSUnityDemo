using System;
using UnityEngine;

public class Health : Photon.PunBehaviour
{
    public int playerHealth = 100;
    private int originalHealth;

    GameObject Armature;
    GameObject Mesh;
   

    private void Start()
    {
        if (photonView.isMine)
        {
            Armature = GameObject.Find("Armature");
            Mesh = GameObject.Find("Mesh_LOD");
        }
        originalHealth = playerHealth;
    }


    [PunRPC]
    public void TakeDamage(int damage)
    {
        playerHealth -= damage;

        if (playerHealth <= 0)
        {
            if (photonView.isMine)
            {
                Armature.SetActive(false);
                Mesh.SetActive(false);
                Respawn();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Respawn()
    {
        gameObject.GetComponent<PhotonView>().RPC("Activate", PhotonTargets.All);
    }

    [PunRPC]
    public void Activate()
    {
        if (photonView.isMine)
        {
            transform.position = new Vector3(0, 5f, 0); // respawn position

            Armature.SetActive(true);
            Mesh.SetActive(true);
            this.playerHealth = originalHealth;
        }
        else
        {
            gameObject.SetActive(true);
            gameObject.GetComponent<Health>().playerHealth = originalHealth;
        }
    }
}
