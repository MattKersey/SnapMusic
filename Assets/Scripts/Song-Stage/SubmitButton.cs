using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class SubmitButton : OVRGrabbable
{
    public GameObject songBitesContainer;
    public bool isPlaying = false;
    protected List<EventInstance> instances = new List<EventInstance>();
    protected List<Coroutine> coroutines = new List<Coroutine>();

    /**
    If the controller 'grabs' the button, activate the PlayAudioInOrder command 
    which will iteratively play the audioclip from every sound bite on stage in
    the order that it's placed.
    **/
    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        Debug.Log("Submit: Grabs");
        EmissionStatus(true);
    }

    /**
    If the controller 'lets go' of the button, activate the StopAudio command 
    which will stop the audio from playing.
    **/
    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        Debug.Log("Submit: Lets go");
        if (isPlaying)
        {
            StopAudio();
        }
        else
            PlayAudioInOrder();
    }

    // Turns on/off the button's emission to indicate selected status
    private void EmissionStatus(bool status)
    {
        if (status)
        {
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
        else
        {
            GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }

    /**
    Stop the app from playing any audioclip from the cubes on stage.
    **/
    public void StopAudio()
    {
        Debug.Log("Stopping Audio");
        foreach (Coroutine coroutine in coroutines)
        {
            StopCoroutine(coroutine);
        }
        foreach (EventInstance instance in instances)
        {
            instance.stop(STOP_MODE.ALLOWFADEOUT);
        }
        instances.Clear();
        coroutines.Clear();
        isPlaying = false;
        EmissionStatus(false);
    }

    static int SortByX(Transform t0, Transform t1)
    {
        return t0.position.x.CompareTo(t1.position.x);
    }

    /**
    Iterate on every sound bite on stage and play it's corresponding audioclip by
    unmuting the audio clip, stopping it from looping, and schedule the song's playing.
    **/
    private void PlayAudioInOrder()
    {
        List<Transform> bites = new List<Transform>();
        for (int i = 0; i < songBitesContainer.transform.childCount; i++)
        {
            bites.Add(songBitesContainer.transform.GetChild(i));
        }
        bites.Sort(SortByX);

        int totalLength = 0;
        int index = 0;
        foreach (Transform bite in bites)
        {
            EventDescription description;
            EventInstance instance = bite.GetComponent<BiteAudio>().fmodInstance;
            instance.getDescription(out description);
            float direction;
            instance.getParameterByName("Direction", out direction);
            EventInstance instanceCopy;
            description.createInstance(out instanceCopy);
            instanceCopy.setParameterByName("Direction", direction);
            instanceCopy.setVolume(3.0f);
            int length;
            description.getLength(out length);
            instances.Add(instanceCopy);
            coroutines.Add(StartCoroutine(PlayBites(totalLength, length, instanceCopy, bite, index==5)));
            totalLength += length;
            index += 1;
        }
        isPlaying = true;
    }

    IEnumerator PlayBites(int pre, int post, EventInstance instance, Transform bite, bool final = false)
    {
        yield return new WaitForSeconds(((float)pre) / 1000f);
        instance.start();
        bite.GetComponent<BiteSelf>().ColorBite(bite.gameObject, false);
        yield return new WaitForSeconds(((float)post) / 1000f);
        if (final)
        {
            instance.stop(STOP_MODE.ALLOWFADEOUT);
            instances.Clear();
            coroutines.Clear();
            isPlaying = false;
            EmissionStatus(false);
        }
        else
            instance.stop(STOP_MODE.ALLOWFADEOUT);

        bite.GetComponent<BiteSelf>().UnColorBite(bite.gameObject);
    }
}