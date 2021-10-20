using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Dummiesman;
using System;

public class PlacingProcedure : MonoBehaviour
{
    private Camera mainCamera;
    //LoadedEquipObject equipObject;
    TemporaryProvision temporary;
    public Vector3 GridSize = new Vector3(1000, 100,1000);
    Vector3 center;
    Bounds bounds;
    //uint y_scaling = 0;
    List<Bounds> AllBounds = new List<Bounds>();
    // Start is called before the first frame update
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    public void LoadNewObjOnScene()
    {
        if (temporary != null)
        {
            Destroy(temporary.gameObject);
        }
        string filePath = @"E:\Koltuk.obj";
        if (!File.Exists(filePath))
        {
            Debug.LogError("ObjNotFound");
            return;
        }
        temporary = new TemporaryProvision();
        temporary.gameObject = new OBJLoader().Load(filePath);
        center = new Vector3();
        center = Vector3.zero;

        foreach (Transform child in temporary.gameObject.transform)
        {
            center += child.gameObject.GetComponent<Renderer>().bounds.center;
        }
        center /= temporary.gameObject.transform.childCount; //center is average center of children
        

        //Now you have a center, calculate the bounds by creating a zero sized 'Bounds', 
        bounds = new Bounds(center, Vector3.zero);

        foreach (Transform child in temporary.gameObject.transform)
        {
            bounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
        }
        temporary.rend = temporary.gameObject.GetComponentsInChildren<Renderer>();
    }

    
    // Update is called once per frame
    void Update()
    {
        if (temporary != null)
        {
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetKey(KeyCode.R))
            {
                temporary.gameObject.transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);
            }
            //if (Input.GetKey(KeyCode.E))
            //{
            //    y_scaling += 1;
            //}
            //else if (Input.GetKey(KeyCode.Q)) {
            //    y_scaling -= 1;
            //}
            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);
                float x,y,z;
                if (!GGrid.IsEnabled)
                {
                    x = worldPosition.x;
                    y = center.y;
                    z = worldPosition.z;
                }
                else
                {
                    x = (float)((int)(worldPosition.x / GGrid.g_size) * GGrid.g_size);
                    y = center.y;
                    z = (float)((int)(worldPosition.z / GGrid.g_size) * GGrid.g_size);
                }
                bool available = true;
                
                temporary.gameObject.transform.position = new Vector3(x, y, z);

                if (2* x - bounds.size.x < 0 || x > GridSize.x - bounds.size.x/2) available = false;
                //if (y - bounds.size.y < 0 || y > GridSize.y - bounds.size.y) available = false;
                if (2* z - bounds.size.z < 0 || z > GridSize.z - bounds.size.z/2) available = false;
                if (available)
                {
                    foreach (Bounds bounds in AllBounds)
                    {
                        if (GetBounds().Intersects(bounds))
                        {
                            available = false;
                            break;
                        }
                    }
                }
                temporary.SetTransparent(available);
                if (available && Input.GetMouseButtonDown(0))
                {
                    PlaceFlyingBuilding(x, y,z);
                }
            }
        }
    }

    private Bounds GetBounds() {
        Vector3 center = Vector3.zero;

        foreach (Transform child in temporary.gameObject.transform)
        {
            center += child.gameObject.GetComponent<Renderer>().bounds.center;
        }
        center /= temporary.gameObject.transform.childCount; //center is average center of children

        //Now you have a center, calculate the bounds by creating a zero sized 'Bounds', 
        bounds = new Bounds(center, Vector3.zero);

        foreach (Transform child in temporary.gameObject.transform)
        {
            bounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
        }
        return bounds;
    }

    private void PlaceFlyingBuilding(float x,float y,float z)
    {
        AllBounds.Add(GetBounds());
        temporary.rend = temporary.gameObject.GetComponentsInChildren<Renderer>();
        temporary.SetNormal();
        
        temporary = null;
    }
}
