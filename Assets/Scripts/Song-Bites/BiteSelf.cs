using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiteSelf : MonoBehaviour
{

    private BiteController _biteController;
    private AudioSource _audioSource;
    private int biteIdx;
    public int rotateSpeed;

    private void Start()
    {
        _biteController = gameObject.GetComponentInParent<BiteController>();
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }

    public void SetBiteIdx(int idx)
    {
        biteIdx = idx;
    }

    private void ScaleVolume()
    {
        // raise or lower the volume depending on movement
        // change color depending on volume
        // raise -> darker
        // lower -> lighter

        // constraints
        // min = 0.0f, max = 1.0f

        float newVolume = 0.75f; // example
        _audioSource.volume = newVolume;
    }

    private void Reverse()
    {
        // left and right (color?) indicators
        // red or left-arrow -> reverse order of current song
        // green or right-arrow -> restore order of current song
    }

    // NOTE: If not using Triggers, then delete this function
    // and uncheck the `isTrigger` boolean in `Bite`
    private void OnTriggerEnter(Collider other)
    {
        _biteController.FoundBite();
        // add some animation (if possible)
    }

    // NOTE: If not using rigidbodies, then delete this function
    // and remove the `Rigidbody` component from `Bite`
    private void OnCollisionEnter(Collision collision)
    {
        _biteController.FoundBite();
        // add some animation (if possible)
    }
}
