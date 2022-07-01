using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SciFiShipController;

public class Boost : MonoBehaviour
{
    [SerializeField]
    PlayerInput _playerInput;

    [SerializeField]
    ShipControlModule _shipControlModule;

    [SerializeField]
    private bool boosting;

    // Start is called before the first frame update
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _shipControlModule = GetComponent<ShipControlModule>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boosting)
        {
            _shipControlModule.shipInstance.GetMaxThrust(1);
        }
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }
}
