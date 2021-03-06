using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SpaceGraphicsToolkit;
using CW.Common;

public class SpaceshipWarp : MonoBehaviour
{
    private SgtFloatingWarpSmoothstep _warpSmoothstep;

    public LayerMask IgnoreLayer;

    [SerializeField]
    private GameObject _warpEffectPrefab;

    [SerializeField]
    private SpaceshipController _spaceshipController;

    [SerializeField]
    private SpaceshipAudio _spaceshipAudio;

    [SerializeField]
    private SgtFloatingTarget _warpTarget, _previousWarpTarget, _noWarpSphere;

    [SerializeField]
    private float 
        _warpTime, 
        _initialCameraFOV, 
        _warpCameraFOV = 75f, 
        _targetFOV, 
        _warpEffectLerpDuration = 1f,
        _progressBarLerpDuration = 2f;

    [SerializeField]
    private bool _canWarp, _warpObstruction, _warping, _warpInitiated, _warpTargetLocked, _showWarpEffect, _stopProgressBar;

    [SerializeField]
    private WarpPrompt _radarWarpPrompt;

    [SerializeField]
    private RectTransform _progressBar;

    [SerializeField]
    private Camera _activeCamera;

    [SerializeField]
    FTL_Infos _FTLInfos;

    private GameObject _warpEffect;

    private ParticleSystem _warpParticleSystem;

    private float currentWarpLerpTime;

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
    public SgtFloatingTarget NoWarpSphere { get => _noWarpSphere; set => _noWarpSphere = value; }
    public bool Warping { 
        get => _warping;
        set {
            _warping = value;

            _showWarpEffect = value;

            if (_warping)
            {
                _warpParticleSystem.Play();
            }
            else
            {
                _warpParticleSystem.Stop();
            }
        }
    }

    public bool WarpInitiated { get => _warpInitiated; set => _warpInitiated = value; }

    // Start is called before the first frame update
    void Start()
    {
        _spaceshipController = GetComponent<SpaceshipController>();
        _spaceshipAudio = GetComponent<SpaceshipAudio>();
        _warpEffect = Instantiate(_warpEffectPrefab, _spaceshipController.cameraPosition);
        _warpParticleSystem = _warpEffect.GetComponentInChildren<ParticleSystem>();

        _spaceshipAudio.EngineWarpBackground.gameObject.AddComponent<CwFollow>().Target = _warpParticleSystem.transform;

        _warpParticleSystem.Stop();

        _warpSmoothstep = FindObjectOfType<SgtFloatingWarpSmoothstep>();
        _activeCamera = transform.parent.gameObject.GetComponentInChildren<Camera>();
        _initialCameraFOV = _activeCamera.fieldOfView;
        _targetFOV = _activeCamera.fieldOfView;

        _FTLInfos = GetComponentInChildren<FTL_Infos>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Warping = _warpSmoothstep.Warping;

        _warpSmoothstep.WarpTime = WarpTime;
        if (_warpSmoothstep.Warping)
        {
            _FTLInfos.WarpState = WarpStates.ENGAGED;
            _warpEffect.transform.LookAt(WarpTarget.transform);
        }

        LerpCameraFOV(_targetFOV);
        
        //_warpTargetLocked = _warpSmoothstep.Warping;

        //CanWarp = (_previousWarpTarget != WarpTarget) && (NoWarpSphere != WarpTarget);

        if(WarpTarget != null)
        {
            RaycastHit hit;

            Vector3 direction = WarpTarget.transform.position - transform.position;

            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, IgnoreLayer))
            {
                //Debug.Log($"{hit.transform.gameObject.GetComponentInParent<SgtFloatingTarget>().name} is between the starship and  {WarpTarget.name}");
                //Debug.Log($"Blocked by {hit.transform.name}");
                //ToggleWarpPrompt(RadarWarpPrompt, false, "from raycast if");
                _warpObstruction = hit.transform.gameObject.GetComponentInParent<SgtFloatingTarget>() != WarpTarget;
            }
            else
            {
                _warpObstruction = false;
                //ToggleWarpPrompt(RadarWarpPrompt, CanWarp, "from raycast else");
            }
        }
        else
        {
        }

    }

    private void ToggleWarpPrompt(WarpPrompt warpPrompt, bool toggleActive)
    {
        if (_warpObstruction)
        {
            toggleActive = false;
        }
        warpPrompt.transform.parent.gameObject.SetActive(toggleActive);
    }

    private void TurnTowardsWarpTarget()
    {
        if(WarpTarget != null)
        {
            StartCoroutine(TurnTowardsTarget(WarpTarget.transform, 1f));
        }
    }

    public void WarpTo()
    {
        if(WarpTarget != null && CanWarp)
        {
            _targetFOV = _warpCameraFOV;
            if (!_spaceshipAudio.EngineWarp.loop)
            {
                _spaceshipAudio.EngineWarp.PlayOneShot(_spaceshipAudio.EngineWarp.clip);
            }
            //_spaceshipAudio.AudioSource.clip = _spaceshipAudio.WarpClip;
            //_spaceshipAudio.AudioSource.PlayOneShot(_spaceshipAudio.AudioSource.clip);

            _warpSmoothstep.WarpTo(WarpTarget.GetComponent<SgtFloatingObject>().Position);
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
        //Debug.Log($"Aborting Warp");
        _warpSmoothstep.AbortWarp();
        _targetFOV = _initialCameraFOV;

        _warpTargetLocked = false;

        //_warpParticleSystem.Stop();
        _FTLInfos.WarpState = WarpStates.CANCELED;
        if (!_spaceshipAudio.EngineStopWarp.isPlaying)
        {
            if (!_spaceshipAudio.EngineWarp.loop)
            {
                _spaceshipAudio.EngineWarp.Stop();
            }
            _spaceshipAudio.EngineStopWarp.PlayOneShot(_spaceshipAudio.EngineStopWarp.clip);
        }
    }

/*    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NullifyWarp"))
        {
            //Debug.Log($"{other.transform.GetComponentInParent<SgtFloatingObject>()} - NullifyWarp");
            NoWarpSphere = other.transform.GetComponentInParent<SgtFloatingTarget>();
            if (Warping)
            {
                AbortWarp();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NullifyWarp"))
        {
            NoWarpSphere = null;
        }
    }*/

    public void OnThrust(InputAction.CallbackContext context)
    {
        if(context.ReadValue<Vector2>().y < 0 && Warping)
        {
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

        WarpInitiated = context.started;

        if (context.started)
        {
            //Debug.Log($"Warp started");
            //LerpWarpProgressBar(1f);
            //StopCoroutine(coCanceled);
            _stopProgressBar = false;
            StartCoroutine(coStarted);

            _warpTargetLocked = true;

            _spaceshipAudio.EngineInitWarp.PlayOneShot(_spaceshipAudio.EngineInitWarp.clip);

            _FTLInfos.WarpState = WarpStates.STARTED;

            TurnTowardsWarpTarget();
        }
        if (context.canceled)
        {
            //Debug.Log($"Warp canceled");
            //LerpWarpProgressBar(0f);
            _stopProgressBar = true;
            ProgressBar.localScale = new Vector3(0f, 1f, 1f);
            StopCoroutine(coStarted);

            _spaceshipAudio.EngineInitWarp.Stop();

            _FTLInfos.WarpState = WarpStates.CANCELED;

            _warpTargetLocked = false;
        }

        if(context.performed)
        {
            WarpTo();
        }
    }
}
