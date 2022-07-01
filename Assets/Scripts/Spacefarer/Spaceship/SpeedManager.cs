using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using UnityEngine.InputSystem;

public class SpeedManager : MonoBehaviour
{
    public event EventHandler OnSphereEntered;

    // Start is called before the first frame update
    void Start()
    {
        OnSphereEntered += Testing_OnSphereEntered;
    }

    // Update is called once per frame
    void Update()
    {
        //if (input.SpacefighterControls.Warp.IsPressed())
        //{
        //    OnSphereEntered?.Invoke(this, EventArgs.Empty);
        //}
    }

    private void Testing_OnSphereEntered(object sender, EventArgs e)
    {
        Debug.Log("Sphere Entered");
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    public void OnWarp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSphereEntered?.Invoke(this, EventArgs.Empty);
        }
    }
}
