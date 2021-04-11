using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBiteGrabbable : OVRGrabbable
{
    public Material m_grabbedMaterial;
    public Material m_defaultMaterial;

    protected Collider m_draggedCollider = null;
    protected OVRGrabber m_draggedBy = null;
    protected Vector3 m_startScale;
    protected float m_startDist;

    void Update()
    {
        if (m_draggedBy != null && m_grabbedBy != null)
        {
            float scale = (m_draggedBy.transform.position - m_grabbedBy.transform.position).magnitude / m_startDist;
            transform.localScale = scale * m_startScale;
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

    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        GetComponent<Renderer>().material = m_grabbedMaterial;
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        ScaleEnd();
        GetComponent<Renderer>().material = m_defaultMaterial;
    }
}
