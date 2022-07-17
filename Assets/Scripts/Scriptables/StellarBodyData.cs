using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu]
public class StellarBodyData : ScriptableObject
{

    public string Name;

    [Tooltip("Description of the stellar body")]
    public string Details;

    [Tooltip("Prefab to use for the stellar body")]
    public GameObject Prefab;

    [Tooltip("Is the prefab already setup")]
    public bool ReadyMadePrefab;

    [Header("Is the stellar body gaseous?")]
    public bool Gaseous;

    ///<summary>Texture file (cubemap) for Gaseous planets
    ///First one: texture, Second one: flow
    /// </summary>
    [ShowIf("Gaseous"), DisableIf("ReadyMadePrefab"), Header("Gaseous planets: 0 - texture, 1 - flow")]
    public Texture[] Texture;

    //Material file for planet
    [HideIf("Gaseous"), DisableIf("ReadyMadePrefab"), Header("Material for rocky planets")]
    public Material Material;

    [Tooltip("Does the stellar body have clouds?"), DisableIf("ReadyMadePrefab")]
    public bool Clouds;

    [Tooltip("Texture file for clouds (if any)"), EnableIf("Clouds")]
    public Material CloudsMaterial;

    [Tooltip("Is the stellar body always showing the same face to its parent?")]
    public bool TidallyLocked;

    [Tooltip("Orbit in UA")]
    public float Orbit;

    [Tooltip("Orbit Tilt (in degrees)")]
    public float OrbitTilt;


    [Tooltip("Size (relative to Earth)")]
    public float Size;

    [Tooltip("Mass (* 10^24 kg)")]
    public float Mass;

    [Tooltip("Stellar body Tilt (in degrees)")]
    public float BodyTilt;

    [Tooltip("Year length in Earth years")]
    public float YearLength;

    [Tooltip("Day length in Earth days"), DisableIf("TidallyLocked")]
    public float DayLength;

    [Tooltip("Angle of stellar body on orbit plane")]
    public float AngleOnPlane;

    [Tooltip("Does the stellar body have rings?")]
    public bool Rings;

    [EnableIf("Rings")]
    public GameObject RingsPrefab;

    [Tooltip("If not specified, will rotate around the stellar system's center")]
    public StarData OrbitsAround;

    public StellarBodyData[] ChildrenItem;

    public AsteroidBeltData[] AsteroidBeltItem;

    public WarpGateData warpGate;
}
