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
    private Color originalColor;
    private Vector3 originalPosition;
    private Quaternion originalAngles;
    private Vector3 foundPosition;
    private Material originalMaterial;
    public Material highlightMaterial;
    public Material grabbedMaterial;
    private GameObject cubeInContact;


    public bool currentlySelected = false;
    public int rotateSpeed;
    public float playbackOrder;


    // Store the initail positions, rotation, color, and pitch
    private void Start()
    {
        _biteController = gameObject.GetComponentInParent<BiteController>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        originalMaterial = gameObject.GetComponent<Renderer>().material;
        originalColor = gameObject.GetComponent<Renderer>().material.color;
        originalPosition = transform.position;
        // playbackOrder = _audioSource.pitch;
        originalAngles = transform.rotation;
    }

    /**
    If the bite has not been found, then have it bounce in air.
    If the bite is not currently selected, then allow it to rotate according
    to it's playback order (pitch).
    **/
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

    // Give the bite a random index/sample from the song 
    public void SetBiteIdx(int idx)
    {
        biteIdx = idx;
        audioName = "sample-" + idx.ToString();
        //StartCoroutine(LoadAudio());
        //audioClip = Resources.Load<AudioClip>(soundPath + audioName);
        //audioClip.name = audioName;
        //_audioSource.clip = audioClip;
        //PlayAudioFile();
    }

    // Give the bite a random pitch. Note: 1=normal, -1=reverse
    public void SetRandomPitch()
    {
        System.Random random = new System.Random();
        List<int> pitches = new List<int> { -1, 1 };
        playbackOrder = pitches[random.Next(pitches.Count)];
        Debug.Log("idx: " + biteIdx + ", pitch: " + playbackOrder);
        //_audioSource.pitch = playbackOrder;
    }

    // Public method to get the bite (song) index
    public int GetBiteIdx()
    {
        return biteIdx;
    }

    // Public method to get playback order (pitch)
    public float GetPlayBackOrder()
    {
        return playbackOrder;
    }

    // Play the audio clip and set it to loop
    private void PlayAudioFile()
    {
        _audioSource.Play();
        _audioSource.loop = true;
    }

    /**
    Calculuates the math to determine the new volume based on how much the
    user scales the bite.
    **/
    private void GetNewVolume()
    {
        // raise or lower the volume depending on movement
        // min = 0.0f, max = 1.0f
        // prefab default is 0.5
        float newVolume = 0.5f; // insert funny math for calculating new volume
        _audioSource.volume = newVolume;
        // insert fancy math to pass onto implement volume color
        // ImplementVolumeColor();
    }

    /**
    Change the bite material's color and emission depending on how much the user
    scales the cube's volume. 
    **/
    public void ImplementVolumeColor(float scalar)
    {
        // raise volume -> darker shade
        // lower volume -> lighter shade
        Color newColor = new Color(
            originalColor.r,                            // r
            1 - ((255 * scalar) / 255),    // g
            originalColor.b);                           // b
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", newColor); 
        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", newColor);
        //originalMaterial = gameObject.GetComponent<Renderer>().material;
    }

    /**
    Public method to reverse the pitch of the audio.
    Source: https://forum.unity.com/threads/playing-audio-backwards.95770/
    **/
    public void Reverse()
    {
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

    // Performs the swap when the bite is in contact with another bite
    public void PerformSwap()
    {
        if (cubeInContact != null)
        {
            _biteController.AddSwap();
            SwapInHierarchy(cubeInContact);
        }
    }

    // Removes the color from the collided bite
    public void RemoveCollidedColor()
    {
        if (cubeInContact != null)
        {
            UnColorBite(cubeInContact);
        }
    }

    // Color a bite to show that it is selected or in collision with (highlight)
    public void ColorBite(GameObject obj, bool isHighlight)
    {
        if (isHighlight) // collided case
        {
            obj.GetComponent<Renderer>().material = highlightMaterial;
        }
        else // grabbed
        {
            obj.GetComponent<Renderer>().material = grabbedMaterial;
        }
    }

    // Uncolors a bite to show that it is no longer selected or in collision with
    public void UnColorBite(GameObject obj)
    {
        obj.GetComponent<Renderer>().material = originalMaterial;
    }

    /**
    If the bite collides with another bite and both bites have been found (i.e. on stage),
    then swap them. Else if the bite hasn't been found (i.e. the user has just encountered
    it and collides it's controller with the bite), then mark the bite as found.
    **/
    private void OnTriggerEnter(Collider other)
    {
        // if the current bite enters the collider of another bite 
        if (other.CompareTag("Sound Bite"))
        {
            // and both are already on stage, then highlight the other one
            if (found && other.gameObject.GetComponent<BiteSelf>().found && currentlySelected)
            {
                cubeInContact = other.gameObject;
                ColorBite(other.gameObject, true);
            }
        }
        // if the current bite hasn't been found
        else if (!found)
        {
            // mark it as found
            _biteController.FoundBite(gameObject, biteIdx);
            found = true;
        }
    }

    private void UnSelectTheOther(GameObject other)
    {
        other.GetComponent<BiteSelf>().SetCurrentlySelected(false);
    }

    private void OnTriggerExit(Collider other)
    {
        // if the current bite leaves the collider of another bite
        if (other.CompareTag("Sound Bite"))
        {
            // and both are already on stage, then unhighlight the other one
            if (found && other.gameObject.GetComponent<BiteSelf>().found && currentlySelected)
            {
                UnColorBite(other.gameObject);
                SetCurrentlySelected(false);
                cubeInContact = null;
            }
        }
    }

    /**
    With the current object and the one it collided with, swap both item in
    terms of position on the stage and their index on the hierarchy. 
    **/
    public void SwapInHierarchy(GameObject collideObj, bool programmatic = false)
    {
        if (currentlySelected || programmatic)
        {
            // swap in position
            BiteSelf otherBiteSelf = collideObj.GetComponent<BiteSelf>();
            Vector3 otherPosition = otherBiteSelf.GetFoundPosition();
            Vector3 currPosition = GetFoundPosition();
            SetFoundPosition(otherPosition);
            transform.position = otherPosition;
            otherBiteSelf.SetFoundPosition(currPosition);
            collideObj.transform.position = currPosition;

            // swap in hierarchy
            int otherSiblingIndex = collideObj.transform.GetSiblingIndex();
            int currSiblingIndex = transform.GetSiblingIndex();
            collideObj.transform.SetSiblingIndex(currSiblingIndex);
            transform.SetSiblingIndex(otherSiblingIndex);
            if (!programmatic)
                CustomController.AddAction(MoveLogEntry.MoveType.Swap, GetComponent<SoundBiteGrabbable>(), collideObj.GetComponent<SoundBiteGrabbable>(), null);
        }
    }

    /**
    Sets whether the bite is currently selected or not. If no longer selected,
    then reset to its found position and original rotation (in order to prevent
    any unwanted rotation).
    **/
    public void SetCurrentlySelected(bool isSelected)
    {
        currentlySelected = isSelected;
        if (!isSelected){
            transform.position = foundPosition;
            transform.rotation = originalAngles;
        }
    }

    /**
    Public method to get the found position (i.e. the position where the bite
    will be placed on the stage according to the number of bites found so far
    **/
    public Vector3 GetFoundPosition()
    {
        return foundPosition;
    }

    // Public method to set the found position.
    public void SetFoundPosition(Vector3 position)
    {
        foundPosition = position;
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
