using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryProvision
{
    public GameObject gameObject;
    public Renderer[] rend;
    
    public void SetTransparent(bool available)
    {
        if (available)
        {
            foreach (Renderer r in rend)
            {
                r.material.color = Color.green;
            }
        }
        else
        {
            foreach (Renderer r in rend)
            {
                r.material.color = Color.red;
            }
        }
    }
    public void SetNormal()
    {
        foreach (Renderer r in rend)
        {
            r.material.color = Color.white;
        }
    }
}
