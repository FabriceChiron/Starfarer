using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipAudio : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private SpaceshipWarp _spaceshipWarp;

    [SerializeField]
    private SpaceshipController _spaceshipController;

    [SerializeField]
    private SpaceshipCollisions _spaceshipCollisions;

    [SerializeField]
    protected AnimationCurve throttleToEngineVolumeCurve = AnimationCurve.Linear(0f, 0.2f, 1f, 1f);

    [SerializeField]
    private AudioSource _engineIdle, _engineNormal, _engineBoost, _engineSuperBoost, _engineRoll, _engineStrafe, _engineUpDown, _engineWarp, _engineStopWarp, _engineInitWarp, _engineWarpBackground;
    public AudioSource EngineIdle { get => _engineIdle; set => _engineIdle = value; }
    public AudioSource EngineNormal { get => _engineNormal; set => _engineNormal = value; }
    public AudioSource EngineBoost { get => _engineBoost; set => _engineBoost = value; }
    public AudioSource EngineSuperBoost { get => _engineSuperBoost; set => _engineSuperBoost = value; }
    public AudioSource EngineStrafe { get => _engineStrafe; set => _engineStrafe = value; }
    public AudioSource EngineUpDown { get => _engineUpDown; set => _engineUpDown = value; }
    public AudioSource EngineWarp { get => _engineWarp; set => _engineWarp = value; }
    public AudioSource EngineStopWarp { get => _engineStopWarp; set => _engineStopWarp = value; }
    public AudioSource EngineInitWarp { get => _engineInitWarp; set => _engineInitWarp = value; }
    public AudioSource EngineWarpBackground { get => _engineWarpBackground; set => _engineWarpBackground = value; }
    public AudioSource EngineRoll { get => _engineRoll; set => _engineRoll = value; }

    public Vector3 ThrustValues;

    // Start is called before the first frame update
    void Start()
    {
        _spaceshipController = GetComponent<SpaceshipController>();
        _spaceshipWarp = GetComponent<SpaceshipWarp>();
    }

    float GoToVolume(float currentVolume, float targetVolume)
    {
        currentVolume = Mathf.Lerp(currentVolume, targetVolume, 3f * Time.deltaTime);

        return currentVolume;
    }

    void FixedUpdate()
    {
        UpdateEnginesAudio();
        if (EngineWarp.loop)
        {
            EngineWarp.volume = GoToVolume(EngineWarp.volume, _spaceshipWarp.Warping ? 1f : 0f);
        }

        EngineWarpBackground.volume = GoToVolume(EngineWarpBackground.volume, (_spaceshipWarp.Warping || _spaceshipCollisions.IsInAtmosphere) ? 1f : 0f);

        EngineRoll.volume = GoToVolume(EngineRoll.volume, Mathf.Abs(_spaceshipController.Roll) > 0f ? 0.5f : 0f);
    }

    private void UpdateEnginesAudio()
    {
        ThrustValues.x = Mathf.Abs(_spaceshipController.ProgressiveLateralThrust);
        ThrustValues.y = Mathf.Abs(_spaceshipController.ProgressiveUpDownThrust);
        ThrustValues.z = Mathf.Abs(_spaceshipController.ProgressiveForwardThrust);

        //EngineStrafe.volume = GoToVolume(EngineStrafe.volume, (ThrustValues.x > 0.025f) ? ThrustValues.x : 0f);
        EngineStrafe.volume = (ThrustValues.x > 0.025f) ? ThrustValues.x : 0f;
        //EngineUpDown.volume = GoToVolume(EngineUpDown.volume, (ThrustValues.y > 0.025f) ? ThrustValues.y : 0f);
        EngineUpDown.volume = (ThrustValues.y > 0.025f) ? ThrustValues.y : 0f;

        if (ThrustValues.z > 0.025f)
        {
            //If Forward
            if (_spaceshipController.ForwardThrust > 0f)
            {
                if (_spaceshipController.SuperBoosting)
                {
                    /*EngineIdle.volume = GoToVolume(EngineIdle.volume, 0f);
                    EngineNormal.volume = GoToVolume(EngineNormal.volume, 0f);
                    EngineBoost.volume = GoToVolume(EngineBoost.volume, 0f);
                    EngineSuperBoost.volume = GoToVolume(EngineSuperBoost.volume, ThrustValues.z);*/

                    EngineIdle.volume = 0f;
                    EngineNormal.volume = 0f;
                    EngineBoost.volume = 0f;
                    EngineSuperBoost.volume = ThrustValues.z;
                }
                else if (_spaceshipController.Boosting)
                {
                    /*EngineIdle.volume = GoToVolume(EngineIdle.volume, 0f);
                    EngineNormal.volume = GoToVolume(EngineNormal.volume, 0f);
                    EngineBoost.volume = GoToVolume(EngineBoost.volume, ThrustValues.z);
                    EngineSuperBoost.volume = GoToVolume(EngineSuperBoost.volume, 0f);*/  
                    EngineIdle.volume = 0f;
                    EngineNormal.volume = 0f;
                    EngineBoost.volume = ThrustValues.z;
                    EngineSuperBoost.volume = 0f;
                }
                else
                {
                    /*EngineIdle.volume = GoToVolume(EngineIdle.volume, 0f);
                    EngineNormal.volume = GoToVolume(EngineNormal.volume, ThrustValues.z);
                    EngineBoost.volume = GoToVolume(EngineBoost.volume, 0f);
                    EngineSuperBoost.volume = GoToVolume(EngineSuperBoost.volume, 0f);*/
                    EngineIdle.volume = 0f;
                    EngineNormal.volume = ThrustValues.z;
                    EngineBoost.volume = 0f;
                    EngineSuperBoost.volume = 0f;
                }
            }
            //if Backward
            else
            {
                /*EngineIdle.volume = GoToVolume(EngineIdle.volume, 0f);
                EngineNormal.volume = GoToVolume(EngineNormal.volume, ThrustValues.z);
                EngineBoost.volume = GoToVolume(EngineBoost.volume, 0f);
                EngineSuperBoost.volume = GoToVolume(EngineSuperBoost.volume, 0f);*/
                EngineIdle.volume = 0f;
                EngineNormal.volume = ThrustValues.z;
                EngineBoost.volume = 0f;
                EngineSuperBoost.volume = 0f;
            }
        }
        //if 0
        else
        {
            /*EngineIdle.volume = GoToVolume(EngineIdle.volume, 1f);
            EngineNormal.volume = GoToVolume(EngineNormal.volume, 0f);
            EngineBoost.volume = GoToVolume(EngineBoost.volume, 0f);
            EngineSuperBoost.volume = GoToVolume(EngineSuperBoost.volume, 0f);*/        
            EngineIdle.volume = 1f;
            EngineNormal.volume = 0f;
            EngineBoost.volume = 0f;
            EngineSuperBoost.volume = 0f;
        }
    }
}