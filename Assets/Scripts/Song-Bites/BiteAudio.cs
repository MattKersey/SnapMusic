using UnityEngine;
using FMODUnity;

public class BiteAudio : StudioEventEmitterOcclusion
{
    public bool isPlaying = false;

    public void LoadBite(int biteNum)
    {
        fmodEvent = "event:/Bite " + biteNum.ToString();
        fmodInstance = RuntimeManager.CreateInstance(fmodEvent);
        RuntimeManager.AttachInstanceToGameObject(fmodInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    public void PlayBite()
    {
        fmodInstance.start();
        isPlaying = true;
    }

    public void StopBite()
    {
        fmodInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isPlaying = false;
    }

    public void SetDirection(int direction)
    {
        fmodInstance.setParameterByName("Direction", direction);
    }
}
