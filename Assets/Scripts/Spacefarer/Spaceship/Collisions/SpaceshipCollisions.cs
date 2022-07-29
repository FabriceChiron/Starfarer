using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CW.Common;
using SpaceGraphicsToolkit;

public class SpaceshipCollisions : MonoBehaviour
{
    [SerializeField]
    private bool _isInSlowZone, _isInSlowZoneBuffer, _isInAsteroidBelt, _isInAtmosphere, _enableOrbiting;

    [SerializeField]
    private BoolVariable _orbitsActive;

    [SerializeField]
    private SpaceshipController _spaceshipController;

    [SerializeField]
    private SpaceshipWarp _spaceshipWarp;

    [SerializeField]
    private AsteroidFieldInfo _asteroidFieldInfo;
    

    public bool IsInSlowZone { 
        get => _isInSlowZone;
        set {
            _isInSlowZone = value;

            //Change value in _spaceshipController;
            _spaceshipController.IsInSlowZone = _isInSlowZone;
        }
    }
    public bool IsInSlowZoneBuffer { 
        get => _isInSlowZoneBuffer;
        set {
            _isInSlowZoneBuffer = value;

            //Change value in _spaceshipController;
            _spaceshipController.IsInSlowZoneBuffer = _isInSlowZoneBuffer;
        }  
    }
    public bool EnableOrbiting { 
        get => _enableOrbiting;
        set {

            //When value changes
            if(_enableOrbiting != value)
            {
                _orbitsActive.BoolValue = value;

                foreach (SgtGravitySource gravitySource in FindObjectsOfType<SgtGravitySource>())
                {
                    gravitySource.enabled = value;
                }
            }

            _enableOrbiting = value;
        }
    }

    public bool IsInAsteroidBelt { 
        get => _isInAsteroidBelt; 
        set
        {
            _isInAsteroidBelt = value;

            _asteroidFieldInfo.GetComponent<Text>().enabled = value;

            _spaceshipController.IsInAsteroidField = value;

            //foreach(SgtFloatingSpawnerRing spawnerRing in FindObjectsOfType<SgtFloatingSpawnerRing>())
            //{
            //    Debug.Log($"{spawnerRing.name} activated: {value}");
            //    spawnerRing.enabled = value;
            //}
        }
    }

    public bool IsInAtmosphere { get => _isInAtmosphere; set => _isInAtmosphere = value; }

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _spaceshipController = GetComponent<SpaceshipController>();
        _spaceshipWarp = GetComponent<SpaceshipWarp>();
        _asteroidFieldInfo = FindObjectOfType<AsteroidFieldInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        EnableOrbiting = !(IsInSlowZone || IsInAsteroidBelt || IsInSlowZoneBuffer || _spaceshipWarp.Warping || _spaceshipWarp.WarpInitiated);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SlowZoneBuffer"))
        {
            IsInSlowZoneBuffer = true;

            //Debug.Log($"Entered slow zone buffer - IsInSlowZoneBuffer {IsInSlowZoneBuffer}");
            _spaceshipController.WarpGateCenter = other.gameObject.GetComponentInParent<WarpGateCenter>();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("SlowZone"))
        {
            IsInSlowZone = true;
            //Debug.Log($"Entered slow zone - IsInSlowZone {IsInSlowZone}");
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Atmosphere"))
        {
            IsInAtmosphere = true;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("NullifyWarp"))
        {
            //Debug.Log($"{other.transform.GetComponentInParent<SgtFloatingObject>()} - NullifyWarp");
            _spaceshipWarp.NoWarpSphere = other.transform.GetComponentInParent<SgtFloatingTarget>();
            if (_spaceshipWarp.Warping)
            {
                _spaceshipWarp.AbortWarp();
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("AsteroidBelt"))
        {
            IsInAsteroidBelt = true;
            //_debrisGrid.gameObject.SetActive(true);
            Debug.Log($"AsteroidBelt: {other.gameObject.GetComponentInParent<AsteroidBelt>().name}");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SlowZoneBuffer"))
        {
            IsInSlowZoneBuffer = false;
            _spaceshipController.WarpGateCenter = null;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("SlowZone"))
        {
            IsInSlowZone = false;
            //Debug.Log($"Left slow zone - IsInSlowZone {IsInSlowZone}");
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Atmosphere"))
        {
            IsInAtmosphere = false;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("NullifyWarp"))
        {
            _spaceshipWarp.NoWarpSphere = null;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("AsteroidBelt"))
        {
            IsInAsteroidBelt = false;
            //_debrisGrid.gameObject.SetActive(false);
            Debug.Log("Leaving Asteroid Belt");
        }
    }
}
