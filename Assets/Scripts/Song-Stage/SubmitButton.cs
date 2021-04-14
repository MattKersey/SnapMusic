using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitButton : OVRGrabbable
{
    public GameObject songBitesContainer;

    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        //PlayAudioInOrder();
        Debug.Log("Submit: Grab Begin");
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        StopAudio();
    }

    private void StopAudio()
    {
        Debug.Log("Stopping Audio");
    }

    private void PlayAudioInOrder()
    {
        for (int i=0; i< songBitesContainer.transform.childCount; i++)
        {
            GameObject songBite = songBitesContainer.transform.GetChild(i).gameObject;
            PlayAudioFile(songBite.GetComponent<AudioSource>());
        }
    }

    private void PlayAudioFile(AudioSource audioSource)
    {
        AudioClip audioclip = audioSource.clip;
        double duration = (double)audioclip.samples / audioclip.frequency;
        audioSource.mute = false;
        audioSource.loop = false;
        audioSource.PlayScheduled(duration);
    }
}

