using UnityEngine;

public class AmbienceDoorTrigger : MonoBehaviour
{
    public GameObject patrol1;
    public GameObject patrol2;

    AmbiencePatrol patrolAmbience1;
    AmbiencePatrol patrolAmbience2;

    float enterDirection;
    float exitDirection;
    string state = "none";


    private void Start()
    {
        patrolAmbience1 = patrol1.GetComponent<AmbiencePatrol>();
        patrolAmbience2 = patrol2.GetComponent<AmbiencePatrol>();
    }

    void ComputeDirection(Vector3 player, Vector3 trigger, bool enter)
    {
        // Get direction of player movement
        Vector3 heading = player - trigger;
        float direction = 0.0f;
        if (patrolAmbience1.endPosition.x == patrolAmbience2.startPosition.x)
        {
            direction = (heading / heading.magnitude).z;
        }

        else if (patrolAmbience1.endPosition.z == patrolAmbience2.startPosition.z)
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
                ActivatePatrols();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state != "enter")
        {
            ComputeDirection(other.transform.position, transform.position, true);
            state = "enter";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (state != "exit")
        {
            ComputeDirection(other.transform.position, transform.position, false);
            state = "exit";
        }
    }

    void ActivatePatrols()
    {
        patrolAmbience1.StopPatrol();
        patrolAmbience2.StopPatrol();

        if (enterDirection > 0)
        {
            patrolAmbience1.activated = false;
            patrolAmbience2.StartPatrol();
            patrolAmbience2.activated = true;
        }

        else if (enterDirection < 0)
        {
            patrolAmbience2.activated = false;
            patrolAmbience1.StartPatrol();
            patrolAmbience1.activated = true;
        }
    }
}
