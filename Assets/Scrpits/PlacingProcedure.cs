using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Dummiesman;
using System;
using System.Threading;
using UnityEngine.UI;

public class PlacingProcedure : MonoBehaviour
{
    private Camera mainCamera;
    //LoadedEquipObject equipObject;
    public TemporaryProvision temporary;
    private UnityServer unityServer;
    public Vector3 GridSize = new Vector3(250, 100,250);
    Vector3 center;
    Bounds bounds;
    float y_scaling = 0;
    public Dictionary<string, Bounds> AllBounds = new Dictionary<string,Bounds>();
    public Text Coordinates;
    private string filename;
    // Start is called before the first frame update
    private void Awake()
    {
        mainCamera = Camera.main;
        unityServer = mainCamera.GetComponent<UnityServer>();
        Coordinates.gameObject.SetActive(false);
    }

    public void RelocateObject(TemporaryProvision temporary) {
        y_scaling = 0;
        this.temporary = temporary;
    }
    public void LoadNewObjOnScene(string filename)
    {
        this.filename = filename;
        if (temporary != null)
        {
            if (temporary.IsMounted)
            {
                EquipmentObject equipmentObject;
                equipmentObject = Scene.GetObject(temporary.mountedIndex);
                temporary.gameObject.transform.position = new Vector3(equipmentObject.location.x, equipmentObject.location.y, equipmentObject.location.z);
                temporary.gameObject.transform.rotation.eulerAngles.Set(0, equipmentObject.location.yAngle, 0);
                AllBounds.Add(temporary.gameObject.name, GetBounds());
                temporary.SetNormal();
            }
            Destroy(temporary.gameObject);
        }
        string appPath = System.IO.Path.GetDirectoryName(Application.dataPath);
        appPath = appPath.Substring(0, appPath.LastIndexOf("FCC") + 3);
        //string filePath = "E:\\repos\\FCC\\Objects\\" + $"{filename}.obj", mtlPath = "E:\\repos\\FCC\\Textures\\" + $"{filename}.mtl";
        string filePath = appPath + "\\Objects\\" + $"{filename}.obj", mtlPath = appPath + "\\Textures\\" + $"{filename}.mtl";
        if (!File.Exists(filePath))
        {
            return;
        }
        else if (!File.Exists(mtlPath))
        {
            temporary = new TemporaryProvision();
            temporary.gameObject = new OBJLoader().Load(filePath);
        }
        else {
            temporary = new TemporaryProvision();
            temporary.gameObject = new OBJLoader().Load(filePath, mtlPath);
        }
        temporary.IsMounted = false;
        
        y_scaling = 0;
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
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (temporary != null)
        {
            Coordinates.gameObject.SetActive(true);
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            
            if (Input.GetKey(KeyCode.R))
            {
                temporary.gameObject.transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);
                Thread.Sleep(250);
            }
            
            if (GGrid.IsEnabled)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    y_scaling += (float)GGrid.g_size;
                    Thread.Sleep(250);
                }
                else if (Input.GetKey(KeyCode.Q))
                {
                    if (center.y + (y_scaling - (float)GGrid.g_size) > 0)
                    {
                        y_scaling -= (float)GGrid.g_size;
                    }
                    Thread.Sleep(250);
                }
            }
            else{
                if (Input.GetKey(KeyCode.E))
                {
                    y_scaling += (float)0.0015;
                    Thread.Sleep(2);
                }
                else if (Input.GetKey(KeyCode.Q))
                {
                    y_scaling -= (float)0.0015;
                    Thread.Sleep(2);
                }
            }
            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);
                float x,y,z;
                float yAngle = temporary.gameObject.transform.rotation.eulerAngles.y; ;
                if (!GGrid.IsEnabled)
                {
                    x = worldPosition.x;
                    y = 0 + y_scaling;
                    z = worldPosition.z;
                }
                else
                {
                    x = (float)((int)(worldPosition.x / GGrid.g_size) * GGrid.g_size);
                    y = 0 + y_scaling;
                    z = (float)((int)(worldPosition.z / GGrid.g_size) * GGrid.g_size);
                }
                bool available = true;
              
                temporary.gameObject.transform.position = new Vector3(x, y, z);
                Coordinates.text = ($"Координаты центра объекта: X: {Math.Round(x,2)} Y: {Math.Round(y,2)} Z: {Math.Round(z,2)}" );
                if (2* x - bounds.size.x < 0 || x > GridSize.x - bounds.size.x/2) available = false;
                if (y < 0 || y > GridSize.y) available = false;
                if (2* z - bounds.size.z < 0 || z > GridSize.z - bounds.size.z/2) available = false;
                if (available)
                {
                    foreach (Bounds bounds in AllBounds.Values)
                    {
                        if (GetBounds().Intersects(bounds))
                        {
                            available = false;
                            break;
                        }
                    }
                    temporary.SetTransparent(available, GGrid.IsEnabled);
                    if (available && Input.GetMouseButtonDown(0) || !GGrid.IsEnabled && Input.GetMouseButtonDown(0))
                    {
                        PlaceFlyingBuilding(x, y, z,yAngle);
                        Coordinates.gameObject.SetActive(false);
                    }
                }
                else {
                    temporary.SetTransparent(available, true);
                }
                if ((Input.GetMouseButtonDown(1) || Input.GetKey(KeyCode.Space)) && temporary != null) {
                    UnityServer unityServer = gameObject.GetComponent<UnityServer>();
                    if (temporary.IsMounted)
                    {
                        EquipmentObject equipmentObject;
                        equipmentObject =  Scene.GetObject(temporary.mountedIndex);
                        temporary.gameObject.transform.position = new Vector3(equipmentObject.location.x, equipmentObject.location.y, equipmentObject.location.z);
                        Quaternion quaternion = new Quaternion();
                        quaternion.eulerAngles = new Vector3(0, equipmentObject.location.yAngle, 0);
                        temporary.gameObject.transform.rotation = quaternion;
                        AllBounds.Add(temporary.gameObject.name, GetBounds());
                        temporary.SetNormal();
                    }
                    else {
                        Destroy(temporary.gameObject);
                    }
                    temporary = null;
                    Coordinates.gameObject.SetActive(false);
                    unityServer.SendMessageToClient("d");
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
            center /= temporary.gameObject.transform.childCount;
            bounds = new Bounds(center, Vector3.zero);

            foreach (Transform child in temporary.gameObject.transform)
            {
                bounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
            }
            return bounds;
    }

    private void PlaceFlyingBuilding(float x,float y,float z,float yAngle)
    {
        Location location = new Location(x, y, z, yAngle);
        EquipmentObject equipmentObject = new EquipmentObject();
        Bounds e_bds = GetBounds();
        temporary.SetNormal();
        if (!temporary.IsMounted)
        {
            equipmentObject.location = location;
            equipmentObject.filename = filename;
            Scene.AddToSceneList(equipmentObject, Scene.numerator);
            temporary.rend = temporary.gameObject.GetComponentsInChildren<Renderer>();
            temporary.gameObject.name = Scene.basic_name + Scene.numerator;
            AllBounds.Add(temporary.gameObject.name, e_bds);
            unityServer.SendMessageToClient("p " + Scene.numerator);
            Scene.IncrementNum();
            if (Scene.highest_point < e_bds.max.y)
            {
                Scene.highest_point = e_bds.max.y;
                unityServer.SendMessageToClient("h " + Math.Round(Scene.highest_point, 2));
            }
        }
        else {
            AllBounds.Add(Scene.basic_name + temporary.mountedIndex, e_bds);
            equipmentObject = Scene.GetObject(temporary.mountedIndex);
            double y_old = equipmentObject.location.y;
            equipmentObject.location = location;
            if (y_old != y)
            {
                    Scene.highest_point = 0;
                    foreach (Bounds bd in AllBounds.Values)
                    {
                        if (Scene.highest_point < bd.max.y)
                        {
                            Scene.highest_point = bd.max.y;
                        }
                    }
                    unityServer.SendMessageToClient("h " + Math.Round(Scene.highest_point, 2));
            }
        }
        if (y == 0)
        {
            if (Scene.X_max < e_bds.max.x)
            {
                Scene.X_max = e_bds.max.x;
            }
            if ((Scene.X_min > e_bds.min.x) || Scene.X_min == 0)
            {
                Scene.X_min = e_bds.min.x;
            }
            if (Scene.Z_max < e_bds.max.z)
            {
                Scene.Z_max = e_bds.max.z;
            }
            if ((Scene.Z_min > e_bds.min.z) || Scene.Z_min == 0)
            {
                Scene.Z_min = e_bds.min.z;
            }
            Thread.Sleep(50);
            unityServer.SendMessageToClient("s " + Math.Round(Scene.ReturnArea(), 2));
        }
        temporary = null;
        
    }
}
