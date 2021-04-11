using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GogoSettings
{
    public const float Alpha = 15.0f;
    public const float D = 0.35f;
    public const float MaxChange = 1.0f;
}

public class MoveLogEntry
{
    public enum MoveType {Scale, Move};
    public OVRGrabbable obj;
    public List<Vector3> data;
    public MoveType type;
}

public class CustomController : OVRGrabber
{
    public Transform m_headPose;
    public GameObject ray;
    public Renderer[] controllerRenderers;
    public Material m_grabbedMaterial;
    public Material m_defaultMaterial;
    public AudioSource m_wallSound;
    public AudioSource m_objSound;
    private float m_prevLocation = 0;
    private float m_currentVibration = 0.0f;
    protected SoundBiteGrabbable m_draggedObj = null;
    protected static List<MoveLogEntry> m_undoList = null;
    protected static List<MoveLogEntry> m_redoList = null;

    new void Start()
    {
        base.Start();
        if (m_undoList == null)
            m_undoList = new List<MoveLogEntry>();

        if (m_redoList == null)
            m_redoList = new List<MoveLogEntry>();
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

    public static void Undo()
    {
        int count = m_undoList.Count;
        if (count > 0)
        {
            MoveLogEntry entry = m_undoList[count - 1];
            if (entry.type == MoveLogEntry.MoveType.Move)
            {
                entry.obj.transform.position = entry.data[0];
                entry.obj.transform.rotation = Quaternion.Euler(entry.data[1]);
            }
            else
            {
                entry.obj.transform.localScale = entry.data[0];
            }
            m_redoList.Add(entry);
            m_undoList.RemoveAt(count - 1);
        }
    }

    public static void Redo()
    {
        int count = m_redoList.Count;
        if (count > 0)
        {
            MoveLogEntry entry = m_redoList[count - 1];
            if (entry.type == MoveLogEntry.MoveType.Move)
            {
                entry.obj.transform.position = entry.data[2];
                entry.obj.transform.rotation = Quaternion.Euler(entry.data[3]);
            }
            else
            {
                entry.obj.transform.localScale = entry.data[1];
            }
            m_redoList.RemoveAt(count - 1);
            m_undoList.Add(entry);
        }
    }

    new void GrabBegin()
    {
        base.GrabBegin();
        if (m_grabbedObj == null && m_grabCandidates.Count > 0)
        {
            SoundBiteGrabbable sbGrabbable = null;
            foreach (OVRGrabbable grabbable in m_grabCandidates.Keys)
            {
                if (grabbable is SoundBiteGrabbable)
                {
                    sbGrabbable = (SoundBiteGrabbable)grabbable;
                    break;
                }
            }
            if (sbGrabbable != null)
            {
                sbGrabbable.ScaleBegin(this);
                m_draggedObj = sbGrabbable;
            }
        }
        if (m_grabbedObj != null || m_draggedObj != null)
        {
            m_objSound.Play();
            foreach (Renderer controllerRenderer in controllerRenderers)
                controllerRenderer.material = m_grabbedMaterial;
        }
    }

    new void GrabEnd()
    {
        if (m_grabbedObj != null)
        {
            MoveLogEntry newEntry = new MoveLogEntry();
            newEntry.obj = m_grabbedObj;
            newEntry.type = MoveLogEntry.MoveType.Move;
            newEntry.data = new List<Vector3>();
            newEntry.data.Add(((SoundBiteGrabbable)m_grabbedObj).m_startPosition);
            newEntry.data.Add(((SoundBiteGrabbable)m_grabbedObj).m_startRotation);
            newEntry.data.Add(m_grabbedObj.transform.position);
            newEntry.data.Add(m_grabbedObj.transform.rotation.eulerAngles);
            m_undoList.Add(newEntry);
        }
        if (m_draggedObj != null)
        {
            MoveLogEntry newEntry = new MoveLogEntry();
            newEntry.obj = m_draggedObj;
            newEntry.type = MoveLogEntry.MoveType.Scale;
            newEntry.data = new List<Vector3>();
            newEntry.data.Add(((SoundBiteGrabbable)m_draggedObj).m_startScale);
            newEntry.data.Add(m_grabbedObj.transform.localScale);
            m_undoList.Add(newEntry);
            m_draggedObj.ScaleEnd();
            m_draggedObj = null;
        }
        m_redoList.Clear();
        base.GrabEnd();
        foreach (Renderer controllerRenderer in controllerRenderers)
            controllerRenderer.material = m_defaultMaterial;
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
                m_currentVibration = 0.0f;
                OVRInput.SetControllerVibration(0.0f, 0.0f, m_controller);
            }
        }
    }

    IEnumerator WarnWall()
    {
        m_wallSound.Play();
        OVRInput.SetControllerVibration(1.0f, 1.0f, m_controller);
        yield return new WaitForSeconds(0.25f);
        OVRInput.SetControllerVibration(m_currentVibration, m_currentVibration, m_controller);
    }
}
