using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalControls : MonoBehaviour
{
    public ButtonLabelController[] labelControllers;
    protected OVRPlayerController playerController;
    protected bool labelsActive = false;

    void Start()
    {
        playerController = GetComponent<OVRPlayerController>();
        foreach (ButtonLabelController labelController in labelControllers)
        {
            labelController.Activate(labelsActive);
        }
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

        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            Debug.Log("Start");
            labelsActive = !labelsActive;
            foreach (ButtonLabelController labelController in labelControllers)
            {
                labelController.Activate(labelsActive);
            }
        }
    }
}
