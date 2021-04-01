using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GogoSettings
{
    public const float Alpha = 100.0f;
    public const float D = 0.5f;
}

public class CustomController : OVRGrabber
{
    new void Update()
    {
        base.Update();
        float trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
        float dist = (m_player.transform.position - m_parentTransform.position).magnitude;
        float location = 0.0f;
        if (trigger > 0.0f && dist > GogoSettings.D)
        {
            location = (trigger * GogoSettings.Alpha) * Mathf.Pow(dist - GogoSettings.D, 2);
        }
        m_gripTransform.localPosition = new Vector3(0, 0, location);
    }

    new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag("Wall"))
        {
            StartCoroutine(WarnWall());
        }
    }

    IEnumerator WarnWall()
    {
        OVRInput.SetControllerVibration(1.0f, 1.0f, m_controller);
        yield return new WaitForSeconds(0.5f);
        OVRInput.SetControllerVibration(0.0f, 0.0f, m_controller);
    }
}
