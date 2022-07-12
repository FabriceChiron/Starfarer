using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;

public class AsteroidBelt : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> _asteroidPrefabs;

    [SerializeField]
    private AsteroidBeltData _asteroidBeltData;

    [SerializeField]
    private SgtFloatingObject _center;

    [SerializeField]
    private Scales _scales;

    public AsteroidBeltData AsteroidBeltData { get => _asteroidBeltData; set => _asteroidBeltData = value; }
    public SgtFloatingObject Center { get => _center; set => _center = value; }
    public Scales Scales { get => _scales; set => _scales = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
