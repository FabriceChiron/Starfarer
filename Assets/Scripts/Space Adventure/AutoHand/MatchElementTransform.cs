using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchElementTransform : MonoBehaviour
{
    public Transform elementToMatch;

    [SerializeField]
    private bool useGlobal;

    [SerializeField]
    private bool matchPosition = true;
    
    [SerializeField]
    private bool matchRotation = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        if(useGlobal)
        {
            if (matchPosition)
            {
                transform.position = elementToMatch.position;
            }

            if (matchRotation)
            {
                transform.rotation = elementToMatch.rotation;
            }
        }

        else
        {
            if (matchPosition)
            {
                transform.localPosition = elementToMatch.localPosition;
            }

            if (matchRotation)
            {
                transform.localRotation = elementToMatch.localRotation;
            }
        }
    }
}
