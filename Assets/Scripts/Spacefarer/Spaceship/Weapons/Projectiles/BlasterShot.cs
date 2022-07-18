using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;

public class BlasterShot : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float _blasterSpeed;
    public float BlasterSpeed { get => _blasterSpeed; set => _blasterSpeed = value; }
    public SpaceshipWeapons SpaceshipWeapons { get => _spaceshipWeapons; set => _spaceshipWeapons = value; }

    [SerializeField]
    private SpaceshipWeapons _spaceshipWeapons;

    private float lifeTime = 3f;

    public LayerMask IgnoreLayer;

    [SerializeField]
    private GameObject _bolt, _explosion;

    private bool _hasExploded;

    [SerializeField]
    private int damage = 25;


    private void Awake()
    {
        //gameObject.GetComponent<SgtFloatingObject>();
        gameObject.AddComponent<SgtFloatingObject>();
        _spaceshipWeapons = FindObjectOfType<SpaceshipWeapons>();
    }

    void Start()
    {
        //gameObject.GetComponent<SgtFloatingObject>();
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
            transform.Translate(Vector3.forward * _spaceshipWeapons.BlasterSpeed * Time.deltaTime, Space.Self);
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
        if(other.gameObject.layer != IgnoreLayer)
        {
            if(other.transform.GetComponentInParent<SetupAsteroid>() != null)
            {
                other.transform.GetComponentInParent<SetupAsteroid>().Health -= damage;
            }

            Debug.Log($"Trigger - Explode! against {other.gameObject.name}");
            Explode();
        }
    }
}
