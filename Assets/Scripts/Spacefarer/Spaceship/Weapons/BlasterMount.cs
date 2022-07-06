using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterMount : MonoBehaviour
{
    [SerializeField]
    private GameObject _blasterShotPrefab;

    [SerializeField]
    private float _blasterSpeed = 100f;

    public float BlasterSpeed { get => _blasterSpeed; set => _blasterSpeed = value; }

    public void Shoot()
    {
        BlasterShot blasterShot = Instantiate(_blasterShotPrefab, transform.position, transform.rotation).GetComponent<BlasterShot>();
        blasterShot.BlasterSpeed = BlasterSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
