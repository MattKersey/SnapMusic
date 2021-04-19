using UnityEngine;
using FMODUnity;

public class Avril14th : StudioEventEmitterOcclusion
{
    void Start()
    {
        fmodInstance = RuntimeManager.CreateInstance(fmodEvent);
        RuntimeManager.AttachInstanceToGameObject(fmodInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    public void PlaySong()
    {
        fmodInstance.start();
    }

    public void StopSong()
    {
        fmodInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
