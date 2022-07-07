using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CW.Common;

[RequireComponent(typeof(CwFollow))]
public class AttachButton : MonoBehaviour
{
    CwFollow followComp;
 
    private void Awake()
    {
        followComp = GetComponent<CwFollow>();
        followComp.Target = FindObjectOfType<AnchorButton>().transform;
    }

    private void Start()
    {
        followComp.enabled = false;
    }
}
