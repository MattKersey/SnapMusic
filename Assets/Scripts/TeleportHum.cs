using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class TeleportHum : StudioEventEmitterOcclusion
{
    void Start()
    {
        fmodInstance = RuntimeManager.CreateInstance(fmodEvent);
        RuntimeManager.AttachInstanceToGameObject(fmodInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        //fmodInstance.start();
    }

    public void PlaySound()
    {
        fmodInstance.start();
    }

    public void StopSound()
    {
        fmodInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
