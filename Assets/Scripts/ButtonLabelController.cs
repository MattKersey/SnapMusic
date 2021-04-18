using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLabelController : MonoBehaviour
{
    public Transform headPose;
    public string labelTextInitial;
    public bool isGhostInitial;
    public bool isLeft;
    protected GameObject line;
    protected GameObject label;
    protected RectTransform labelTransform;
    protected Text text = null;

    void Start()
    {
        line = transform.Find("Line").gameObject;
        label = transform.Find("Canvas").gameObject;
        labelTransform = label.GetComponent<RectTransform>();
        text = label.transform.Find("Text").GetComponent<Text>();
        text.text = labelTextInitial;
        Ghost(isGhostInitial);
        Activate(false);
    }

    void Update()
    {
        if (label.activeSelf)
        {
            labelTransform.forward = labelTransform.position - headPose.position;
        }
    }

    public void SetText(string labelText)
    {
        if (text != null)
            text.text = labelText;
    }

    public void Ghost(bool isGhost)
    {
        if (text != null)
        {
            Color newColor = isGhost ? Color.grey : Color.white;
            text.color = newColor;
        }
    }

    // Update is called once per frame
    public void Activate(bool isActive)
    {
        if (line != null && label != null)
        {
            line.SetActive(isActive);
            label.SetActive(isActive);
        }
    }
}
