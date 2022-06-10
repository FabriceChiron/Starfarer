using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float speed = 4f, rotation = 100f, curSpeed;



    // Update is called once per frame
    void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.LeftControl))
        {
            curSpeed = speed * 1.5f;
        }
        else
        {
            curSpeed = speed;
        }


        transform.Rotate(Vector3.up * rotation * Time.fixedDeltaTime * Input.GetAxis("Horizontal"));

        transform.Translate(Vector3.forward * curSpeed * Time.fixedDeltaTime * Input.GetAxis("Vertical"));


    }
}
