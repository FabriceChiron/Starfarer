using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipCamera : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _flatScreenGameObjects;

    [SerializeField]
    private List<GameObject> _XRGameObjects;

    [SerializeField]
    private BoolVariable _useXRStoredInfo, _useHandsStoredInfo;

    [SerializeField]
    private List<Transform> _playerAnchors;

    [SerializeField]
    private bool _useHands;

    [SerializeField]
    private GameObject _handsControlsContainer;

    [SerializeField]
    private bool _useXR;

    [SerializeField]
    Transform _XRPlayer, _XRPlayerControls, _XRPlayerContainer, _XRCamera, _XRControlsCamera;

    public bool UseXR { 
        get => _useXR;
        set {
            if(_useXR != _useXRStoredInfo.BoolValue)
            {
                _useXRStoredInfo.BoolValue = value;
            }
            
            _useXR = value;

            foreach(GameObject gameObject in _flatScreenGameObjects)
            {
                gameObject.SetActive(!_useXR);
            }

            foreach(GameObject gameObject in _XRGameObjects)
            {
                gameObject.SetActive(_useXR);
            }

            if (_useXR)
            {
                foreach (Transform trackerOffset in _playerAnchors)
                {
                    trackerOffset.localPosition = new Vector3(0f, trackerOffset.localPosition.y, 0f);
                    trackerOffset.parent.gameObject.SetActive(true);
                }
            }

        } 
    }

    public bool UseHands { 
        get => _useHands;
        set { 
            _useHands = value;
            if (_useHands != _useHandsStoredInfo.BoolValue)
            {
                _useHandsStoredInfo.BoolValue = value;
            }

            _useHands = value;
        } 
    }

    private void OnEnable()
    {
        _XRPlayerContainer = GetComponentInChildren<XRPlayerContainer>().transform;
        UseXR = _useXRStoredInfo.BoolValue;
        UseHands = _useHandsStoredInfo.BoolValue;
        _handsControlsContainer.SetActive(UseHands);
        _XRPlayer.SetParent(_XRPlayerContainer);

        _XRCamera = _XRPlayer.GetComponentInChildren<Camera>().transform;
        //_XRControlsCamera = _XRPlayerControls.GetComponentInChildren<Camera>().transform;
    }

    // Start is called before the first frame update
    void Start()
    {
           
    }

    void Update()
    {
        //_XRControlsCamera.transform.localPosition = _XRCamera.transform.localPosition; 
        //_XRControlsCamera.transform.localRotation = _XRCamera.transform.localRotation; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _XRPlayer.localPosition = Vector3.zero;
        //_XRPlayerControls.localPosition = Vector3.zero;
    }
}
