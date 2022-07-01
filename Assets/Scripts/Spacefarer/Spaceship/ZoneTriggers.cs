using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using VSX.UniversalVehicleCombat.Space;

public class ZoneTriggers : MonoBehaviour
{
    [SerializeField]
    private EngineSoundEffectsController _engineSoundFXSlowZone;
    [SerializeField]
    private VehicleEngines3D _vehicleEnginesSlowZone;
    [SerializeField]
    private PlayerInput_InputSystem_SpaceshipControls _spaceshipControlsSlowZone;

    [SerializeField]
    private EngineSoundEffectsControllerCustom _engineSoundFXSpace;
    [SerializeField]
    private VehicleEngines3DCustom _vehicleEnginesSpace;
    [SerializeField]
    private PlayerInput_InputSystem_SpaceshipControls_Custom _spaceshipControlsSpace;

    [SerializeField]
    private bool _isInSlowZone;

    public bool IsInSlowZone { get => _isInSlowZone; set => _isInSlowZone = value; }

    // Start is called before the first frame update
    void Start()
    {
        _engineSoundFXSlowZone = GetComponentInChildren<EngineSoundEffectsController>();
        _engineSoundFXSpace = GetComponentInChildren<EngineSoundEffectsControllerCustom>();

        _vehicleEnginesSlowZone = GetComponentInChildren<VehicleEngines3D>();
        _vehicleEnginesSpace = GetComponentInChildren<VehicleEngines3DCustom>();

        _spaceshipControlsSlowZone = FindObjectOfType<PlayerInput_InputSystem_SpaceshipControls>();
        _spaceshipControlsSpace = FindObjectOfType<PlayerInput_InputSystem_SpaceshipControls_Custom>();
    }

    // Update is called once per frame
    void Update()
    {
        _engineSoundFXSlowZone.enabled = IsInSlowZone;
        _engineSoundFXSpace.enabled = !IsInSlowZone;

        _vehicleEnginesSlowZone.enabled = IsInSlowZone;
        _vehicleEnginesSpace.enabled = !IsInSlowZone;

        _spaceshipControlsSlowZone.enabled = IsInSlowZone;
        _spaceshipControlsSpace.enabled = !IsInSlowZone;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Entering {other.name}");

        IsInSlowZone = true;
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Leaving {other.name}");

        IsInSlowZone = false;
    }
}
