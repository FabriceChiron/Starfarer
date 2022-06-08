using UnityEngine;

[CreateAssetMenu]
public class StellarSystemData : ScriptableObject
{
    public string Name;

    public float StarSize;

    public StarData[] StarsItem;

    public float ScaleFactor = 1f;

    public float Top;

    public float Left;

    public WarpGateData warpGate;

    public StellarBodyData[] ChildrenItem;

    public AsteroidBeltData[] AsteroidBeltItem;

    [Header("Objectives")]
    public int Platinum;
    public int Turrets;
}
