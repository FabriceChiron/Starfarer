using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterShot : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float _blasterSpeed;
    public float BlasterSpeed { get => _blasterSpeed; set => _blasterSpeed = value; }

    private float lifeTime = 3f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * BlasterSpeed * Time.deltaTime, Space.Self);

        lifeTime -= Time.deltaTime;

        if(lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
