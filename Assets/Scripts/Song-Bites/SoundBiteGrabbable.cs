using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBiteGrabbable : OVRGrabbable
{
    public Material m_grabbedMaterial;
    public Material m_defaultMaterial;

    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        gameObject.GetComponent<Renderer>().material = m_grabbedMaterial;
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        gameObject.GetComponent<Renderer>().material = m_defaultMaterial;

    }
}
