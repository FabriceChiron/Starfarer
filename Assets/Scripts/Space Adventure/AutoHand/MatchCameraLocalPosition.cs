using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCameraLocalPosition : MonoBehaviour
{

    public Transform cameraToMatch;

    [SerializeField]
    private bool matchPosition = true;

    [SerializeField]
    private bool matchRotation = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (matchPosition)
        {
            transform.localPosition = cameraToMatch.localPosition;
        }

        if (matchRotation)
        {
            transform.localRotation = cameraToMatch.localRotation;  
        }
    }
}
