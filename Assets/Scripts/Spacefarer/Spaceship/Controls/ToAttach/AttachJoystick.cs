using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CW.Common;

[RequireComponent(typeof(CwFollow))]
public class AttachJoystick : MonoBehaviour
{
    CwFollow followComp;

    private void Awake()
    {
        followComp = GetComponent<CwFollow>();
        followComp.Target = FindObjectOfType<AnchorJoystick>().transform;
    }

    private void Start()
    {
        followComp.enabled = false;
    }
}
