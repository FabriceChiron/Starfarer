using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachRadar : MonoBehaviour
{
    [SerializeField]
    RadarAnchor radarAnchor;

    private void OnEnable()
    {
        radarAnchor = transform.parent.GetComponentInChildren<RadarAnchor>();
        transform.position = radarAnchor.transform.position;
        transform.localRotation = radarAnchor.transform.localRotation;

        transform.GetChild(0).localEulerAngles = new Vector3(
            radarAnchor.transform.localEulerAngles.x * -1f, 
            radarAnchor.transform.localEulerAngles.y * -1f, 
            radarAnchor.transform.localEulerAngles.z * -1f
        );
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
