using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BiteSelf : MonoBehaviour
{
    public int rotateSpeed;

    private BiteController _biteController;
    private AudioSource _audioSource;
    private int biteIdx;
    private bool found = false;
    private bool continueRotating = true;
    private string soundPath = "file://" + Application.streamingAssetsPath + "/Test-Song-1/";
    private string audioName;
    private AudioClip audioClip;

    private float playbackOrder;
    private Color originalColor;

    private void Start()
    {
        _biteController = gameObject.GetComponentInParent<BiteController>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        originalColor = gameObject.GetComponentInChildren<Renderer>().material.color;
        Debug.Log("original color: " + originalColor);
        playbackOrder = _audioSource.pitch;
    }

    private void FixedUpdate()
    {
        if (continueRotating)
        {
            transform.Rotate(0f, -playbackOrder * rotateSpeed * Time.deltaTime, 0f);
        }
        // Testing Purposes - Manipulate inspector valus while in debug mode
        ImplementVolumeColor();
        if (playbackOrder != _audioSource.pitch) { Reverse(); }
    }

    public void SetBiteIdx(int idx)
    {
        biteIdx = idx;
        audioName = "sample-" + idx.ToString() + ".mp3";
        StartCoroutine(LoadAudio());
    }

    public void StopRotating()
    {
        continueRotating = false;
    }

    public int GetBiteIdx()
    {
        return biteIdx;
    }

    // Source: https://youtu.be/9gAHZGArDgU
    // Note: The type is deprecated so I tried upgrading it to the suggested type
    // (UnityWebRequest) but there was a method issue with the 3rd line, so I just
    // reverted back. 
    private IEnumerator LoadAudio()
    {
        WWW request = GetAudioFromFile(soundPath, audioName);
        yield return request;

        audioClip = request.GetAudioClip();
        audioClip.name = audioName;
        _audioSource.clip = audioClip;
        PlayAudioFile(); // to verify attachment, just unclick "mute" on the `AudioSouce` component
    }

    private void PlayAudioFile()
    {
        _audioSource.Play();
        _audioSource.loop = true;
    }

    private WWW GetAudioFromFile(string path, string filename)
    {
        string audioToLoad = string.Format(path + "{0}", filename);
        WWW request = new WWW(audioToLoad);
        return request;
    }

    private void GetNewVolume()
    {
        // raise or lower the volume depending on movement
        // min = 0.0f, max = 1.0f
        // prefab default is 0.5
        float newVolume = 0.5f; // insert funny math for calculating new volume
        _audioSource.volume = newVolume;
        ImplementVolumeColor();
    }

    public void ImplementVolumeColor()
    {
        // raise volume -> darker shade
        // lower volume -> lighter shade
        Color newColor = new Color(
            originalColor.r,                            // r
            1 - ((255 * _audioSource.volume) / 255),    // g
            originalColor.b);                           // b
        gameObject.GetComponentInChildren<Renderer>().material.SetColor("_Color", newColor);
        gameObject.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", newColor);
    }

    // Source: https://forum.unity.com/threads/playing-audio-backwards.95770/
    private void Reverse()
    {
        // left and right (color?) indicators
        switch (playbackOrder) // to undo the reverse, just set to -1 or 1
        {
            case 1:
                _audioSource.pitch = -1;
                playbackOrder = -1;
                break;
            case -1:
                _audioSource.pitch = 1;
                playbackOrder = 1;
                break;
        }
        //_audioSource.timeSamples = _audioSource.clip.samples - 1;  // keeping for now...
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!found)
        {
            _biteController.FoundBite(gameObject, biteIdx);
            found = true;
            // add some animation (if possible)
        }
    }
}
