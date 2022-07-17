using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int Health
    {
        get => health;
        set
        {
            health = value;

            if (value <= 0)
            {
                isDead = true;
                DestroyAsteroid();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Health = initialHealth.IntValue;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        liveParts.GetComponent<Rigidbody>().mass = transform.localScale.x;

        if (isDead)
        {
            foreach(Rigidbody shardRigidbody in deadParts.GetComponentsInChildren<Rigidbody>())
            {
                shardRigidbody.mass = transform.localScale.x / deadParts.GetComponentsInChildren<Rigidbody>().Length;
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
