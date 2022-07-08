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

    [SerializeField]
    private GameObject _bolt, _explosion;

    private bool _hasExploded;

    void Start()
    {
        
    }

    void Explode()
    {
        lifeTime = 1f;
        _bolt.SetActive(false);
        _explosion.SetActive(true);
        _hasExploded = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_hasExploded)
        {
            transform.Translate(Vector3.forward * BlasterSpeed * Time.deltaTime, Space.Self);
        }

        lifeTime -= Time.deltaTime;

        if(lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision - Explode!");
        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger - Explode! against {other.gameObject.name}");
        Explode();
    }
}
