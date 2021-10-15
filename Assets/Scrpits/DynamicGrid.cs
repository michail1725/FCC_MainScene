using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class DynamicGrid : MonoBehaviour
{
    public double size = 1;

    void Awake()
    {
        int len = 1000;
        double GridAmount = len / size;
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();        
        var mesh = new Mesh();
        var verticies = new List<Vector3>();

        var indicies = new List<int>();
        for (double i = 0; i < GridAmount; i+=size)
        {
            verticies.Add(new Vector3((float)i, 0, 0));
            verticies.Add(new Vector3((float)i, 0, len));

            indicies.Add(4 * (int)i + 0);
            indicies.Add(4 * (int)i + 1);

            verticies.Add(new Vector3(0, 0, (float)i));
            verticies.Add(new Vector3(len, 0, (float)i));

            indicies.Add(4 * (int)i + 2);
            indicies.Add(4 * (int)i + 3);
        }

        mesh.vertices = verticies.ToArray(); 
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Lines, 0);
        filter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = Color.gray;
    }
}
