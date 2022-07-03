using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SpaceGraphicsToolkit;

public class SpaceshipWarp : MonoBehaviour
{
    private SgtFloatingWarpSmoothstep _warpSmoothstep;

    [SerializeField]
    private GameObject _warpEffectPrefab;

    [SerializeField]
    private SpaceshipController _spaceshipController;

    [SerializeField]
    private SgtFloatingTarget _warpTarget, _previousWarpTarget, _isInsideSphere;

    [SerializeField]
    private float 
        _warpTime, 
        _initialCameraFOV, 
        _warpCameraFOV = 90f, 
        _targetFOV, 
        _warpEffectLerpDuration = 1f,
        _progressBarLerpDuration = 2f;

    [SerializeField]
    private bool _canWarp, _warpTargetLocked, _showWarpEffect, _stopProgressBar;

    [SerializeField]
    private WarpPrompt _radarWarpPrompt;

    [SerializeField]
    private RectTransform _progressBar;

    [SerializeField]
    private Camera _activeCamera;

    private GameObject _warpEffect;

    private ParticleSystem _warpParticleSystem;

    private float currentWarpLerpTime, currentProgressBarLerpTime;

    private Vector3 _targetWidth;

    public SgtFloatingTarget WarpTarget { 
        get => _warpTarget;
        set { 
            if(_warpTarget != value)
            {
                _previousWarpTarget = _warpTarget;
            }

            _warpTarget = value;
        } 
    }
    public float WarpTime { get => _warpTime; set => _warpTime = value; }
    public bool CanWarp { get => _canWarp; set => _canWarp = value; }
    public bool WarpTargetLocked { get => _warpTargetLocked; set => _warpTargetLocked = value; }
    public WarpPrompt RadarWarpPrompt { 
        get => _radarWarpPrompt;
        set
        {
            if (_radarWarpPrompt != value && _radarWarpPrompt != null)
            {
                ToggleWarpPrompt(_radarWarpPrompt, false);
            }

            _radarWarpPrompt = value;

            ToggleWarpPrompt(_radarWarpPrompt, CanWarp);
        }
    }
    public RectTransform ProgressBar { get => _progressBar; set => _progressBar = value; }

    // Start is called before the first frame update
    void Start()
    {
        _warpEffect = Instantiate(_warpEffectPrefab, FindObjectOfType<SpaceshipController>().cameraPosition);
        _warpParticleSystem = _warpEffect.GetComponentInChildren<ParticleSystem>();
        _warpParticleSystem.Stop();

        _warpSmoothstep = FindObjectOfType<SgtFloatingWarpSmoothstep>();
        _spaceshipController = GetComponent<SpaceshipController>();
        _activeCamera = transform.parent.gameObject.GetComponentInChildren<Camera>();
        _initialCameraFOV = _activeCamera.fieldOfView;
        _targetFOV = _activeCamera.fieldOfView;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _warpSmoothstep.WarpTime = WarpTime;
        if (_warpSmoothstep.Warping)
        {
            _warpEffect.transform.LookAt(WarpTarget.transform);
            //TurnTowardsWarpTarget();
            //_warpEffect.transform.LookAt(WarpTarget.transform);
        }

        LerpCameraFOV(_targetFOV);
        
        _warpTargetLocked = _warpSmoothstep.Warping;

        CanWarp = (_previousWarpTarget != WarpTarget) && (_isInsideSphere != WarpTarget);


    }

    private void ToggleWarpPrompt(WarpPrompt warpPrompt, bool toggleActive)
    {
        warpPrompt.transform.parent.gameObject.SetActive(toggleActive);
    }

    private void TurnTowardsWarpTarget()
    {
        StartCoroutine(TurnTowardsTarget(_warpTarget.transform, 1f));
    }

    public void WarpTo()
    {
        if(WarpTarget != null && CanWarp)
        {
            _targetFOV = _warpCameraFOV;
            _showWarpEffect = true;


            _warpSmoothstep.WarpTo(WarpTarget.GetComponent<SgtFloatingObject>().Position);
            _warpParticleSystem.Play();
        }
    }

    IEnumerator TurnTowardsTarget(Transform target, float duration)
    {
        float time = 0;
        Quaternion targetRotation = Quaternion.identity;
        Quaternion startValue = transform.rotation;
        while (time < duration)
        {
            Vector3 targetDirection = (target.position - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(targetDirection);

            _spaceshipController.LookRotation = Quaternion.Lerp(startValue, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }


    IEnumerator LerpProgressBar(Vector3 targetLocalScale, float duration, RectTransform ProgressBar)
    {
        float time = 0;
        Vector3 startLocalScale = ProgressBar.localScale;
        while (time < duration)
        {
            if (_stopProgressBar)
            {
                //_stopProgressBar = false;
                ProgressBar.localScale = new Vector3(0f, 1f, 1f);
                yield break;
            }

            else
            {
                ProgressBar.localScale = Vector3.Lerp(startLocalScale, targetLocalScale, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
        }
        
        if(time >= duration)
        {
            ProgressBar.localScale = targetLocalScale;
        }
    }

    //public void LerpWarpProgressBar(float targetWidth)
    //{
    //    if(_progressBar != null)
    //    {
    //        currentProgressBarLerpTime += Time.deltaTime;

    //        if(currentProgressBarLerpTime > _progressBarLerpDuration)
    //        {
    //            currentProgressBarLerpTime = _progressBarLerpDuration;
    //        }

    //        float perc = currentProgressBarLerpTime / _progressBarLerpDuration;

    //        _progressBar.localScale = new Vector3(
    //            Mathf.Lerp(_progressBar.localScale.x, targetWidth, perc),
    //            1f,
    //            1f
    //        );
    //    }
    //    else
    //    {
    //        currentProgressBarLerpTime = 0f;
    //    }
    //}

    public void LerpCameraFOV(float targetFOV)
    {
        if(targetFOV != 0f)
        {
            currentWarpLerpTime += Time.deltaTime;
            if (currentWarpLerpTime > _warpEffectLerpDuration)
            {
                currentWarpLerpTime = _warpEffectLerpDuration;
            }

            _activeCamera.fieldOfView = Mathf.Lerp(_activeCamera.fieldOfView, targetFOV, Time.deltaTime * 3f);

            if(Mathf.Abs(_activeCamera.fieldOfView - targetFOV) < 1f){
                _activeCamera.fieldOfView = targetFOV;
                _targetFOV = targetFOV;

                if(_targetFOV == _initialCameraFOV)
                {
                    _showWarpEffect = false;
                }
            }
        }

        else
        {
            _activeCamera.fieldOfView = _initialCameraFOV;
        }
    }

    public void AbortWarp()
    {
        _warpSmoothstep.AbortWarp();
        _targetFOV = _initialCameraFOV;
        _warpParticleSystem.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NullifyWarp"))
        {
            Debug.Log($"{other.transform.GetComponentInParent<SgtFloatingObject>()} - NullifyWarp");
            _isInsideSphere = other.transform.GetComponentInParent<SgtFloatingTarget>();
            AbortWarp();
        }
    }

    public void OnThrust(InputAction.CallbackContext context)
    {
        if(context.ReadValue<Vector2>().y < 0)
        {
            _isInsideSphere = null;
            AbortWarp();
        }
    }
    public void OnWarp(InputAction.CallbackContext context)
    {

        //context.performed;
        //_warpButtonPressed = context.started;

        IEnumerator coStarted, coCanceled;
        coStarted = LerpProgressBar(new Vector3(1f, 1f, 1f), 2f, ProgressBar);
        coCanceled = LerpProgressBar(new Vector3(0f, 1f, 1f), 2f, ProgressBar);

        if (context.started)
        {
            Debug.Log($"Warp started");
            //LerpWarpProgressBar(1f);
            //StopCoroutine(coCanceled);
            _stopProgressBar = false;
            StartCoroutine(coStarted);
            TurnTowardsWarpTarget();
        }
        if (context.canceled)
        {
            Debug.Log($"Warp canceled");
            //LerpWarpProgressBar(0f);
            _stopProgressBar = true;
            ProgressBar.localScale = new Vector3(0f, 1f, 1f);
            StopCoroutine(coStarted);
        }

        if(context.performed)
        {
            WarpTo();
        }
    }
}
