using UnityEngine;

public class AmbienceLTrigger : MonoBehaviour
{
    public GameObject patrol1;
    public GameObject patrol2;

    AmbiencePatrol patrolAmbience1;
    AmbiencePatrol patrolAmbience2;

    float enterDirection = 0.0f;
    float exitDirection = 0.0f;

    private void Start()
    {
        patrolAmbience1 = patrol1.GetComponent<AmbiencePatrol>();
        patrolAmbience2 = patrol2.GetComponent<AmbiencePatrol>();
    }

    public void ComputeDirection(Vector3 player, Vector3 trigger, string name, bool enter)
    {
        // Get direction of player movement based on respective child trigger
        Vector3 heading = player - trigger;
        float direction = 0.0f;
        Vector3 endPosition = Vector3.zero;
        Vector3 startPosition = Vector3.zero;
        if (name == "Front Trigger")
        {
            endPosition = patrolAmbience1.endPosition;
            startPosition = patrolAmbience1.startPosition;
        }

        else if (name == "Back Trigger")
        {
            endPosition = patrolAmbience2.endPosition;
            startPosition = patrolAmbience2.startPosition;
        }

        if (endPosition.x == startPosition.x)
        {
            direction = (heading / heading.magnitude).z;
        }

        else if (endPosition.z == startPosition.z)
        {
            direction = (heading / heading.magnitude).x;
        }

        if (enter)
        {
            enterDirection = direction;
        }

        else
        {
            exitDirection = direction;
            if (Mathf.Sign(enterDirection) != Mathf.Sign(exitDirection))
            {
                ActivatePatrols(name);
            }
        }
    }

    private void ActivatePatrols(string name)
    {
        // Stop all patrols
        patrolAmbience1.StopPatrol();
        patrolAmbience2.StopPatrol();

        // Re-activate patrols based on enter direction and trigger name
        if (enterDirection > 0)
        {
            if (name == "Front Trigger")
            {
                patrolAmbience1.ForwardPatrol();
                patrolAmbience2.ForwardPatrol();
            }

            else if (name == "Back Trigger")
            {
                patrolAmbience1.ReversePatrol();
                patrolAmbience2.ReversePatrol();
            }

            patrolAmbience1.activated = true;
            patrolAmbience1.StartPatrol();
            patrolAmbience2.activated = true;
            patrolAmbience2.StartPatrol();
        }

        else if (enterDirection < 0 && name == "Front Trigger")
        {
            patrolAmbience1.activated = true;
            patrolAmbience1.StartPatrol();
            patrolAmbience2.activated = false;
        }

        else if (enterDirection < 0 && name == "Back Trigger")
        {
            patrolAmbience1.activated = false;
            patrolAmbience2.activated = true;
            patrolAmbience2.StartPatrol();
        }
    }

    public void StartAllPatrols()
    {
        patrolAmbience1.activated = true;
        patrolAmbience1.StartPatrol();
        patrolAmbience2.activated = true;
        patrolAmbience2.StartPatrol();
    }

    public void StopAllPatrols()
    {
        patrolAmbience1.activated = false;
        patrolAmbience1.StopPatrol();
        patrolAmbience2.activated = false;
        patrolAmbience2.StopPatrol();
    }
}
