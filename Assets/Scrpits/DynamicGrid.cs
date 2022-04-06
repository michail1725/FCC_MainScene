using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class DynamicGrid : MonoBehaviour
{
    public Text DisableGridButtonText;
    public Text GridSizeButtonText;
    void Awake()
    {
        GGrid.IsEnabled = true;
        GGrid.g_size = 1;
        DrawGrid(GGrid.g_size);
    }

    private void DrawGrid(double size) {
        int len = 250;
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
        if (GGrid.g_size == 0.1)
        {
            GGrid.g_size = 1.0;
        }
        else if (GGrid.g_size == 0.25)
        {
            GGrid.g_size = 0.1;
        }
        else {
            GGrid.g_size = GGrid.g_size / 2;
        }
        if (GGrid.g_size == 0.25) {
            GridSizeButtonText.text = GGrid.g_size.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }
        else
            GridSizeButtonText.text = GGrid.g_size.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
        DrawGrid(GGrid.g_size);
        GGrid.IsEnabled = true;
    }


    public void DisableGrid() {
        if (GGrid.IsEnabled) {
            DrawGrid(0.0);
            GGrid.IsEnabled = false;
            DisableGridButtonText.text = "Включить сетку";
        }
        else {
            DrawGrid(GGrid.g_size);
            GGrid.IsEnabled = true;
            DisableGridButtonText.text = "Отключить сетку";
        }
    }
}
