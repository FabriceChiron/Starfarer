using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;
using SpaceGraphicsToolkit.Starfield;
using SpaceGraphicsToolkit.Belt;
using SpaceGraphicsToolkit.Atmosphere;
using SpaceGraphicsToolkit.Jovian;
using CW.Common;
using VSX.UniversalVehicleCombat.Radar;
using VSX.FloatingOriginSystem;

public class StellarSystemGeneratorSCSM : MonoBehaviour
{
    [SerializeField]
    private Utils Utils;

    [SerializeField]
    private Scales _scales;

    [SerializeField]
    private GameObject _playerPrefab, _forRadarPrefab;

    [SerializeField]
    private GameObject _rockyPlanetRadarPrefab, _gaseousPlanetRadarPrefab, _starRadarPrefab, _zoneDisableCruiseSpeedPrefab, _nullifyWarpPrefab;

    [SerializeField]
    private StellarSystemData _currentSystemData;

    [SerializeField]
    private Transform stellarSystemContainer;

    [SerializeField]
    private SgtFloatingObject stellarSystemCenter;

    [SerializeField]
    private SgtFloatingCamera initialFloatingCamera;

    [SerializeField]
    private List<SgtFloatingObject> _floatingObjectsList;

    [SerializeField]
    private bool _useXR;

    [SerializeField]
    private bool _useRadar, _useCockpitProjection, _enableGravity, _canSpawnPlayer, _isPlayerSpawned;

    [SerializeField]
    SgtFloatingObject _spawnerFloatingObject;
    
    CwFollow playerFollow;

    private GameObject _player;

    public bool UseXR { get => _useXR; set => _useXR = value; }
    public List<SgtFloatingObject> FloatingObjectsList { get => _floatingObjectsList; set => _floatingObjectsList = value; }
    public bool UseRadar { get => _useRadar; set => _useRadar = value; }
    public bool UseCockpitProjection { get => _useCockpitProjection; set => _useCockpitProjection = value; }
    public bool EnableGravity { get => _enableGravity; set => _enableGravity = value; }


    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        GenerateStellarSystem(_currentSystemData);
    }

    // Update is called once per frame
    void Update()
    {
        if (_canSpawnPlayer)
        {
            if (!_isPlayerSpawned)
            {
                SpawnPlayer();
            }
            else
            {
                playerFollow.enabled = false;
            }
        }
    }

    private void SpawnPlayer()
    {
        if (_playerPrefab != null)
        {
            _player = Instantiate(_playerPrefab);

            playerFollow = _player.transform.GetChild(0).gameObject.AddComponent<CwFollow>();

            playerFollow.Target = _spawnerFloatingObject.gameObject.GetComponentInChildren<AnchorSpawn>().transform;

            _isPlayerSpawned = true;

            SgtFloatingCamera _playerCamera = _player.GetComponentInChildren<SgtFloatingCamera>();
            initialFloatingCamera.gameObject.SetActive(false);
            _playerCamera.enabled = true;
            
            //_playerCamera.Position = _spawnerFloatingObject.Position;
        }
    }

    private void GenerateStellarSystem(StellarSystemData stellarSystemData)
    {

        foreach (StarData starData in stellarSystemData.StarsItem)
        {
            CreateStar(starData, stellarSystemData.StarsItem.Length);
        }

        if (stellarSystemData.ChildrenItem.Length > 0)
        {
            foreach (StellarBodyData stellarBodyData in stellarSystemData.ChildrenItem)
            {
                CreateStellarObject(stellarBodyData, stellarSystemCenter, "Planet");
            }
        }

        //_player.GetComponentInChildren<RadarUI>().FloatingObjectsList = FloatingObjectsList;
    }

    private void CreateStar(StarData starData, int starsCount)
    {
        GameObject newStar = Instantiate(starData.Prefab, stellarSystemContainer);

        newStar.name = starData.Name;

        newStar.transform.localScale = new Vector3(1f, 1f, 1f);
        newStar.transform.localScale *= starData.Size * _scales.Planet;

        SgtFloatingOrbit orbitComp = newStar.GetComponent<SgtFloatingOrbit>();

        orbitComp.ParentPoint = stellarSystemCenter;

        orbitComp.Radius = starData.Orbit * _scales.Orbit + newStar.transform.localScale.x;

        //Debug.Log($"{starData.name} orbit: {starData.Orbit * _scales.Orbit + newStar.transform.localScale.x}");

        orbitComp.Angle = starData.AngleOnPlane;

        //orbitComp.enabled = false;

        orbitComp.DegreesPerSecond = (starData.YearLength != 0f) ? 360f / (starData.YearLength * _scales.Year) : 0;

        GameObject newStarFlare = Instantiate(starData.FlarePrefab, stellarSystemContainer);

        newStarFlare.name = $"{starData.Name} - Flare";

        newStarFlare.transform.localScale = new Vector3(1f, 1f, 1f);
        newStarFlare.transform.localScale *= starData.Size * _scales.Planet / 109f;

        SgtFloatingOrbit orbitFlareComp = newStarFlare.GetComponent<SgtFloatingOrbit>();

        orbitFlareComp.ParentPoint = stellarSystemCenter;

        orbitFlareComp.Radius = starData.Orbit * _scales.Orbit + newStar.transform.localScale.x;

        orbitFlareComp.Angle = starData.AngleOnPlane;

        orbitFlareComp.DegreesPerSecond = (starData.YearLength != 0f) ? 360f / (starData.YearLength * _scales.Year) : 0;


        if(UseRadar)
        {
            GameObject starToRadar = Instantiate(_forRadarPrefab, newStarFlare.transform);
            RaycastToRadar starRaycastToRadar = starToRadar.GetComponent<RaycastToRadar>();
            starRaycastToRadar.RadarPrefab = _starRadarPrefab;
            starRaycastToRadar.StellarBodyName = starData.Name;
            starRaycastToRadar.CurrentScales = _scales;
            starRaycastToRadar.Type = "Star";
            starRaycastToRadar.UseRadar = UseRadar;
            starRaycastToRadar.UseCockpitProjection = UseCockpitProjection;
            starRaycastToRadar.StarData = starData;
            starRaycastToRadar.UseXR = UseXR;
        }

        if (starData.warpGate != null)
        {
            SpawnWarpGate(starData.warpGate, newStar.GetComponent<SgtFloatingObject>());
        }
    }

    private void CreateStellarObject(StellarBodyData stellarBodyData, SgtFloatingObject thisCenter, string Type)
    {
        //Debug.Log($"{stellarBodyData.Name}");
        //Debug.Log($"{stellarBodyData.Prefab}");
        //Debug.Log($"{stellarSystemContainer}");

        //Debug.Log(stellarBodyData.Prefab);

        GameObject stellarBody = Instantiate(stellarBodyData.Prefab, stellarSystemContainer);

        if(_zoneDisableCruiseSpeedPrefab != null)
        {
            GameObject zoneDisableCruiseSpeed;

            if (stellarBody.GetComponentInChildren<SgtAtmosphere>())
            {
                zoneDisableCruiseSpeed = Instantiate(_zoneDisableCruiseSpeedPrefab, stellarBody.GetComponentInChildren<SgtAtmosphere>().transform.GetChild(0));
                zoneDisableCruiseSpeed.transform.localScale = new Vector3(2f, 2f, 2f);
            }
            else if (stellarBody.GetComponentInChildren<SgtJovian>())
            {
                zoneDisableCruiseSpeed = Instantiate(_zoneDisableCruiseSpeedPrefab, stellarBody.transform);
                zoneDisableCruiseSpeed.transform.localScale = new Vector3(2f, 2f, 2f);
            }
            else
            {
                zoneDisableCruiseSpeed = Instantiate(_zoneDisableCruiseSpeedPrefab, stellarBody.transform); 
            }

        }


        if (UseRadar)
        {
            GameObject stellarBodyToRadar = Instantiate(_forRadarPrefab, stellarBody.transform);

            RaycastToRadar stellarBodyRaycastToRadar = stellarBodyToRadar.GetComponent<RaycastToRadar>();

            stellarBodyRaycastToRadar.RadarPrefab = stellarBodyData.Gaseous ? _gaseousPlanetRadarPrefab : _rockyPlanetRadarPrefab;
            stellarBodyRaycastToRadar.StellarBodyName = stellarBodyData.Name;
            stellarBodyRaycastToRadar.CurrentScales = _scales;
            stellarBodyRaycastToRadar.Type = Type;
            stellarBodyRaycastToRadar.UseRadar = UseRadar;
            stellarBodyRaycastToRadar.UseCockpitProjection = UseCockpitProjection;
            stellarBodyRaycastToRadar.StellarBodyData = stellarBodyData;
            stellarBodyRaycastToRadar.UseXR= UseXR;

            if(_nullifyWarpPrefab != null)
            {
                GameObject nullifyWarp = Instantiate(_nullifyWarpPrefab, stellarBody.transform.GetChild(0));
                //nullifyWarp.transform.localScale *= 0.1f;
                stellarBodyRaycastToRadar.NullifyWarp = nullifyWarp.transform;
            }
            
            if(Type == "Moon")
            {
                stellarBodyRaycastToRadar.ParentName = thisCenter.name;
            }
        }

        SphereCollider stellarBodyCollider = stellarBody.AddComponent<SphereCollider>();

        stellarBodyCollider.radius = stellarBodyData.Gaseous ? 0.9f : 1f;

        Rigidbody stellarBodyRb = stellarBody.GetComponent<Rigidbody>();

        stellarBodyRb.useGravity = false;
        stellarBodyRb.isKinematic = true;
        stellarBodyRb.angularDrag = 0f;
        
        if(EnableGravity)
        {
            SgtGravitySource stellarBodyGravitySource = (stellarBody.GetComponent<SgtGravitySource>() != null 
                ? stellarBody.GetComponent<SgtGravitySource>() 
                : stellarBody.AddComponent<SgtGravitySource>()
            );
        
            if(stellarBodyGravitySource != null)
            {
                stellarBodyGravitySource.Mass = stellarBodyData.Mass * (float)10e+10;
            }
        }

        stellarBody.AddComponent<FloatingOriginObject>();

        stellarBody.name = stellarBodyData.Name;

        //Reset prefab scale to 1;
        stellarBody.transform.localScale = new Vector3(1f, 1f, 1f);

        //Assign scale: Stellar Body Size * desired scale;
        stellarBody.transform.localScale *= stellarBodyData.Size * _scales.Planet;

        stellarBody.transform.localEulerAngles = new Vector3(stellarBodyData.BodyTilt, stellarBody.transform.localEulerAngles.y, stellarBody.transform.localEulerAngles.z);

        SgtFloatingTarget warpComp = stellarBody.AddComponent<SgtFloatingTarget>();
        warpComp.WarpName = stellarBodyData.Name;
        warpComp.WarpDistance = stellarBodyData.Size * _scales.Planet * 3f;

        SgtFloatingOrbit orbitComp = stellarBody.GetComponent<SgtFloatingOrbit>();

        if(Type == "Planet" && stellarBodyData.OrbitsAround != null)
        {
            orbitComp.ParentPoint = GameObject.Find($"{stellarBodyData.OrbitsAround.Name}").GetComponent<SgtFloatingObject>();
        }
        else
        {
            orbitComp.ParentPoint = thisCenter;
        }


        orbitComp.Radius = stellarBodyData.Orbit * _scales.Orbit + ((Type == "Moon") ? (orbitComp.ParentPoint.transform.localScale.x + stellarBody.transform.localScale.x) * 5f : orbitComp.ParentPoint.transform.localScale.x);

        //Debug.Log($"{stellarBodyData.name} orbit: {stellarBodyData.Orbit * _scales.Orbit + ((Type == "Moon") ? (thisCenter.transform.localScale.x + stellarBody.transform.localScale.x) * 5f : 0f)}");

        orbitComp.Angle = stellarBodyData.AngleOnPlane;

        orbitComp.Tilt = new Vector3(stellarBodyData.OrbitTilt, 0f, 0f);

        double revolutionPeriod = 360 / (stellarBodyData.YearLength * _scales.Year);

        orbitComp.DegreesPerSecond = revolutionPeriod;
        
        CwRotate rotateComp = stellarBody.transform.GetChild(0).gameObject.AddComponent<CwRotate>();

        rotateComp.RelativeTo = Space.Self;
        
        //rotateComp.enabled = false;

        float rotationY = (stellarBodyData.TidallyLocked) ? (float)orbitComp.DegreesPerSecond : (float)(360 / (stellarBodyData.DayLength * _scales.Day));

        rotateComp.AngularVelocity = new Vector3(0f, rotationY, 0f);

        if (stellarBodyData.warpGate != null)
        {
            SpawnWarpGate(stellarBodyData.warpGate, stellarBody.GetComponent<SgtFloatingObject>());
        }

        if (stellarBodyData.ChildrenItem.Length > 0)
        {
            foreach (StellarBodyData moonBodyData in stellarBodyData.ChildrenItem)
            {
                CreateStellarObject(moonBodyData, stellarBody.transform.GetComponent<SgtFloatingObject>(), "Moon");
            }
        }
    }

    private void SpawnWarpGate(WarpGateData warpGateData, SgtFloatingObject thisCenter)
    {
        GameObject newWarpGate = Instantiate(warpGateData.Prefab, stellarSystemContainer);

        newWarpGate.name = warpGateData.Name;

        newWarpGate.transform.localScale *= warpGateData.Scale;

        newWarpGate.AddComponent<SetupStellarBody>();

        SgtFloatingObject objectComp = newWarpGate.GetComponent<SgtFloatingObject>();

        SgtFloatingOrbit orbitComp = newWarpGate.GetComponent<SgtFloatingOrbit>();


        orbitComp.ParentPoint = thisCenter;

        orbitComp.Radius = (warpGateData.Orbit * _scales.Orbit) + thisCenter.transform.localScale.x;

        orbitComp.Angle = warpGateData.AngleOnPlane;

        orbitComp.Tilt = new Vector3(warpGateData.OrbitTilt, 0f, 0f);

        double revolutionPeriod = 360 / (warpGateData.YearLength * _scales.Year);

        //orbitComp.DegreesPerSecond = revolutionPeriod;
        orbitComp.DegreesPerSecond = 0;


        if (warpGateData.spawnsPlayer && _playerPrefab != null)
        {
            _spawnerFloatingObject = newWarpGate.GetComponent<SgtFloatingObject>();

            _canSpawnPlayer = true;
        }
    }
}


