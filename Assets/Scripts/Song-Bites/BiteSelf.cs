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


    private void Start()
    {
        _biteController = gameObject.GetComponentInParent<BiteController>();
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (continueRotating)
        {
            transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
        }
    }

    public void SetBiteIdx(int idx)
    {
        biteIdx = idx;
        audioName = "sample-" + idx.ToString() + ".mp3";
        StartCoroutine(LoadAudio());
    }

    public void stopRotating()
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
        // to verify attachment, just unclick "mute" on the `AudioSouce` component
        PlayAudioFile();
        Reverse(); // uncomment to verify play in reverse order
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

    private void ScaleVolume()
    {
        // raise or lower the volume depending on movement
        // change color depending on volume
        // raise -> darker
        // lower -> lighter

        // constraints
        // min = 0.0f, max = 1.0f
        float newVolume = 0.75f; // prefab default is 0.5
        _audioSource.volume = newVolume;
    }

    // Source: https://forum.unity.com/threads/playing-audio-backwards.95770/
    private void Reverse()
    {
        // left and right (color?) indicators
        // red or left-arrow -> reverse order of current song
        // green or right-arrow -> restore order of current song

        _audioSource.pitch = -1; // to undo the reverse, just set to 1
        //_audioSource.timeSamples = _audioSource.clip.samples - 1;  // keep for now...
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
