using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipWeapons : MonoBehaviour
{

    [SerializeField]
    private List<BlasterMount> _blasterMounts;

    [SerializeField]
    private float _delayBetweenShots = 0.25f;

    private float _nextShotTime;

    private int blasterIndex = 0;

    [SerializeField]
    private bool _isFiring;

    [SerializeField]
    private float _blasterSpeed = 1500f;

    public float BlasterSpeed { get => _blasterSpeed; set => _blasterSpeed = value; }



    // Start is called before the first frame update
    void Start()
    {
        //GetComponentInChildren<WeaponsContainer>().transform.SetParent(transform);

        foreach(BlasterMount blasterMount in GetComponentsInChildren<BlasterMount>())
        {
            _blasterMounts.Add(blasterMount);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (_isFiring)
        {
            FireBlaster();
        }
    }

    private void TriggerBlasterShot()
    {
        if(blasterIndex >= _blasterMounts.Count)
        {
            blasterIndex = 0;
        }

        _blasterMounts[blasterIndex].BlasterSpeed = BlasterSpeed;
        _blasterMounts[blasterIndex].Shoot();

        blasterIndex++;
    }

    private void FireBlaster()
    {
        if (Time.time >= _nextShotTime)
        {
            TriggerBlasterShot();
            _nextShotTime = Time.time + _delayBetweenShots;
        }
    }

    public void Firing(InputAction.CallbackContext context)
    {
        //context.performed;
        _isFiring = context.performed;

        if (!context.performed)
        {
            blasterIndex = 0;
        }
    }
}
