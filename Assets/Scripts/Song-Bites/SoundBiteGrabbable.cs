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
    public float maxSize;
    public float minSize;
    protected float m_startDist;

    /**
    In every frame update, check if the bite is grabbed and dragged by the two
    controllers. If so, scale the bite.
    **/
    void Update()
    {
        if (m_draggedBy != null && m_grabbedBy != null)
        {
            float scalar = (m_draggedBy.transform.position - m_grabbedBy.transform.position).magnitude / m_startDist;
            // EDIT THE COLOR
            gameObject.GetComponent<BiteSelf>().ImplementVolumeColor(scalar);
            // transform.localScale = scalar * m_startScale;
        }
    }

    /**
    Begins the scaling procedure. Calculate the distance between the two controllers
    in order to compute the magnitude of the scaling.
    **/
    public void ScaleBegin(OVRGrabber hand)
    {
        m_draggedBy = hand;
        m_startScale = transform.localScale;
        m_startDist = (m_draggedBy.transform.position - m_grabbedBy.transform.position).magnitude;
    }

    // Ends the scaling procedure
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
        //GetComponent<Renderer>().material = m_grabbedMaterial;
    }

    /**
    If the controller 'lets go' of the bite gameobject, set that bite to `not` currently selected
    and reset its material. 
    **/
    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        ScaleEnd();
        GetComponent<BiteSelf>().UnColorBite(gameObject);
        GetComponent<BiteSelf>().RemoveCollidedColor();
        GetComponent<BiteSelf>().PerformSwap();
        GetComponent<BiteSelf>().SetCurrentlySelected(false);
        //GetComponent<Renderer>().material = m_defaultMaterial;
    }
}