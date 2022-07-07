using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CW.Common;

[RequireComponent(typeof(CwFollow))]
public class AttachThrottle : MonoBehaviour
{
    CwFollow followComp;

    private void OnEnable()
    {
        followComp = GetComponent<CwFollow>();
        followComp.Target = FindObjectOfType<AnchorThrottle>().transform;
    }

    private void Start()
    {
        followComp.enabled = false;
    }
}
