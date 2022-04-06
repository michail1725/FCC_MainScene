using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Scene
{
    public static int numerator = 0;
    public static string basic_name = "equip";
    public static double total_area_value = 0;
    public static double highest_point = 0;
    private static GameObject gm;
    private static PlacingProcedure sc;
    private static Dictionary<int, EquipmentObject> EquipSceneList = new Dictionary<int, EquipmentObject>();
    public static double X_max = 0;
    public static double X_min = 0;
    public static double Z_max = 0;
    public static double Z_min = 0;
    public static void DeleteFromSceneList(int index)
    {
        gm = GameObject.Find("Plane");
        sc = gm.GetComponent<PlacingProcedure>();
        EquipSceneList.Remove(index);
        Bounds bounds;
        sc.AllBounds.TryGetValue(basic_name + index, out bounds);
        sc.AllBounds.Remove(basic_name + index);
        if (highest_point == bounds.max.y) {
            highest_point = 0;
            foreach (Bounds bd in sc.AllBounds.Values) {
                if (highest_point < bd.max.y) {
                    highest_point = bd.max.y;
                }
            }
        }
        if (X_max == bounds.max.x)
        {
            X_max = 0;
            foreach (Bounds bd in sc.AllBounds.Values) {
                if (X_max < bd.max.x) {
                    X_max = bd.max.x;
                }
            }
        }
        if (X_min == bounds.min.x)
        {
            X_min = 0;
            foreach (Bounds bd in sc.AllBounds.Values)
            {
                if ((X_min > bd.min.x) || X_min == 0)
                {
                    X_min = bd.min.x;
                }
            }
        }
        if (Z_max == bounds.max.z)
        {
            Z_max = 0;
            foreach (Bounds bd in sc.AllBounds.Values)
            {
                if (Z_max < bd.max.z)
                {
                    Z_max = bd.max.z;
                }
            }
        }
        if (Z_min == bounds.min.z)
        {
            Z_min = 0;
            foreach (Bounds bd in sc.AllBounds.Values)
            {
                if ((Z_min > bd.min.z) || Z_min ==0)
                {
                    Z_min = bd.min.z;
                }
            }
        }
    }


    public static void RelocateObject(int index)
    {
        gm = GameObject.Find("Plane");
        sc = gm.GetComponent<PlacingProcedure>();
        sc.AllBounds.Remove(basic_name + index);
    }

    public static void AddToSceneList(EquipmentObject equipmentObject,int index)
    {
        EquipSceneList.Add(index,equipmentObject);
    }

    public static double ReturnArea() 
    {
        total_area_value = (X_max - X_min)*(Z_max-Z_min);
        return total_area_value;
    }

    public static void IncrementNum() 
    {
        numerator += 1;   
    }

    public static EquipmentObject GetObject(int index) {
        EquipmentObject eo;
        EquipSceneList.TryGetValue(index, out eo);
        return eo;
    }
}
