using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;

public class SpaceshipWarp : MonoBehaviour
{
    private SgtFloatingWarpSmoothstep _warpSmoothstep;

    [SerializeField]
    private SgtFloatingTarget _warpTarget;

    [SerializeField]
    private float _warpTime;

    public SgtFloatingTarget WarpTarget { get => _warpTarget; set => _warpTarget = value; }
    public float WarpTime { get => _warpTime; set => _warpTime = value; }



    // Start is called before the first frame update
    void Start()
    {
        _warpSmoothstep = FindObjectOfType<SgtFloatingWarpSmoothstep>();
    }

    // Update is called once per frame
    void Update()
    {
        _warpSmoothstep.WarpTime = WarpTime;
    }

    public void WarpTo()
    {
        if(WarpTarget != null)
        {
            _warpSmoothstep.WarpTo(WarpTarget.GetComponent<SgtFloatingObject>().Position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NullifyWarp"))
        {
            Debug.Log($"{other.transform.GetComponentInParent<SgtFloatingObject>()} - NullifyWarp");
            _warpSmoothstep.AbortWarp();
        }
    }
}
