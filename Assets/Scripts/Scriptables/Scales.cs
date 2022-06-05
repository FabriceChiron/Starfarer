using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu]
public class Scales : ScriptableObject
{
    [Header("1 UA, in Kilometers")]
    public float Orbit = 1f;

    [Header("Rationalize values for very long distances")]
    public bool RationalizeValues = true;
    
    [Header("Size of Earth, in kilometers")]
    public float Planet = 1f;
    
    [Header("Length of 1 Earth Year, in minutes")]
    public float Year = 30f;
    
    [Header("Length of 1 Earth Day, in minutes")]
    public float Day = 10f;
    
}
