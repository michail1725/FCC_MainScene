using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Dummiesman;

public class PlacingProcedure : MonoBehaviour
{
    private Camera mainCamera;
    // Start is called before the first frame update
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    public void LoadNewObjOnScene()
    {
        string filePath = @"I:\random\cylinder.obj";
        if (!File.Exists(filePath))
        {
            Debug.LogError("ObjNotFound");
            return;
        }
        var loadedObj = new OBJLoader().Load(filePath);
        if (equipObject != null)
        {
            Destroy(equipObject.gameObject);
        }

        tempObj = Instantiate(equipObject);
    }
    public void StartPlacingObject(LoadedEquipObject equipObject)
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (flyingBuilding != null)
        {
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.z);

                bool available = true;

                if (x < 0 || x > GridSize.x - flyingBuilding.Size.x) available = false;
                if (y < 0 || y > GridSize.y - flyingBuilding.Size.y) available = false;

                if (available && IsPlaceTaken(x, y)) available = false;

                flyingBuilding.transform.position = new Vector3(x, 0, y);
                flyingBuilding.SetTransparent(available);

                if (available && Input.GetMouseButtonDown(0))
                {
                    PlaceFlyingBuilding(x, y);
                }
            }
        }
    }
}
