using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum WarpStates
{
    CANCELED,
    STARTED,
    ENGAGED
}

public class FTL_Infos : MonoBehaviour
{

    [SerializeField]
    private WarpStates _warpState = WarpStates.CANCELED;

    [SerializeField]
    private Text _label;

    [SerializeField]
    private RectTransform _progressBar;

    [SerializeField]
    private bool _stopProgressBar;

    public WarpStates WarpState { 
        get => _warpState;
        set { 
            
            if(_warpState != value)
            {
                UpdateFTL_infos(value);
            }

            _warpState = value;
        } 
    }

    public bool StopProgressBar { get => _stopProgressBar; set => _stopProgressBar = value; }

    public void UpdateFTL_infos(WarpStates warpState)
    {
        StopProgressBar = warpState == WarpStates.CANCELED;

        IEnumerator coStarted = LerpProgressBar(new Vector3(1f, 1f, 1f), 2f, _progressBar);

        switch (warpState)
        {
            case WarpStates.CANCELED:
                _label.text = "";
                StopCoroutine(coStarted);
                _progressBar.localScale = new Vector3(0f, 1f, 1f);
                break;

            case WarpStates.STARTED:
                _label.text = "ENGAGING FTL DRIVE";
                StartCoroutine(coStarted);
                break;

            case WarpStates.ENGAGED:
                _label.text = "FTL DRIVE - ENGAGED";
                _progressBar.localScale = new Vector3(0f, 1f, 1f);
                break;
        }
    }

    IEnumerator LerpProgressBar(Vector3 targetLocalScale, float duration, RectTransform ProgressBar)
    {
        float time = 0;
        Vector3 startLocalScale = ProgressBar.localScale;
        while (time < duration)
        {
            if (StopProgressBar)
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

        if (time >= duration)
        {
            ProgressBar.localScale = targetLocalScale;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        WarpState = WarpStates.CANCELED;
    }
}
