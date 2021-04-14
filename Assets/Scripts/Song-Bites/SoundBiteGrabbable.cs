using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBiteGrabbable : OVRGrabbable
{
    public Material m_grabbedMaterial;
    public Material m_defaultMaterial;

    /**
    If the controller 'grabs' the bite gameobject, set that bite to currently selected
    and change its material. 
    **/
    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        GetComponent<BiteSelf>().SetCurrentlySelected(true);
        GetComponent<Renderer>().material = m_grabbedMaterial;
    }

    /**
    If the controller 'lets go' of the bite gameobject, set that bite to `not` currently selected
    and reset its material. 
    **/
    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        GetComponent<BiteSelf>().SetCurrentlySelected(false);
        GetComponent<Renderer>().material = m_defaultMaterial;
    }
}
