﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : Photon.PunBehaviour
{

    public int hp = 100;
    private int originalHp;
    private Material originMaterial;
    

    void Start()
    {
        originMaterial = getOriginMaterial();
        originalHp = hp;
        
    }

    [PunRPC]
    public void Hit(int damage)
    {
        hp -= damage;
        float damagePercent = (float)hp / (float)originalHp;
        if (hp <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
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
