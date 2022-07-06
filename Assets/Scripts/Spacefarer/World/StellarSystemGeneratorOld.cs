using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;
using SpaceGraphicsToolkit.Starfield;
using SpaceGraphicsToolkit.Belt;
using CW.Common;

public class StellarSystemGeneratorOld : MonoBehaviour
{
    [SerializeField]
    private Utils Utils;

    [SerializeField]
    private Scales _scales;

    [SerializeField]
    private GameObject _playerPrefab, _forRadarPrefab, _followWarpGatePrefab;

    [SerializeField]
    private GameObject _rockyPlanetRadarPrefab, _gaseousPlanetRadarPrefab, _starRadarPrefab;

    [SerializeField]
    private StellarSystemData _currentSystemData;

    [SerializeField]
    private Transform stellarSystemContainer;

    [SerializeField]
    private SgtFloatingObject stellarSystemCenter;

    [SerializeField]
    private List<SgtFloatingObject> _floatingObjectsList;

    [SerializeField]
    private bool _useXR;

    [SerializeField]
    private bool _useRadar, _useCockpitProjection, _enableGravity;

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

        if(Type == "Moon")
        {
            stellarBodyRaycastToRadar.ParentName = thisCenter.name;
        }

        SphereCollider stellarBodyCollider = stellarBody.AddComponent<SphereCollider>();

        stellarBodyCollider.radius = stellarBodyData.Gaseous ? 0.9f : 1f;

        Rigidbody stellarBodyRb = stellarBody.AddComponent<Rigidbody>();

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

        stellarBody.name = stellarBodyData.Name;

        //Reset prefab scale to 1;
        stellarBody.transform.localScale = new Vector3(1f, 1f, 1f);

        //Assign scale: Stellar Body Size * desired scale;
        stellarBody.transform.localScale *= stellarBodyData.Size * _scales.Planet;

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
            /*GameObject _followWarpGate = Instantiate(_followWarpGatePrefab);
            SgtFloatingCamera warpGateFloatingCamera = _followWarpGate.AddComponent<SgtFloatingCamera>();

            _followWarpGate.GetComponent<MatchElementTransform>().elementToMatch = newWarpGate.transform;*/

            _player = Instantiate(_playerPrefab);





            SgtFloatingCamera _playerCamera = _player.GetComponentInChildren<SgtFloatingCamera>();

            RadarUI radarUI = _player.GetComponentInChildren<RadarUI>();

            Spaceship _spaceship = _player.GetComponentInChildren<Spaceship>();

            SgtFloatingOrbit spaceshipOrbitComp = _spaceship.gameObject.AddComponent<SgtFloatingOrbit>();

            spaceshipOrbitComp.ParentPoint = orbitComp.ParentPoint;
            spaceshipOrbitComp.Radius = orbitComp.Radius;
            spaceshipOrbitComp.Angle = orbitComp.Angle;
            spaceshipOrbitComp.Tilt = orbitComp.Tilt;
            spaceshipOrbitComp.DegreesPerSecond = spaceshipOrbitComp.DegreesPerSecond;

            _spaceship.EnableGravity = EnableGravity;

            _spaceship.UseRadar = UseRadar;

            /*
            _player.transform.position = _followWarpGate.transform.position;
            _spaceship.transform.localPosition = Vector3.zero;


            Debug.Log(warpGateFloatingCamera.Position);



            _playerCamera.SetPosition(warpGateFloatingCamera.Position);
            _playerCamera.ApplyPosition();

            warpGateFloatingCamera.enabled = false;*/
            _playerCamera.enabled = true;
        }
    }
}


