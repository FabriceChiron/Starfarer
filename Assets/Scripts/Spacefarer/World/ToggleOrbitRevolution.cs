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
    private double _degreesPerSecond;

    public double DegreesPerSecond { get => _degreesPerSecond; set => _degreesPerSecond = value; }
    public SgtFloatingOrbit OrbitComp { get => orbitComp; set => orbitComp = value; }

    void OnEnable()
    {
        OrbitComp = transform.GetComponent<SgtFloatingOrbit>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
