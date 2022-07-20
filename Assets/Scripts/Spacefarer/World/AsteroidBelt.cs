using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;
using SpaceGraphicsToolkit.Shapes;
using SpaceGraphicsToolkit.Debris;
using CW.Common;

public class AsteroidBelt : MonoBehaviour
{

    [SerializeField]
    private List<SgtDebris> _rockyAsteroidPrefabs;

    [SerializeField]
    private List<SgtDebris> _icyAsteroidPrefabs;

    [SerializeField]
    private AsteroidBeltData _asteroidBeltData;

    [SerializeField]
    private SgtFloatingObject _center;

    [SerializeField]
    private Scales _scales;

    [SerializeField]
    private SgtShapeGroup _spawnerGroup;

    [SerializeField]
    private SgtShapeTorus _spawnerTorus;

    [SerializeField]
    private SgtDebrisSpawner _debrisSpawner;
    
    public AsteroidBeltData AsteroidBeltData { get => _asteroidBeltData; set => _asteroidBeltData = value; }
    public SgtFloatingObject Center { get => _center; set => _center = value; }
    public Scales Scales { get => _scales; set => _scales = value; }

    [SerializeField]
    private List<Transform> Colliders;

    [SerializeField]
    float meanOrbit, beltThickness, beltHeight;

    // Start is called before the first frame update
    void Start()
    {
        _debrisSpawner.name = $"{name} - spawner";
        _debrisSpawner.transform.SetParent(FindObjectOfType<SgtFloatingRoot>().transform);

        if(_asteroidBeltData.asteroidType == AsteroidType.Mixed || _asteroidBeltData.asteroidType == AsteroidType.Rocky)
        {
            foreach(SgtDebris rockyAsteroidPrefab in _rockyAsteroidPrefabs)
            {
                _debrisSpawner.Prefabs.Add(rockyAsteroidPrefab);
            }
        }

        if(_asteroidBeltData.asteroidType == AsteroidType.Mixed || _asteroidBeltData.asteroidType == AsteroidType.Icy)
        {
            foreach (SgtDebris icyAsteroidPrefab in _icyAsteroidPrefabs)
            {
                _debrisSpawner.Prefabs.Add(icyAsteroidPrefab);
            }
        }

        _debrisSpawner.SpawnLimit = _asteroidBeltData.Density;
        _debrisSpawner.SpawnScaleMin = _asteroidBeltData.asteroidMinSize;
        _debrisSpawner.SpawnScaleMax = _asteroidBeltData.asteroidMaxSize;

        _debrisSpawner.ShowDistance = Mathf.Max(_asteroidBeltData.asteroidMaxSize, 2000f) * 250f;
        _debrisSpawner.HideDistance = Mathf.Max(_asteroidBeltData.asteroidMaxSize, 2000f) * 500f;

        _debrisSpawner.Target = FindObjectOfType<SpaceshipController>().transform;
        //_debrisSpawner.SpawnAllDebrisInside();

        GetComponent<CwFollow>().Target = _center.transform;

        //Define mean orbit of asteroid belt
        meanOrbit = ((_asteroidBeltData.OrbitMin + _asteroidBeltData.OrbitMax) / 20f) * _scales.Orbit;
        
        //Define thickness of asteroid belt
        beltThickness = ((_asteroidBeltData.OrbitMax - _asteroidBeltData.OrbitMin) / 2f) * _scales.Orbit;

        beltHeight = _asteroidBeltData.Height * _scales.Planet;

        foreach(Transform transformCollider in Colliders)
        {
            transformCollider.localScale = new Vector3(meanOrbit, meanOrbit, meanOrbit);
        }
        //transform.localScale = new Vector3(meanOrbit, meanOrbit, meanOrbit);

        _spawnerTorus.Radius = ((_asteroidBeltData.OrbitMin + _asteroidBeltData.OrbitMax) / 2f) * _scales.Orbit;
        _spawnerTorus.Thickness = beltThickness = ((_asteroidBeltData.OrbitMax - _asteroidBeltData.OrbitMin) / 2f) * _scales.Orbit;
        _spawnerTorus.transform.localScale = new Vector3(1f, (1f / meanOrbit) * beltHeight, 1f);
        //_spawnerRing.RadiusMin = _asteroidBeltData.OrbitMin * 20f;
        //_spawnerRing.RadiusMax = _asteroidBeltData.OrbitMax * 20f;
        //_spawnerRing.Range = _spawnerRing.RadiusMax * 1.1;

        foreach (MeshCollider meshCollider in GetComponentsInChildren<MeshCollider>())
        {
            //x : height
            //y : lenght
            //z : thickness

            //Bring back height and thickness to 1 unit;
            meshCollider.transform.localScale = new Vector3((1f / meanOrbit) * beltHeight * 10f, 1f, (1f / meanOrbit) * beltThickness * 2f);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
