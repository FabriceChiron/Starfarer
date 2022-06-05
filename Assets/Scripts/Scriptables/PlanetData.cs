using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlanetData : ScriptableObject
{
    public string Name;

    //Descirption of the planet
    public string Details;

    public GameObject Prefab;

    public bool Gaseous;

    ///<summary>Texture file (cubemap) for Gaseous planets
    ///First one: texture, Second one: flow
    /// </summary>
    [Header("Gaseous planets: 0 - texture, 1 - flow")]
    public Texture[] Texture;

    //Material file for planet
    [Header("Material for rocky planets")]
    public Material Material;

    //Texture file for clouds (if any)
    public bool Clouds;

    //Texture file for clouds (if any)
    public Material CloudsMaterial;

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

    [Tooltip("Day length in Earth days")]
    public float DayLength;

    [Tooltip("Coordinates of stellar body on orbit plane (e.g \"nw\")")]
    public string Coords;

    [Tooltip("Does the stellar body have rings?")]
    public bool Rings;

    public GameObject RingsPrefab;

    public PlanetData[] ChildrenItem;
}
