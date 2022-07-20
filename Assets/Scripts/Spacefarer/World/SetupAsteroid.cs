using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;
using SpaceGraphicsToolkit.Debris;

public class SetupAsteroid : MonoBehaviour
{
    [SerializeField]
    private IntVariable initialHealth;

    [SerializeField]
    private int health = 100;

    [SerializeField]
    private GameObject liveParts, deadParts;

    [SerializeField]
    private bool isDead;

    private float _radius, _power = 100.0F;

    public int Health
    {
        get => health;
        set
        {
            health = value;

            if (value <= 0)
            {
                isDead = true;
                GetComponent<SgtDebris>().Pool = false;
                DestroyAsteroid();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Health = initialHealth.IntValue;
        gameObject.AddComponent<SgtFloatingObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        liveParts.GetComponent<Rigidbody>().mass = transform.localScale.x;
        _radius = transform.localScale.x * 5f;
        _power = _radius * 50f;

        if (isDead)
        {
            foreach(Rigidbody shardRigidbody in deadParts.GetComponentsInChildren<Rigidbody>())
            {
                shardRigidbody.mass = transform.localScale.x / deadParts.GetComponentsInChildren<Rigidbody>().Length;

                shardRigidbody.AddExplosionForce(_power, transform.position, _radius, 0f);
            }
        }
    }

    void DestroyAsteroid()
    {
        Debug.Log($"Destroying asteroid!");
        liveParts.SetActive(false);
        deadParts.SetActive(true);
    }
}
