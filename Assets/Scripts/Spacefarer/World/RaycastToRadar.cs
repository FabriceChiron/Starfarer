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


    public GameObject RadarPrefab { get => _radarPrefab; set => _radarPrefab = value; }
    public string StellarBodyName { get => stellarBodyName; set => stellarBodyName = value; }
    public Scales CurrentScales { get => _currentScales; set => _currentScales = value; }
    public string Type { get => _type; set => _type = value; }

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
        

        if (_radarUI != null)
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

        if(_spaceship != null)
        {
            IsSpaceshipFound = true;
            _cameraTransform = _spaceship.transform.GetComponentInChildren<Camera>().transform;

            //Debug.Log(_spaceship.transform.GetComponentInChildren<Camera>());

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

        if(IsSpaceshipFound && isCockpitIconSpawned)
        {

            //if (Physics.Raycast(_cameraTransform.position, transform.position - _cameraTransform.position, out RaycastHit hitinfo, Mathf.Infinity))
            //{
                //Debug.Log($"{transform.parent.name} hit: {hitinfo.transform.name}");
                //_lineRenderer.SetPosition(0, _cameraTransform.transform.position);
                //_lineRenderer.SetPosition(1, hitinfo.point);
                //Debug.DrawRay(_cameraTransform.position, transform.position - _cameraTransform.position * hitinfo.distance, Color.red);

                float stellarBodyDistance = Mathf.Round(Vector3.Distance(_cameraTransform.position, transform.position) / _currentScales.Orbit * 100f) / 100f;

                radarCockpitIcon.SetActive(!(stellarBodyDistance > 1f && Type == "Moon"));


                radarCockpitIcon.transform.position = _cameraTransform.position + Vector3.Normalize(transform.position - _cameraTransform.transform.position) * 0.75f;
                _textCockpitComp.text = $"{(Type == "Moon" ? StellarBodyName : StellarBodyName.ToUpper())} \n {stellarBodyDistance} AU";

                cockpitLineRenderer.SetPosition(0, _radarUI.transform.position);
                cockpitLineRenderer.SetPosition(1, radarCockpitIcon.transform.position);
            //}
        }
        
        if (IsRadarFound && isIconSpawned)
        {
            /*if(Physics.Raycast(_radarUI.transform.position, transform.position - _radarUI.transform.position, out RaycastHit hitinfo, 0.3f))
            {
                Debug.Log($"{transform.parent.name} hit: {hitinfo.transform.name}");
                _lineRenderer.SetPosition(0, _radarUI.transform.position);
                _lineRenderer.SetPosition(1, hitinfo.point);
                //Debug.DrawRay(transform.position, _radarUI.transform.position - transform.position * hitinfo.distance, Color.red);
            }
            else
            {
            }*/
            //_lineRenderer.SetPosition(0, _radarUI.transform.position);
            //_lineRenderer.SetPosition(1, _radarUI.transform.position + Vector3.Normalize(transform.position - _radarUI.transform.position) * 0.15f);

            float stellarBodyDistance = Mathf.Round(Vector3.Distance(_radarUI.transform.position, transform.position) / _currentScales.Orbit * 100f) /100f;

            radarIcon.SetActive(!(stellarBodyDistance > 1f && Type == "Moon"));

            radarIcon.transform.position = _radarUI.transform.position + Vector3.Normalize(transform.position - _radarUI.transform.position) * 0.1f;
            _textComp.text = $"{(Type == "Moon" ? StellarBodyName : StellarBodyName.ToUpper())} \n {stellarBodyDistance} AU";


            /*Vector3 raycastDir = transform.position - _radarUI.transform.position;
            RaycastHit raycastHit;

            Ray ray = new(_radarUI.transform.position, raycastDir);

            Vector3 endPosition = _radarUI.transform.position;

            if(Physics.Raycast(ray, out raycastHit, Mathf.Infinity, ~layerMask, QueryTriggerInteraction.Collide)){
                
                endPosition = raycastHit.point;
                Debug.Log($"{transform.parent.name}'s raycast hit: {raycastHit.transform.name}");
            }

            //Debug.Log($"{transform.parent.name} distance: {Vector3.Distance(transform.position, endPosition)}");


            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, endPosition);*/
        }
    }
}
