using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GogoSettings
{
    public const float Alpha = 15.0f;
    public const float D = 0.35f;
    public const float MaxChange = 1.0f;
}

public class CustomController : OVRGrabber
{
    public Transform m_headPose;
    public GameObject ray;
    public Renderer[] controllerRenderers;
    private float m_prevLocation = 0;
    private float m_currentVibration = 0.0f;

    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();
        float trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller);
        float dist = (m_headPose.position - m_parentTransform.position).magnitude;
        // Vector3 forward = m_headPose.rotation * Vector3.forward;
        // Quaternion rotation = new Quaternion();
        // rotation.SetFromToRotation(forward, m_headPose.position - m_parentTransform.position);
        // Vector3 eulerAngles = rotation.eulerAngles;
        // for (int i = 0; i < 3; i++)
        //     eulerAngles[i] = eulerAngles[i] > 180 ? 360 - eulerAngles[i] : eulerAngles[i];
        
        float location = 0.0f;
        if (trigger > 0.0f && dist > GogoSettings.D)
        {
            location = (trigger * GogoSettings.Alpha) * Mathf.Pow(dist - GogoSettings.D, 2);
        }
        foreach (Renderer controllerRenderer in controllerRenderers)
            controllerRenderer.enabled = trigger > 0.0f && location > 0.025f;
        // if (location - m_prevLocation > GogoSettings.MaxChange)
        //     location = m_prevLocation + GogoSettings.MaxChange;
        // else if (m_prevLocation - location > GogoSettings.MaxChange)
        //     location = m_prevLocation + GogoSettings.MaxChange;
        m_anchorOffsetPosition = new Vector3(0, 0, location);
        m_gripTransform.localPosition = m_anchorOffsetPosition;
        ray.transform.localPosition = new Vector3(0, -0.01f, location / 2.0f);
        ray.transform.localScale = new Vector3(0.015f, Mathf.Max((location - 0.025f) / 2.0f, 0f), 0.015f);
        m_prevLocation = location;
    }

    new void OnTriggerEnter(Collider otherCollider)
    {
        base.OnTriggerEnter(otherCollider);
        if (otherCollider.CompareTag("Wall"))
        {
            StartCoroutine(WarnWall());
        }
        else if (otherCollider.CompareTag("Sound Bite"))
        {
            otherCollider.gameObject.GetComponent<BiteSelf>().SetCurrentlySelected(true);
            m_currentVibration = 0.25f;
            OVRInput.SetControllerVibration(m_currentVibration, m_currentVibration, m_controller);
        }
    }

    new void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (other.CompareTag("Sound Bite"))
        {
            if (m_grabCandidates.Count == 0)
            {
                other.gameObject.GetComponent<BiteSelf>().SetCurrentlySelected(false);
                m_currentVibration = 0.0f;
                OVRInput.SetControllerVibration(0.0f, 0.0f, m_controller);
            }
        }
    }

    IEnumerator WarnWall()
    {
        OVRInput.SetControllerVibration(1.0f, 1.0f, m_controller);
        yield return new WaitForSeconds(0.25f);
        OVRInput.SetControllerVibration(m_currentVibration, m_currentVibration, m_controller);
    }
}
