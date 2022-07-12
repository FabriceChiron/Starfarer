using UnityEngine;

public enum AsteroidType
{
    Rocky,
    Icy
}

[CreateAssetMenu]
public class AsteroidBeltData : ScriptableObject
{
    public int Quantity;

    public int AsteroidsWithPlatinum, AsteroidsWithTurrets;

    public AsteroidType asteroidType;

    public float Orbit;

    public float OrbitMin;
    public float OrbitMax;

    public float HeightMin;
    public float HeightMax;

    public float YearLengthMin = 1f;
    public float YearLengthMax = 1f;
}
