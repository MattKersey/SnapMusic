using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidateButton : OVRGrabbable
{
    public GameObject songBitesContainer;

    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        //songBitesContainer.GetComponent<BiteController>().AllInOrder();
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        Debug.Log("Let go");
    }
}
