using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float speed = 4f, rotation = 100f, curSpeed;


    [SerializeField]
    private float forwardMove, lateralMove;

    [SerializeField]
    private Vector2 lookOrientation;

    [SerializeField]
    private bool isSprinting;

    private Transform mainCamera;

    private void Start()
    {
        mainCamera = GetComponentInChildren<Camera>().transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        if (isSprinting)
        {
            curSpeed = speed * 1.5f;
        }
        else
        {
            curSpeed = speed;
        }

        
        //transform.Rotate(Vector3.up * rotation * Time.fixedDeltaTime * lateralMove);

        transform.Translate(Vector3.forward * curSpeed * Time.fixedDeltaTime * forwardMove);
        
        transform.Translate(Vector3.right * curSpeed * Time.fixedDeltaTime * lateralMove);


    }


    #region Input Methods
    public void OnMove(InputAction.CallbackContext context)
    {
        forwardMove = context.ReadValue<Vector2>().y;
        lateralMove = context.ReadValue<Vector2>().x;
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookOrientation = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.performed;
    }
    #endregion
}
