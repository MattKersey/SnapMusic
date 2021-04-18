using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalControls : MonoBehaviour
{
    public enum States { EDIT, EXPLORE };
    public float maxSpeed = 0.25f;
    public float minSpeed = 0.15f;
    protected bool leftInContact = false;
    protected bool rightInContact = false;
    protected States currentState = States.EXPLORE;
    protected HashSet<string> EditGhosts;
    protected HashSet<string> ExploreGhosts;
    protected Dictionary<string, ButtonLabelController[]> labelControllers = new Dictionary<string, ButtonLabelController[]>();
    protected Dictionary<string, string> labelTexts = new Dictionary<string, string>();
    protected OVRPlayerController playerController;
    protected bool labelsActive = false;
    [SerializeField]
    protected string aLabelText;
    [SerializeField]
    protected ButtonLabelController[] aLabels;
    [SerializeField]
    protected string bLabelText;
    [SerializeField]
    protected ButtonLabelController[] bLabels;
    [SerializeField]
    protected string xLabelText;
    [SerializeField]
    protected ButtonLabelController[] xLabels;
    [SerializeField]
    protected string yLabelText;
    [SerializeField]
    protected ButtonLabelController[] yLabels;
    [SerializeField]
    protected string iLabelText;
    [SerializeField]
    protected ButtonLabelController[] iLabels;
    [SerializeField]
    protected string hLabelText;
    [SerializeField]
    protected ButtonLabelController[] hLabels;
    [SerializeField]
    protected string lLabelText;
    [SerializeField]
    protected ButtonLabelController[] lLabels;
    [SerializeField]
    protected string rLabelText;
    [SerializeField]
    protected ButtonLabelController[] rLabels;

    void Start()
    {
        playerController = GetComponent<OVRPlayerController>();
        string[] ed = { "a", "b", "h" };
        EditGhosts = new HashSet<string>(ed);
        string[] ex = { "l", "r", "x", "y", "a", "h" };
        ExploreGhosts = new HashSet<string>(ex);
        labelControllers.Add("a", aLabels);
        labelTexts.Add("a", aLabelText);
        labelControllers.Add("b", bLabels);
        labelTexts.Add("b", bLabelText);
        labelControllers.Add("x", xLabels);
        labelTexts.Add("x", xLabelText);
        labelControllers.Add("y", yLabels);
        labelTexts.Add("y", yLabelText);
        labelControllers.Add("i", iLabels);
        labelTexts.Add("i", iLabelText);
        labelControllers.Add("h", hLabels);
        labelTexts.Add("h", hLabelText);
        labelControllers.Add("l", lLabels);
        labelTexts.Add("l", lLabelText);
        labelControllers.Add("r", rLabels);
        labelTexts.Add("r", rLabelText);
        foreach (string buttonTitle in labelControllers.Keys)
        {
            Debug.Log(labelControllers[buttonTitle].Length);
            Debug.Log(labelTexts[buttonTitle]);
            foreach (ButtonLabelController label in labelControllers[buttonTitle])
            {
                label.labelTextInitial = labelTexts[buttonTitle];
                label.isGhostInitial = ExploreGhosts.Contains(buttonTitle);
                label.SetText(labelTexts[buttonTitle]);
                label.Activate(false);
            }
        }
        SetGhostLabels();
    }

    public void UpdateState(States newState)
    {
        if (newState != currentState)
        {
            currentState = newState;
            SetGhostLabels();
        }
    }

    protected void SetGhostLabels()
    {
        HashSet<string> ghosts = new HashSet<string>();
        switch (currentState)
        {
            case States.EDIT:
                ghosts = EditGhosts;
                break;
            case States.EXPLORE:
                ghosts = ExploreGhosts;
                break;
        }
        foreach (string buttonTitle in labelControllers.Keys)
        {
            foreach (ButtonLabelController label in labelControllers[buttonTitle])
            {
                label.Ghost(ghosts.Contains(buttonTitle));
            }
        }
        SetContactLabels();
    }

    public void SetInMainRoom(bool isInRoom)
    {
        foreach (ButtonLabelController label in labelControllers["a"])
        {
            label.Ghost(isInRoom);
        }
    }

    public void SetInContact(bool isLeft, bool isInContact)
    {
        if (isLeft)
            leftInContact = isInContact;
        else
            rightInContact = isInContact;
        SetContactLabels();
    }

    protected void SetContactLabels()
    {
        if (currentState == States.EDIT)
        {
            foreach (ButtonLabelController label in labelControllers["y"])
            {
                label.Ghost(!leftInContact && !rightInContact);
            }
        }
        foreach (ButtonLabelController label in labelControllers["h"])
        {
            if (label.isLeft)
                label.Ghost(!leftInContact);
            else
                label.Ghost(!rightInContact);
        }
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            Debug.Log("r1");
            //playerController.Jump();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
        {
            Debug.Log("rs");
            CustomController.Redo();
        }
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch))
        {
            Debug.Log("ls");
            CustomController.Undo();
        }

        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            Debug.Log("Start");
            labelsActive = !labelsActive;
            foreach (ButtonLabelController[] buttonLabelControllers in labelControllers.Values)
            {
                foreach (ButtonLabelController label in buttonLabelControllers)
                {
                    label.Activate(labelsActive);
                }
            }
        }

        if (
            currentState == States.EXPLORE &&
            OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) > 0.0f &&
            OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > 0.0f
        )
        {
            playerController.Acceleration = Mathf.Clamp(playerController.Acceleration + .002f, minSpeed, maxSpeed);
        }
        else
        {
            playerController.Acceleration = minSpeed;
        }

        SetInMainRoom(!(
            transform.position.x > 8 ||
            transform.position.z > 8 ||
            transform.position.x < -8 ||
            transform.position.z < -8
        ));
    }
}
