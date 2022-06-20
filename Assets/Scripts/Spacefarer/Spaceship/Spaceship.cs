using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Autohand.Demo;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using SpaceGraphicsToolkit;
using CW.Common;

public enum controlType
{
    PHYSICS,
    TRANSFORM
}

[RequireComponent (typeof(Rigidbody))]
public class Spaceship : MonoBehaviour
{

    [SerializeField]
    private controlType _controlType;

    [SerializeField]
    private bool _useXR;

    [SerializeField]
    private StellarSystemGenerator stellarSystemGenerator;

    [SerializeField]
    private PlayerInput _playerInput;

    [SerializeField]
    private List<Transform> _playerAnchors;

    [SerializeField]
    private float lockPitchYawDurationOnStart = 2f;


    [Header("=== Ship Movement Settings ===")]
    [SerializeField]
    private float yawTorque = 500f;
    [SerializeField]
    private float pitchTorque = 1000f;
    [SerializeField]
    private float rollTorque = 1000f;
    [SerializeField]
    private float thrust= 100f;
    [SerializeField]
    private float upThrust= 50f;
    [SerializeField]
    private float strafeThrust= 50f;

    [Header("=== Boost Settings ===")]
    [SerializeField]
    private float maxBoostAmount = 2f;
    [SerializeField]
    private float boostDeprecationRate = 0.25f;
    [SerializeField]
    private float boostRechargeRate = 0.5f;
    [SerializeField]
    private float boostMultiplier = 5f;


    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftRightGlideReduction = 0.111f;
    float glide, verticalGlide, horizontalGlide = 0f;


    /* TRANSFORM CONTROLS */

    [Header("=== Transform controls ===")]
    public float normalSpeed = 25f;
    public float accelerationSpeed = 45f;
    public float warpSpeed = 1000f;
    public float strafeSpeed = 10f;
    public float upDownSpeed = 10f;

    [SerializeField]
    private float _maxSpeed;
    public Transform spaceshipRoot;
    public float rotationSpeed = 2.0f;
    public float cameraSmoothFlat = 0.1f;
    public float cameraSmoothXR = 2f;
    public float cameraSmooth;
    private float throttleAmount;
    private float verticalAxis;
    private bool wasBoosting, wasWarping;
    private bool _freelook;
    private bool _isCameraAligned = true;

    public RectTransform crosshairTexture;

    [SerializeField]
    private float _timeToMaxSpeed = 3f;
    //private float _resetTimeToMaxSpeed;

    float speed;
    Rigidbody r;
    [SerializeField]
    Quaternion lookRotation;
    Quaternion cameraLookRotation;
    
    [SerializeField]
    float rotationZ = 0f, rotationZTmp, mouseXSmooth = 0f, mouseYSmooth = 0f;
    [SerializeField]
    Vector3 defaultShipRotation;

    public float TimeToMaxSpeed { get => _timeToMaxSpeed; set => _timeToMaxSpeed = value; }


    /* /TRANSFORM CONTROLS/ */

    public bool boosting, warping, warpingReleased, warpingStarted, _showCockpitRadar, _enableGravity, _useRadar;

    [SerializeField]
    private GameObject _radar;

    [SerializeField]
    private GetInputValues getInputValues;

    [SerializeField]
    private List<GameObject> forVR, forFlatScreens;

    [SerializeField]
    private Camera _cameraVR, _cameraFlat;

    [SerializeField]
    private bool joystickGrabbed, throttleGrabbed, spaceShipLocked;

    Rigidbody rb;

    // Input Values
    [SerializeField]
    private float thrust1D, thrust1DInput, upDown1D, upDownFlat1D, upDownXR1D, strafe1D, strafeFlat1D, strafeXR1D, roll1D, rollXR1D;

    [SerializeField]
    private Vector2 pitchYaw;

    public bool ThrottleGrabbed { get => throttleGrabbed; set => throttleGrabbed = value; }
    public bool JoystickGrabbed { get => joystickGrabbed; set => joystickGrabbed = value; }
    public bool SpaceShipLocked { get => spaceShipLocked; set => spaceShipLocked = value; }
    public bool UseXR { get => _useXR; set => _useXR = value; }
    public bool ShowCockpitRadar { get => _showCockpitRadar; set => _showCockpitRadar = value; }
    public bool EnableGravity { get => _enableGravity; set => _enableGravity = value; }
    public bool UseRadar { get => _useRadar; set => _useRadar = value; }
    public StellarSystemGenerator StellarSystemGenerator { get => stellarSystemGenerator; set => stellarSystemGenerator = value; }
    public Camera CameraVR { get => _cameraVR; set => _cameraVR = value; }
    public Camera CameraFlat { get => _cameraFlat; set => _cameraFlat = value; }
    public Vector2 PitchYaw { get => pitchYaw; set => pitchYaw = value; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        _radar.SetActive(UseRadar);

        StellarSystemGenerator _stellarSystemGenerator = GameObject.FindObjectOfType<StellarSystemGenerator>();

        if (EnableGravity)
        {
            transform.gameObject.AddComponent<SgtGravityReceiver>();
        }

        if (_stellarSystemGenerator != null)
        {
            UseXR = _stellarSystemGenerator.UseXR;
        }

        if (UseXR)
        {
            foreach(Transform trackerOffset in _playerAnchors)
            {
                trackerOffset.localPosition = new Vector3(0f, trackerOffset.localPosition.y, 0f);
                trackerOffset.parent.gameObject.SetActive(true);
            }
        }

        /* TRANSFORM CONTROLS */
        lookRotation = transform.rotation;
        defaultShipRotation = spaceshipRoot.localEulerAngles;
        rotationZ = defaultShipRotation.z;
        /* /TRANSFORM CONTROLS/ */

        //Debug.Log($"XR device active: {XRSettings.isDeviceActive}");

        foreach (GameObject vrGameObject in forVR)
        {
            vrGameObject.SetActive(UseXR);
        }

        foreach (GameObject flatScreenGameObject in forFlatScreens)
        {
            flatScreenGameObject.SetActive(!UseXR);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void OnEnable()
    {
    }

    private void Update()
    {
        rotationZTmp = ((roll1D != 0) ? roll1D : rollXR1D) * -1f;

        ApllyForces();

        //ManageWarp();
        cameraSmooth = (JoystickGrabbed) ? cameraSmoothXR : cameraSmoothFlat;
    }

    void FixedUpdate()
    {
        /*if(ThrottleGrabbed)
        {
            thrust1D = getInputValues.throttleValue;
        }*/
        
        if (!SpaceShipLocked)
        {
            lockPitchYawDurationOnStart -= Time.deltaTime;
        
            if(lockPitchYawDurationOnStart < 0f)
            {
                UnlockSpaceShip();
            }
        }


        //ApllyForces();


        if (JoystickGrabbed && !SpaceShipLocked && UseXR)
        {
            PitchYaw = getInputValues.joystickValues.normalized;
        }

        switch (_controlType)
        {
            case controlType.PHYSICS:
                HandleMovement();
                break;

            case controlType.TRANSFORM:
                ApplyThrust();

                //rotationZTmp = ((roll1D != 0) ? roll1D : rollXR1D) * -1f;


                mouseXSmooth = Mathf.Lerp(mouseXSmooth, PitchYaw.x * rotationSpeed, Time.deltaTime * cameraSmooth);
                //Debug.Log($"pitchYaw.x: {pitchYaw.x}");
                //Debug.Log($"mouseXSmooth: {mouseXSmooth}");

                mouseXSmooth = Mathf.Round(mouseXSmooth * 100f) / 100f;

                mouseYSmooth = Mathf.Lerp(mouseYSmooth, -PitchYaw.y * rotationSpeed, Time.deltaTime * cameraSmooth);

                mouseYSmooth = Mathf.Round(mouseYSmooth * 100f) / 100f;

                Quaternion localRotation = Quaternion.Euler(-mouseYSmooth, mouseXSmooth, rotationZTmp * rotationSpeed);
                lookRotation = lookRotation * localRotation;

                RotateShip();

                Vector3 moveDirection = new Vector3(strafe1D * strafeSpeed, upDown1D * upDownSpeed, speed);
                moveDirection = transform.TransformDirection(moveDirection);
                rb.velocity = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);

                wasBoosting = boosting && speed > normalSpeed;
                wasWarping = warping && speed > accelerationSpeed;

                break;
        }
    }

    private void UnlockSpaceShip()
    {
        SgtFloatingOrbit spaceShipOrbitComp = transform.GetComponent<SgtFloatingOrbit>();

        if(spaceShipOrbitComp != null)
        {
            spaceShipOrbitComp.enabled = false;
            Destroy(spaceShipOrbitComp);
        }

        CwFollow spaceShipFollowComp = transform.GetComponent<CwFollow>();

        if(spaceShipFollowComp != null)
        {
            spaceShipFollowComp.enabled = false;
        }

        //transform.GetComponent<SgtFloatingCamera>().enabled = true;
    }

    void ManageWarp()
    {
        if (warpingStarted)
        {
            warping = true;
        }

        if (warpingReleased)
        {
            warping = false;
        }

    }

    private void ApllyForces()
    {
        
            if(getInputValues != null)
            {
                thrust1D = (Mathf.Abs(getInputValues.throttleValue) >= Mathf.Abs(thrust1DInput)) ? getInputValues.throttleValue : thrust1DInput;
            }
            else
            {
                thrust1D = thrust1DInput;
            }
        
        //strafe1D = (Mathf.Abs(strafeXR1D) >= Mathf.Abs(strafeFlat1D)) ? strafeXR1D : strafeFlat1D;
        //upDown1D = (Mathf.Abs(upDownXR1D) >= Mathf.Abs(upDownFlat1D)) ? upDownXR1D : upDownFlat1D;

        strafe1D = (strafeFlat1D != 0) ? strafeFlat1D : strafeXR1D;
        upDown1D = (upDownFlat1D != 0) ? upDownFlat1D : upDownXR1D;
    }

    void RotateShip()
    {
        transform.rotation = lookRotation;
        rotationZ -= mouseXSmooth;
        rotationZ = Mathf.Clamp(rotationZ, -45, 45);
        spaceshipRoot.transform.localEulerAngles = new Vector3(defaultShipRotation.x, defaultShipRotation.y, rotationZ);
        rotationZ = Mathf.Lerp(rotationZ, defaultShipRotation.z, Time.deltaTime * cameraSmooth);
    }

    void ApplyThrust()
    {

        //speed = thrust1D;

        //if(boosting)
        //{
        //    speed = Mathf.Lerp(speed, accelerationSpeed, Time.deltaTime * 3);
        //}

        //else if(warping)
        //{
        //    speed = Mathf.Lerp(speed, warpSpeed, Time.deltaTime * 3);
        //}

        //else
        //{
        //    speed = Mathf.Lerp(speed, normalSpeed, Time.deltaTime * 10);
        //}


        //if (thrust1D > 0)
        //{
        //    if (boosting)
        //    {

        //    }
        //}

        if (thrust1D > 0)
        {
            float maxSpeed = GoToSpeed(
                speed,
                warping ?
                    warpSpeed :
                    boosting ?
                        accelerationSpeed :
                        normalSpeed,
                3f);

            _timeToMaxSpeed -= Time.deltaTime;

            speed += thrust1D *
                Time.deltaTime *
                Mathf.Max(
                    speed,
                    wasWarping ?
                        warpSpeed :
                        wasBoosting ?
                            accelerationSpeed :
                            normalSpeed);

            if (speed <= 50f)
            {
                speed = 50f;
            }
            if (speed >= maxSpeed)
            {
                speed = maxSpeed;
            }
        }

        else if (thrust1D < 0)
        {
            speed += thrust1D * Time.deltaTime * Mathf.Min(
                    speed, normalSpeed * -1f);

            //if (speed >= -125f)
            //{
            //    speed = -125f;
            //}
        }

        else
        {
            speed = GoToSpeed(speed, 0f, 3f);
        }

        
    }

    float GoToSpeed(float thisSpeed, float targetSpeed, float thrust)
    {
        if (Mathf.Abs(thisSpeed) - targetSpeed <= 0.1)
        {
            thisSpeed = targetSpeed;
        }
        else
        {
            thisSpeed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * thrust);
        }

        return thisSpeed;
    }

    void HandleMovement()
    {
        // Roll
        rb.AddRelativeTorque(Vector3.back * roll1D * rollTorque * Time.deltaTime);
        // Pitch
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(PitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);
        // Yaw
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(PitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);


        // THRUST
        if(thrust1D > 0.1f || thrust1D < -0.1f)
        {
            float currentThrust = thrust;

            rb.AddRelativeForce(Vector3.forward * thrust1D * currentThrust * Time.deltaTime);

            glide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;
        }

        // UP/DOWN
        if(upDown1D > 0.1f || upDown1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.up * upDown1D * upThrust * Time.deltaTime);

            verticalGlide = upDown1D * upThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.deltaTime);
            verticalGlide *= upDownGlideReduction;
        }

        // STRAFING
        if(strafe1D > 0.1f || strafe1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.right * strafe1D * strafeThrust * Time.deltaTime);

            horizontalGlide = strafe1D * strafeThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.deltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }


    }

    #region Input Methods
    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1DInput =  context.ReadValue<float>();
    }
    public void OnThrustRoll(InputAction.CallbackContext context)
    {
        thrust1DInput = context.ReadValue<Vector2>().y;
        roll1D = context.ReadValue<Vector2>().x;
    }

    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }

    public void OnUpDown(InputAction.CallbackContext context)
    {
        upDown1D = context.ReadValue<float>();
    }

    public void OnUpDownStrafeXR(InputAction.CallbackContext context)
    {
        upDownXR1D = context.ReadValue<Vector2>().y;
        strafeXR1D = context.ReadValue<Vector2>().x;
    }

    public void OnUpDownStrafe(InputAction.CallbackContext context)
    {
        upDown1D = context.ReadValue<Vector2>().y;
        strafe1D = context.ReadValue<Vector2>().x;
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        //Debug.Log($"OnRoll {context.ReadValue<float>()}");
        roll1D = context.ReadValue<float>();
    }

    public void OnRollXR(InputAction.CallbackContext context)
    {
        //Debug.Log($"OnRollXR {context.ReadValue<Vector2>().x}");
        rollXR1D = context.ReadValue<Vector2>().x;
    }

    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        if(lockPitchYawDurationOnStart < 0f)
        {
            PitchYaw = (JoystickGrabbed) ? getInputValues.joystickValues :  context.ReadValue<Vector2>();
            PitchYaw = PitchYaw.normalized;
        }
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }

    public void OnWarp(InputAction.CallbackContext context)
    {
        //warpingStarted = context.performed;
        warping = context.performed;
        //warpingStarted = context.started;
        //warpingReleased = context.canceled;
    }

    public void OnWarpReleased(InputAction.CallbackContext context)
    {
        //warpingReleased = context.performed;
        warping = !context.performed;
    }

    public void OnToggleCockpitRadar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ShowCockpitRadar = !ShowCockpitRadar;
        }
    }
    #endregion
}
