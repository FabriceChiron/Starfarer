using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipLocator : MonoBehaviour
{
    [SerializeField]
    private bool _useRadar, _showCockpitRadar;
    public bool UseRadar { get => _useRadar; set => _useRadar = value; }
    public bool ShowCockpitRadar { get => _showCockpitRadar; set => _showCockpitRadar = value; }

    [SerializeField]
    private Camera _cameraVR, _cameraFlat;
    public Camera CameraVR { get => _cameraVR; set => _cameraVR = value; }
    public Camera CameraFlat { get => _cameraFlat; set => _cameraFlat = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnToggleCockpitRadar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ShowCockpitRadar = !ShowCockpitRadar;
        }
    }
}
