using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLabelController : MonoBehaviour
{
    public Transform headPose;
    public string labelText;
    protected GameObject line;
    protected GameObject label;
    protected RectTransform labelTransform;
    void Start()
    {
        line = transform.Find("Line").gameObject;
        label = transform.Find("Canvas").gameObject;
        label.transform.Find("Text").GetComponent<Text>().text = labelText;
        labelTransform = label.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (label.activeSelf)
        {
            labelTransform.forward = labelTransform.position - headPose.position;
        }
    }

    // Update is called once per frame
    public void Activate(bool isActive)
    {
        line.SetActive(isActive);
        label.SetActive(isActive);
    }
}
