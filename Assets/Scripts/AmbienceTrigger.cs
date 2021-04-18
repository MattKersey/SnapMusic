using UnityEngine;

public class AmbienceTrigger : MonoBehaviour
{

    public GameObject patrol1;
    public GameObject patrol2;

    AmbiencePatrol patrolAmbience1;
    AmbiencePatrol patrolAmbience2;

    private void Start()
    {
        patrolAmbience1 = patrol1.GetComponent<AmbiencePatrol>();
        patrolAmbience2 = patrol2.GetComponent<AmbiencePatrol>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Get direction of player movement
        Vector3 heading = other.transform.position - transform.position;
        float direction = 0.0f;
        if (patrolAmbience1.endPosition.x == patrolAmbience2.startPosition.x) 
        {
            direction = (heading / heading.magnitude).z;
        }

        else if (patrolAmbience1.endPosition.z == patrolAmbience2.startPosition.z)
        {
            direction = (heading / heading.magnitude).x;
        }

        patrolAmbience1.StopPatrol();
        patrolAmbience2.StopPatrol();

        if (direction > 0)
        {
            patrolAmbience1.activated = false;
            patrolAmbience2.StartPatrol();
            patrolAmbience2.activated = true;
        }
        
        else if (direction < 0)
        {
            patrolAmbience2.activated = false;
            patrolAmbience1.StartPatrol();
            patrolAmbience1.activated = true;
        }
    }
}
