using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpaceGraphicsToolkit;

public class RaycastAim : MonoBehaviour
{
    LineRenderer _lineRenderer;

    public LayerMask IgnoreLayer;

    [SerializeField]
    private Transform _weaponsAim;

    [SerializeField]
    private float hitDistance;

    private Vector3 _initialAimPosition;

    [SerializeField]
    private RectTransform _infoAim;

    [SerializeField]
    private Text _infoAimText;

    [SerializeField]
    private SgtFloatingObject _aimTarget;

    public SgtFloatingObject AimTarget { get => _aimTarget; set => _aimTarget = value; }

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _initialAimPosition = _weaponsAim.localPosition;
        _infoAim = FindObjectOfType<AimingAt>().GetComponent<RectTransform>();
        if(_infoAim != null)
        {
            _infoAimText = _infoAim.GetComponent<Text>();
        }

        _weaponsAim.localPosition = new Vector3(0f, 0f, 1000f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Vector3 fwd = transform.TransformDirection(Vector3.forward);

        RaycastHit hit;

        if (Physics.Raycast(transform.localPosition, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, ~IgnoreLayer))
        {
            if(hit.transform.GetComponentInParent<SgtFloatingObject>() != null && hit.transform.GetComponentInParent<SgtFloatingObject>() != AimTarget)
            {
                AimTarget = hit.transform.GetComponentInParent<SgtFloatingObject>();

                //Vector3 direction = transform.position - AimTarget.transform.position;
                //_weaponsAim.position = transform.position + Vector3.Normalize(direction) * Vector3.Distance(transform.position, AimTarget.transform.position);
            }
            else if(hit.transform.GetComponentInChildren<SgtFloatingObject>() != null && hit.transform.GetComponentInChildren<SgtFloatingObject>() != AimTarget)
            {
                AimTarget = hit.transform.GetComponentInChildren<SgtFloatingObject>();
            }
            else
            {
                AimTarget = null;
                //_weaponsAim.localPosition = new Vector3(0f, 0f, 1000f);
            }

            //_lineRenderer?.SetPosition(0, transform.localPosition);
            //_lineRenderer?.SetPosition(1, hit.point);

            //_infoAimText.text = hit.transform.name;
        }
        else
        {
            AimTarget = null;
            //_weaponsAim.localPosition = new Vector3(0f, 0f, 1000f);

            //_lineRenderer?.SetPosition(0, transform.localPosition);
            //_lineRenderer?.SetPosition(1, new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 3f));

            //_infoAimText.text = "";
        }


        if(_infoAimText != null)
        {
            if (AimTarget != null)
            {
                _infoAimText.text = $"{AimTarget.name} \n{Mathf.Round(Vector3.Distance(transform.position, AimTarget.transform.position))}";
            }
            else
            {
                _infoAimText.text = "";
            }

        }

        /*if (Physics.Raycast(transform.localPosition, Vector3.forward, out hit, 1000.0f, ~IgnoreLayer))
        {

            hitDistance = hit.distance;

            //Debug.Log($"layer: {hit.transform.gameObject.layer} -- Spaceship layer: {LayerMask.NameToLayer("Spaceship")}");
            //print($"Found {hit.transform.name} - distance: " + hit.distance);
            //Debug.DrawLine(transform.localPosition, hit.point);
            _lineRenderer?.SetPosition(0, transform.localPosition);
            _lineRenderer?.SetPosition(1, hit.point);
            _weaponsAim.localPosition = hit.point;
        }
        else
        {
            _weaponsAim.localPosition = new Vector3(0f, 0f, 1000f);
            _lineRenderer?.SetPosition(0, transform.localPosition);
            _lineRenderer?.SetPosition(1, new Vector3(0f, 0f, 1000f));
        }*/
    }
}
