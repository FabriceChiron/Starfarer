using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaycastToRadar : MonoBehaviour
{

    private RadarUI _radarUI;
    private Spaceship _spaceship;

    private bool IsRadarFound, IsSpaceshipFound;

    LayerMask mask;

    [SerializeField]
    private GameObject _radarPrefab, _planetNamePrefab;

    [SerializeField]
    private string stellarBodyName, _type, _parentName;

    [SerializeField]
    private StellarBodyData _stellarBodyData;

    [SerializeField]
    private StarData _starData;

    private LineRenderer _lineRenderer;

    private Scales _currentScales;

    private Transform _cameraTransform;

    private LineRenderer cockpitLineRenderer;

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

    private bool isIconSpawned, isCockpitIconSpawned;

    GameObject radarIcon, radarCockpitIcon;

    private int layer = 11;
    private LayerMask layerMask;

    private TextMeshPro _textComp, _nameCockpitComp;

    private GameObject _infosCockpitComp;

    private TextMeshPro _name, _distance, _bodyType, _orbit, _orbitTilt, _radius, _bodyTilt, _mass, _revolution, _rotation, _details;

    // Start is called before the first frame update
    void Start()
    {
        Physics.queriesHitBackfaces = true;

        layerMask = 1 << (int)layer;

        _lineRenderer = GetComponent<LineRenderer>();

        _textComp = transform.GetComponentInChildren<TextMeshPro>();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        _radarUI = GameObject.FindObjectOfType<RadarUI>();

        _spaceship = GameObject.FindObjectOfType<Spaceship>();

        _angleThreshold = UseXR ? 9f : 3f;
        

        if (_radarUI != null && UseRadar)
        {
            IsRadarFound = true;
        
            if(!isIconSpawned)
            {
                radarIcon = Instantiate(RadarPrefab, _radarUI.transform.GetChild(0));
                _textComp = radarIcon.GetComponentInChildren<TextMeshPro>();
                radarIcon.transform.GetChild(1).gameObject.SetActive(false);
                if (Type == "Moon")
                {
                    radarIcon.transform.localScale *= 0.75f;
                    _textComp.fontSize *= 1.25f;
                }
                isIconSpawned = true;
            }
        }

        if(_spaceship != null && UseCockpitProjection)
        {
            IsSpaceshipFound = true;
            _cameraTransform = UseXR ? _spaceship.CameraVR.transform : _spaceship.CameraFlat.transform;

            if (!isCockpitIconSpawned)
            {
                radarCockpitIcon = Instantiate(RadarPrefab, _cameraTransform);
                radarCockpitIcon.transform.localScale *= 10f;
                radarCockpitIcon.transform.localScale = new Vector3(-radarCockpitIcon.transform.localScale.x, radarCockpitIcon.transform.localScale.y, radarCockpitIcon.transform.localScale.z);
                _nameCockpitComp = radarCockpitIcon.GetComponentsInChildren<TextMeshPro>()[0];
                _infosCockpitComp = radarCockpitIcon.transform.GetChild(1).gameObject;

                /*if (Type == "Moon")
                {
                    radarCockpitIcon.transform.localScale *= 0.75f;
                    _nameCockpitComp.fontSize *= 1.25f;
                }*/

                FillDetails();


                isCockpitIconSpawned = true;
            }
        }

        if(IsSpaceshipFound && isCockpitIconSpawned && UseCockpitProjection)
        {

            stellarBodyDistance = Mathf.Round(Vector3.Distance(_cameraTransform.position, transform.position) / _currentScales.Orbit * 100f) / 100f;

            radarCockpitIcon.SetActive(!(stellarBodyDistance > 1f && Type == "Moon") && _spaceship.ShowCockpitRadar);

            //radarCockpitIcon.SetActive(_spaceship.ShowCockpitRadar);

            Vector3 direction = transform.position - _spaceship.transform.position;
            float Angle = Vector3.Angle(_cameraTransform.forward, direction);

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

            radarIcon.SetActive(!(stellarBodyDistance > 1f && Type == "Moon"));

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
        }

        else
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
        }
    }
}
