using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialHolder : MonoBehaviour {

    public static MaterialHolder instance = null;
    public Material ThreeQuarterHealthMaterial;
    public Material HalfHealthMaterial;
    public Material QuarterHealthMaterial;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
	}
}
