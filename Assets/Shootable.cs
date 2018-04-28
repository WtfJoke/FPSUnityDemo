using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{

    public int hp = 100;
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
        float damagePercent = (float)hp / (float)originalHp;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }else if (damagePercent <= 0.25)
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
