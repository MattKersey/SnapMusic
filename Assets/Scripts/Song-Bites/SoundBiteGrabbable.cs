using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBiteGrabbable : OVRGrabbable
{
    public Material m_grabbedMaterial;
    public Material m_defaultMaterial;
    protected Collider m_draggedCollider = null;
    protected OVRGrabber m_draggedBy = null;
    public Vector3 m_startScale;
    public Vector3 m_startPosition;
    public Vector3 m_startRotation;
    protected float m_startDist;

    void Update()
    {
        if (m_draggedBy != null && m_grabbedBy != null)
        {
            float scalar = (m_draggedBy.transform.position - m_grabbedBy.transform.position).magnitude / m_startDist;
            transform.localScale = scalar * m_startScale;
        }
    }

    public void ScaleBegin(OVRGrabber hand)
    {
        m_draggedBy = hand;
        m_startScale = transform.localScale;
        m_startDist = (m_draggedBy.transform.position - m_grabbedBy.transform.position).magnitude;
    }

    public void ScaleEnd()
    {
        m_draggedBy = null;
    }

    /**
    If the controller 'grabs' the bite gameobject, set that bite to currently selected
    and change its material. 
    **/
    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        m_startPosition = transform.position;
        m_startRotation = transform.rotation.eulerAngles;
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
        ScaleEnd();
        GetComponent<BiteSelf>().SetCurrentlySelected(false);
        GetComponent<Renderer>().material = m_defaultMaterial;
    }
}
