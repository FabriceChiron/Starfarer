using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CW.Common;
using SpaceGraphicsToolkit;

[RequireComponent(typeof(Rigidbody))]

public class SpaceshipController : MonoBehaviour
{
    public float normalSpeed = 250000f;
    public float boostSpeed = 450000f;
    public float superBoostSpeed = 15000000f;

    public float strafeSpeed = 250000f;

    public Transform cameraPosition;
    public Camera mainCamera;
    public Transform spaceshipRoot, seatBase;
    public float rotationSpeed = 2.0f, warpRotationSpeed = 0.02f, currentRotationSpeed;
    public float cameraSmooth = 4f;
    public RectTransform crosshairTexture;

    [SerializeField]
    float pitchYawMagnitude;

    [SerializeField]
    float speed, lateralSpeed;
    
    Rigidbody r;

    [SerializeField]
    Quaternion lookRotation;

    float rotationZ = 0;
    float mouseXSmooth = 0;
    float mouseYSmooth = 0;
    Vector3 defaultShipRotation;

    [SerializeField]
    Vector2 pitchYaw;

    [SerializeField]
    float roll, forwardThrust, lateralThrust;

    [SerializeField]
    bool _boosting, _warpButtonPressed, _warping, _invertYAxis;

    [SerializeField]
    private SgtFloatingWarpSmoothstep _warpSmoothstep;

    public Vector2 PitchYaw { get => pitchYaw; set => pitchYaw = value; }

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
        r.useGravity = false;

        _warpSmoothstep = FindObjectOfType<SgtFloatingWarpSmoothstep>();

        lookRotation = transform.rotation;
        defaultShipRotation = spaceshipRoot.localEulerAngles;
        rotationZ = defaultShipRotation.z;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private float GetMaxSpeed()
    {
        float maxSpeed;

        maxSpeed = forwardThrust * normalSpeed;

        if(forwardThrust > 0f)
        {
            if (_warping)
            {
                maxSpeed = forwardThrust * superBoostSpeed;
            }
            else if (_boosting)
            {
                maxSpeed = forwardThrust * boostSpeed;
            }

        }

        return maxSpeed;
    }

    private float GetLateralSpeed()
    {
        return lateralThrust * strafeSpeed;
    }

    void FixedUpdate()
    {
        
        speed = Mathf.Round(Mathf.Lerp(speed, GetMaxSpeed(), Time.deltaTime * 3f));

        lateralSpeed = Mathf.Round(Mathf.Lerp(lateralSpeed, GetLateralSpeed(), Time.deltaTime * 3f));

        //Set moveDirection to the vertical axis (up and down keys) * speed
        Vector3 moveDirection = new Vector3(lateralSpeed, 0, speed);

        //Transform the vector3 to local space
        moveDirection = transform.TransformDirection(moveDirection);

        //Set the velocity, so you can move
        r.velocity = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);

        //Camera follow
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraPosition.position, Time.deltaTime * cameraSmooth);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, cameraPosition.rotation, Time.deltaTime * cameraSmooth);


        //currentRotationSpeed = _warpSmoothstep.Warping ? warpRotationSpeed : rotationSpeed;

        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, _warpSmoothstep.Warping ? warpRotationSpeed : rotationSpeed, Time.deltaTime * cameraSmooth);

        mouseXSmooth = Mathf.Lerp(mouseXSmooth, PitchYaw.x * currentRotationSpeed, Time.deltaTime * cameraSmooth);
        mouseYSmooth = Mathf.Lerp(mouseYSmooth, PitchYaw.y * (_invertYAxis ? -1f : 1f) * currentRotationSpeed, Time.deltaTime * cameraSmooth);
        
        Quaternion localRotation = Quaternion.Euler(-mouseYSmooth, mouseXSmooth, roll * -1f * currentRotationSpeed);
        lookRotation = lookRotation * localRotation;
        transform.rotation = lookRotation;
        rotationZ -= mouseXSmooth;
        rotationZ = Mathf.Clamp(rotationZ, -45, 45);
        seatBase.localEulerAngles = new Vector3(0f, 0f, rotationZ * -0.75f);
        spaceshipRoot.localEulerAngles = new Vector3(defaultShipRotation.x, defaultShipRotation.y, rotationZ);
        rotationZ = Mathf.Lerp(rotationZ, defaultShipRotation.z, Time.deltaTime * cameraSmooth);
    }


    public void OnThrust(InputAction.CallbackContext context)
    {
        forwardThrust = context.ReadValue<Vector2>().y;
        //forwardThrust = Mathf.Lerp(forwardThrust, context.ReadValue<Vector2>().y, Time.deltaTime * 10);
    }

    public void OnStrafe(InputAction.CallbackContext context)
    {
        lateralThrust = context.ReadValue<Vector2>().x;
        //lateralThrust = Mathf.Lerp(lateralThrust, context.ReadValue<Vector2>().x, Time.deltaTime * 10);
    }

    public void OnUpDown(InputAction.CallbackContext context)
    {
        //context.ReadValue<float>();
    }

    //public void OnUpDownStrafeXR(InputAction.CallbackContext context)
    //{
    //    upDownXR1D = context.ReadValue<Vector2>().y;
    //    strafeXR1D = context.ReadValue<Vector2>().x;
    //}

    //public void OnUpDownStrafe(InputAction.CallbackContext context)
    //{
    //    upDown1D = context.ReadValue<Vector2>().y;
    //    strafe1D = context.ReadValue<Vector2>().x;
    //}

    public void OnRoll(InputAction.CallbackContext context)
    {
        roll = context.ReadValue<float>();
        //context.ReadValue<float>();
    }

    //public void OnRollXR(InputAction.CallbackContext context)
    //{
    //    //Debug.Log($"OnRollXR {context.ReadValue<Vector2>().x}");
    //    rollXR1D = context.ReadValue<Vector2>().x;
    //}

    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        PitchYaw = context.ReadValue<Vector2>();

        pitchYawMagnitude = PitchYaw.magnitude;
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        //context.performed;
        _boosting = context.performed;
    }

    public void OnWarp(InputAction.CallbackContext context)
    {
        //context.performed;
        _warpButtonPressed = context.started;
        _warping = context.performed;
    }
}
