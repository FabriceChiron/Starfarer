using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastToRadar : MonoBehaviour
{

    private RadarUI _radarUI;

    private bool IsRadarFound;

    LayerMask mask;

    [SerializeField]
    private GameObject _radarPrefab;

    private LineRenderer _lineRenderer;


    public GameObject RadarPrefab { get => _radarPrefab; set => _radarPrefab = value; }

    private bool isIconSpawned;

    GameObject radarIcon;

    private int layer = 11;
    private LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        Physics.queriesHitBackfaces = true;

        layerMask = 1 << (int)layer;

        _lineRenderer = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        _radarUI = GameObject.FindObjectOfType<RadarUI>();
        

        if (_radarUI != null)
        {
            IsRadarFound = true;
        
            if(!isIconSpawned)
            {
                radarIcon = Instantiate(RadarPrefab, _radarUI.transform.GetChild(0));
                isIconSpawned = true;
            }
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

            radarIcon.transform.position = _radarUI.transform.position + Vector3.Normalize(transform.position - _radarUI.transform.position) * 0.1f;

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
