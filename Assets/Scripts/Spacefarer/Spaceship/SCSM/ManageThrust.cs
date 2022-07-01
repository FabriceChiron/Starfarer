using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SciFiShipController;

public class ManageThrust : MonoBehaviour
{
    [SerializeField]
    PlayerInput _playerInput;

    [SerializeField]
    ShipControlModule _shipControlModule;

    [SerializeField]
    Ship _shipInstance;

    [SerializeField]
    private bool _isInSlowZone, _isBoosting;

    [SerializeField]
    private float
        initialForwardMaxThrust = (int)1e+07,
        boostForce = (float)1e+12,
        boostDuration = Mathf.Infinity,
        newMaxThrust, 
        mediumDensityChangeDuration = 1f;

    [SerializeField]
    private int 
        boostMultiplier = 3;

    [SerializeField]
    float currentLerpTime;

    public bool IsBoosting
    {
        get => _isBoosting;
        set { 
            if(_isBoosting != value)
            {
                currentLerpTime = 0f;
            }
            _isBoosting = value; 
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {

        _playerInput = GetComponent<PlayerInput>();
        _shipControlModule = GetComponent<ShipControlModule>();
        _shipInstance = _shipControlModule.shipInstance;

        initialForwardMaxThrust = _shipInstance.GetMaxThrust(1);

        Debug.Log($"Start: {initialForwardMaxThrust}");
    }

    // Update is called once per frame
    void Update()
    {

        currentLerpTime += Time.deltaTime;
        if(currentLerpTime > mediumDensityChangeDuration)
        {
            currentLerpTime = mediumDensityChangeDuration;
        }

        float perc = currentLerpTime / mediumDensityChangeDuration;
        _shipInstance.mediumDensity = Mathf.Lerp(_shipInstance.mediumDensity, IsBoosting ? 0.001f : 1.293f, perc);


        //float newMaxThrustRound = Mathf.Round(Mathf.Lerp(initialForwardMaxThrust, IsBoosting ? initialForwardMaxThrust * boostMultiplier : initialForwardMaxThrust, perc));

        //_shipInstance.SetMaxThrust(1, Mathf.RoundToInt(newMaxThrust * 1000000000));

        //_shipInstance.SetMaxThrust(1, IsBoosting ? boostForwardMaxThrust : initialForwardMaxThrust);

        //_shipInstance.mediumDensity = Mathf.Lerp(_shipInstance.mediumDensity, IsBoosting ? 0.001f : 1.293f, ;

        /*if (boosting)
        {
            _shipControlModule.shipInstance.AddBoost(new Vector3(0,0,1), boostForce, boostDuration);
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("SlowZone"))
        {
            _isInSlowZone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("SlowZone"))
        {
            _isInSlowZone = false;
        }
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        IsBoosting = context.performed;

        if (context.performed)
        {
            _shipInstance.AddBoost(new Vector3(0, 0, 1), boostForce, boostDuration);
        }
        else
        {
            _shipInstance.StopBoost();
        }
    }
}
