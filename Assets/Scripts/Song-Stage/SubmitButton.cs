using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitButton : MonoBehaviour
{
    public GameObject songBitesContainer;
    private BiteController _biteController;

    private void Start()
    {
        _biteController = songBitesContainer.GetComponent<BiteController>();
    }

    // TODO: Delete these two and have a trigger command that would
    // enable `PlayAudioInOrder`
    private void OnTriggerEnter(Collider other)
    {
        PlayAudioInOrder();
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
