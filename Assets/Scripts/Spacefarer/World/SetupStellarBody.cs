using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;
using CW.Common;
using VSX.UniversalVehicleCombat.Radar;


[
    RequireComponent(typeof(SgtFloatingObject)), 
    RequireComponent(typeof(SgtFloatingOrbit)),
    /*RequireComponent(typeof(CwRotate)),*/
    RequireComponent(typeof(Rigidbody)),
    RequireComponent(typeof(SgtGravitySource)),
    RequireComponent(typeof(Trackable))
]
public class SetupStellarBody : MonoBehaviour
{

    Trackable _trackable;

    // Start is called before the first frame update
    void Start()
    {
        _trackable = GetComponent<Trackable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
