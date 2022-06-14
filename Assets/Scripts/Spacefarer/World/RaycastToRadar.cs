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
    private string stellarBodyName, _type;

    private LineRenderer _lineRenderer;

    private Scales _currentScales;

    private Transform _cameraTransform;

    private LineRenderer cockpitLineRenderer;

    [SerializeField]
    private bool _useXR, _useRadar, _useCockpitProjection;

    public GameObject RadarPrefab { get => _radarPrefab; set => _radarPrefab = value; }
    public string StellarBodyName { get => stellarBodyName; set => stellarBodyName = value; }
    public Scales CurrentScales { get => _currentScales; set => _currentScales = value; }
    public string Type { get => _type; set => _type = value; }
    public bool UseXR { get => _useXR; set => _useXR = value; }
    public bool UseRadar { get => _useRadar; set => _useRadar = value; }
    public bool UseCockpitProjection { get => _useCockpitProjection; set => _useCockpitProjection = value; }

    private bool isIconSpawned, isCockpitIconSpawned;

    GameObject radarIcon, radarCockpitIcon;

    private int layer = 11;
    private LayerMask layerMask;

    private TextMeshPro _textComp, _textCockpitComp;

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
        

        if (_radarUI != null && UseRadar)
        {
            IsRadarFound = true;
        
            if(!isIconSpawned)
            {
                radarIcon = Instantiate(RadarPrefab, _radarUI.transform.GetChild(0));
                _textComp = radarIcon.GetComponentInChildren<TextMeshPro>();
                if(Type == "Moon")
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
            _cameraTransform = _spaceship.transform.GetComponentInChildren<Camera>().transform;

            if (!isCockpitIconSpawned)
            {
                radarCockpitIcon = Instantiate(RadarPrefab, _cameraTransform);
                //radarCockpitIcon.transform.localScale *= 2f;
                _textCockpitComp = radarCockpitIcon.GetComponentInChildren<TextMeshPro>();
                cockpitLineRenderer = radarCockpitIcon.GetComponent<LineRenderer>();
                cockpitLineRenderer.enabled = false;
                if (Type == "Moon")
                {
                    radarCockpitIcon.transform.localScale *= 0.75f;
                    _textCockpitComp.fontSize *= 1.25f;
                }
                isCockpitIconSpawned = true;
            }
        }

        if(IsSpaceshipFound && isCockpitIconSpawned && UseCockpitProjection)
        {

            float stellarBodyDistance = Mathf.Round(Vector3.Distance(_cameraTransform.position, transform.position) / _currentScales.Orbit * 100f) / 100f;

            radarCockpitIcon.SetActive(!(stellarBodyDistance > 1f && Type == "Moon"));


            radarCockpitIcon.transform.position = _cameraTransform.position + Vector3.Normalize(transform.position - _cameraTransform.transform.position) * 0.75f;
            _textCockpitComp.text = $"{(Type == "Moon" ? StellarBodyName : StellarBodyName.ToUpper())} \n {stellarBodyDistance} AU";

            /*cockpitLineRenderer.SetPosition(0, _radarUI.transform.position);
            cockpitLineRenderer.SetPosition(1, radarCockpitIcon.transform.position);*/
        }
        
        if (IsRadarFound && isIconSpawned && UseRadar)
        {

            float stellarBodyDistance = Mathf.Round(Vector3.Distance(_radarUI.transform.position, transform.position) / _currentScales.Orbit * 100f) /100f;

            radarIcon.SetActive(!(stellarBodyDistance > 1f && Type == "Moon"));

            radarIcon.transform.position = _radarUI.transform.position + Vector3.Normalize(transform.position - _radarUI.transform.position) * 0.1f;

            _textComp.text = $"{(Type == "Moon" ? StellarBodyName : StellarBodyName.ToUpper())} \n {stellarBodyDistance} AU";

        }
    }
}
