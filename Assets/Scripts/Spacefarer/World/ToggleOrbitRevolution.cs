using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;

public class ToggleOrbitRevolution : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    SgtFloatingOrbit orbitComp;

    [SerializeField]
    private BoolVariable _orbitsActive;

    [SerializeField]
    private bool _isOrbiting;

    [SerializeField]
    private double _degreesPerSecond;

    public double DegreesPerSecond { get => _degreesPerSecond; set => _degreesPerSecond = value; }
    public SgtFloatingOrbit OrbitComp { get => orbitComp; set => orbitComp = value; }

    public bool IsOrbiting { 
        get => _isOrbiting;
        set
        {
            if (_isOrbiting != value)
            {
                ToggleOrbit(value);
            }
            _isOrbiting = value;
        }
    }

    public BoolVariable OrbitsActive { get => _orbitsActive; set => _orbitsActive = value; }

    void OnEnable()
    {
        OrbitComp = GetComponent<SgtFloatingOrbit>();
        DegreesPerSecond = OrbitComp.DegreesPerSecond;
    }

    // Update is called once per frame
    void Update()
    {
        IsOrbiting = OrbitsActive.BoolValue;
    }

    private void ToggleOrbit(bool isActive)
    {
        OrbitComp.DegreesPerSecond = isActive ? DegreesPerSecond : 0;
    }
}
