using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu]
public class WarpGateData : ScriptableObject
{

    public string Name;

    [Tooltip("Prefab to use for the Warp Gate")]
    public GameObject Prefab;

    [Tooltip("Orbit in UA")]
    public float Orbit;

    [Tooltip("Orbit Tilt (in degrees)")]
    public float OrbitTilt;


    [Tooltip("Scale")]
    public float Scale;

    [Tooltip("Year length in Earth years")]
    public float YearLength;

    [Tooltip("Angle of stellar body on orbit plane")]
    public float AngleOnPlane;

    [Tooltip("Does it spawn the player?")]
    public bool spawnsPlayer;
}
