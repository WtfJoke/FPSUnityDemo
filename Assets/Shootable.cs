﻿using UnityEngine;

public class Shootable : Photon.PunBehaviour
{

    public int hp = 100;
    public GameObject belongsTo;
    public bool isPlayer = false;
    private int originalHp;
    private Material originMaterial;

    void Start()
    {
        originMaterial = getOriginMaterial();
        originalHp = hp;

    }

    public void Hit(int damage)
    {
        hp -= damage;
        if (isPlayer)
        {
            var view = GetComponent<PhotonView>();
            Debug.LogWarning("isMine " + view.isMine);
            if (hp <= 0)
            {
                transform.position = new Vector3(0, 5f, 0);
                hp = originalHp;
            }
            return;
        }
        float damagePercent = (float)hp / (float)originalHp;
        if (hp <= 0)
        {
            Destroy(belongsTo);
            Destroy(gameObject);
        }
        else if (damagePercent <= 0.25)
        {
            getRenderer().material = MaterialHolder.instance.QuarterHealthMaterial;
        }
        else if (damagePercent <= 0.5)
        {
            getRenderer().material = MaterialHolder.instance.HalfHealthMaterial;
        }
        else if (damagePercent <= 0.75)
        {
            getRenderer().material = MaterialHolder.instance.ThreeQuarterHealthMaterial;
        }
        else if (damagePercent <= 0.95)
        {
            getRenderer().material = MaterialHolder.instance.OneHitMaterial;
        }

    }


    private Material getOriginMaterial()
    {

        return getRenderer().material;

    }

    private Renderer getRenderer()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }
        return renderer;
    }
}