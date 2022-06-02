using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum gameModes
{
    PILOTING,
    ROAMING
}

public class ToggleModes : MonoBehaviour
{

    [SerializeField]
    private gameModes _gameMode;

    public gameModes GameMode { get => _gameMode; set => _gameMode = value; }

    [SerializeField]
    private GameObject[] pilotingGameObjects, roamingGameObjects;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchModes()
    {
        if(GameMode == gameModes.PILOTING)
        {
            enableRoaming();
        }

        if (GameMode == gameModes.ROAMING)
        {
            enablePiloting();
        }
    }

    public void enableRoaming()
    {

        if (GameMode == gameModes.PILOTING)
        {
            Debug.Log("enableRoaming");

            transform.localRotation = Quaternion.Euler(Vector3.zero);
            rb.isKinematic = true;
            rb.freezeRotation = true;

            GameMode = gameModes.ROAMING;

            foreach (GameObject gameObject in pilotingGameObjects)
            {
                gameObject.SetActive(false);
            }
            foreach (GameObject gameObject in roamingGameObjects)
            {
                gameObject.SetActive(true);
            }

            GetComponent<Spaceship>().enabled = false;
        }

        
    }

    public void enablePiloting()
    {
        if (GameMode == gameModes.ROAMING)
        {
            Debug.Log("enablePiloting");

            GameMode = gameModes.PILOTING;

            foreach (GameObject gameObject in pilotingGameObjects)
            {
                gameObject.SetActive(true);
            }
            foreach (GameObject gameObject in roamingGameObjects)
            {
                gameObject.SetActive(false);
            }

            GetComponent<Spaceship>().enabled = true;
        }
        
    }
}
