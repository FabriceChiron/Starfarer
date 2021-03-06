using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoHand;
using Autohand.Demo;


public class GetInputValues : MonoBehaviour
{
    [SerializeField]
    private JoystickAutohand Joystick;


    [SerializeField]
    private ThrottleAutohand Throttle;

    public Vector2 joystickValues;
    public float throttleValue;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        joystickValues = Joystick.GetValue();
        throttleValue = Throttle.GetValue();
    }
}


