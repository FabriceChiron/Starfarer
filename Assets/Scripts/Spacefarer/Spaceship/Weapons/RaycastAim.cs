using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAim : MonoBehaviour
{
    LineRenderer _lineRenderer;

    public LayerMask IgnoreLayer;

    [SerializeField]
    private Transform _weaponsAim;

    [SerializeField]
    private float hitDistance;

    private Vector3 _initialAimPosition;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _initialAimPosition = _weaponsAim.localPosition;

    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 fwd = transform.TransformDirection(Vector3.forward);

        RaycastHit hit;

        if (Physics.Raycast(transform.localPosition, Vector3.forward, out hit, 1000.0f, ~IgnoreLayer))
        {

            hitDistance = hit.distance;

            Debug.Log($"layer: {hit.transform.gameObject.layer} -- Spaceship layer: {LayerMask.NameToLayer("Spaceship")}");
            print($"Found {hit.transform.name} - distance: " + hit.distance);
            Debug.DrawLine(transform.localPosition, hit.point);
            _lineRenderer?.SetPosition(0, transform.localPosition);
            _lineRenderer?.SetPosition(1, hit.point);
            _weaponsAim.localPosition = hit.point;
        }
        else
        {
            _weaponsAim.localPosition = new Vector3(0f, 0f, 1000f);
            _lineRenderer?.SetPosition(0, transform.localPosition);
            _lineRenderer?.SetPosition(1, new Vector3(0f, 0f, 1000f));
        }
    }
}
