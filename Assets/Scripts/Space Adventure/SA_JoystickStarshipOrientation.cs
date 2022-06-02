using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Autohand.Demo
{
    public class SA_JoystickStarshipOrientation : MonoBehaviour
    {
        public Transform spaceshipRoot;
        public float speed = 2;

        private float HorizontalAxis, VerticalAxis, HorizontalOrientationAxis, VerticalOrientationAxis;

        [SerializeField]
        private GetInputValues getInputValues;

        public float rotationSpeed = 2.0f;
        public float cameraSmooth = 4f;

        float rotationXTmp;
        float rotationZTmp;
        Rigidbody r;
        Quaternion lookRotation;
        float rotationX = 0;
        float rotationZ = 0;
        float orientationXSmooth = 0;
        float orientationYSmooth = 0;
        Vector3 defaultShipRotation;


        private void Start()
        {
            lookRotation = transform.rotation;
            defaultShipRotation = spaceshipRoot.localEulerAngles;
            rotationX = defaultShipRotation.y;
            rotationZ = defaultShipRotation.z;
        }

        void FixedUpdate()
        {


            HorizontalOrientationAxis = getInputValues.joystickValues.x;
            VerticalOrientationAxis = getInputValues.joystickValues.y;

            orientationXSmooth = Mathf.Lerp(orientationXSmooth, HorizontalOrientationAxis * rotationSpeed, Time.deltaTime * cameraSmooth);
            orientationYSmooth = Mathf.Lerp(orientationYSmooth, VerticalOrientationAxis * rotationSpeed, Time.deltaTime * cameraSmooth);

            //Debug.Log($"orientationXSmooth: {orientationXSmooth}");
            //Debug.Log($"orientationYSmooth: {orientationYSmooth}");

            Quaternion localRotation = Quaternion.Euler(-orientationYSmooth, orientationXSmooth, rotationZTmp * rotationSpeed);
            lookRotation = lookRotation * localRotation;

            //var moveAxis = new Vector3(axis.x * Time.deltaTime * speed, 0, axis.y * Time.deltaTime * speed);
            //spaceshipRoot.transform.localPosition += moveAxis;


            RotateShip();
        }

        private void RotateShip()
        {
            transform.rotation = lookRotation;
            rotationZ -= orientationXSmooth;
            //rotationZ = Mathf.Clamp(rotationZ, -45, 45);


            rotationX -= orientationYSmooth;
            //rotationX = Mathf.Clamp(rotationX, -45, 45);

            //spaceshipRoot.transform.localEulerAngles = new Vector3(rotationX, defaultShipRotation.y, rotationZ);
            spaceshipRoot.transform.localEulerAngles = new Vector3(rotationX, spaceshipRoot.transform.localEulerAngles.y, rotationZ);
            rotationZ = Mathf.Lerp(rotationZ, defaultShipRotation.z, Time.deltaTime * cameraSmooth);
            rotationX = Mathf.Lerp(rotationX, defaultShipRotation.y, Time.deltaTime * cameraSmooth);

            defaultShipRotation = spaceshipRoot.transform.localEulerAngles;
        }
    }
}