using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitButton : OVRGrabbable
{
    public GameObject songBitesContainer;

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
        //PlayAudioInOrder();
    }

    /**
    If the controller 'lets go' of the button, activate the StopAudio command 
    which will stop the audio from playing.
    **/
    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        Debug.Log("Submit: Lets go");
        EmissionStatus(false);
        // StopAudio();
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
    private void StopAudio()
    {
        Debug.Log("Stopping Audio");
    }

    /**
    Iterate on every sound bite on stage and play it's corresponding audioclip by
    unmuting the audio clip, stopping it from looping, and schedule the song's playing.
    **/
    private void PlayAudioInOrder()
    {
        for (int i=0; i< songBitesContainer.transform.childCount; i++)
        {
            AudioSource audioSource = songBitesContainer.transform.GetChild(i).gameObject.GetComponent<AudioSource>();
            AudioClip audioclip = audioSource.clip;
            double duration = (double)audioclip.samples / audioclip.frequency;
            audioSource.mute = false;
            audioSource.loop = false;
            audioSource.PlayScheduled(duration * (i+1)); // needs fixing
        }
    }
}

