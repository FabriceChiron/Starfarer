using UnityEngine;
using NaughtyAttributes;

public enum AsteroidType
{
    Rocky,
    Icy,
    Mixed
}

[CreateAssetMenu]
public class AsteroidBeltData : ScriptableObject
{
    public int AsteroidsWithPlatinum, AsteroidsWithTurrets;

    public AsteroidType asteroidType;

    [Tooltip("Inner Orbit in AU")]
    public float OrbitMin;

    [Tooltip("Outer Orbit in AU")]
    public float OrbitMax;

    [Tooltip("Asteroid belt height, relative to Earth ")]
    public float Height;

    public int Density;

    public float asteroidMinSize, asteroidMaxSize;
}
