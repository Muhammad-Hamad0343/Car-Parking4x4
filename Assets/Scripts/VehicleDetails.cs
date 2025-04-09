using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Offroad Vehicle Properties",menuName = "Offroad Vehicle Properties/Vehicle Detal")]
public class VehicleDetails : ScriptableObject
{
    public string vehicleName;
    [Range(0, 1)]
    public float speed;
    [Range(0, 1)]
    public float Brake;
    [Range(0, 1)]
    public float Grip;
    [Range(0, 1)]
    public float Tyre;

    public int price;
}
