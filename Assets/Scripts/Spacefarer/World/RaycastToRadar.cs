using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpaceGraphicsToolkit;

public class RaycastToRadar : MonoBehaviour
{

    private RadarUI _radarUI;
    private SpaceshipLocator _spaceshipLocator;
    private SpaceshipWarp _spaceshipWarp;
    private SgtFloatingTarget _floatingTarget;
    private SgtFloatingWarpSmoothstep _warpSmoothStep;

    private bool IsRadarFound, IsSpaceshipFound;

    [SerializeField]
    private Transform _nullifyWarp;

    LayerMask mask;

    [SerializeField]
    private GameObject _radarPrefab, _planetNamePrefab;

    [SerializeField]
    private string stellarBodyName, _type, _parentName;

    [SerializeField]
    private StellarBodyData _stellarBodyData;

    [SerializeField]
    private StarData _starData;

    [SerializeField]
    private WarpGateData _warpGateData;

    private LineRenderer _lineRenderer;

    private Scales _currentScales;

    private Transform _cameraTransform;

    private LineRenderer cockpitLineRenderer;

    [SerializeField]
    private BoolVariable _useXRStoredInfo;

    [SerializeField]
    private bool _useXR, _useRadar, _useCockpitProjection;

    [SerializeField]
    private float _angleThreshold = 3f, stellarBodyDistance;

    public GameObject RadarPrefab { get => _radarPrefab; set => _radarPrefab = value; }
    public string StellarBodyName { get => stellarBodyName; set => stellarBodyName = value; }
    public string ParentName { get => _parentName; set => _parentName = value; }
    public Scales CurrentScales { get => _currentScales; set => _currentScales = value; }
    public string Type { get => _type; set => _type = value; }
    public bool UseXR { get => _useXR; set => _useXR = value; }
    public bool UseRadar { get => _useRadar; set => _useRadar = value; }
    public bool UseCockpitProjection { get => _useCockpitProjection; set => _useCockpitProjection = value; }
    public StellarBodyData StellarBodyData { get => _stellarBodyData; set => _stellarBodyData = value; }
    public StarData StarData { get => _starData; set => _starData = value; }
    public Transform NullifyWarp { get => _nullifyWarp; set => _nullifyWarp = value; }
    public WarpGateData WarpGateData { get => _warpGateData; set => _warpGateData = value; }
    public GameObject RadarWarpPrompt { get => radarWarpPrompt; set => radarWarpPrompt = value; }

    private bool isIconSpawned, isCockpitIconSpawned;

    [SerializeField]
    GameObject radarIcon, radarCockpitIcon, radarWarpPrompt;

    RectTransform radarWarpPromptProgressBar;

    private int layer = 11;
    private LayerMask layerMask;

    private TextMeshPro _textComp, _nameCockpitComp;

    private GameObject _infosCockpitComp;

    private TextMeshPro _name, _distance, _bodyType, _orbit, _orbitTilt, _radius, _bodyTilt, _mass, _revolution, _rotation, _details, _warpPromptName;

    private void OnEnable()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        UseXR = _useXRStoredInfo.BoolValue;
        Physics.queriesHitBackfaces = true;

        layerMask = 1 << (int)layer;

        _lineRenderer = GetComponent<LineRenderer>();

        _textComp = transform.GetComponentInChildren<TextMeshPro>();

        _floatingTarget = transform.GetComponentInParent<SgtFloatingTarget>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (UseRadar && _radarUI == null)
        {
            _radarUI = FindObjectOfType<RadarUI>();
        }

        if (UseCockpitProjection && _spaceshipLocator == null)
        {
            _spaceshipLocator = FindObjectOfType<SpaceshipLocator>();

            _spaceshipWarp = FindObjectOfType<SpaceshipWarp>();

            _warpSmoothStep = FindObjectOfType<SgtFloatingWarpSmoothstep>();
        }

        _angleThreshold = UseXR ? 9f : 3f;
        
        if (_radarUI != null && UseRadar)
        {
            IsRadarFound = true;
        
            if(!isIconSpawned && Type != "Space Station")
            {
                radarIcon = Instantiate(RadarPrefab, _radarUI.transform.GetChild(0));
                _textComp = radarIcon.GetComponentInChildren<TextMeshPro>();
                radarIcon.transform.GetChild(1).gameObject.SetActive(false);

                GameObject spaceStationIcon = radarIcon.transform.Find("Picto").GetComponentInChildren<SpaceStationIcon>(true)?.gameObject;

                if(spaceStationIcon != null)
                {
                    if((StarData != null && StarData.warpGate != null) || (StellarBodyData != null && StellarBodyData.warpGate != null))
                    {
                        spaceStationIcon.SetActive(true);
                    }

                }


                Destroy(radarIcon.GetComponentInChildren<WarpPrompt>()?.transform.parent.gameObject);

                if (Type == "Moon")
                {
                    radarIcon.transform.localScale *= 0.75f;
                    _textComp.fontSize *= 1.25f;
                }
                isIconSpawned = true;
            }
        }

        if(_spaceshipLocator != null && UseCockpitProjection)
        {
            IsSpaceshipFound = true;
            _cameraTransform = UseXR ? _spaceshipLocator.CameraVR.transform : _spaceshipLocator.CameraFlat.transform;

            if (!isCockpitIconSpawned)
            {
                radarCockpitIcon = Instantiate(RadarPrefab, _cameraTransform);
                radarCockpitIcon.transform.localScale *= 10f;
                radarCockpitIcon.transform.localScale = new Vector3(-radarCockpitIcon.transform.localScale.x, radarCockpitIcon.transform.localScale.y, radarCockpitIcon.transform.localScale.z);
                _nameCockpitComp = radarCockpitIcon.GetComponentsInChildren<TextMeshPro>()[0];
                _infosCockpitComp = radarCockpitIcon.transform.GetChild(1).gameObject;

                RadarWarpPrompt = radarCockpitIcon.GetComponentInChildren<WarpPrompt>(true)?.transform.parent.gameObject;
                radarWarpPromptProgressBar = RadarWarpPrompt?.GetComponentInChildren<RectMask2D>().rectTransform;

                RadarWarpPrompt?.SetActive(false);

                FillDetails();


                isCockpitIconSpawned = true;
            }
        }

        if(IsSpaceshipFound && isCockpitIconSpawned && UseCockpitProjection)
        {

            stellarBodyDistance = Mathf.Round(Vector3.Distance(_cameraTransform.position, transform.position) / _currentScales.Orbit * 100f) / 100f;

            if (_spaceshipLocator.ShowCockpitRadar)
            {
                bool toggleActive;
                //If distance is greater than 1AU
                if(stellarBodyDistance > 1f)
                {
                    switch (Type)
                    {
                        case "Moon":
                            toggleActive = false;
                            break;

                        case "Planet":
                            toggleActive = !(StellarBodyData.warpGate != null);
                            break;

                        case "Star":
                            toggleActive = !(StarData.warpGate != null);
                            break;

                        default: //Space Station
                            toggleActive = true;
                            break;
                    }
                }
                //If distance is lower than 1AU
                else
                {
                    switch (Type)
                    {
                        case "Moon":
                            toggleActive = true;
                            break;

                        case "Planet":
                            toggleActive = true;
                            break;

                        case "Star":
                            toggleActive = true;
                            break;

                        default: //Space Station
                            toggleActive = true;
                            break;
                    }
                }
                radarCockpitIcon.SetActive(toggleActive);
            }


            //radarCockpitIcon.SetActive(!(stellarBodyDistance > 1f && Type == "Moon") && _spaceshipLocator.ShowCockpitRadar);

            //radarCockpitIcon.SetActive(_spaceship.ShowCockpitRadar);

            Vector3 direction = transform.position - _spaceshipLocator.transform.position;
            float Angle = Vector3.Angle(_cameraTransform.forward, direction);

            if(Angle <= _angleThreshold && !_warpSmoothStep.Warping && radarCockpitIcon.activeSelf && !_spaceshipWarp.WarpTargetLocked)
            {
                if (_spaceshipWarp.NoWarpSphere != null)
                {
                    if ((_floatingTarget.name == _spaceshipWarp.NoWarpSphere.name) || (_floatingTarget.name.Contains(_spaceshipWarp.NoWarpSphere.name)) || (_spaceshipWarp.NoWarpSphere.name.Contains(_floatingTarget.name)))
                    {
                        
                    }
                    else
                    {
                        _spaceshipWarp.WarpTarget = _floatingTarget;
                        _spaceshipWarp.WarpTime = Mathf.Max(stellarBodyDistance, 3f);
                        _spaceshipWarp.CanWarp = true;

                        if (RadarWarpPrompt != null)
                        {

                            _spaceshipWarp.RadarWarpPrompt = RadarWarpPrompt.GetComponentInChildren<WarpPrompt>();
                            _spaceshipWarp.ProgressBar = radarWarpPromptProgressBar;
                        }
                    }
                }
                else
                {
                    _spaceshipWarp.WarpTarget = _floatingTarget;
                    _spaceshipWarp.WarpTime = Mathf.Max(stellarBodyDistance, 3f);
                    _spaceshipWarp.CanWarp = true;

                    if (RadarWarpPrompt != null)
                    {

                        _spaceshipWarp.RadarWarpPrompt = RadarWarpPrompt.GetComponentInChildren<WarpPrompt>();
                        _spaceshipWarp.ProgressBar = radarWarpPromptProgressBar;
                    }
                }
            }

            radarCockpitIcon.transform.position = _cameraTransform.position + Vector3.Normalize(direction) * 7.5f;
            _nameCockpitComp.text = $"{(Type == "Moon" ? StellarBodyName : StellarBodyName.ToUpper())} \n {stellarBodyDistance} AU";

            radarCockpitIcon.transform.LookAt(_cameraTransform);
            radarCockpitIcon.transform.eulerAngles = new Vector3(radarCockpitIcon.transform.eulerAngles.x, radarCockpitIcon.transform.eulerAngles.y, -_cameraTransform.eulerAngles.z);
            
            if(stellarBodyDistance > 2.5f)
            {
                _nameCockpitComp.enabled = true;
                _infosCockpitComp.SetActive(false);
            }
            else
            {
                _nameCockpitComp.enabled = Angle > _angleThreshold;
                _infosCockpitComp.SetActive(Angle < _angleThreshold);
                _distance.text = $"{stellarBodyDistance} AU";
            }

        }
        
        if (IsRadarFound && isIconSpawned && UseRadar)
        {

            float stellarBodyDistance = Mathf.Round(Vector3.Distance(_radarUI.transform.position, transform.position) / _currentScales.Orbit * 100f) /100f;

            radarIcon.SetActive(!(stellarBodyDistance > 1f && (Type == "Moon" || Type == "Space Station")));

            //radarIcon.transform.position = _radarUI.transform.position + Vector3.Normalize(transform.position - _radarUI.transform.position) * 0.1f;
            radarIcon.transform.position = _radarUI.transform.position + Vector3.Normalize(transform.position - _radarUI.transform.position) * (stellarBodyDistance < 2.5f ? stellarBodyDistance : 2.5f) * 0.05f;

            _textComp.text = $"{(Type == "Moon" ? StellarBodyName : StellarBodyName.ToUpper())} \n {stellarBodyDistance} AU";

        }
    }

    private void FillDetails()
    {
        Transform _infos = radarCockpitIcon.GetComponentInChildren<Infos>(true).transform;

        
        radarCockpitIcon.name = $"{StellarBodyName} - Cockpit Icon";

        if (StellarBodyData != null)
        {
            _name = _infos.GetChild(0).Find("Name").GetComponent<TextMeshPro>();
            _name.text = StellarBodyData.Name;

            _distance = _infos.GetChild(0).Find("Distance").GetComponent<TextMeshPro>();

            _bodyType = _infos.Find("Type").GetComponent<TextMeshPro>();
            _bodyType.text = $"{(StellarBodyData.Gaseous ? "Gaseous" : "Rocky")} {Type}{(ParentName != "" ? $"of {ParentName}" : "")}";

            _orbit = _infos.Find("Orbit").GetComponent<TextMeshPro>();
            _orbit.text = _orbit.text.Replace("{}", $"{StellarBodyData.Orbit}");

            _orbitTilt = _infos.Find("Orbit Tilt").GetComponent<TextMeshPro>();
            _orbitTilt.gameObject.SetActive(StellarBodyData.OrbitTilt != 0f);
            _orbitTilt.text = _orbitTilt.text.Replace("{}", $"{StellarBodyData.OrbitTilt}");

            _radius = _infos.Find("Radius").GetComponent<TextMeshPro>();
            _radius.text = _radius.text.Replace("{}", $"{StellarBodyData.Size}");

            _bodyTilt = _infos.Find("Axial Tilt").GetComponent<TextMeshPro>();
            _bodyTilt.gameObject.SetActive(StellarBodyData.BodyTilt != 0f);

            _mass = _infos.Find("Mass").GetComponent<TextMeshPro>();
            _mass.text = _mass.text.Replace("{}", $"{StellarBodyData.Mass}");

            _revolution = _infos.Find("Revolution").GetComponent<TextMeshPro>();
            _revolution.text = _revolution.text.Replace("{}", $"{StellarBodyData.YearLength}");

            _rotation = _infos.Find("Rotation").GetComponent<TextMeshPro>();
            if (StellarBodyData.TidallyLocked)
            {
                _rotation.text = "Tidally Locked";
            }
            else
            {
                _rotation.text = _rotation.text.Replace("{}", $"{StellarBodyData.DayLength}");
            }

            _details = _infos.Find("Description").GetComponent<TextMeshPro>();
            _details.text = StellarBodyData.Details;

            Transform _warpPrompt = radarCockpitIcon.GetComponentInChildren<WarpPrompt>(true).transform;

            _warpPromptName = _warpPrompt.Find("Message").GetComponent<TextMeshPro>();
            _warpPromptName.text = _warpPromptName.text.Replace("{}", $"{StellarBodyData.Name}");
        }

        else if(StarData != null)
        {
            _name = _infos.GetChild(0).Find("Name").GetComponent<TextMeshPro>();
            _name.text = StarData.Name;

            _distance = _infos.GetChild(0).Find("Distance").GetComponent<TextMeshPro>();
            
            _bodyType = _infos.Find("Type").GetComponent<TextMeshPro>();
            _bodyType.text = $"{StarData.FlarePrefab.name.Replace(" - Light", "")}";

            _orbit = _infos.Find("Orbit").GetComponent<TextMeshPro>();
            _orbit.gameObject.SetActive(StarData.Orbit != 0f);
            _orbit.text = _orbit.text.Replace("{}", $"{StarData.Orbit}");

            _radius = _infos.Find("Radius").GetComponent<TextMeshPro>();
            _radius.text = _radius.text.Replace("{}", $"{StarData.Size}");

            _revolution = _infos.Find("Revolution").GetComponent<TextMeshPro>();
            _revolution.gameObject.SetActive(StarData.YearLength != 0f);
            _revolution.text = _revolution.text.Replace("{}", $"{StarData.YearLength}");

            _details = _infos.Find("Description").GetComponent<TextMeshPro>();
            _details.text = StarData.StarDescription;

            Transform _warpPrompt = radarCockpitIcon.GetComponentInChildren<WarpPrompt>(true).transform;

            _warpPromptName = _warpPrompt.Find("Message").GetComponent<TextMeshPro>();
            _warpPromptName.text = _warpPromptName.text.Replace("{}", $"{StarData.Name}");
        }

        else
        {
            _name = _infos.GetChild(0).Find("Name").GetComponent<TextMeshPro>();
            _name.text = WarpGateData.Name;

            _distance = _infos.GetChild(0).Find("Distance").GetComponent<TextMeshPro>();

            _bodyType = _infos.Find("Type").GetComponent<TextMeshPro>();
            _bodyType.text = Type;

            _orbit = _infos.Find("Orbit").GetComponent<TextMeshPro>();
            _orbit.gameObject.SetActive(WarpGateData.Orbit != 0f);
            _orbit.text = _orbit.text.Replace("{}", $"{WarpGateData.Orbit}");

            Transform _warpPrompt = radarCockpitIcon.GetComponentInChildren<WarpPrompt>(true).transform;

            _warpPromptName = _warpPrompt.Find("Message").GetComponent<TextMeshPro>();
            _warpPromptName.text = _warpPromptName.text.Replace("{}", $"{WarpGateData.Name}");
        }
    }
}
