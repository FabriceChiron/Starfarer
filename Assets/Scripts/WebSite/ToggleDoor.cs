using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDoor : MonoBehaviour
{

    Animator doorAnimator;
    AudioSource audioSource;

    [SerializeField]
    private AudioClip sfxDoorOpen, sfxDoorClose;

    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        ToggleDoorAnimation(true, sfxDoorOpen);
    }

    void OnTriggerExit(Collider other)
    {
        ToggleDoorAnimation(false, sfxDoorClose);
    }

    void ToggleDoorAnimation(bool value, AudioClip sfx)
    {
        doorAnimator.SetBool("character_nearby", value);
        audioSource.clip = sfx;
        audioSource.Play();
    }
}
