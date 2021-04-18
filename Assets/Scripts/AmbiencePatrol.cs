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

    // Start is called before the first frame update
    void Start()
    {
        startPosition = startMark.transform.position;
        endPosition = endMark.transform.position;
        transform.position = startPosition;
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
