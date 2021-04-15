using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidateButton : OVRGrabbable
{
    public GameObject songBitesContainer;

    /**
    If the controller 'grabs' the button, activate the allInOrder command 
    which will check if all bites are arranged correctly. Note that the
    controller won't physically grab the button (sphere) as the button's
    rigidbody position is locked on all axises.
    **/
    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        Debug.Log("Validate: Grabs");
        EmissionStatus(true);
        songBitesContainer.GetComponent<BiteController>().AllInOrder();
    }

    /**
    If the controller 'lets go' of the button, do nothing. The function was mainly written
    for debugging purposes. 
    **/
    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        Debug.Log("Validate: Let go");
        EmissionStatus(false);
    }

    // Turns on/off the button's emission to indicate selected status
    private void EmissionStatus(bool status)
    {
        if (status)
        {
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
        else
        {
            GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
}
