using System.Collections;
using UnityEngine;
using FMODUnity;

public class AmbiencePatrol : StudioEventEmitterOcclusion
{
    bool coroutineAllowed = true;
    public bool activated;
    public Vector3 startPosition;
    public Vector3 endPosition;

    public GameObject startMark;
    public GameObject endMark;
    GameObject player;
    float distance;

    private void Awake()
    {
        // Get player, start marker, and end marker positions
        player = GameObject.Find("OVRPlayerController");
        startPosition = startMark.transform.position;
        endPosition = endMark.transform.position;
        transform.position = startPosition;

        // Compute distance between start marker and end marker
        if (startPosition.x == endPosition.x)
        {
            distance = Mathf.Abs(endPosition.z - startPosition.z);
        }

        else if (startPosition.z == endPosition.z)
        {
            distance = Mathf.Abs(endPosition.x - startPosition.x);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize FMOD Instance
        fmodInstance = RuntimeManager.CreateInstance(fmodEvent);
        RuntimeManager.AttachInstanceToGameObject(fmodInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());

        if (name == "Patrol 1")
        {
            activated = true;
        }

        else
        {
            activated = false;
        }

        if (activated)
        {
            StartPatrol();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            if (coroutineAllowed)
            {
                /*
                float angle = Vector3.Angle(player.transform.forward, startMark.transform.position - player.transform.position);
                Debug.Log("Angle");
                Debug.Log(angle);
                */
                StartCoroutine("Move");
            }
        }

        else
        {
            StopPatrol();
        }
    }

    public void StartPatrol()
    {
        fmodInstance.setParameterByName("Attenuation", distance);
        fmodInstance.start();
    }

    public void StopPatrol()
    {
        StopCoroutine("Move");
        coroutineAllowed = true;
        fmodInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ForwardPatrol()
    {
        startPosition = startMark.transform.position;
        endPosition = endMark.transform.position;
    }

    public void ReversePatrol()
    {
        startPosition = endMark.transform.position;
        endPosition = startMark.transform.position;
    }

    IEnumerator Move()
    {
        coroutineAllowed = false;

        float elapsed = 0;
        float duration = 4.559f;
        transform.position = startPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float time = Vector3.Distance(transform.position, endPosition) / (duration - elapsed) * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, endPosition, time);

            yield return null;
        }

        coroutineAllowed = true;
    }
}
