using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BiteSelf : MonoBehaviour
{
    private BiteController _biteController;
    private AudioSource _audioSource;
    private int biteIdx;
    private bool found = false;
    private string soundPath = "Songs/Test-Song-1/";
    private string audioName;
    private AudioClip audioClip;
    private float playbackOrder;
    private Color originalColor;
    private Vector3 originalPosition;
    private Quaternion originalAngles;
    private Vector3 foundPosition;

    public bool currentlySelected = false;
    public int rotateSpeed;

    private void Start()
    {
        _biteController = gameObject.GetComponentInParent<BiteController>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        originalColor = gameObject.GetComponent<Renderer>().material.color; // solo: used to be child
        originalPosition = transform.position;
        playbackOrder = _audioSource.pitch;
        originalAngles = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (!found)
        {
           float newY = Mathf.Sin(Time.time * 1.5f) * 0.5f + originalPosition.y;
           transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        if (!currentlySelected) {
            transform.Rotate(0f, -playbackOrder * rotateSpeed * Time.deltaTime, 0f);        
        }
        //if (playbackOrder != _audioSource.pitch) { Reverse(); }
    }

    public void SetBiteIdx(int idx)
    {
        biteIdx = idx;
        audioName = "sample-" + idx.ToString();
        //StartCoroutine(LoadAudio());
        audioClip = Resources.Load<AudioClip>(soundPath + audioName);
        audioClip.name = audioName;
        _audioSource.clip = audioClip;
        PlayAudioFile();
    }

    public int GetBiteIdx()
    {
        return biteIdx;
    }

    private void PlayAudioFile()
    {
        _audioSource.Play();
        _audioSource.loop = true;
    }

    private void GetNewVolume()
    {
        // raise or lower the volume depending on movement
        // min = 0.0f, max = 1.0f
        // prefab default is 0.5
        float newVolume = 0.5f; // insert funny math for calculating new volume
        _audioSource.volume = newVolume;
        // insert fancy math to pass onto implement volume color
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
        // both: used to be parent
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", newColor); 
        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", newColor);
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
        }
        else
        {
            if (other.CompareTag("Sound Bite"))
            {
                SwapInHierarchy(other.gameObject);
            }
        }
    }

    private void SwapInHierarchy(GameObject obj)
    {
        if (currentlySelected)
        {
            BiteSelf otherBiteSelf = obj.GetComponent<BiteSelf>();
            Vector3 otherPosition = otherBiteSelf.GetFoundPosition();
            Vector3 currPosition = GetFoundPosition();
            SetFoundPosition(otherPosition);
            transform.position = otherPosition;
            otherBiteSelf.SetFoundPosition(currPosition);
            obj.transform.position = currPosition;

            int otherSiblingIndex = obj.transform.GetSiblingIndex();
            int currSiblingIndex = transform.GetSiblingIndex();
            obj.transform.SetSiblingIndex(currSiblingIndex);
            transform.SetSiblingIndex(otherSiblingIndex);
        }
    }

    public void SetCurrentlySelected(bool isSelected)
    {
        currentlySelected = isSelected;
        if (!isSelected){
            transform.position = foundPosition;
            transform.rotation = originalAngles;
        }
    }

    public void SetFoundPosition(Vector3 position)
    {
        foundPosition = position;
    }

    public Vector3 GetFoundPosition()
    {
        return foundPosition;
    }

    //// Source: https://youtu.be/9gAHZGArDgU
    //// Note: The type is deprecated so I tried upgrading it to the suggested type
    //// (UnityWebRequest) but there was a method issue with the 3rd line, so I just
    //// reverted back. 
    //private IEnumerator LoadAudio()
    //{
    //    WWW request = GetAudioFromFile(soundPath, audioName);
    //    yield return request;

    //    audioClip = request.GetAudioClip();
    //    audioClip.name = audioName;
    //    _audioSource.clip = audioClip;
    //    PlayAudioFile(); // to verify attachment, just unclick "mute" on the `AudioSouce` component
    //}

    //private WWW GetAudioFromFile(string path, string filename)
    //{
    //    string audioToLoad = string.Format(path + "{0}", filename);
    //    WWW request = new WWW(audioToLoad);
    //    return request;
    //}
}
