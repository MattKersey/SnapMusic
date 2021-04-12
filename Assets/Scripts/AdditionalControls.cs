using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalControls : MonoBehaviour
{
    protected OVRPlayerController playerController;

    void Start()
    {
        playerController = GetComponent<OVRPlayerController>();
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            Debug.Log("r1");
            playerController.Jump();
        }

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            Debug.Log("l1");
            CustomController.Redo();
        }
        
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            Debug.Log("l2");
            CustomController.Undo();
        }
    }
}
