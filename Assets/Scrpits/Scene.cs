using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene : MonoBehaviour
{
    public Dictionary<LoadedEquipObject, Location> objectsOnScene;
    public TemporaryProvision tempObj;
    public double gridSize { get; set; }

    
}
