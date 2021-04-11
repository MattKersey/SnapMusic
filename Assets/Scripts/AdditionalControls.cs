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

    void FixedUpdate()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
            playerController.Jump();

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
            CustomController.Redo();
        
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
            CustomController.Undo();
    }
}
