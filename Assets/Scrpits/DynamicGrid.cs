using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class DynamicGrid : MonoBehaviour
{
    public double size = 1;
    bool IsGridEnabled;
    public Text DisableGridButtonText;
    public Text GridSizeButtonText;
    void Awake()
    {
        IsGridEnabled = true;
        DrawGrid(size);
    }

    private void DrawGrid(double size) {
        int len = 1000;
        double GridAmount;
        if (size > 0.0)
        {
            GridAmount = len / size;
        }
        else {
            GridAmount = 0.0;
        } 
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        var mesh = new Mesh();
        var verticies = new List<Vector3>();

        var indicies = new List<int>();
        for (double i = 0; i < GridAmount; i += size)
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

    public void ChangeGridSize() {
        if (size == 0.1)
        {
            size = 1.0;
        }
        else if (size == 0.25)
        {
            size = 0.1;
        }
        else {
            size = size / 2;
        }
        if (size == 0.25) {
            GridSizeButtonText.text = size.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }
        else
            GridSizeButtonText.text = size.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
        DrawGrid(size);
        IsGridEnabled = true;
    }


    public void DisableGrid() {
        if (IsGridEnabled) {
            DrawGrid(0.0);
            IsGridEnabled = false;
            DisableGridButtonText.text = "Включить сетку";
        }
        else {
            DrawGrid(size);
            IsGridEnabled = true;
            DisableGridButtonText.text = "Отключить сетку";
        }
    }
}
