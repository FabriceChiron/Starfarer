using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDoor : MonoBehaviour
{

    Animator doorAnimator;
    
    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        ToggleDoorAnimation(true);
    }

    void OnTriggerExit(Collider other)
    {
        ToggleDoorAnimation(false);
    }

    void ToggleDoorAnimation(bool value)
    {
        doorAnimator.SetBool("character_nearby", value);
    }
}
