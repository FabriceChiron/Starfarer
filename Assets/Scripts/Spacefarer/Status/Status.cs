using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField]
    private IntVariable initialHealth;

    [SerializeField]
    private int health;

    [SerializeField]
    private bool isDead;

    public int Health { 
        get => health;
        set { 
            health = value;
            
            if(value <= 0)
            {
                isDead = true;
            }
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        Health = initialHealth.IntValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
