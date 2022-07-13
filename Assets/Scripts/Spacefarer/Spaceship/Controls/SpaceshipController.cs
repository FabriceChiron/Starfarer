using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using CW.Common;
using SpaceGraphicsToolkit;

[RequireComponent(typeof(Rigidbody))]

public class SpaceshipController : MonoBehaviour
{
    [Header("Normal Speed")]
    public float normalSpeed = 250000f;
    public float boostSpeed = 450000f;
    public float superBoostSpeed = 15000000f;
    public float strafeSpeed = 250000f;

    [Header("Slow Zone Speed")]
    public float normalSpeedSlowZone = 250f;
    public float boostSpeedSlowZone = 450f;
    public float superBoostSpeedSlowZone = 15000f;
    public float strafeSpeedSlowZone = 250f;

    [Header("Buffer Speed")]
    public float normalSpeedSlowZoneBuffer = 250f;
    public float boostSpeedSlowZoneBuffer = 450f;
    public float superBoostSpeedSlowZoneBuffer = 15000f;
    public float strafeSpeedSlowZoneBuffer = 250f;

    [Header("Current Speed")]
    [SerializeField]
    float speed;
    [SerializeField]
    float lateralSpeed;
    [SerializeField]
    float verticalSpeed;

    [Header("Distance to Warp Gate Center")]
    public float distance;
    
    [SerializeField]
    WarpGateCenter _warpGateCenter;
    public WarpGateCenter WarpGateCenter { get => _warpGateCenter; set => _warpGateCenter = value; }

    [Header("Necessary Game Objects")]
    public Transform cameraPosition;
    public Camera mainCamera;
    public Camera flatScreenCamera;
    public Camera XRCamera;
    public Transform spaceshipRoot, seatBase;

    [SerializeField]
    private BoolVariable _useXRStoredInfo, _useHandsStoredInfo;

    [SerializeField]
    private bool _useXR, _useHands;

    [SerializeField]
    private GetInputValues _XRControlValues;

    [SerializeField]
    private bool joystickGrabbed, throttleGrabbed;



    [Header("Damping")]
    public float rotationSpeed = 2.0f, warpRotationSpeed = 0.02f, currentRotationSpeed;
    public float cameraSmooth = 4f;


    [Header("Zones")]
    [SerializeField]
    private bool _isInSlowZone;
    [SerializeField]
    private bool _isInSlowZoneBuffer;

    [Header("Debug")]
    private RectTransform _infos;

    Rigidbody r;

    private SpaceshipWeapons _spaceshipWeapons;
    private SpaceshipAudio _spaceshipAudio;

    Quaternion lookRotation;

    float rotationZ = 0;
    float mouseXSmooth = 0;
    float mouseYSmooth = 0;
    Vector3 defaultShipRotation;

    Vector2 pitchYaw;

    float
        roll,
        rollXR,
        rollXRControls,
        forwardThrust,
        lateralThrust,
        lateralThrustXR,
        lateralThrustXRControls,
        upDownThrust,
        upDownThrustXR,
        upDownThrustXRControls;

    [SerializeField]
    private float
        progressiveForwardThrust,
        progressiveLateralThrust,
        progressiveUpDownThrust,
        smoothInputSpeed = 0.2f,
        smoothInputVelocity;

    [Range(-1f, 1f)]
    float forwardSpeedRatio;

    bool _boosting, _superBoosting; 
    
    [SerializeField]
    bool _invertYAxis;

    [SerializeField]
    private SgtFloatingWarpSmoothstep _warpSmoothstep;

    public Vector2 PitchYaw { get => pitchYaw; set => pitchYaw = value; }
    public float Roll { get => roll; set => roll = value; }
    public Quaternion LookRotation { get => lookRotation; set => lookRotation = value; }
    public bool IsInSlowZone { 
        get => _isInSlowZone;
        set
        {
            _isInSlowZone = value;
            
            if(_spaceshipWeapons != null)
            {
                _spaceshipWeapons.BlasterSpeed = (_isInSlowZone ? boostSpeedSlowZone : boostSpeed) * 1.5f;
            }
        }
    }
    public bool IsInSlowZoneBuffer { get => _isInSlowZoneBuffer; set => _isInSlowZoneBuffer = value; }
    public bool JoystickGrabbed { get => joystickGrabbed; set => joystickGrabbed = value; }
    public bool ThrottleGrabbed { get => throttleGrabbed; set => throttleGrabbed = value; }

    public bool UseXR
    {
        get => _useXR;
        set
        {
            if (_useXR != _useXRStoredInfo.BoolValue)
            {
                _useXRStoredInfo.BoolValue = value;
                mainCamera = value ? XRCamera : flatScreenCamera;
            }

            _useXR = value;

        }
    }

    public bool UseHands { get => _useHands; set => _useHands = value; }
    public float ForwardThrust { get => forwardThrust; set => forwardThrust = value; }
    public float LateralThrust { get => lateralThrust; set => lateralThrust = value; }
    public float UpDownThrust { get => upDownThrust; set => upDownThrust = value; }
    public float ForwardSpeedRatio { get => forwardSpeedRatio; set => forwardSpeedRatio = value; }
    public float ProgressiveForwardThrust { get => progressiveForwardThrust; set => progressiveForwardThrust = value; }
    public float ProgressiveLateralThrust { get => progressiveLateralThrust; set => progressiveLateralThrust = value; }
    public float ProgressiveUpDownThrust { get => progressiveUpDownThrust; set => progressiveUpDownThrust = value; }
    public bool Boosting { get => _boosting; set => _boosting = value; }
    public bool SuperBoosting { get => _superBoosting; set => _superBoosting = value; }

    private void OnEnable()
    {
        UseXR = _useXRStoredInfo.BoolValue;
    }

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
        r.useGravity = false;

        _spaceshipWeapons = GetComponent<SpaceshipWeapons>();
        _spaceshipAudio = GetComponent<SpaceshipAudio>();

        _warpSmoothstep = FindObjectOfType<SgtFloatingWarpSmoothstep>();

        LookRotation = transform.rotation;
        defaultShipRotation = spaceshipRoot.localEulerAngles;
        rotationZ = defaultShipRotation.z;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _infos = GetComponentInChildren<InfoValues>()?.GetComponent<RectTransform>();
    }


    private float GetMaxSpeed()
    {
        float maxSpeed;

        //maxSpeed = forwardThrust * (IsInSlowZone ? normalSpeedSlowZone : normalSpeed);
        //maxSpeed = ProgressiveForwardThrust * GetSpeedStage(normalSpeed, normalSpeedSlowZone);
        
        
        if(Mathf.Abs(ForwardThrust) > 0.025f) 
        {
            maxSpeed = GetSpeedStage(normalSpeed, normalSpeedSlowZone) * (ForwardThrust > 0 ? 1f : -1f);

        
            if (ForwardThrust > 0f)
            {
                if (SuperBoosting)
                {
                    //maxSpeed = forwardThrust * (IsInSlowZone ? superBoostSpeedSlowZone : superBoostSpeed);
                    maxSpeed = GetSpeedStage(superBoostSpeed, superBoostSpeedSlowZone);
                }
                else if (Boosting)
                {
                    //maxSpeed = forwardThrust * (IsInSlowZone ? boostSpeedSlowZone : boostSpeed);
                    maxSpeed = GetSpeedStage(boostSpeed, boostSpeedSlowZone);
                }

            }
        }
        else
        {
            maxSpeed = 0f;
        }


        return maxSpeed;
    }

    private float StopIfMinimumSpeed(float thisSpeed, string directionSpeed)
    {
        if (_isInSlowZone)
        {

        }
        return thisSpeed;
    }

    private float GetVerticalSpeed()
    {
        return UpDownThrust * GetSpeedStage(strafeSpeed, strafeSpeedSlowZone);
    }

    private float GetLateralSpeed()
    {
        return LateralThrust * GetSpeedStage(strafeSpeed, strafeSpeedSlowZone);
    }

    private float GetSpeedStage(float speed, float speedSlowZone)
    {
        return IsInSlowZone ?
            speedSlowZone :
            speed;
    }

    private float GetProgressiveThrust(float inputValue, float currentValue)
    {
        //if (1f - Mathf.Abs(currentValue) < 0.025f)
        //{
        //    currentValue = inputValue;
        //}
        //else if(Mathf.Abs(currentValue) > 0.025f)
        //{
        //    currentValue = inputValue;
        //}
        //currentValue = Mathf.MoveTowards(currentValue, inputValue, 0.25f * Time.deltaTime);

        currentValue = Mathf.SmoothDamp(currentValue, inputValue, ref smoothInputVelocity, smoothInputSpeed);

        return currentValue;
    }

    void FixedUpdate()
    {
        //Override Control inputs
        PitchYaw = JoystickGrabbed ? _XRControlValues.joystickValues.normalized * 0.3f : PitchYaw;
        ForwardThrust = ThrottleGrabbed ? _XRControlValues.throttleValue : ForwardThrust;

        ProgressiveForwardThrust = GetProgressiveThrust(ForwardThrust, ProgressiveForwardThrust);
        ProgressiveLateralThrust = GetProgressiveThrust(LateralThrust, ProgressiveLateralThrust);
        ProgressiveUpDownThrust = GetProgressiveThrust(UpDownThrust, ProgressiveUpDownThrust);

        if (IsInSlowZoneBuffer)
        {
            distance = (Vector3.Distance(transform.position, WarpGateCenter.transform.position) - 1000f) / 100;

            normalSpeedSlowZoneBuffer = normalSpeedSlowZone * distance;
            boostSpeedSlowZoneBuffer = boostSpeedSlowZone * distance;
            superBoostSpeedSlowZoneBuffer = superBoostSpeedSlowZone * distance;
        }
        
        speed = Mathf.Round(Mathf.Lerp(speed, GetMaxSpeed(), Time.deltaTime * 10f));
        //speed = GetProgressiveThrust(GetMaxSpeed(), speed);

        //ForwardSpeedRatio = (GetMaxSpeed() != 0f) ? speed / GetMaxSpeed() : 0f;

        lateralSpeed = Mathf.Round(Mathf.Lerp(lateralSpeed, GetLateralSpeed(), Time.deltaTime * 10f));
        //lateralSpeed = GetProgressiveThrust(GetLateralSpeed(), lateralSpeed);

        verticalSpeed = Mathf.Round(Mathf.Lerp(verticalSpeed, GetVerticalSpeed(), Time.deltaTime * 10f));
        //verticalSpeed = GetProgressiveThrust(GetVerticalSpeed(), verticalSpeed);

        //Set moveDirection to the vertical axis (up and down keys) * speed
        Vector3 moveDirection = new Vector3(lateralSpeed, verticalSpeed, speed);
        
        //Transform the vector3 to local space
        moveDirection = transform.TransformDirection(moveDirection);

        //Set the velocity, so you can move
        r.velocity = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);

        //Camera follow
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraPosition.position, Time.deltaTime * cameraSmooth);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, cameraPosition.rotation, Time.deltaTime * cameraSmooth);

        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, _warpSmoothstep.Warping ? warpRotationSpeed : rotationSpeed, Time.deltaTime * cameraSmooth);

        mouseXSmooth = Mathf.Lerp(mouseXSmooth, PitchYaw.x * currentRotationSpeed, Time.deltaTime * cameraSmooth);
        mouseYSmooth = Mathf.Lerp(mouseYSmooth, PitchYaw.y * (_invertYAxis ? -1f : 1f) * currentRotationSpeed, Time.deltaTime * cameraSmooth);
        
        Quaternion localRotation = Quaternion.Euler(-mouseYSmooth, mouseXSmooth, Roll * -1f * currentRotationSpeed);
        LookRotation = LookRotation * localRotation;
        transform.rotation = LookRotation;
        rotationZ -= mouseXSmooth;
        rotationZ = Mathf.Clamp(rotationZ, -30, 30);
        if (!UseXR)
        {
            seatBase.localEulerAngles = new Vector3(0f, 0f, rotationZ * -0.75f);
        }
        spaceshipRoot.localEulerAngles = new Vector3(defaultShipRotation.x, defaultShipRotation.y, rotationZ);
        rotationZ = Mathf.Lerp(rotationZ, defaultShipRotation.z, Time.deltaTime * cameraSmooth);

        if(_infos != null)
        {
            _infos.GetComponentsInChildren<Text>()[0].text = $"Joystick Value: \n{_XRControlValues.joystickValues}";
            _infos.GetComponentsInChildren<Text>()[1].text = $"PitchYaw: \n{PitchYaw}";
            _infos.GetComponentsInChildren<Text>()[2].text = $"Throttle Value: \n{_XRControlValues.throttleValue}";
            _infos.GetComponentsInChildren<Text>()[3].text = $"ForwardThrust: \n{ForwardThrust}";
        }
    }

    public void OnThrust(InputAction.CallbackContext context)
    {
        if (!ThrottleGrabbed)
        {
            ForwardThrust = context.ReadValue<Vector2>().y;
        }
    }

    public void OnStrafe(InputAction.CallbackContext context)
    {
        LateralThrust = context.ReadValue<Vector2>().x;
    }

    public void OnUpDown(InputAction.CallbackContext context)
    {
        UpDownThrust = context.ReadValue<float>();
    }

    public void OnUpDownStrafeXR(InputAction.CallbackContext context)
    {
        //upDownXR1D = context.ReadValue<Vector2>().y;
        //strafeXR1D = context.ReadValue<Vector2>().x;
        if (UseHands)
        {
            if (JoystickGrabbed)
            {
                LateralThrust = context.ReadValue<Vector2>().x;
                UpDownThrust = context.ReadValue<Vector2>().y;
            }
            else
            {

            }
        }
        else
        {
            PitchYaw = context.ReadValue<Vector2>();
        }
    }

    public void OnUpDownStrafeXRPressed(InputAction.CallbackContext context)
    {
        if (!UseHands && !JoystickGrabbed)
        {
            LateralThrust = context.ReadValue<Vector2>().x;
            UpDownThrust = context.ReadValue<Vector2>().y;
        }
    }

    public void OnThrustRollXR(InputAction.CallbackContext context)
    {
        //forwardThrust = context.ReadValue<Vector2>().y;
        if (ThrottleGrabbed)
        {
            ForwardThrust = _XRControlValues.throttleValue;
            Roll = context.ReadValue<Vector2>().x;
        }
    }

    //public void OnUpDownStrafe(InputAction.CallbackContext context)
    //{
    //    upDown1D = context.ReadValue<Vector2>().y;
    //    strafe1D = context.ReadValue<Vector2>().x;
    //}

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (!ThrottleGrabbed)
        {
            Roll = context.ReadValue<float>();
        }
    }

    //public void OnRollXR(InputAction.CallbackContext context)
    //{
    //    //Debug.Log($"OnRollXR {context.ReadValue<Vector2>().x}");
    //    rollXR1D = context.ReadValue<Vector2>().x;
    //}

    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        PitchYaw = context.ReadValue<Vector2>();
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        Boosting = context.performed;
    }

    public void OnSuperBoost(InputAction.CallbackContext context)
    {
        SuperBoosting = context.performed;
    }
}
