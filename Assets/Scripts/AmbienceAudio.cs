using System.Collections;
using UnityEngine;
using FMODUnity;

public class AmbienceAudio : StudioEventEmitterOcclusion
{
    bool coroutineAllowed = true;
    Vector3 startPosition;
    Vector3 endPosition;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Loading Bite");
        fmodEvent = "event:/Ambience";
        fmodInstance = RuntimeManager.CreateInstance(fmodEvent);
        RuntimeManager.AttachInstanceToGameObject(fmodInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        fmodInstance.start();
        startPosition = transform.position;
        endPosition = transform.position;
        endPosition.x = -startPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        coroutineAllowed = false;

        float counter = 0;
        float duration = 5f;
        transform.position = startPosition;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float time = Vector3.Distance(transform.position, endPosition) / (duration - counter) * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, endPosition, time);

            yield return null;
        }
        coroutineAllowed = true;
    }
}
