using UnityEngine;

public class AmbienceDoorTrigger : MonoBehaviour
{
    public GameObject patrol1;
    public GameObject patrol2;

    AmbiencePatrol patrolAmbience1;
    AmbiencePatrol patrolAmbience2;

    Vector3 start;
    Vector3 end;

    float enterDirection;
    float exitDirection;
    string state = "none";
    bool firstDoor = false;

    private void Start()
    {
        if (patrol1 != null)
        {
            patrolAmbience1 = patrol1.GetComponent<AmbiencePatrol>();
        }
        
        if (patrol2 != null)
        {
            patrolAmbience2 = patrol2.GetComponent<AmbiencePatrol>();
        }

        if (name == "Door Trigger 1")
        {
            firstDoor = true;
            start = patrolAmbience1.endPosition;
            end = patrolAmbience2.startPosition;
        }

        else
        {
            if (patrol1 != null)
            {
                start = patrolAmbience1.startPosition;
                end = patrolAmbience1.endPosition;
            }

            else if (patrol2 != null)
            {
                start = patrolAmbience2.startPosition;
                end = patrolAmbience2.endPosition;
            }
        }
    }

    void ComputeDirection(Vector3 player, Vector3 trigger, bool enter)
    {
        // Get direction of player movement
        Vector3 heading = player - trigger;
        float direction = 0.0f;

        if (start.x == end.x)
        {
            if (firstDoor)
            {
                direction = (heading / heading.magnitude).z;
            }
            
            else
            {
                direction = (heading / heading.magnitude).x;
            }

        }

        else if (start.z == end.z)
        {
            if (firstDoor)
            {
                direction = (heading / heading.magnitude).x;
            }

            else
            {
                direction = (heading / heading.magnitude).z;
            }
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
        if (patrolAmbience1 != null)
        {
            patrolAmbience1.StopPatrol();
        }
        
        if (patrolAmbience2 != null)
        {
            patrolAmbience2.StopPatrol();
        }

        if (enterDirection > 0)
        {
            if (patrolAmbience1 != null)
            {
                patrolAmbience1.activated = false;
            }
            
            if (patrolAmbience2 != null)
            {
                patrolAmbience2.StartPatrol();
                patrolAmbience2.activated = true;
            }
        }

        else if (enterDirection < 0)
        {
            if (patrolAmbience2 != null)
            {
                patrolAmbience2.activated = false;
            }

            if (patrolAmbience1 != null)
            {
                patrolAmbience1.StartPatrol();
                patrolAmbience1.activated = true;
            }
        }
    }
}
