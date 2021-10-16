using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryProvision : MonoBehaviour
{
    private new Renderer renderer;
    public TemporaryProvision(Renderer MainRenderer)
    {
        this.renderer = MainRenderer;
    }
    public void SetTransparent(bool available)
    {
        if (available)
        {
            renderer.material.color = Color.green;
        }
        else
        {
            renderer.material.color = Color.red;
        }
    }

    public void SetNormal()
    {
        renderer.material.color = Color.white;
    }
}
