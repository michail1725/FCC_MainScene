using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryProvision
{
    public GameObject gameObject;
    public Renderer[] rend;
    public bool IsMounted;
    public int mountedIndex;
    public void SetTransparent(bool available,bool by_grid)
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
            if (by_grid)
            {
                foreach (Renderer r in rend)
                {
                    r.material.color = Color.red;
                }
            }
            else {
                foreach (Renderer r in rend)
                {
                    r.material.color = Color.yellow;
                }
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

    public void SetSelected() {
        foreach (Renderer r in rend)
        {
            r.material.color = Color.blue;
        }
    }
}
