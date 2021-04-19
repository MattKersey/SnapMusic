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
    public enum MoveType { Scale, Swap, Reverse };
    public OVRGrabbable obj0;
    public OVRGrabbable obj1;
    public List<Vector3> scales;
    public MoveType type;
}

public class CustomController : OVRGrabber
{
    public Transform m_headPose;
    public GameObject ray;
    public AdditionalControls additionalControls;
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
    private bool touchedBite = false;
    private GameObject bite;

    //minimap camera
    public GameObject hud;
    private bool hudIsActive = true;

    //bool for switching to edit play mode
    public bool inEditMode = false;

    //player controller script
    public OVRPlayerController pCont;

    new void Start()
    {
        base.Start();
        if (m_undoList == null)
            m_undoList = new List<MoveLogEntry>();

        if (m_redoList == null)
            m_redoList = new List<MoveLogEntry>();

        pCont = GameObject.Find("OVRPlayerController").GetComponent<OVRPlayerController>();
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

        if (OVRInput.GetDown(OVRInput.Button.Two) && !inEditMode)
        {
            //check for current state of hud and toggle on/off
            if (hudIsActive)
            {
                hud.SetActive(false);
                hudIsActive = false;
            }
            else
            {
                hud.SetActive(true);
                hudIsActive = true;
            }
        }

        if (touchedBite && OVRInput.GetDown(OVRInput.Button.Four))
        {
            CustomController.AddAction(MoveLogEntry.MoveType.Reverse, bite.GetComponent<SoundBiteGrabbable>(), null, null);
            bite.GetComponent<BiteSelf>().Reverse();
        }

        if (trigger > 0.0f)
        {
            pCont.Acceleration = Mathf.Clamp(pCont.Acceleration + .002f, .15f, .3f);
        }
        else
        {
            pCont.Acceleration = .15f;
        }
    }

    public static void AddAction(MoveLogEntry.MoveType moveType, OVRGrabbable obj0, OVRGrabbable obj1, List<Vector3> scales)
    {
        MoveLogEntry newEntry = new MoveLogEntry();
        newEntry.obj0 = obj0;
        newEntry.obj1 = obj1;
        newEntry.scales = scales;
        newEntry.type = moveType;
        m_undoList.Add(newEntry);
        m_redoList.Clear();
    }

    public static void Undo()
    {
        int count = m_undoList.Count;
        if (count > 0)
        {
            MoveLogEntry entry = m_undoList[count - 1];
            switch (entry.type)
            {
                case MoveLogEntry.MoveType.Scale:
                    entry.obj0.transform.localScale = entry.scales[0];
                    break;
                case MoveLogEntry.MoveType.Swap:
                    entry.obj0.GetComponent<BiteSelf>().SwapInHierarchy(entry.obj1.gameObject, true);
                    break;
                case MoveLogEntry.MoveType.Reverse:
                    entry.obj0.GetComponent<BiteSelf>().Reverse();
                    break;
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
            switch (entry.type)
            {
                case MoveLogEntry.MoveType.Scale:
                    entry.obj0.transform.localScale = entry.scales[1];
                    break;
                case MoveLogEntry.MoveType.Swap:
                    entry.obj1.GetComponent<BiteSelf>().SwapInHierarchy(entry.obj0.gameObject, true);
                    break;
                case MoveLogEntry.MoveType.Reverse:
                    entry.obj0.GetComponent<BiteSelf>().Reverse();
                    break;
            }
            m_redoList.RemoveAt(count - 1);
            m_undoList.Add(entry);
        }
    }

    override protected void GrabBegin()
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
        base.GrabBegin();
        Debug.Log(m_controller + " " + m_grabbedObj + " " + m_grabCandidates.Count);
        if (m_grabbedObj == null && sbGrabbable != null)
        {
            sbGrabbable.ScaleBegin(this);
            m_draggedObj = sbGrabbable;
        }
        if (m_grabbedObj != null || m_draggedObj != null)
        {
            //m_objSound.Play();
            foreach (Renderer controllerRenderer in controllerRenderers)
                controllerRenderer.material = m_grabbedMaterial;
        }
    }

    override protected void GrabEnd()
    {
        if (m_draggedObj != null && m_draggedObj is SoundBiteGrabbable)
        {
            List<Vector3> scales = new List<Vector3>();
            scales.Add(((SoundBiteGrabbable)m_draggedObj).m_startScale);
            scales.Add(m_draggedObj.transform.localScale);
            AddAction(MoveLogEntry.MoveType.Scale, m_draggedObj, null, scales);
            m_draggedObj.ScaleEnd();
            m_draggedObj = null;
        }
        base.GrabEnd();
        foreach (Renderer controllerRenderer in controllerRenderers)
            controllerRenderer.material = m_defaultMaterial;
    }

    new void OnTriggerEnter(Collider otherCollider)
    {
        Debug.Log(otherCollider.tag);
        base.OnTriggerEnter(otherCollider);
        if (otherCollider.CompareTag("Wall"))
        {
            Debug.Log("good");
            StartCoroutine(WarnWall());
        }
        else if (otherCollider.CompareTag("Sound Bite") || otherCollider.CompareTag("Stage-Button"))
        {
            m_currentVibration = 0.25f;
            OVRInput.SetControllerVibration(m_currentVibration, m_currentVibration, m_controller);
            if (otherCollider.CompareTag("Sound Bite"))
            {
                touchedBite = true;
                bite = otherCollider.gameObject;
                bite.GetComponent<BiteSelf>().ColorBite(bite, false);
                additionalControls.SetInContact(m_controller == OVRInput.Controller.LTouch, true);
            }
        }
    }

    new void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (other.CompareTag("Sound Bite") || other.CompareTag("Stage-Button"))
        {
            if (m_grabCandidates.Count == 0)
            {
                touchedBite = false;
                bite.GetComponent<BiteSelf>().SetCurrentlySelected(false);
                bite.GetComponent<BiteSelf>().UnColorBite(bite);
                bite = null;
                m_currentVibration = 0.0f;
                OVRInput.SetControllerVibration(0.0f, 0.0f, m_controller);
                additionalControls.SetInContact(m_controller == OVRInput.Controller.LTouch, false);
            }
        }
    }

    IEnumerator WarnWall()
    {
        //m_wallSound.Play();
        OVRInput.SetControllerVibration(1.0f, 1.0f, m_controller);
        yield return new WaitForSeconds(0.25f);
        OVRInput.SetControllerVibration(m_currentVibration, m_currentVibration, m_controller);
    }
}