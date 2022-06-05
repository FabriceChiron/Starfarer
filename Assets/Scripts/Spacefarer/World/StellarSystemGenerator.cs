using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;
using CW.Common;

public class StellarSystemGenerator : MonoBehaviour
{
    [SerializeField]
    private Utils Utils;

    [SerializeField]
    private Scales _scales;

    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private StellarSystemData _currentSystemData;

    [SerializeField]
    private Transform stellarSystemContainer;

    [SerializeField]
    private SgtFloatingObject stellarSystemCenter;

    private GameObject _player;

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
        Debug.Log(stellarSystemData.ChildrenItem.Length);

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
    }

    private void CreateStar(StarData starData, int starsCount)
    {
        GameObject newStar = Instantiate(starData.Prefab, stellarSystemContainer);

        newStar.name = starData.Name;

        newStar.transform.localScale *= _scales.Planet;

        SgtFloatingOrbit orbitComp = newStar.GetComponent<SgtFloatingOrbit>();

        orbitComp.ParentPoint = stellarSystemCenter;

        orbitComp.Radius = starData.Orbit * _scales.Orbit;

        orbitComp.Angle = starData.AngleOnPlane;

        orbitComp.DegreesPerSecond = (starData.YearLength != 0f) ? 360 / (starData.YearLength * _scales.Year) : 0;

        if(starData.ChildrenItem.Length > 0)
        {
            foreach (StellarBodyData stellarBodyData in starData.ChildrenItem)
            {
                CreateStellarObject(stellarBodyData, newStar.transform.GetComponent<SgtFloatingObject>(), "Planet");
            }
        }
    }

    private void CreateStellarObject(StellarBodyData stellarBodyData, SgtFloatingObject thisCenter, string Type)
    {
        GameObject newStellarBody = Instantiate(stellarBodyData.Prefab, stellarSystemContainer);

        newStellarBody.name = stellarBodyData.Name;

        newStellarBody.transform.localScale *= stellarBodyData.Size * _scales.Planet;

        SgtFloatingOrbit orbitComp = newStellarBody.GetComponent<SgtFloatingOrbit>();

        orbitComp.ParentPoint = thisCenter;

        orbitComp.Radius = stellarBodyData.Orbit * _scales.Orbit + ((Type == "Moon") ? (thisCenter.transform.localScale.x + newStellarBody.transform.localScale.x) * 5f : 0f);

        orbitComp.Angle = stellarBodyData.AngleOnPlane;

        orbitComp.Tilt = new Vector3(stellarBodyData.OrbitTilt, 0f, 0f);

        double revolutionPeriod = 360 / (stellarBodyData.YearLength * _scales.Year);

        orbitComp.DegreesPerSecond = revolutionPeriod;

        if (stellarBodyData.warpGate != null)
        {
            SpawnWarpGate(stellarBodyData.warpGate, newStellarBody.GetComponent<SgtFloatingObject>());
        }

        if (stellarBodyData.ChildrenItem.Length > 0)
        {
            foreach (StellarBodyData moonBodyData in stellarBodyData.ChildrenItem)
            {
                CreateStellarObject(moonBodyData, newStellarBody.transform.GetComponent<SgtFloatingObject>(), "Moon");
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

        orbitComp.DegreesPerSecond = revolutionPeriod;

        if (warpGateData.spawnsPlayer && _playerPrefab != null)
        {
            _player = Instantiate(_playerPrefab);

            SgtFloatingCamera _playerCamera = _player.GetComponentInChildren<SgtFloatingCamera>();

            

            SgtFloatingOrbit _playerOrbitTemp = _playerCamera.gameObject.AddComponent<SgtFloatingOrbit>();

            _playerOrbitTemp.ParentPoint = objectComp;

            _playerOrbitTemp.Radius = 10f;
            _playerOrbitTemp.Angle = 0f;
            _playerOrbitTemp.DegreesPerSecond = 0f;

            //_playerOrbitTemp.enabled = false;

            //CwFollow _playerFollowComp = _playerCamera.gameObject.AddComponent<CwFollow>();

            //_playerFollowComp.Target = newWarpGate.transform;

            //_playerFollowComp.enabled = false;



            _playerCamera.transform.position = new Vector3(newWarpGate.transform.position.x, newWarpGate.transform.position.y, newWarpGate.transform.position.z + 20f);
            //_playerCamera.SetPosition(new SgtPosition(_playerCamera.transform.position));
            
            //_playerCamera.UpdatePositionNow();

            _playerCamera.enabled = true;
        }
    }
}


